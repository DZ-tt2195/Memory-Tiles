using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class CanvasOverlay : MonoBehaviour
{
    [SerializeField] List<GameObject> disableIfNotScene = new();
    [SerializeField] List<CanvasGroup> listOfGroups;
    Scene myScene;

    void Awake()
    {
        myScene = this.gameObject.scene;
    }

    void Update()
    {
        foreach (GameObject next in disableIfNotScene)
        {
            next.SetActive(!ThisIsActiveScene());
            next.transform.SetAsLastSibling();
        }
        foreach (CanvasGroup group in listOfGroups)
        {
            group.alpha = ThisIsActiveScene() ? 1 : 0.25f;
            group.blocksRaycasts = ThisIsActiveScene();
        }
    }

    bool ThisIsActiveScene()
    {
        return myScene == SceneManager.GetActiveScene();
    }
}
