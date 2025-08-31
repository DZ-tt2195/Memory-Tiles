using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] List<Vector2> travelPoints = new();
    [SerializeField] float speed;
    private int currentTarget = 0;

    private void Start()
    {
        if (travelPoints.Count >= 1)
            this.transform.position = travelPoints[0];
    }

    void Update()
    {
        if (travelPoints.Count >= 1)
        {
            Vector2 target = travelPoints[currentTarget];
            transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, target) < 0.05f)
                currentTarget = (currentTarget + 1) % travelPoints.Count;
        }
    }
}
