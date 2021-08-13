using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsThirdPersonNetwork : Photon.MonoBehaviour
{
    private ItemData itemScript;
    private bool appliedInitialUpdate;
    private Vector3 correctPos = Vector3.zero; //We lerp towards this
    private Quaternion correctRot = Quaternion.identity; //We lerp towards this
    private bool isDone = false;

    void Awake()
    {
        itemScript = GetComponent<ItemData>();

    }
    // Start is called before the first frame update
    void Start()
    {
        itemScript.enabled = true;

        gameObject.name = gameObject.name + photonView.viewID;
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(GetComponent<Rigidbody>().velocity);
            stream.SendNext(GetComponent<ItemData>().attack);
            stream.SendNext(GetComponent<ItemData>().isDone);
        }
        else
        {
            //Network player, receive data
            correctPos = (Vector3)stream.ReceiveNext();
            correctRot = (Quaternion)stream.ReceiveNext();
            GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();
            GetComponent<ItemData>().attack = (float)stream.ReceiveNext();
            GetComponent<ItemData>().isDone = (bool)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPos;
                transform.rotation = correctRot;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<ItemData>().attack = 0;
                GetComponent<ItemData>().isDone = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctRot, Time.deltaTime * 5);
            GetComponent<ItemData>().isDone = isDone;
        }
    }
}
