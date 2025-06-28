using NUnit.Framework;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SetUpFixture]
public class AddTestScenes
{
    [OneTimeSetUp]
    public void BeforeAnyTestsRun()
    {
        string[] sceneFiles = Directory.GetFiles("Assets/Tests/PlayMode/Scenes", "*.unity", SearchOption.TopDirectoryOnly);
        List<EditorBuildSettingsScene> allScenes = EditorBuildSettings.scenes.ToList();

        foreach (string scenePath in sceneFiles)
        {
            if (!allScenes.Any(scene => scene.path == scenePath)) //if current scene manager doesn't have new scene
            {
                allScenes.Add(new EditorBuildSettingsScene(scenePath, true));
                Debug.Log($"add {scenePath}");
            }
        }
        //apply all the new scenes into the scene manager
        EditorBuildSettings.scenes = allScenes.ToArray();
    }
}