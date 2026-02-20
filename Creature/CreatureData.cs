using UnityEngine;
using Game.Creatures;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "New Creature", menuName = "Creature Data")]

public class CreatureData : ScriptableObject
{
    public string creatureName;
    public int creatureID;

    [Header("identity")]
    public float interestWeight;

    [Header("stat")]
    public int maxHP;
    public float speed;

    [Header("relationship")]
    public List<int> foodCreatureIds = new List<int>();
    public List<int> enemyCreatureIds = new List<int>();
    public List<int> friendCreatureIds = new List<int>();

    public RelationType GetRelationTo(int targetCreatureId)
    {
        // 우선순위: Enemy > Food > Friend > Neutral
        if (targetCreatureId == 0) return RelationType.Neutral;

        if (enemyCreatureIds != null && enemyCreatureIds.Contains(targetCreatureId))
            return RelationType.Enemy;

        if (foodCreatureIds != null && foodCreatureIds.Contains(targetCreatureId))
            return RelationType.Food;

        if (friendCreatureIds != null && friendCreatureIds.Contains(targetCreatureId))
            return RelationType.Friend;

        return RelationType.Neutral;
    }


    //[Header("sprite")]
    //public Sprite icon;      
    //public GameObject prefab;  
}