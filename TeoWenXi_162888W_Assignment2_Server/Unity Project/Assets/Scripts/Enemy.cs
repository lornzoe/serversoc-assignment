using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum AI_States
    {
        CHASE,
        RANDOM_MOVEMENT,
        RETURN_TO_POS
    }

    public float hp = 5;
    public float maxHp = 5;

    [HideInInspector]
    public Vector3 previousPos;
    [HideInInspector]
    public Vector3 targetPos;
    public float movementSpeed;
    public GameObject targetPlayer;

    public AI_States currentState;

    private void Start()
    {
        currentState = AI_States.RANDOM_MOVEMENT;
    }

    public void AIUpdate()
    {
        switch (currentState)
        {
            case AI_States.CHASE:
                {
                    Vector3 dir = targetPlayer.transform.position - transform.position;
                    dir.Normalize();
                    transform.position += dir * Time.deltaTime * movementSpeed;

                    if ((targetPlayer.transform.position - transform.position).magnitude > 10)
                        currentState = AI_States.RETURN_TO_POS;
                }
                break;
            case AI_States.RANDOM_MOVEMENT:
                {
                    if ((targetPos - transform.position).magnitude < 1)
                    {
                        targetPos = transform.position + new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10));
                    }
                    else
                    {
                        Vector3 dir = targetPos - transform.position;
                        dir.Normalize();
                        transform.position += dir * Time.deltaTime * movementSpeed;
                    }

                    float shortestDist = 10;
                    GameObject tempTargetPlayer = null;
                    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                    foreach (GameObject player in players)
                    {
                        float distance = (player.transform.position - transform.position).magnitude;
                        if (distance < shortestDist)
                        {
                            tempTargetPlayer = player;
                        }
                    }

                    if (tempTargetPlayer != null)
                    {
                        currentState = AI_States.CHASE;
                        targetPlayer = tempTargetPlayer;
                        previousPos = transform.position;
                    }
                }
                break;
            case AI_States.RETURN_TO_POS:
                {
                    Vector3 dir = previousPos - transform.position;
                    dir.Normalize();
                    transform.position += dir * Time.deltaTime * movementSpeed;

                    if ((previousPos - transform.position).magnitude < 1)
                        currentState = AI_States.RANDOM_MOVEMENT;
                }
                break;
        }
    }

    private void Update()
    {
        if (hp <= 0)
        {
            for(int i = 0; i < 2; ++i)
            {
                int j = Random.Range(0, 2);

                if (j == 0)
                    PhotonNetwork.Instantiate("Item1", transform.position + new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2)), Quaternion.identity, 0);
                else
                    PhotonNetwork.Instantiate("Item2", transform.position + new Vector3(Random.Range(0, 2), Random.Range(0, 2), Random.Range(0, 2)), Quaternion.identity, 0);
            }

            GetComponent<PhotonView>().RPC("ExplodusDeletus", PhotonTargets.All);
        }
    }

    public void HpCall()
    {
        GetComponent<PhotonView>().RPC("UpdateHp", PhotonTargets.All, new object[] { hp });
    }

    [PunRPC]
    public void UpdateHp(float _hp, PhotonMessageInfo info)
    {
        string test = GetComponent<PhotonView>().owner.NickName;
        Debug.Log(test);
        //Debug.Log(string.Format("Info: {0} {1} {2}", info.sender, info.photonView, info.timestamp));
        if (PhotonNetwork.isMasterClient || GetComponent<PhotonView>().isMine)
        {
            hp = _hp;
        }
    }

    [PunRPC]
    public void ExplodusDeletus(PhotonMessageInfo info)
    {
        //Debug.Log(string.Format("Info: {0} {1} {2}", info.sender, info.photonView, info.timestamp));
        if (PhotonNetwork.isMasterClient || GetComponent<PhotonView>().isMine)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
