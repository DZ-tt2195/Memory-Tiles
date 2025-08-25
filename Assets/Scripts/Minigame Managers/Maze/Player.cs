using UnityEngine;

namespace Maze
{
    public class Player : MonoBehaviour
    {
        Rigidbody2D rb;
        Maze manager;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            manager = (Maze)CurrentMinigame.instance;
        }

        private void FixedUpdate()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            Vector2 movement = new Vector2(moveX, moveY);
            rb.MovePosition(rb.position + 7.5f * Time.fixedDeltaTime * movement);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Coin"))
            {
                manager.GotCoin(collision.gameObject);
            }
            else if (collision.CompareTag("Death"))
            {
                this.transform.position = Vector3.zero;
            }
        }
    }
}