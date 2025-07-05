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
    [SerializeField] TMP_Text stopwatchText;
    Stopwatch gameTimer;

    private void Awake()
    {
        CurrentMinigame.instance = this;

        keysToPress = new();
        for (char c = 'A'; c <= 'Z'; c++)
            keysToPress.Add((KeyCode)Enum.Parse(typeof(KeyCode), c.ToString()));

        randomStart = UnityEngine.Random.Range(0, 26);
        currentPosition = randomStart;
        gameTimer = new Stopwatch();
    }

    public override void StartMinigame()
    {
        base.StartMinigame();
        gameTimer.Start();
        currentLetter.text = keysToPress[currentPosition].ToString();
    }

    private void Update()
    {
        if (gameTimer.IsRunning)
        {
            stopwatchText.text = StopwatchTime(gameTimer);
            performanceSlider.value = (float)gameTimer.Elapsed.TotalSeconds / failGrade;
            if (performanceSlider.value == 1)
            {
                gameTimer.Stop();
                MinigameManager.inst.MinigameEnd(MinigameGrade.Failed);
            }
            if (Input.GetKeyDown(keysToPress[currentPosition]))
            {
                currentPosition = (currentPosition + 1) % 26;
                if (currentPosition == randomStart)
                {
                    currentLetter.text = "";
                    gameTimer.Stop();
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
