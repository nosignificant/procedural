using UnityEngine;

public class InterestObject : MonoBehaviour
{
    [Header("Data Link")]
    // 🔥 아까 만든 데이터 파일(Wolf_Data 등)을 여기에 넣습니다.
    public CreatureData creatureData;

    [Header("Runtime Status")]
    // 게임 도중 바뀔 수 있는 정보들 (데이터에서 복사해옴)
    public Faction faction;
    public float weight = 10f;

    [Header("References")]
    public Transform rootTransform;

    void Awake()
    {
        // 1. 본체 찾기 (루트 트랜스폼 & 크리처 스크립트)
        if (rootTransform == null) rootTransform = transform.root;

        // 2. 데이터 파일이 연결되어 있다면, 초기 정보를 덮어씌웁니다.
        if (creatureData != null)
        {
            // 이름도 디버깅하기 쉽게 바꿔주면 좋습니다
            //gameObject.name = $"[Interest] {creatureData.speciesName}";
        }
    }

    // 💡 개발 편의 기능: 씬 뷰에서 이 오브젝트가 어디 있는지 눈에 띄게 표시
    void OnDrawGizmos()
    {
        // 팩션별로 색깔 다르게 표시
        switch (faction)
        {
            case Faction.Plant: Gizmos.color = Color.green; break;
            case Faction.Carnivore: Gizmos.color = Color.red; break;
            case Faction.Herbivore: Gizmos.color = Color.yellow; break;
            default: Gizmos.color = Color.white; break;
        }

        // 작은 구슬을 그려서 위치 표시
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}