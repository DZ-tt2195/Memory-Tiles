using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class CanvasOverlay : MonoBehaviour
{
    [SerializeField] List<GameObject> disableIfNotScene = new();
    Scene myScene;

    void Awake()
    {
        myScene = this.gameObject.scene;
    }

    void Update()
    {
        foreach (GameObject next in disableIfNotScene)
        {
            next.SetActive(myScene != SceneManager.GetActiveScene());
            next.transform.SetAsLastSibling();
        }
    }
}
