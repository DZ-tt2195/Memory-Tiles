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
                rb.gravityScale = 2.5f;
                float moveX = Input.GetAxis("Horizontal") * 10f;
                rb.velocity = new Vector2(moveX, rb.velocity.y);
            }
            else
            {
                rb.gravityScale = 0;
                rb.velocity = Vector3.zero;
            }

            if (transform.position.y <= -4.5f)
                manager.GameEnded();
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (rb.velocity.y < 0 && collision.CompareTag("Platform"))
            {
                rb.velocity = new Vector2(rb.velocity.x, 12f);
                manager.ReturnPlatform(collision.gameObject);
            }
            else if (collision.CompareTag("Death"))
            {
                manager.GameEnded();
            }
        }
    }
}