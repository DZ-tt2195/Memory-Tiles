using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] Slider tileSlider;
    [SerializeField] TMP_Text tileLabel;

    [SerializeField] Slider minigameSlider;
    [SerializeField] TMP_Text minigameLabel;
    [SerializeField] TMP_Text totalMinigames;

    [SerializeField] TMP_Dropdown minigameDropdown;
    [SerializeField] Button minigamePlay;

    [SerializeField] Button deleteScoreButton;
    [SerializeField] TMP_Text bestScore;

    void Start()
    {
        if (!PlayerPrefs.HasKey("Tiles")) PlayerPrefs.SetInt("Tiles", 10);
        tileSlider.onValueChanged.AddListener(UpdateTileSlider);
        tileSlider.value = PlayerPrefs.GetInt("Tiles");
        UpdateTileSlider(PlayerPrefs.GetInt("Tiles"));

        void UpdateTileSlider(float value)
        {
            tileLabel.text = $"{Translator.inst.Translate("Tiles")}: {value}";
            PlayerPrefs.SetInt("Tiles", (int)value);
            UpdateTotalMinigames();
        }

        if (!PlayerPrefs.HasKey("Minigame")) PlayerPrefs.SetInt("Minigame", 4);
        minigameSlider.onValueChanged.AddListener(UpdateMinigameSlider);
        minigameSlider.value = PlayerPrefs.GetInt("Minigame");
        UpdateMinigameSlider(PlayerPrefs.GetInt("Minigame"));

        void UpdateMinigameSlider(float value)
        {
            minigameLabel.text = Translator.inst.Translate("Minigame Count", new() { ("Num", $"{(int)value}") });
            PlayerPrefs.SetInt("Minigame", (int)value);
            UpdateTotalMinigames();
        }

        void UpdateTotalMinigames()
        {
            int amount = (int)(tileSlider.value / minigameSlider.value);
            if ((int)tileSlider.value % (int)minigameSlider.value == 0)
                amount--;
            totalMinigames.text = $"{amount} {Translator.inst.Translate($"Minigames")}";
        }

        List<string> minigameScenes = MinigameManager.inst.GetMinigames();
        for (int i = 0; i < minigameScenes.Count; i++)
            minigameDropdown.AddOptions(new List<string>() { Translator.inst.Translate(minigameScenes[i]) });

        minigamePlay.onClick.AddListener(PlayMinigame);
        void PlayMinigame()
        {
            MinigameManager.inst.LoadMinigame(minigameScenes[minigameDropdown.value]);
        }

        FindScore();
        void FindScore()
        {
            if (PlayerPrefs.GetInt("High Score") > 0)
                bestScore.text = $"{Translator.inst.Translate("High Score")}: {PlayerPrefs.HasKey("High Score")}";
            else
                bestScore.text = Translator.inst.Translate("No Score");
        }
        deleteScoreButton.onClick.AddListener(DeleteScores);
        void DeleteScores()
        {
            PlayerPrefs.SetInt("High Score", 0);
            FindScore();
        }
    }
}
