using UnityEngine;
using System.Collections.Generic;
using System;
using TMPro;
using System.Diagnostics;

public class Alphabet : CurrentMinigame
{
    List<KeyCode> keysToPress;
    int randomStart;
    int currentPosition;

    [SerializeField] TMP_Text currentLetter;
    Stopwatch gameTimer;

    private void Awake()
    {
        instance = this;

        keysToPress = new();
        for (char c = 'A'; c <= 'Z'; c++)
            keysToPress.Add((KeyCode)Enum.Parse(typeof(KeyCode), c.ToString()));

        randomStart = UnityEngine.Random.Range(0, 26);
        currentPosition = randomStart;
        gameTimer = new Stopwatch();
    }

    protected override MinigameGrade CurrentGrade(float score)
    {
        if (score <= amazingGrade)
            return MinigameGrade.Amazing;
        else if (score >= amazingGrade && score <= goodGrade)
            return MinigameGrade.Good;
        else if (score >= goodGrade && score <= barelyGrade)
            return MinigameGrade.Barely;
        else
            return MinigameGrade.Failed;
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
        PlaceMarker(amazingGrade / barelyGrade);
        PlaceMarker(goodGrade / barelyGrade);
        gameTimer.Start();
        currentLetter.text = keysToPress[currentPosition].ToString();
    }

    private void Update()
    {
        if (gameTimer.IsRunning)
        {
            performanceSlider.value = (float)gameTimer.Elapsed.TotalSeconds / barelyGrade;
            if (performanceSlider.value == 1)
            {
                gameTimer.Stop();
                gameState = GameState.Ended;
                MinigameManager.inst.MinigameEnd(MinigameGrade.Failed);
            }
            if (Input.GetKeyDown(keysToPress[currentPosition]))
            {
                currentPosition = (currentPosition + 1) % 26;
                if (currentPosition == randomStart)
                {
                    currentLetter.text = "";
                    gameTimer.Stop();
                    gameState = GameState.Ended;
                    MinigameManager.inst.MinigameEnd(CurrentGrade((float)gameTimer.Elapsed.TotalSeconds));
                }
                else
                {
                    currentLetter.text = keysToPress[currentPosition].ToString();
                }
            }
        }
    }
}
