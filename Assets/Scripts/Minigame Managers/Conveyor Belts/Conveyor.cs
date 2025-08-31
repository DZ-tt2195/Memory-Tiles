using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [SerializeField] bool state;
    float speed = 1f;
    [SerializeField] GameObject bomb;

    private void Awake()
    {
        FlipConveyor();
    }

    public void ChangeConveyor()
    {
        state = !state;
        speed += 0.2f;
        FlipConveyor();
    }

    void FlipConveyor()
    {
        foreach (Transform child in this.transform)
        {
            if (state)
                child.transform.localEulerAngles = Vector3.zero;
            else
                child.transform.localEulerAngles = new(0, 0, 180);
        }
    }

    private void Update()
    {
        if (CurrentMinigame.instance.gameState == GameState.Started)
        {
            bomb.transform.Translate(Time.deltaTime * new Vector3(state ? speed : -speed, 0, 0));
            if (bomb.transform.position.x <= -8 || bomb.transform.position.x >= 8)
            {

            }
        }
    }
}
