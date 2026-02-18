using UnityEngine;

public enum Faction { Neutral, Herbivore, Carnivore, Plant, Obstacle }

[CreateAssetMenu(fileName = "New Creature", menuName = "Creature Data")]

public class CreatureData : ScriptableObject
{
    public string creatureName;
    public int maxHP;           // 체력: 100
    public float speed;         // 속도: 5.5
    public Sprite icon;         // 아이콘 이미지
    public GameObject prefab;   // 소환할 때 쓸 프리팹
}