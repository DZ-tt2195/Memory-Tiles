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
        Queue<MoveLeft> coinQueue = new();
        [SerializeField] MoveLeft coinPrefab;

        Queue<MoveLeft> rockQueue = new();
        [SerializeField] MoveLeft rockPrefab;

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
            InvokeRepeating(nameof(CreateCoin), 0f, 1f);
            InvokeRepeating(nameof(CreateRock), 0f, 2f);
        }

        private void Update()
        {
            if (gameState == GameState.Started)
            {
                timerText.text = StopwatchTime(gameTimer);
                if (gameTimer.Elapsed.TotalSeconds >= 30)
                {
                    gameTimer.Stop();
                    gameState = GameState.Ended;
                    MinigameManager.inst.MinigameEnd(CurrentGrade((float)score));
                }
            }
        }

        public void ReturnToQueue(MoveLeft obj, bool coin, bool affectScore)
        {
            if (coin)
            {
                coinQueue.Enqueue(obj);
                obj.gameObject.SetActive(false);
                if (affectScore)
                    score = (int)Mathf.Clamp(score + 1, 0, amazingGrade);
            }
            else
            {
                rockQueue.Enqueue(obj);
                obj.gameObject.SetActive(false);
                if (affectScore)
                    score = (int)Mathf.Clamp(score - 3, 0, amazingGrade);
            }
            performanceSlider.value = (float)score / amazingGrade;
        }

        void CreateCoin()
        {
            if (gameState != GameState.Started)
                return;

            MoveLeft newCoin = (coinQueue.Count > 0) ? coinQueue.Dequeue() : Instantiate(coinPrefab);
            newCoin.transform.position = new(9f, Random.Range(-4.5f, 4.5f));
            newCoin.AssignInfo(new(-9f, newCoin.transform.position.y), Random.Range(5f, 10f));
        }

        void CreateRock()
        {
            if (gameState != GameState.Started)
                return;

            MoveLeft newRock = (rockQueue.Count > 0) ? rockQueue.Dequeue() : Instantiate(rockPrefab);
            newRock.transform.position = new(9f, Random.Range(-4.5f, 4.5f));
            newRock.AssignInfo(new(-9f, newRock.transform.position.y), Random.Range(15f, 20f));
        }

        public void GameEnded()
        {
            gameTimer.Stop();
            gameState = GameState.Ended;
            MinigameManager.inst.MinigameEnd(CurrentGrade((float)gameTimer.Elapsed.TotalSeconds));
        }
    }
}