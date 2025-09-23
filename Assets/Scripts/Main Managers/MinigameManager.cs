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
        minigameScenes = FindMinigames();

        firstMinigameBG.gameObject.SetActive(false);
        playMinigame.onClick.AddListener(StartMinigame);

        failMinigame.gameObject.SetActive(false);
        failMinigame.onClick.AddListener(() => MinigameEnd(MinigameGrade.Failed));

        secondMinigameBG.gameObject.SetActive(false);
        doneButton.onClick.AddListener(UnloadMinigame);

        List<string> FindMinigames()
        {
            TextAsset file = Resources.Load<TextAsset>($"MinigamesList");
            List<string> listOfNames = new();
            string[] lines = file.text.Split('\n');

            foreach (string line in lines)
            {
                if (line != "")
                    listOfNames.Add(line);
            }
            return listOfNames;
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
