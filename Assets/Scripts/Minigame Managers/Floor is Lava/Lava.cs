using UnityEngine;

namespace FloorIsLava
{
    public class Lava : MonoBehaviour
    {
        float timer;
        enum CurrentState { Inactive, Waiting, Deadly }
        CurrentState myState = CurrentState.Inactive;
        SpriteRenderer spriteRenderer;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.color = Color.blue;
            timer = Random.Range(0f, 2f);
        }

        void Update()
        {
            if (CurrentMinigame.instance.gameState == GameState.Started)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                {
                    switch (myState)
                    {
                        case CurrentState.Inactive:
                            if (Random.Range(0, 3) == 2)
                            {
                                myState = CurrentState.Waiting;
                                spriteRenderer.color = Color.yellow;
                                timer = 2f;
                            }
                            else
                            {
                                timer = Random.Range(0.25f, 2f);
                            }
                            break;
                        case CurrentState.Waiting:
                            myState = CurrentState.Deadly;
                            spriteRenderer.color = Color.red;
                            timer = Random.Range(1.5f, 3f);
                            break;
                        case CurrentState.Deadly:
                            myState = CurrentState.Inactive;
                            spriteRenderer.color = Color.blue;
                            timer = Random.Range(0.25f, 2f);
                            break;
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (myState == CurrentState.Deadly && collision.CompareTag("Player"))
            {
                FloorIsLava manager = (FloorIsLava)CurrentMinigame.instance;
                manager.GameEnded();
            }
        }
    }
}
