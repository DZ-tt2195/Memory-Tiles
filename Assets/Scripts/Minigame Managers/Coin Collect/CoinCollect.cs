using System;
using System.Collections.Generic;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace CoinCollect
{
    public class CoinCollect : CurrentMinigame
    {
        int score = 0;
        int currentLevel = -1;
        [SerializeField] TMP_Text textBox;

        private void Awake()
        {
            instance = this;
        }

        public void ChangeScore(int change)
        {
            score = Mathf.Max(0, score + change);
            performanceSlider.value = (float)score / 20;
            textBox.text = $"{score}";
        }

        public void NextLevel()
        {
            currentLevel++;
            for (int i = 0; i < this.transform.childCount; i++)
                this.transform.GetChild(i).gameObject.SetActive(i == currentLevel);

            if (currentLevel == this.transform.childCount)
            {
                gameCompleted = true;
                MinigameManager.inst.MinigameEnd(CurrentGrade((float)score));
            }
        }

        protected override MinigameGrade CurrentGrade(float score)
        {
            if (score >= amazingGrade)
                return MinigameGrade.Amazing;
            else if (score >= goodGrade && score <= amazingGrade)
                return MinigameGrade.Good;
            else if (score >= barelyGrade && score <= goodGrade)
                return MinigameGrade.Barely;
            else
                return MinigameGrade.Failed;
        }

        public override void StartMinigame()
        {
            base.StartMinigame();
            PlaceMarker(amazingGrade / 20);
            PlaceMarker(goodGrade / 20);
            PlaceMarker(barelyGrade / 20);
        }
    }
}