using System;
using System.Diagnostics;
using UnityEngine;
using TMPro;

public class Timer : CurrentMinigame
{
    float targetTime;
    float currentTime = 0f;

    [SerializeField] TMP_Text targetTimeText;
    [SerializeField] TMP_Text currentTimeText;

    private void Awake()
    {
        instance = this;
        targetTime = UnityEngine.Random.Range(7f, 15f);
        targetTimeText.text = $"{targetTime:F1}";
        currentTimeText.text = "0.0";
    }

    protected override MinigameGrade CurrentGrade(float score)
    {
        currentTimeText.gameObject.SetActive(true);
        if (score <= amazingGrade)
            return MinigameGrade.Amazing;
        else if (score >= amazingGrade && score <= goodGrade)
            return MinigameGrade.Good;
        else if (score >= goodGrade && score <= barelyGrade)
            return MinigameGrade.Barely;
        else
            return MinigameGrade.Failed;
    }

    private void Update()
    {
        if (gameState == GameState.Started)
        {
            currentTime += Time.deltaTime;
            currentTimeText.text = $"{currentTime:F1}";
            if (currentTime >= 2f)
                currentTimeText.gameObject.SetActive(false);

            if (Input.GetKeyDown(KeyCode.Space) || currentTime >= (targetTime + barelyGrade))
            {
                gameState = GameState.Ended;
                MinigameManager.inst.MinigameEnd(CurrentGrade((float)Mathf.Abs(targetTime - currentTime)));
            }
        }
    }
}
