using UnityEngine;
using Game.Creatures;
using System.Collections.Generic;


[CreateAssetMenu(fileName = "New Creature", menuName = "Creature Data")]

public class CreatureData : ScriptableObject
{
    public string creatureName;
    public int creatureID;

    [Header("identity")]
    public float interestWeight = 1;

    [Header("stat")]
    public int maxHP;
    public float speed;

    [Header("relationship")]
    public List<CreatureData> foodCreatures = new List<CreatureData>();
    public List<CreatureData> enemyCreatures = new List<CreatureData>();
    public List<CreatureData> friendCreatures = new List<CreatureData>();

    //[Header("sprite")]
    //public Sprite icon;      
    //public GameObject prefab;  

    public RelationType GetRelationTo(CreatureData target)
    {
        if (target == null) return RelationType.Neutral;

        if (enemyCreatures != null && enemyCreatures.Contains(target))
            return RelationType.Enemy;

        if (foodCreatures != null && foodCreatures.Contains(target))
            return RelationType.Food;

        if (friendCreatures != null && friendCreatures.Contains(target))
            return RelationType.Friend;

        return RelationType.Neutral;
    }

    public RelationType GetRelationTo(int targetCreatureId)
    {
        if (targetCreatureId == 0) return RelationType.Neutral;

        if (ContainsId(enemyCreatures, targetCreatureId)) return RelationType.Enemy;
        if (ContainsId(foodCreatures, targetCreatureId)) return RelationType.Food;
        if (ContainsId(friendCreatures, targetCreatureId)) return RelationType.Friend;

        return RelationType.Neutral;
    }

    private static bool ContainsId(List<CreatureData> list, int id)
    {
        if (list == null) return false;
        for (int i = 0; i < list.Count; i++)
        {
            var d = list[i];
            if (d != null && d.creatureID == id) return true;
        }
        return false;
    }

}