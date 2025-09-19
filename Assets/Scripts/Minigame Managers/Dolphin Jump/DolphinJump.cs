using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using UnityEngine;
using TMPro;

namespace DolphinJump
{
    public class DolphinJump : CurrentMinigame
    {
        Stopwatch gameTimer;
        [SerializeField] TMP_Text timerText;

        int score = 0;
        Queue<GameObject> coinQueue = new();
        [SerializeField] GameObject coinPrefab;

        Queue<GameObject> rockQueue = new();
        [SerializeField] GameObject rockPrefab;

        private void Awake()
        {
            instance = this;
            gameTimer = new Stopwatch();
            timerText.text = "";
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
            InvokeRepeating(nameof(CreateCoin), 0f, 1);
            InvokeRepeating(nameof(CreateRock), 0f, 2f);
        }

        private void Update()
        {
            if (gameState == GameState.Started)
            {
                performanceSlider.value = (float)score / amazingGrade;
                timerText.text = StopwatchTime(gameTimer);
            }
        }

        public void ReturnToQueue(GameObject obj, bool coin, bool affectScore)
        {
            if (coin)
            {
                coinQueue.Enqueue(obj);
                obj.SetActive(false);
                if (affectScore)
                    score = (int)Mathf.Clamp(score + 1, 0, amazingGrade);
            }
            else
            {
                rockQueue.Enqueue(obj);
                obj.SetActive(false);
                if (affectScore)
                    score = (int)Mathf.Clamp(score - 3, 0, amazingGrade);
            }
        }

        void CreateCoin()
        {
            if (gameState != GameState.Started)
                return;

            GameObject newCoin = (coinQueue.Count > 0) ? coinQueue.Dequeue() : Instantiate(coinPrefab);
            newCoin.SetActive(true);

            float randomY = Random.Range(-4f, 4f);
            Vector2 starting = new(10f, randomY);
            Vector2 ending = new(-10f, randomY);
            StartCoroutine(MoveObject(newCoin, false, Random.Range(20, 30f), starting, ending));
        }

        void CreateRock()
        {
            if (gameState != GameState.Started)
                return;

            GameObject newRock = (rockQueue.Count > 0) ? rockQueue.Dequeue() : Instantiate(rockPrefab);
            newRock.SetActive(true);

            float randomY = Random.Range(-5f, 4f);
            Vector2 starting = new(10f, randomY);
            Vector2 ending = new(-10f, randomY);
            StartCoroutine(MoveObject(newRock, false, Random.Range(10f, 15f), starting, ending));
        }

        IEnumerator MoveObject(GameObject obj, bool coin, float targetTime, Vector2 start, Vector2 end)
        {
            float elapsedTime = 0f;
            while (elapsedTime < targetTime)
            {
                elapsedTime += Time.deltaTime;
                obj.transform.position = Vector2.Lerp(start, end, elapsedTime / targetTime);
                yield return null;
                if (!obj.activeSelf)
                    yield break;
            }
            ReturnToQueue(obj, coin, false);
        }

        public void GameEnded()
        {
            gameTimer.Stop();
            gameState = GameState.Ended;
            MinigameManager.inst.MinigameEnd(CurrentGrade((float)gameTimer.Elapsed.TotalSeconds));
        }
    }
}