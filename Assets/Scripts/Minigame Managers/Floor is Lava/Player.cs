using UnityEngine;

namespace FloorIsLava
{
    public class Player : MonoBehaviour
    {
        Rigidbody2D rb;
        FloorIsLava manager;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            manager = (FloorIsLava)CurrentMinigame.instance;
        }

        private void FixedUpdate()
        {
            if (manager.gameState == GameState.Started)
            {
                float moveX = Input.GetAxis("Horizontal");
                float moveY = Input.GetAxis("Vertical");

                Vector2 movement = new Vector2(moveX, moveY);
                rb.MovePosition(rb.position + 7.5f * Time.fixedDeltaTime * movement);
            }
        }
    }
}