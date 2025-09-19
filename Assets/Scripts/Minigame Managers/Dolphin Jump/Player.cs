using UnityEngine;

namespace DolphinJump
{
    public class Player : MonoBehaviour
    {
        enum WaterState { AboveWater, BelowWater }
        WaterState state;
        Rigidbody2D rb;
        DolphinJump manager;
        float lowestPoint = 0f;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            manager = (DolphinJump)CurrentMinigame.instance;
        }

        private void FixedUpdate()
        {
            if (manager.gameState == GameState.Started)
            {
                float moveX = Input.GetAxis("Horizontal") * 10f;
                float moveY = Input.GetAxis("Vertical") * 10f;

                WaterState newState = (transform.position.y >= 0) ? WaterState.AboveWater : WaterState.BelowWater;
                rb.gravityScale = (newState == WaterState.AboveWater) ? 3f : 0f;

                if (newState == WaterState.AboveWater && state == WaterState.BelowWater)
                {
                    rb.velocity = new Vector2(moveX, Mathf.Abs(lowestPoint*4.2f));
                    Debug.Log($"set velocity to {lowestPoint} -> {rb.velocity.y}");
                }
                else if (transform.position.y >= 0)
                {
                    rb.velocity = new Vector2(moveX, rb.velocity.y);
                }
                else
                {
                    if (moveY <= 0)
                        lowestPoint = Mathf.Abs(this.transform.position.y);
                    rb.velocity = new Vector2(moveX, moveY);
                }
                state = newState;
            }
            else
            {
                rb.gravityScale = 0;
                rb.velocity = Vector3.zero;
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Coin"))
            {
                manager.ReturnToQueue(collision.gameObject, true, true);
            }
            else if (collision.CompareTag("Death"))
            {
                manager.ReturnToQueue(collision.gameObject, false, true);
            }
        }
    }
}