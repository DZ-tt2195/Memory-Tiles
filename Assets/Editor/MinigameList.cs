#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public static class MinigameList
{
    private const string OutputPath = "Assets/Resources/MinigamesList.txt";

    [MenuItem("Tools/Update Minigames List")]
    public static void UpdateMinigamesList()
    {
        string[] list = Directory.GetFiles($"Assets/Minigame Scenes", "*.unity", SearchOption.TopDirectoryOnly);
        List<EditorBuildSettingsScene> allScenes = EditorBuildSettings.scenes.ToList();
        List<string> listOfMinigames = new();
        for (int i = 0; i < list.Length; i++)
        {
            listOfMinigames.Add(Path.GetFileNameWithoutExtension(list[i]));
            if (!allScenes.Any(scene => scene.path == list[i])) //if current scene manager doesn't have new scene
            {
                allScenes.Add(new EditorBuildSettingsScene(list[i], true));
                Debug.Log($"add {list[i]}");
            }
        }
        EditorBuildSettings.scenes = allScenes.ToArray();
        File.WriteAllLines(OutputPath, listOfMinigames);
        AssetDatabase.Refresh();
    }
}
#endif