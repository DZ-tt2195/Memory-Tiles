using UnityEngine;
using UnityEngine.UI;

public class ClickToWin : MonoBehaviour
{
    void Start()
    {
        this.GetComponent<Button>().onClick.AddListener(Debug);

        void Debug()
        {
            UnityEngine.Debug.Log($"clicked {this.name}");
        }
    }
}
