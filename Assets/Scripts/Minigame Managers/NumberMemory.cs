using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class NumberMemory : CurrentMinigame
{
    [SerializeField] Transform buttonDisplays;
    int currentNumber = -4;
    List<int> allNumbers = new();
    [SerializeField] TMP_Text currentNumberText;
    [SerializeField] TMP_Text nextNumberText;

    private void Awake()
    {
        instance = this;
    }

    protected override MinigameGrade CurrentGrade(float score)
    {
        if (score >= amazingGrade)
            return MinigameGrade.Amazing;
        else if (score <= amazingGrade && score >= goodGrade)
            return MinigameGrade.Good;
        else if (score <= goodGrade && score >= barelyGrade)
            return MinigameGrade.Barely;
        else
            return MinigameGrade.Failed;
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
        PlaceMarker(barelyGrade / amazingGrade);
        PlaceMarker(goodGrade / amazingGrade);
        NextNumber();
    }

    void NextNumber()
    {
        currentNumber++;
        currentNumberText.gameObject.SetActive(currentNumber >= 0);
        buttonDisplays.gameObject.SetActive(currentNumber >= 0);

        nextNumberText.text = "";
        Invoke(nameof(RevealNumber), 0.25f);

        if (currentNumber < 0)
        {
            Invoke(nameof(NextNumber), 1.5f);
        }
        else
        {
            buttonDisplays.Shuffle();
            Debug.Log(allNumbers[currentNumber]);
        }
    }

    void RevealNumber()
    {
        int newNumber = Random.Range(0, 10);
        allNumbers.Add(newNumber);
        nextNumberText.text = $"{newNumber}";
    }

    public void GotNumber(int number)
    {
        if (number == allNumbers[currentNumber])
        {
            NextNumber();
            performanceSlider.value = (float)currentNumber / amazingGrade;
        }
        else
        {
            gameState = GameState.Ended;
            MinigameManager.inst.MinigameEnd(CurrentGrade(currentNumber));
        }
    }
}
