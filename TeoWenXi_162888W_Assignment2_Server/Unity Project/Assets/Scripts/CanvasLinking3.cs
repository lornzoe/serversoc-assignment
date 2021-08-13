using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLinking3 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DataPasser.Instance.gamesparksCanvas = gameObject;
        gameObject.SetActive(false);
    }
}
