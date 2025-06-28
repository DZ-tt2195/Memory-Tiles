using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GenerateSquares : MonoBehaviour
{
    [SerializeField] GameObject square;
    List<GameObject> listOfObjects = new();

    void Start()
    {
        for (float i = -7.5f; i<=7.5f; i+=1.5f)
        {
            for (float j = -4f; j<= 2f; j+=1.5f)
            {
                GameObject newSquare = Instantiate(square);
                listOfObjects.Add(newSquare);
                newSquare.transform.position = new(i, j);
            }
        }
    }
}
