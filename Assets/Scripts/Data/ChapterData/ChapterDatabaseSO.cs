using System.Linq;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG Setup/Chapter Data/Chapter Database", fileName = "CHAPTER DATABASE")]
public class ChapterDatabaseSO : ScriptableObject
{
    public ChapterDataSO[] allChapters;

    public ChapterDataSO GetChapterByName(string name)
    {
        return allChapters.FirstOrDefault(c => c.chapterName == name);
    }
    #if UNITY_EDITOR
    [ContextMenu("Auto-fill with all ChapterDataSO")]
    public void CollectChaptersData()
    {
        string[] guids = AssetDatabase.FindAssets("t:ChapterDataSO");
        allChapters = guids
            .Select(guid => AssetDatabase.LoadAssetAtPath<ChapterDataSO>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(c => c != null)
            .OrderBy(c => c.name) // 可按名字排序或去掉
            .ToArray();

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
        Debug.Log($"Collected {allChapters.Length} ChapterDataSO into {name}");
    }
    #endif
}
