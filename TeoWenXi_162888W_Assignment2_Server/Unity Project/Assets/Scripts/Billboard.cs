using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.Find("Text").GetComponent<Text>().text = transform.parent.GetComponent<PhotonView>().owner.NickName;
        transform.parent.name = transform.parent.GetComponent<PhotonView>().owner.NickName + "Avatar";
    }

    // Update is called once per frame
    void Update()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        transform.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.down);
    }
}
