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

    float moveDuration = 2f;

    private Coroutine moveCoroutine;
    public Transform target;

    bool isMoving = false;
    private bool isCentered = false;
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
        if (fromThis2Target(this.transform.position, target.position) > 3f)
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
            isCentered = false;
            yield return StartCoroutine(MoveFoot(0));
            yield return new WaitForSeconds(0.05f);
            yield return StartCoroutine(MoveFoot(1));
            yield return StartCoroutine(MoveBody());
            yield return new WaitForSeconds(0.5f);

        }
        isMoving = false;
    }

    IEnumerator MoveFoot(int index)
    {
        Vector3 dir = foots[index].transform.position - target.transform.position;
        Vector3 startPos = foots[index].stablePosition;
        Vector3 expectPos = foots[index].RestPosition(dir.normalized);
        float t = 0f;
        float stepHeight = 0.5f;

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
        float x = foots[0].transform.position.x + foots[1].transform.position.x;
        float z = foots[0].transform.position.z + foots[1].transform.position.z;
        Vector3 startPos = this.transform.position;
        Vector3 avgPos = new Vector3(x / 2, this.transform.position.y, z / 2);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.fixedDeltaTime / moveDuration;
            Vector3 currentPos = Vector3.Lerp(startPos, avgPos, t);
            this.transform.position = currentPos;
            yield return null;
        }
        this.transform.position = avgPos;
        yield return null;



    }

    float fromThis2Target(Vector3 posA, Vector3 posB)
    {
        return Vector3.Distance(posA, posB);
    }
}
