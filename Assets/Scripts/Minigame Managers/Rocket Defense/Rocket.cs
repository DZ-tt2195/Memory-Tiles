using UnityEngine;
using TMPro;

public class Rocket : MonoBehaviour
{
    [SerializeField] GameObject border;
    [SerializeField] TMP_Text timeText;
    RocketDefense manager;
    public float time { get; private set; }

    private void Start()
    {
        time = 15f;
        manager = (RocketDefense)CurrentMinigame.instance;
        border.SetActive(false);
    }

    private void Update()
    {
        if (!border.activeInHierarchy && manager.gameState == GameState.Started)
        {
            time = Mathf.Max(0, time - Time.deltaTime);
            timeText.text = $"{time:F1}";
        }
    }

    public void ToggleBorder()
    {
        border.SetActive(!border.activeInHierarchy);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent<Bullet>(out Bullet touchedBullet))
        {
            manager.ReturnBullet(touchedBullet);
            if (!border.activeInHierarchy)
                manager.GameOver();
        }
    }
}
