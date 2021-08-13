using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetFollow : MonoBehaviour
{
    public float distanceLimit, movementSpeed;
    public float followRadius;
    public float yOffsetAbovePlayer;
    public GameObject player;

    private Vector3 vel;
    public float frictionPower;
    void Start()
    {
        player = DataPasser.Instance.character;
    }

    // Update is called once per frame
    void Update()
    {
        if(!player)
            player = DataPasser.Instance.character;

        float dist = (player.transform.position - transform.position).magnitude;

        if(dist > distanceLimit)
        {
            transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

            if (transform.position.y != player.transform.position.y + yOffsetAbovePlayer)
            {
                vel.y = (player.transform.position.y + yOffsetAbovePlayer) - transform.position.y;
            }

            Vector3 playerOffset = player.transform.position;
            playerOffset.y = player.transform.position.y + yOffsetAbovePlayer;

            if (Vector3.Distance(playerOffset, transform.position) > followRadius)
            {
                vel = playerOffset - transform.position;
            }
            else
            {
                vel *= frictionPower;

                if (vel.x > -0.25f && vel.x < 0.25f && vel.z > -0.25f && vel.z < 0.25f && vel.y > -0.25f && vel.y < 0.25f)
                    vel = new Vector3(0, 0, 0);
            }

            GetComponent<Rigidbody>().velocity = vel * movementSpeed * Time.deltaTime;
        }
        else
        {
            GetComponent<Rigidbody>().velocity *= 0.5f;
        }
    }
}
