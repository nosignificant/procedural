using UnityEngine;

[CreateAssetMenu(fileName = "New Creature", menuName = "Creature Data")]

public class CreatureData : ScriptableObject
{
    public string creatureName;
    public int creatureID;

    [Header("identity")]
    public Faction faction;
    public float interestWeight;

    [Header("stat")]
    public int maxHP;
    public float speed;

    //[Header("sprite")]
    //public Sprite icon;      
    //public GameObject prefab;  
}