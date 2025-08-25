using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using MyBox;

public enum MinigameGrade { Amazing, Good, Barely, Failed, None }

public class MinigameManager : MonoBehaviour
{

#region Setup

    public static MinigameManager inst;
    List<string> minigameScenes = new();

    [Foldout("First Minigame Popup", true)]
    [SerializeField] Image firstMinigameBG;
    [SerializeField] TMP_Text minigameName;
    [SerializeField] TMP_Text minigameTutorial;
    [SerializeField] Button failMinigame;
    [SerializeField] Button playMinigame;
    string currentMinigame = "";

    [Foldout("Second Minigame Popup", true)]
    [SerializeField] Image secondMinigameBG;
    [SerializeField] Image gradeColoring;
    [SerializeField] TMP_Text gradeText;
    [SerializeField] TMP_Text gradeLetter;
    [SerializeField] Button doneButton;
    public MinigameGrade grade { get; private set; }

    private void Awake()
    {
        inst = this;
        minigameScenes = GetMinigames();

        firstMinigameBG.gameObject.SetActive(false);
        playMinigame.onClick.AddListener(StartMinigame);

        failMinigame.gameObject.SetActive(false);
        failMinigame.onClick.AddListener(() => MinigameEnd(MinigameGrade.Failed));

        secondMinigameBG.gameObject.SetActive(false);
        doneButton.onClick.AddListener(UnloadMinigame);

        List<string> GetMinigames()
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
            //apply all the new scenes into the scene manager
            EditorBuildSettings.scenes = allScenes.ToArray();

            return listOfMinigames;
        }
    }

    public List<string> GetMinigames()
    {
        return minigameScenes;
    }

    #endregion

#region Minigames

    public void LoadMinigame(string sceneName)
    {
        currentMinigame = sceneName;
        this.grade = MinigameGrade.None;

        minigameName.text = Translator.inst.Translate(sceneName);
        minigameTutorial.text = Translator.inst.Translate($"{sceneName} Tutorial");

        firstMinigameBG.gameObject.SetActive(true);
        failMinigame.gameObject.SetActive(true);
        StartCoroutine(LoadAndSetActive(sceneName));
    }

    IEnumerator LoadAndSetActive(string sceneName)
    {
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        Scene loadedScene = SceneManager.GetSceneByName(sceneName);
        SceneManager.SetActiveScene(loadedScene);
    }

    void StartMinigame()
    {
        CurrentMinigame.instance.StartMinigame();
        firstMinigameBG.gameObject.SetActive(false);
    }

    public void MinigameEnd(MinigameGrade grade)
    {
        if (this.grade == MinigameGrade.None)
        {
            firstMinigameBG.gameObject.SetActive(false);
            secondMinigameBG.gameObject.SetActive(true);
            failMinigame.gameObject.SetActive(false);

            this.grade = grade;
            gradeText.text = Translator.inst.Translate($"{grade} Text");
            gradeLetter.text = Translator.inst.Translate($"{grade} Letter");
            gradeColoring.color = this.grade switch
            {
                MinigameGrade.Amazing => new Color(0, 0.8f, 0),
                MinigameGrade.Good => Color.blue,
                MinigameGrade.Barely => Color.red,
                _ => Color.gray
            };
        }
    }

    void UnloadMinigame()
    {
        firstMinigameBG.gameObject.SetActive(false);
        secondMinigameBG.gameObject.SetActive(false);
        SceneManager.UnloadSceneAsync(currentMinigame);
        currentMinigame = "";
    }

    #endregion

}
