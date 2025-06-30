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

    [SerializeField] TMP_Dropdown minigameDropdown;
    [SerializeField] Button minigamePlay;

    [SerializeField] Button deleteScoreButton;

    void Start()
    {
        if (!PlayerPrefs.HasKey("Tiles")) PlayerPrefs.SetInt("Tiles", 10);
        tileSlider.onValueChanged.AddListener(UpdateTileSlider);
        tileSlider.value = PlayerPrefs.GetInt("Tiles");
        UpdateTileSlider(PlayerPrefs.GetInt("Tiles"));

        void UpdateTileSlider(float value)
        {
            tileLabel.text = $"{Translator.inst.GetText("Tiles")}: {value}";
            PlayerPrefs.SetInt("Tiles", (int)value);
        }

        if (!PlayerPrefs.HasKey("Minigame")) PlayerPrefs.SetInt("Minigame", 4);
        minigameSlider.onValueChanged.AddListener(UpdateMinigameSlider);
        minigameSlider.value = PlayerPrefs.GetInt("Minigame");
        UpdateMinigameSlider(PlayerPrefs.GetInt("Minigame"));

        void UpdateMinigameSlider(float value)
        {
            minigameLabel.text = $"{Translator.inst.GetText("Minigame Count")} {(int)value} {Translator.inst.GetText("Tiles")}";
            PlayerPrefs.SetInt("Minigame", (int)value);
        }

        List<string> minigameScenes = MinigameManager.inst.GetMinigames();
        for (int i = 0; i < minigameScenes.Count; i++)
            minigameDropdown.AddOptions(new List<string>() { Translator.inst.GetText(minigameScenes[i]) });

        minigamePlay.onClick.AddListener(PlayMinigame);
        void PlayMinigame()
        {
            MinigameManager.inst.LoadMinigame(minigameScenes[minigameDropdown.value]);
        }
    }
}
