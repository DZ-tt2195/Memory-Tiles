using System.Diagnostics;
using UnityEngine;

namespace FloorIsLava
{
    public class FloorIsLava : CurrentMinigame
    {
        Stopwatch gameTimer;

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

        public void GameEnded()
        {
            gameTimer.Stop();
            gameState = GameState.Ended;
            MinigameManager.inst.MinigameEnd(CurrentGrade((float)gameTimer.Elapsed.TotalSeconds));
        }
    }
}
