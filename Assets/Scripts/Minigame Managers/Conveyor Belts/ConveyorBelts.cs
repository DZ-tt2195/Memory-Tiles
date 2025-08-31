using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ConveyorBelts : CurrentMinigame
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
            {
                gameTimer.Stop();
                gameState = GameState.Ended;
                MinigameManager.inst.MinigameEnd(MinigameGrade.Amazing);
            }

            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider != null && hit.collider.gameObject.TryGetComponent<Conveyor>(out Conveyor touchedConveyor))
                    touchedConveyor.ChangeConveyor();
            }
        }
    }
}
