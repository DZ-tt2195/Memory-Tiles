using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected Vector3 direction;

    public void AssignInfo(Vector3 direction)
    {
        this.direction = direction;
        this.transform.position = Vector3.zero;
        this.gameObject.SetActive(true);
    }

    private void Update()
    {
        this.transform.Translate(3f * Time.deltaTime * direction, Space.World);
    }
}
