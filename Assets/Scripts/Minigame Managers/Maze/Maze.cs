using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace Maze
{
    public class Maze : CurrentMinigame
    {
        GameObject[] allCoins;
        Stopwatch gameTimer;

        private void Awake()
        {
            instance = this;
            gameTimer = new Stopwatch();
            allCoins = GameObject.FindGameObjectsWithTag("Coin");
        }

        public void GotCoin(GameObject coin)
        {
            if (gameTimer.IsRunning)
            {
                coin.SetActive(false);
                bool anyLeft = false;
                foreach (GameObject checkCoin in allCoins)
                {
                    if (checkCoin.activeSelf)
                        anyLeft = true;
                }

                if (!anyLeft)
                {
                    gameTimer.Stop();
                    gameCompleted = true;
                    MinigameManager.inst.MinigameEnd(CurrentGrade((float)gameTimer.Elapsed.TotalSeconds));
                }
            }
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
        }

        private void Update()
        {
            if (gameTimer.IsRunning)
            {
                performanceSlider.value = (float)gameTimer.Elapsed.TotalSeconds / barelyGrade;
                if (performanceSlider.value == 1)
                {
                    gameTimer.Stop();
                    gameCompleted = true;
                    MinigameManager.inst.MinigameEnd(MinigameGrade.Failed);
                }
            }
        }

    }
}