using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPartitioning : MonoBehaviour
{
    public float mapSizeX, mapSizeZ;
    public int tileNumX, tileNumZ;
    private float tileSizeX, tileSizeZ;

    public GameObject cube;
    // Start is called before the first frame update
    void Start()
    {
        tileSizeX = mapSizeX / tileNumX;
        tileSizeZ = mapSizeZ / tileNumZ;

        for(int i = 0; i < tileNumX; ++i)
        {
            for (int j = 0; j < tileNumZ; ++j)
            {
                GameObject newObject = Instantiate(cube);
                newObject.transform.localScale = new Vector3(tileSizeX, 10, tileSizeZ);
                newObject.transform.position = new Vector3(tileSizeX * i + (tileSizeX * 0.5f), 0, tileSizeZ * j + (tileSizeZ * 0.5f));
                newObject.name = "" + (i * tileNumX + j);
                newObject.transform.parent = gameObject.transform;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            int xIndex = (int)(player.transform.position.x / tileSizeX);
            int zIndex = (int)(player.transform.position.z / tileSizeZ);

            player.transform.parent = gameObject.transform.Find("" + (xIndex * tileNumX + zIndex));
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyParentObject");
        foreach (GameObject enemy in enemies)
        {
            int xIndex = (int)(enemy.transform.position.x / tileSizeX);
            int zIndex = (int)(enemy.transform.position.z / tileSizeZ);

            enemy.transform.parent = gameObject.transform.Find("" + (xIndex * tileNumX + zIndex));
        }

        GameObject[] items = GameObject.FindGameObjectsWithTag("ItemParentObject");
        foreach (GameObject item in items)
        {
            int xIndex = (int)(item.transform.position.x / tileSizeX);
            int zIndex = (int)(item.transform.position.z / tileSizeZ);

            item.transform.parent = gameObject.transform.Find("" + (xIndex * tileNumX + zIndex));
        }

        GameObject[] pets = GameObject.FindGameObjectsWithTag("Pet");
        foreach (GameObject pet in pets)
        {
            int xIndex = (int)(pet.transform.position.x / tileSizeX);
            int zIndex = (int)(pet.transform.position.z / tileSizeZ);

            pet.transform.parent = gameObject.transform.Find("" + (xIndex * tileNumX + zIndex));
        }

        if (PhotonNetwork.isMasterClient)
        {
            //foreach (Transform t in gameObject.transform)
            //{
            //    bool hasObj = false;
            //    foreach (Transform f in t)
            //    {
            //        t.gameObject.SetActive(true);
            //        hasObj = true;
            //        break;
            //    }

            //    if (hasObj == false)
            //        t.gameObject.SetActive(false);
            //}

            if (DataPasser.Instance.character)
            {
                GameObject playerObject = DataPasser.Instance.character;
                int xIndex = (int)(playerObject.transform.position.x / tileSizeX);
                int zIndex = (int)(playerObject.transform.position.z / tileSizeZ);

                List<int> tempList = new List<int>();

                //Vision Range
                for (int i = 0; i < playerObject.GetComponent<PlayerData>().visionRange; ++i)
                {
                    for (int j = 0; j < playerObject.GetComponent<PlayerData>().visionRange; ++j)
                    {
                        int tempXIndex = (int)(xIndex - (playerObject.GetComponent<PlayerData>().visionRange * 0.5f));
                        int tempZIndex = (int)(zIndex - (playerObject.GetComponent<PlayerData>().visionRange * 0.5f));

                        tempXIndex += i + 1;
                        tempZIndex += j + 1;

                        tempList.Add(tempXIndex * tileNumX + tempZIndex);
                    }
                }

                foreach (Transform t in gameObject.transform)
                {
                    bool visioned = false;
                    for(int p = 0; p < tempList.Count; ++p)
                    {
                        if (t.name == "" + tempList[p])
                        {
                            visioned = true;
                            foreach (Transform f in t)
                            {
                                switch (f.tag)
                                {
                                    case "EnemyParentObject":
                                        f.GetComponent<MeshRenderer>().enabled = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(true);
                                        }
                                        break;
                                    case "ItemParentObject":
                                        f.GetComponent<MeshRenderer>().enabled = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(true);
                                        }
                                        break;
                                    case "Player":
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(true);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    if(visioned == false)
                    {
                        foreach (Transform f in t)
                        {
                            switch (f.tag)
                            {
                                case "EnemyParentObject":
                                    f.GetComponent<MeshRenderer>().enabled = false;
                                    foreach (Transform g in f)
                                    {
                                        g.gameObject.SetActive(false);
                                    }
                                    break;
                                case "ItemParentObject":
                                    f.GetComponent<MeshRenderer>().enabled = false;
                                    foreach (Transform g in f)
                                    {
                                        g.gameObject.SetActive(false);
                                    }
                                    break;
                                case "Player":
                                    foreach (Transform g in f)
                                    {
                                        g.gameObject.SetActive(false);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        else
        {
            if (DataPasser.Instance.character)
            {
                GameObject playerObject = DataPasser.Instance.character;
                int xIndex = (int)(playerObject.transform.position.x / tileSizeX);
                int zIndex = (int)(playerObject.transform.position.z / tileSizeZ);

                List<int> visionRange = new List<int>();
                List<int> interestRange = new List<int>();

                //Interest Range
                for (int i = 0; i < playerObject.GetComponent<PlayerData>().interestRange; ++i)
                {
                    for (int j = 0; j < playerObject.GetComponent<PlayerData>().interestRange; ++j)
                    {
                        int tempXIndex = (int)(xIndex - (playerObject.GetComponent<PlayerData>().interestRange * 0.5f));
                        int tempZIndex = (int)(zIndex - (playerObject.GetComponent<PlayerData>().interestRange * 0.5f));

                        tempXIndex += i + 1;
                        tempZIndex += j + 1;

                        interestRange.Add(tempXIndex * tileNumX + tempZIndex);
                    }
                }

                //Vision Range
                for (int i = 0; i < playerObject.GetComponent<PlayerData>().visionRange; ++i)
                {
                    for (int j = 0; j < playerObject.GetComponent<PlayerData>().visionRange; ++j)
                    {
                        int tempXIndex = (int)(xIndex - (playerObject.GetComponent<PlayerData>().visionRange * 0.5f));
                        int tempZIndex = (int)(zIndex - (playerObject.GetComponent<PlayerData>().visionRange * 0.5f));

                        tempXIndex += i + 1;
                        tempZIndex += j + 1;

                        visionRange.Add(tempXIndex * tileNumX + tempZIndex);
                    }
                }

                foreach (Transform t in gameObject.transform)
                {
                    bool visioned = false;
                    bool interested = false;

                    for(int p = 0; p < visionRange.Count; ++p)
                    {
                        if(t.name == "" + visionRange[p])
                        {
                            visioned = true;
                            foreach (Transform f in t)
                            {
                                switch (f.tag)
                                {
                                    case "EnemyParentObject":
                                        f.GetComponent<MeshRenderer>().enabled = true;
                                        f.GetComponent<BoxCollider>().enabled = true;
                                        f.GetComponent<EnemyThirdPersonNetwork>().enabled = true;
                                        f.GetComponent<Rigidbody>().useGravity = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(true);
                                        }
                                        break;
                                    case "ItemParentObject":
                                        f.GetComponent<MeshRenderer>().enabled = true;

                                        if (f.GetComponent<SphereCollider>())
                                            f.GetComponent<SphereCollider>().enabled = true;
                                        else if (f.GetComponent<CapsuleCollider>())
                                            f.GetComponent<CapsuleCollider>().enabled = true;

                                        f.GetComponent<ItemsThirdPersonNetwork>().enabled = true;
                                        f.GetComponent<Rigidbody>().useGravity = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(true);
                                        }
                                        break;
                                    case "Player":
                                        f.GetComponent<CapsuleCollider>().enabled = true;
                                        f.GetComponent<ThirdPersonNetworkVik>().enabled = true;
                                        f.GetComponent<Rigidbody>().useGravity = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(true);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    for (int p = 0; p < interestRange.Count; ++p)
                    {
                        if (t.name == "" + interestRange[p] && visioned == false)
                        {
                            interested = true;
                            foreach (Transform f in t)
                            {
                                switch (f.tag)
                                {
                                    case "EnemyParentObject":
                                        f.GetComponent<MeshRenderer>().enabled = false;
                                        f.GetComponent<BoxCollider>().enabled = true;
                                        f.GetComponent<EnemyThirdPersonNetwork>().enabled = true;
                                        f.GetComponent<Rigidbody>().useGravity = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(false);
                                        }
                                        break;
                                    case "ItemParentObject":
                                        f.GetComponent<MeshRenderer>().enabled = false;

                                        if (f.GetComponent<SphereCollider>())
                                            f.GetComponent<SphereCollider>().enabled = true;
                                        else if (f.GetComponent<CapsuleCollider>())
                                            f.GetComponent<CapsuleCollider>().enabled = true;

                                        f.GetComponent<ItemsThirdPersonNetwork>().enabled = true;
                                        f.GetComponent<Rigidbody>().useGravity = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(false);
                                        }
                                        break;
                                    case "Player":
                                        f.GetComponent<CapsuleCollider>().enabled = true;
                                        f.GetComponent<ThirdPersonNetworkVik>().enabled = true;
                                        f.GetComponent<Rigidbody>().useGravity = true;
                                        foreach (Transform g in f)
                                        {
                                            g.gameObject.SetActive(false);
                                        }
                                        break;
                                }
                            }
                        }
                    }

                    if(visioned == false && interested == false)
                    {
                        foreach (Transform f in t)
                        {
                            switch (f.tag)
                            {
                                case "EnemyParentObject":
                                    f.GetComponent<EnemyThirdPersonNetwork>().enabled = false;
                                    f.GetComponent<MeshRenderer>().enabled = false;
                                    f.GetComponent<BoxCollider>().enabled = false;
                                    f.GetComponent<Rigidbody>().useGravity = false;
                                    foreach (Transform g in f)
                                    {
                                        g.gameObject.SetActive(false);
                                    }
                                    break;
                                case "ItemParentObject":
                                    f.GetComponent<ItemsThirdPersonNetwork>().enabled = false;

                                    f.GetComponent<MeshRenderer>().enabled = false;

                                    if (f.GetComponent<SphereCollider>())
                                        f.GetComponent<SphereCollider>().enabled = false;
                                    else if (f.GetComponent<CapsuleCollider>())
                                        f.GetComponent<CapsuleCollider>().enabled = false;

                                    f.GetComponent<Rigidbody>().useGravity = false;
                                    foreach (Transform g in f)
                                    {
                                        g.gameObject.SetActive(false);
                                    }
                                    break;
                                case "Player":
                                    f.GetComponent<ThirdPersonNetworkVik>().enabled = false;
                                    f.GetComponent<CapsuleCollider>().enabled = false;
                                    f.GetComponent<Rigidbody>().useGravity = false;
                                    foreach (Transform g in f)
                                    {
                                        g.gameObject.SetActive(false);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        //for checking
        //foreach(Transform t in gameObject.transform)
        //{
        //    bool hasObj = false;
        //    foreach(Transform f in t)
        //    {
        //        t.gameObject.GetComponent<MeshRenderer>().enabled = true;
        //        hasObj = true;
        //        break;
        //    }

        //    if(hasObj == false)
        //        t.gameObject.GetComponent<MeshRenderer>().enabled = false;
        //}
    }
}
