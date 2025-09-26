using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class OrbManager : CurrentMinigame
{
    int currentNumber = 1;
    int currentGen = 1;
    bool toCollect = false;
    Queue<Orb> orbQueue = new();
    [SerializeField] Orb orbPrefab;

    private void Awake()
    {
        instance = this;
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
    }

    private void Update()
    {
        if (gameState == GameState.Started)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent<Orb>(out Orb touchedOrb))
                {
                    if (toCollect)
                    {
                        if (touchedOrb.number == currentNumber)
                        {
                            toCollect = false;
                            currentNumber++;
                            performanceSlider.value = (float)currentNumber / amazingGrade;

                            orbQueue.Enqueue(touchedOrb);
                            touchedOrb.gameObject.SetActive(false);

                            if (currentNumber >= amazingGrade)
                                GameOver();
                        }
                        else if (touchedOrb.number != 0)
                        {
                            GameOver();
                        }
                    }
                    else
                    {
                        if (touchedOrb.number == 0)
                        {
                            touchedOrb.transform.position = new(Random.Range(-6f, 6f), Random.Range(-1f, -3f));
                            toCollect = true;
                            StartCoroutine(GenerateOrbs());
                        }
                        else
                        {
                            GameOver();
                        }
                    }
                }
            }
        }
    }

    IEnumerator GenerateOrbs()
    {
        for (int i = 0; i < 2; i++)
        {
            Orb newOrb = (orbQueue.Count > 0) ? orbQueue.Dequeue() : Instantiate(orbPrefab);
            newOrb.gameObject.SetActive(true);
            newOrb.spriteRenderer.color = new(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f), 1);
            newOrb.transform.position = new(Random.Range(-6f, 6f), Random.Range(-2f, 4f));

            int newNumber = currentGen;
            newOrb.number = currentGen;
            newOrb.name = $"{newNumber}";
            currentGen++;

            yield return new WaitForSeconds(3.5f);
        }
    }

    void GameOver()
    {
        gameState = GameState.Ended;
        MinigameManager.inst.MinigameEnd(CurrentGrade(currentNumber));
    }
}
