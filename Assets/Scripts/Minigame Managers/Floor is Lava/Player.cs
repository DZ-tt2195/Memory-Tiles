using UnityEngine;

namespace FloorIsLava
{
    public class Player : MonoBehaviour
    {
        Rigidbody2D rb;
        FloorIsLava manager;
        private Vector2 inputMovement;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            manager = (FloorIsLava)CurrentMinigame.instance;
        }

        void Update()
        {
            if (manager.gameState == GameState.Started)
            {
                float moveX = Input.GetAxis("Horizontal");
                float moveY = Input.GetAxis("Vertical");
                inputMovement = new Vector2(moveX, moveY);
            }
            else
            {
                inputMovement = Vector2.zero;
            }
        }

        private void FixedUpdate()
        {
            if (manager.gameState == GameState.Started)
            {
                rb.MovePosition(rb.position + 7.5f * Time.fixedDeltaTime * inputMovement);
            }
        }
    }
}