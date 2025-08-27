using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RocketDefense : CurrentMinigame
{
    Queue<Bullet> bulletQueue = new();
    [SerializeField] Bullet bulletPrefab;
    Rocket[] listOfRockets;

    private void Awake()
    {
        instance = this;
        listOfRockets = FindObjectsByType<Rocket>(FindObjectsSortMode.None);
        InvokeRepeating(nameof(CreateBullet), 0f, 1f);
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

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider != null && hit.collider.gameObject.TryGetComponent<Rocket>(out Rocket touchedRocket))
                    touchedRocket.ToggleBorder();
            }
        }
    }

    void CreateBullet()
    {
        if (gameState != GameState.Started)
            return;

        List<Rocket> remaining = RemainingRockets();
        if (remaining.Count >= 1)
        {
            Vector3 direction = listOfRockets[Random.Range(0, remaining.Count)].transform.position;
            direction.Normalize();

            Bullet newBullet = (bulletQueue.Count > 0) ? bulletQueue.Dequeue() : Instantiate(bulletPrefab);
            newBullet.AssignInfo(direction);
        }
        else
        {
            GameOver();
        }
    }

    List<Rocket> RemainingRockets()
    {
        List<Rocket> availableRockets = new();
        foreach (Rocket rocket in listOfRockets)
        {
            if (rocket.time > 0f)
                availableRockets.Add(rocket);
        }
        performanceSlider.value = (float)availableRockets.Count / amazingGrade;
        return availableRockets;
    }

    public void ReturnBullet(Bullet bullet)
    {
        bulletQueue.Enqueue(bullet);
        bullet.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        gameState = GameState.Ended;
        MinigameManager.inst.MinigameEnd(CurrentGrade(RemainingRockets().Count));
    }
}
