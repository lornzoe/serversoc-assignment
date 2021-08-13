using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyThirdPersonNetwork : Photon.MonoBehaviour
{
    private Enemy enemyScript;
    private bool appliedInitialUpdate;
    private Vector3 correctPos = Vector3.zero; //We lerp towards this
    private Quaternion correctRot = Quaternion.identity; //We lerp towards this
    private float correctHp;

    void Awake()
    {
        enemyScript = GetComponent<Enemy>();

    }
    // Start is called before the first frame update
    void Start()
    {
        if (photonView.isMine)
        {
            enemyScript.enabled = true;
        }
        else
        {
            enemyScript.enabled = false;

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
            stream.SendNext(GetComponent<Enemy>().hp);
        }
        else
        {
            //Network player, receive data
            correctPos = (Vector3)stream.ReceiveNext();
            correctRot = (Quaternion)stream.ReceiveNext();
            GetComponent<Rigidbody>().velocity = (Vector3)stream.ReceiveNext();
            correctHp = (float)stream.ReceiveNext();

            if (!appliedInitialUpdate)
            {
                appliedInitialUpdate = true;
                transform.position = correctPos;
                transform.rotation = correctRot;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                GetComponent<Enemy>().hp = correctHp;
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
            GetComponent<Enemy>().hp = correctHp;
        }
    }
}
