using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLinking2 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataPasser.Instance.friendListCanvas = gameObject;
        gameObject.SetActive(false);
    }
}
