using UnityEngine;

namespace DolphinJump
{
    public class MoveLeft : MonoBehaviour
    {
        enum Thing { Coin, Wall }
        [SerializeField] Thing thing;
        Vector2 target;
        float speed;

        public void AssignInfo(Vector2 target, float speed)
        {
            this.target = target;
            this.speed = speed;
            this.transform.position = Vector3.zero;
            this.gameObject.SetActive(true);
        }

        private void Update()
        {
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (this.transform.position.x <= -8f)
            {
                DolphinJump manager = (DolphinJump)CurrentMinigame.instance;
                manager.ReturnToQueue(this, thing == Thing.Coin, false);
            }
        }
    }
}