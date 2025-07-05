using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class CurrentMinigame : MonoBehaviour
{
    public static CurrentMinigame instance;

    [SerializeField] protected float amazingGrade;
    [SerializeField] protected float goodGrade;
    [SerializeField] protected float barelyGrade;
    [SerializeField] protected float failGrade;

    [SerializeField] protected Slider performanceSlider;
    [SerializeField] Image markerPrefab;

    public virtual void StartMinigame()
    {
        RectTransform rect = performanceSlider.GetComponent<RectTransform>();
        PlaceMarker(amazingGrade / failGrade);
        PlaceMarker(goodGrade / failGrade);
        PlaceMarker(barelyGrade / failGrade);

        void PlaceMarker(float percent)
        {
            Image newMarker = Instantiate(markerPrefab);
            newMarker.transform.SetParent(performanceSlider.transform);

            float position = rect.sizeDelta.x * percent;
            newMarker.transform.localPosition = new(position - (rect.sizeDelta.x/2f), 0);
        }
    }

    protected MinigameGrade CurrentGrade(float score)
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

    protected string StopwatchTime(Stopwatch stopwatch)
    {
        if (stopwatch == null)
        {
            return "0:00.000";
        }
        else
        {
            TimeSpan time = stopwatch.Elapsed;
            string part = time.Seconds < 10 ? $"0{time.Seconds}" : $"{time.Seconds}";
            return $"{time.Minutes}:{part}.{time.Milliseconds}";
        }
    }

}
