using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetThirdPersonNetwork : Photon.MonoBehaviour
{
    PetFollow petScript;
    private bool appliedInitialUpdate;
    private Vector3 correctPetPos = Vector3.zero; //We lerp towards this
    private Quaternion correctPetRot = Quaternion.identity; //We lerp towards this

    void Awake()
    {
        petScript = GetComponent<PetFollow>();

    }
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.isMine)
        {
            petScript.enabled = true;
        }
        else
        {
            petScript.enabled = false;

        }

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

        }
        else
        {
            //Network player, receive data
            correctPetPos = (Vector3)stream.ReceiveNext();
            correctPetRot = (Quaternion)stream.ReceiveNext();
            GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPetPos;
                transform.rotation = correctPetRot;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.isMine)
        {
            //Update remote player (smooth this, this looks good, at the cost of some accuracy)
            transform.position = Vector3.Lerp(transform.position, correctPetPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, correctPetRot, Time.deltaTime * 5);
        }
    }
}
