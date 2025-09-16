using UnityEngine;

namespace CloudAttack
{
    public class Player : MonoBehaviour
    {
        Rigidbody2D rb;
        CloudBounce manager;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            manager = (CloudBounce)CurrentMinigame.instance;
        }

        private void FixedUpdate()
        {
            if (manager.gameState == GameState.Started)
            {
                rb.gravityScale = 3f;
                float moveX = Input.GetAxis("Horizontal") * 7.5f;
                rb.velocity = new Vector2(moveX, rb.velocity.y);
            }
            else
            {
                rb.gravityScale = 0;
            }
        }
    }
}