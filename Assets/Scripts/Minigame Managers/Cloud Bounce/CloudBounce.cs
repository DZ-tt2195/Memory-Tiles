using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using System.Collections;

public class CloudBounce : CurrentMinigame
{
    Stopwatch gameTimer;
    [SerializeField] GameObject platformPrefab;

    Queue<GameObject> deathQueue = new();
    [SerializeField] GameObject deathPrefab;

    private void Awake()
    {
        instance = this;
        gameTimer = new Stopwatch();
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
        gameTimer.Start();

        for (int i = 0; i < 8; i++)
        {
            GameObject newPlatform = Instantiate(platformPrefab);
            ReturnPlatform(newPlatform);
        }

        InvokeRepeating(nameof(NewDeath), 0f, 2f);
    }

    private void Update()
    {
        if (gameState == GameState.Started)
        {
            performanceSlider.value = (float)gameTimer.Elapsed.TotalSeconds / amazingGrade;
            if (performanceSlider.value == 1)
                GameEnded();
        }
    }

    public void ReturnPlatform(GameObject platform)
    {
        platform.transform.position = new(Random.Range(-7.5f, 7.5f), Random.Range(2f, -3.5f));
    }

    void NewDeath()
    {
        if (gameState != GameState.Started)
            return;

        for (int i = 0; i < 2; i++)
        {
            GameObject newDeath = (deathQueue.Count > 0) ? deathQueue.Dequeue() : Instantiate(deathPrefab);
            newDeath.SetActive(true);

            int randomNum = Random.Range(0, 4);
            Vector2 starting = Vector3.one;
            Vector2 ending = Vector3.one;

            switch (randomNum)
            {
                case 0:
                    starting = new(-8f, Random.Range(4f, -4f));
                    ending = new(8f, Random.Range(4f, -4f));
                    break;
                case 1:
                    starting = new(8f, Random.Range(4f, -4f));
                    ending = new(-8f, Random.Range(4f, -4f));
                    break;
                case 2:
                    starting = new(Random.Range(-6f, 6f), 8f);
                    ending = new(Random.Range(-6f, 6f), -8f);
                    break;
                case 3:
                    starting = new(Random.Range(-6f, 6f), -8f);
                    ending = new(Random.Range(-6f, 6f), 8f);
                    break;
            }
            StartCoroutine(MoveDeath(newDeath, Random.Range(10f, 15f), starting, ending));
        }

        IEnumerator MoveDeath(GameObject death, float targetTime, Vector2 start, Vector2 end)
        {
            float elapsedTime = 0f;
            while (elapsedTime < targetTime)
            {
                elapsedTime += Time.deltaTime;
                death.transform.position = Vector2.Lerp(start, end, elapsedTime / targetTime);
                yield return null;
            }
            deathQueue.Enqueue(death);
            death.gameObject.SetActive(false);
        }
    }

    public void GameEnded()
    {
        gameTimer.Stop();
        gameState = GameState.Ended;
        MinigameManager.inst.MinigameEnd(CurrentGrade((float)gameTimer.Elapsed.TotalSeconds));
    }
}
