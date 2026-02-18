using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// 🔥 에디터 기능을 쓰기 위해 꼭 필요함! (빌드 시에는 무시되도록 설정)
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Creature/Database")]
public class CreatureDatabase : ScriptableObject
{
    // 🔥 여기에 만든 모든 생물 데이터를 드래그해서 넣거나, 아래 버튼을 눌러 채웁니다.
    public List<CreatureData> allCreatures;

    // [옵션 1] Resources 폴더 사용 시 (폴더명이 무조건 Resources여야 함)
    [ContextMenu("Load All From Resources")]
    public void LoadAll()
    {
        allCreatures = Resources.LoadAll<CreatureData>("Creatures").ToList();
        Debug.Log($"총 {allCreatures.Count}마리의 생물 데이터 로드 완료!");
    }

    // [옵션 2] 내 마음대로 폴더 사용 시 (작성자님 픽! 👍)
    [ContextMenu("Load From My Folder")]
    public void LoadMyCreatures()
    {
#if UNITY_EDITOR
        // 1. 리스트 초기화
        allCreatures = new List<CreatureData>();

        // 2. 검색할 폴더 경로 (폴더 이름 정확해야 함!)
        string targetPath = "Assets/script/Creature"; 

        // 3. 해당 폴더가 진짜 있는지 확인 (오타 방지용 안전장치)
        if (!AssetDatabase.IsValidFolder(targetPath))
        {
            Debug.LogError($"경로를 찾을 수 없습니다: {targetPath}\n폴더 이름이 정확한지 확인해주세요!");
            return;
        }

        // 4. GUID 찾기 & 데이터 로드
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
        
        // 5. 저장
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();

        Debug.Log($"📂 '{targetPath}' 폴더에서 총 {allCreatures.Count}마리 로드 완료!");
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