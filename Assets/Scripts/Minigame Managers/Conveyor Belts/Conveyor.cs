using UnityEngine;

public class Conveyor : MonoBehaviour
{
    [SerializeField] bool state;
    [SerializeField] float speed;
    [SerializeField] GameObject bomb;
    ConveyorBelts manager;

    private void Start()
    {
        FlipConveyor();
        manager = (ConveyorBelts)CurrentMinigame.instance;
    }

    public void ChangeConveyor()
    {
        state = !state;
        speed += 0.25f;
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
        if (manager.gameState == GameState.Started)
        {
            bomb.transform.Translate(Time.deltaTime * new Vector3(state ? speed : -speed, 0, 0));
            if (Mathf.Abs(bomb.transform.position.x) >= (this.transform.localScale.x / 2f))
                manager.GameEnded();
        }
    }
}
