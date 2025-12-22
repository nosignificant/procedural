using UnityEngine;
using System.Collections;
public class ControlLegs : MonoBehaviour
{
    //1. 앞으로 이동 명령이 있으면 발부터 뻗는다. 
    // - 이전 위치와 입력 받은restPosition() 이 다르면 발을 이동시킨다. 이걸 왼쪽 다리부터 한다 
    //2. 발이 땅에 닿게 한다 
    //3. 몸통을 발이 이동한 만큼 앞으로 가져간다. 
    //4. 발이 다시 땅에 닿으면 stable position을 업데이트한다 

    //0이 왼쪽, 1이 오른쪽
    public Foot[] foots;
    public float stride = 5f;
    public Vector3 defaultRot;

    float moveDuration = 3f;

    private Coroutine moveCoroutine;
    public Transform target;

    bool isMoving = false;
    void Start()
    {
        if (foots != null)
        {
            foots[0].LR = 0;
            foots[1].LR = 1;
        }
    }
    void Update()
    {

        if (Util.fromThis2Target(this.transform.position, target.position) > 3f)
        {
            Move();
        }
        else Stop();

        //발 따라움직이는지 확인용 코드 
        if (Input.GetKeyDown(KeyCode.E))
            foots[0].transform.position =
            new Vector3(foots[0].transform.position.x,
            foots[0].transform.position.y,
            foots[0].transform.position.z + 0.5f);
    }

    void Move()
    {
        if (!isMoving) moveCoroutine = StartCoroutine(moveForward());

    }

    void Stop()
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        isMoving = false;
    }

    IEnumerator moveForward()
    {
        isMoving = true;

        while (isMoving)
        {
            StartCoroutine(RotBody());

            yield return StartCoroutine(MoveFoot(0));
            //yield return new WaitWhile(() => isCentered);

            yield return new WaitForSeconds(0.05f);
            yield return StartCoroutine(MoveFoot(1));
            yield return StartCoroutine(MoveBody());

        }
        isMoving = false;
    }

    IEnumerator MoveFoot(int index)
    {
        // 타겟 - 나 해야 양수 방향 나옴 
        Vector3 dir = target.transform.position - foots[index].transform.position;
        Vector3 startPos = foots[index].stablePosition;
        Vector3 expectPos = foots[index].RestPosition(dir.normalized, stride);
        float t = 0f;
        float stepHeight = 1f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / moveDuration;
            //lerp가 뭐지 
            Vector3 currentPos = Vector3.Lerp(startPos, expectPos, t);
            currentPos.y += Mathf.Sin(t * Mathf.PI) * stepHeight;
            foots[index].transform.position = currentPos;
            yield return null;
        }
        foots[index].transform.position = expectPos;

        foots[index].stablePosition = expectPos;
        //Debug.Log("criteria fulfilled. moving start");
    }
    IEnumerator MoveBody()
    {

        //Vector3 dir = (transform.position - target.transform.position).normalized;
        float x = foots[0].transform.position.x + foots[1].transform.position.x;
        float z = foots[0].transform.position.z + foots[1].transform.position.z;
        Vector3 startPos = this.transform.position;
        Vector3 avgPos = new Vector3(x / 2, this.transform.position.y, z / 2);
        //+ dir * stride;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.fixedDeltaTime / moveDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, avgPos, t);
            this.transform.position = currentPos;
            yield return null;

        }
        this.transform.position = avgPos;
    }

    IEnumerator RotBody()
    {
        Vector3 between = foots[1].transform.position - foots[0].transform.position;
        between.y = 0;
        between.Normalize();
        Vector3 forwardDir = new Vector3(-between.z, 0, between.x);

        if (forwardDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(forwardDir);
            transform.rotation = Quaternion.Slerp(
                        transform.rotation, targetRot, Time.deltaTime * 5f);
        }
        yield return null;

    }


}
