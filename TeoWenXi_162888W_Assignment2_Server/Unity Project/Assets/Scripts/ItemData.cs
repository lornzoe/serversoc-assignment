using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemData : MonoBehaviour
{
    public string itemName;
    [HideInInspector]
    public float attack;
    [HideInInspector]
    public bool isDone;

    private void Awake()
    {
        attack = Random.Range(0, 61);
    }

    private void Update()
    {
        if(isDone)
        {
            if (PhotonNetwork.isMasterClient)
                CallDestroy();
            else
                CallDestroyMaster();
        }
    }

    public void CallDestroyMaster()
    {
        GetComponent<PhotonView>().RPC("DestroyRPC", PhotonTargets.MasterClient);
    }

    public void CallDestroy()
    {
        GetComponent<PhotonView>().RPC("DestroyRPC", PhotonTargets.All);
    }

    [PunRPC]
    void DestroyRPC(PhotonMessageInfo info)
    {
        Debug.Log(string.Format("Info: {0} {1} {2}", info.sender, info.photonView, info.timestamp));
        if (GetComponent<PhotonView>().isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
