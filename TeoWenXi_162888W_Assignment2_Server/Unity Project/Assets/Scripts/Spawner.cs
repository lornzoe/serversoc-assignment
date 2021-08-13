using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject enemyObject;
    public int totalMobs;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.isMasterClient)
        {
            int monsterCount = 0;
            GameObject[] list = GameObject.FindGameObjectsWithTag("EnemyParentObject");
            foreach (GameObject obj in list)
            {
                monsterCount++;
            }

            if(monsterCount < totalMobs)
            {
                int toSpawn = totalMobs - monsterCount;
                for (int i = 0; i < toSpawn; ++i)
                {
                    GameObject newObj = PhotonNetwork.Instantiate("Enemy", transform.position, Quaternion.identity, 0);
                    newObj.transform.position = new Vector3(52 + Random.Range(-3, 3), 4, 46 + Random.Range(-3, 3));
                }
            }
        }
    }
}
