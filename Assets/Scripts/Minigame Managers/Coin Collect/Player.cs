using UnityEngine;

namespace CoinCollect
{     
    public class Player : MonoBehaviour
    {
        bool moving = false;
        Vector2 direction = Vector2.left;
        CoinCollect manager;

        private void Start()
        {
            manager = (CoinCollect)CurrentMinigame.instance;
        }

        private void Update()
        {
            if (!manager.gameCompleted && !moving && Input.GetKeyDown(KeyCode.Space))
                moving = true;

            if (moving)
                this.transform.Translate(10f * Time.deltaTime * direction);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Trigger"))
            {
                moving = false;
                manager.NextLevel();
                if (direction == Vector2.right)
                    direction = Vector2.left;
                else
                    direction = Vector2.right;
            }
            else if (collision.CompareTag("Coin"))
            {
                manager.ChangeScore(1);
                collision.gameObject.SetActive(false);
            }
            else if (collision.CompareTag("Death"))
            {
                manager.ChangeScore(-1);
                collision.gameObject.SetActive(false);
            }
        }
    }
}