using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Slider>().value = transform.parent.parent.GetComponent<Enemy>().hp / transform.parent.parent.GetComponent<Enemy>().maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        transform.parent.LookAt(transform.position + camera.transform.rotation * Vector3.back, camera.transform.rotation * Vector3.down);

        GetComponent<Slider>().value = transform.parent.parent.GetComponent<Enemy>().hp / transform.parent.parent.GetComponent<Enemy>().maxHp;
    }
}
