using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class CurrentMinigame : MonoBehaviour
{
    public static CurrentMinigame instance;
    public bool gameCompleted { get; protected set; }

    [SerializeField] protected float amazingGrade;
    [SerializeField] protected float goodGrade;
    [SerializeField] protected float barelyGrade;

    protected Slider performanceSlider;
    Image markerPrefab;
    RectTransform rect;

    public virtual void StartMinigame()
    {
        performanceSlider = GameObject.Find("Performance Slider").GetComponent<Slider>();
        markerPrefab = Resources.Load<Image>("Marker").GetComponent<Image>();
        rect = performanceSlider.GetComponent<RectTransform>();
    }

    protected void PlaceMarker(float percent)
    {
        Image newMarker = Instantiate(markerPrefab);
        newMarker.transform.SetParent(performanceSlider.transform);

        float position = rect.sizeDelta.x * percent;
        newMarker.transform.localPosition = new(position - (rect.sizeDelta.x / 2f), 0);
    }

    protected virtual MinigameGrade CurrentGrade(float score)
    {
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
