using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Creature/Database")]
public class CreatureDatabase : ScriptableObject
{
    public List<CreatureData> allCreatures;

    [ContextMenu("Load All From Resources")]
    public void LoadAll()
    {
        allCreatures = Resources.LoadAll<CreatureData>("Creatures").ToList();
        Debug.Log($"총 {allCreatures.Count}마리의 생물 데이터 로드 완료!");
    }

    [ContextMenu("Load From My Folder")]
    public void LoadMyCreatures()
    {
#if UNITY_EDITOR
        allCreatures = new List<CreatureData>();

        // 검색할 폴더 경로
        string targetPath = "Assets/script/Creature"; 

        if (!AssetDatabase.IsValidFolder(targetPath))
        {
            Debug.LogError($"경로를 찾을 수 없습니다: {targetPath}\n");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:CreatureData", new[] { targetPath });

        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            CreatureData data = AssetDatabase.LoadAssetAtPath<CreatureData>(assetPath);

            if (data != null)
            {
                allCreatures.Add(data);
            }
        }
        
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log($"📂 '{targetPath}' 폴더에서 총 {allCreatures.Count}마리 로드");
#endif
    }

    // ---------------------------------------------------------
    // 게임 실행 중에 사용하는 검색 함수들
    // ---------------------------------------------------------

    public CreatureData GetCreatureByName(string name)
    {
        return allCreatures.FirstOrDefault(c => c.creatureName == name);
    }
}