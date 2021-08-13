using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;

public class PlayerData : MonoBehaviour
{
    private float timer, perSaveDuration;
    public GameObject pet;
    public List<ItemData> Inventory;
    public List<Enemy> enemiesInRange;
    public float radius;
    public int kills;

    public List<string> friendList;
    public List<string> pendingFriendList;

    public Sprite item1, item2, nothing;

    [HideInInspector]
    public int visionRange, interestRange;
    public bool achievementKills5, achievementJump5, achievementGetItem;
    public int numberOfJumps;

    // Start is called before the first frame update
    void Awake()
    {
        GameSparks.Api.Messages.AchievementEarnedMessage.Listener += AchievementMessageHandler;
    }

    void Start()
    {
        Inventory = new List<ItemData>();
        enemiesInRange = new List<Enemy>();
        friendList = new List<string>();
        pendingFriendList = new List<string>();

        perSaveDuration = 1;
        pet = DataPasser.Instance.pet;

        visionRange = 1;
        interestRange = 3;
        kills = 0;
        numberOfJumps = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > perSaveDuration)
        {
            SaveData();
            SavePlayerKillsGamespark();
            DataPasser.Instance.gamesparksObject.GetComponent<GamesparksLoginUI>().GetKillsLeaderboard();
            //SavePosition();
            //SaveInventory();
            //SaveFriendList();
            timer = 0;
        }

        if (!pet)
            pet = DataPasser.Instance.pet;

        Collider[] cols = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider col in cols)
        {
            if (col && col.tag == "Enemy" && Input.GetKeyDown(KeyCode.F))
            {
                Enemy script = col.transform.parent.GetComponent<Enemy>();
                script.hp--;

                if (script.hp <= 0)
                    kills++;

                if (kills >= 5 && achievementKills5 == false)
                    GiveAchivementKill5();

                script.HpCall();
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (DataPasser.Instance.inventoryCanvas)
            {
                if (DataPasser.Instance.inventoryCanvas.GetActive())
                {
                    DataPasser.Instance.inventoryCanvas.transform.Find("Image").Find("Slot1Selected").gameObject.SetActive(false);
                    DataPasser.Instance.inventoryCanvas.transform.Find("Image").Find("Slot2Selected").gameObject.SetActive(false);
                    DataPasser.Instance.inventoryCanvas.SetActive(false);
                }
                else
                    DataPasser.Instance.inventoryCanvas.SetActive(true);
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (DataPasser.Instance.gamesparksCanvas)
            {
                if (DataPasser.Instance.gamesparksCanvas.GetActive())
                {
                    DataPasser.Instance.gamesparksCanvas.SetActive(false);
                }
                else
                    DataPasser.Instance.gamesparksCanvas.SetActive(true);
            }
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            numberOfJumps++;

            if (numberOfJumps >= 5 && achievementJump5 == false)
                GiveAchivementJump5();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (visionRange == 3)
                visionRange = 1;
            else
                visionRange = 3;
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (DataPasser.Instance.friendListCanvas)
            {
                if (DataPasser.Instance.friendListCanvas.GetActive())
                {
                    DataPasser.Instance.friendListCanvas.SetActive(false);
                }
                else
                    DataPasser.Instance.friendListCanvas.SetActive(true);
            }
        }

        if (DataPasser.Instance.friendListCanvas)
        {
            if (DataPasser.Instance.friendListCanvas.GetActive())
            {
                UpdateFriendList();
            }
        }

        if (DataPasser.Instance.inventoryCanvas)
        {
            if (DataPasser.Instance.inventoryCanvas.GetActive())
            {
                UpdateInventory();

                GameObject inventoryCanvas = DataPasser.Instance.inventoryCanvas;

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (inventoryCanvas.transform.Find("Image").Find("Slot1Selected").gameObject.GetActive())
                    {
                        if (Inventory.Count > 0)
                        {
                            GameObject newObject = PhotonNetwork.Instantiate(Inventory[0].itemName, transform.position + transform.forward * 2, Quaternion.identity, 0);
                            newObject.GetComponent<ItemData>().attack = Inventory[0].attack;
                            Inventory.RemoveAt(0);
                        }
                    }
                    else if (inventoryCanvas.transform.Find("Image").Find("Slot2Selected").gameObject.GetActive())
                    {
                        if (Inventory.Count > 1)
                        {
                            GameObject newObject = PhotonNetwork.Instantiate(Inventory[1].itemName, transform.position + transform.forward * 2, Quaternion.identity, 0);
                            newObject.GetComponent<ItemData>().attack = Inventory[1].attack;
                            Inventory.RemoveAt(1);
                        }
                    }
                }
            }
        }
    }

    void AchievementMessageHandler(GameSparks.Api.Messages.AchievementEarnedMessage _message)
    {
        //Debug.Log("AWARDED ACHIEVEMENT \n " + _message.AchievementName);
    }

    public void LoadAchivements()
    {
        new GameSparks.Api.Requests.AccountDetailsRequest().Send((response) => {
            if (!response.HasErrors)
            {
                Debug.Log("Account Details Found...");
                string playerName = response.DisplayName; // we can get the display name
                List<string> achievementsList = response.Achievements; // we can get a list of achievements earned
                // Then we can print this data out or use it wherever we want //
                Debug.Log("Player Name: " + playerName);
                foreach (string s in achievementsList)
                {
                    switch(s)
                    {
                        case "KILLS_5":
                            achievementKills5 = true;
                            break;
                        case "JUMP_5":
                            achievementJump5 = true;
                            break;
                        case "GET_ITEM":
                            achievementGetItem = true;
                            break;
                    }
                }
            }
            else
            {
                Debug.Log("Error Retrieving Account Details...");
            }
        });
    }

    public void GiveAchivementGetItem()
    {
        achievementGetItem = true;
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("Award_Achievement_GetItem")
            .Send((response) => {
                if (!response.HasErrors)
                {
                    Debug.Log("Achivement given");
                }
                else
                {
                    Debug.Log("Error giving achievement");
                }
            });
    }

    public void GiveAchivementJump5()
    {
        achievementJump5 = true;
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("Award_Achievement_Jump5")
            .Send((response) => {
                if (!response.HasErrors)
                {
                    Debug.Log("Achivement given");
                }
                else
                {
                    Debug.Log("Error giving achievement");
                }
            });
    }

    public void GiveAchivementKill5()
    {
        achievementKills5 = true;
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("Award_Achievement_Kills5")
            .Send((response) => {
                if (!response.HasErrors)
                {
                    Debug.Log("Achivement given");
                }
                else
                {
                    Debug.Log("Error giving achievement");
                }
            });
    }

    public void SavePlayerKillsGamespark()
    {
        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("SAVE_SCORE")
            .SetEventAttribute("KILLS", kills)
            .Send((response) => {
            if (!response.HasErrors)
            {
                Debug.Log("Player Saved To GameSparks...");
            }
            else
            {
                Debug.Log("Error Saving Player Data...");
            }
        });

        new GameSparks.Api.Requests.LogEventRequest()
            .SetEventKey("ADD_SCORE")
            .SetEventAttribute("KILLS", kills)
            .Send((response) => {
                if (!response.HasErrors)
                {
                    Debug.Log("Player Saved To GameSparks...");
                }
                else
                {
                    Debug.Log("Error Saving Player Data...");
                }
            });
    }

    public void LoadPlayerKillsGamespark()
    {
        new GameSparks.Api.Requests.LogEventRequest().SetEventKey("LOAD_SCORE")
            .Send((response) =>
            {
                if (!response.HasErrors)
                {
                    Debug.Log("Received Player Data From GameSparks...");
                    GSData data = response.ScriptData.GetGSData("Player_Data");
                    kills = (int)data.GetInt("playerKills");
                    Debug.Log("Loaded Kills Num: " + kills);
                }
                else
                {
                    Debug.Log("Error Loading Player Data...");
                }
            });
    }

    public void SaveData()
    {
        byte evCode = 2;
        bool reliable = true;
        PlayerDataInfo playerDataInfo = new PlayerDataInfo();
        playerDataInfo.Username = GetComponent<PhotonView>().owner.NickName;

        //Position
        playerDataInfo.posX = transform.position.x;
        playerDataInfo.posY = transform.position.y;
        playerDataInfo.posZ = transform.position.z;
        playerDataInfo.rotX = transform.eulerAngles.x;
        playerDataInfo.rotY = transform.eulerAngles.y;
        playerDataInfo.rotZ = transform.eulerAngles.z;
        playerDataInfo.cameraPosX = transform.Find("Main Camera").localPosition.x;
        playerDataInfo.cameraPosY = transform.Find("Main Camera").localPosition.y;
        playerDataInfo.cameraPosZ = transform.Find("Main Camera").localPosition.z;
        playerDataInfo.cameraRotX = transform.Find("Main Camera").localEulerAngles.x;
        playerDataInfo.cameraRotY = transform.Find("Main Camera").localEulerAngles.y;
        playerDataInfo.cameraRotZ = transform.Find("Main Camera").localEulerAngles.z;
        playerDataInfo.petPosX = pet.transform.position.x;
        playerDataInfo.petPosY = pet.transform.position.y;
        playerDataInfo.petPosZ = pet.transform.position.z;
        playerDataInfo.petRotX = pet.transform.eulerAngles.x;
        playerDataInfo.petRotY = pet.transform.eulerAngles.y;
        playerDataInfo.petRotZ = pet.transform.eulerAngles.z;

        //Inventory
        if(Inventory.Count > 0)
        {
            playerDataInfo.item1Name = Inventory[0].itemName;
            playerDataInfo.item1Attack = Inventory[0].attack;
        }
        else
        {
            playerDataInfo.item1Name = "NOTHING";
            playerDataInfo.item1Attack = 0;
        }

        if (Inventory.Count > 1)
        {
            playerDataInfo.item2Name = Inventory[1].itemName;
            playerDataInfo.item2Attack = Inventory[1].attack;
        }
        else
        {
            playerDataInfo.item2Name = "NOTHING";
            playerDataInfo.item2Attack = 0;
        }

        //Friend List
        if (friendList.Count > 0)
            playerDataInfo.friend1Name = friendList[0];
        else
            playerDataInfo.friend1Name = "NOTHING";

        if (friendList.Count > 1)
            playerDataInfo.friend2Name = friendList[1];
        else
            playerDataInfo.friend2Name = "NOTHING";

        if (friendList.Count > 2)
            playerDataInfo.friend3Name = friendList[2];
        else
            playerDataInfo.friend3Name = "NOTHING";

        //Pending List
        if (pendingFriendList.Count > 0)
            playerDataInfo.pending1Name = pendingFriendList[0];
        else
            playerDataInfo.pending1Name = "NOTHING";

        if (pendingFriendList.Count > 1)
            playerDataInfo.pending2Name = pendingFriendList[1];
        else
            playerDataInfo.pending2Name = "NOTHING";

        if (pendingFriendList.Count > 2)
            playerDataInfo.pending3Name = pendingFriendList[2];
        else
            playerDataInfo.pending3Name = "NOTHING";

        PhotonNetwork.RaiseEvent(evCode, PlayerDataInfo.Serialize(playerDataInfo), reliable, null);
    }

    public void SavePosition()
    {
        byte evCode = 2;
        string contentMessage = "playerName=" + GetComponent<PhotonView>().owner.NickName + ",";

        contentMessage += "position=" + transform.position.x + "_" + transform.position.y + "_" + transform.position.z + ",";
        contentMessage += "rotation=" + transform.eulerAngles.x + "_" + transform.eulerAngles.y + "_" + transform.eulerAngles.z + ",";
        contentMessage += "cameraPos=" + transform.Find("Main Camera").localPosition.x + "_" + transform.Find("Main Camera").localPosition.y + "_" + transform.Find("Main Camera").localPosition.z + ",";
        contentMessage += "cameraRot=" + transform.Find("Main Camera").localEulerAngles.x + "_" + transform.Find("Main Camera").localEulerAngles.y + "_" + transform.Find("Main Camera").localEulerAngles.z + ",";
        contentMessage += "petPosition=" + pet.transform.position.x + "_" + pet.transform.position.y + "_" + pet.transform.position.z + ",";
        contentMessage += "petRotation=" + pet.transform.eulerAngles.x + "_" + pet.transform.eulerAngles.y + "_" + pet.transform.eulerAngles.z;

        //Debug.Log(contentMessage);

        byte[] content = System.Text.Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    public void SaveInventory()
    {
        byte evCode = 3;
        string contentMessage = "playerName=" + GetComponent<PhotonView>().owner.NickName + ",";

        if (Inventory.Count > 0)
        {
            contentMessage += "item1Name=" + Inventory[0].itemName + ",";
            contentMessage += "item1Attack=" + Inventory[0].attack + ",";

            if (Inventory.Count > 1)
            {
                contentMessage += "item2Name=" + Inventory[1].itemName + ",";
                contentMessage += "item2Attack=" + Inventory[1].attack;
            }
            else
            {
                contentMessage += "item2Name=NOTHING,";
                contentMessage += "item2Attack=NOTHING";
            }
        }
        else
        {
            contentMessage += "item1Name=NOTHING,";
            contentMessage += "item1Attack=NOTHING,";
            contentMessage += "item2Name=NOTHING,";
            contentMessage += "item2Attack=NOTHING";
        }

        //Debug.Log(contentMessage);

        byte[] content = System.Text.Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            //Debug.Log(other.transform.parent.GetComponent<ItemData>().attack);

            if (Inventory.Count < 2 && other.transform.parent.GetComponent<ItemData>().isDone == false)
            {
                Inventory.Add(other.transform.parent.GetComponent<ItemData>());
                other.transform.parent.GetComponent<ItemData>().isDone = true;

                if(achievementGetItem == false)
                    GiveAchivementGetItem();
            }
        }
    }

    private void UpdateInventory()
    {
        GameObject inventoryCanvas = DataPasser.Instance.inventoryCanvas;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventoryCanvas.transform.Find("Image").Find("Slot1Selected").gameObject.SetActive(true);
            inventoryCanvas.transform.Find("Image").Find("Slot2Selected").gameObject.SetActive(false);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventoryCanvas.transform.Find("Image").Find("Slot1Selected").gameObject.SetActive(false);
            inventoryCanvas.transform.Find("Image").Find("Slot2Selected").gameObject.SetActive(true);
        }

        for (int i = 0; i < 2; ++i)
        {
            string name = "Slot" + (i + 1);
            GameObject slot = inventoryCanvas.transform.Find("Image").Find(name).gameObject;

            slot.transform.Find("Text").GetComponent<Text>().text = "";
            slot.transform.Find("Image").GetComponent<Image>().sprite = nothing;
        }

        for (int i = 0; i < Inventory.Count; ++i)
        {
            string name = "Slot" + (i + 1);
            GameObject slot = inventoryCanvas.transform.Find("Image").Find(name).gameObject;

            slot.transform.Find("Text").GetComponent<Text>().text = "Attack: " + Inventory[i].attack;
            if (Inventory[i].itemName == "Item1")
                slot.transform.Find("Image").GetComponent<Image>().sprite = item1;
            else if (Inventory[i].itemName == "Item2")
                slot.transform.Find("Image").GetComponent<Image>().sprite = item2;
        }
    }

    private void SaveFriendList()
    {
        byte evCode = 4;
        string contentMessage = "playerName=" + GetComponent<PhotonView>().owner.NickName;

        for (int i = 0; i < friendList.Count; ++i)
        {
            string name = ",friend" + (i + 1) + "Name=";
            contentMessage += name + friendList[i];
        }

        for (int i = friendList.Count; i < 3; ++i)
        {
            string name = ",friend" + (i + 1) + "Name=";
            contentMessage += name + "NOTHING";
        }

        for (int i = 0; i < pendingFriendList.Count; ++i)
        {
            string name = ",pending" + (i + 1) + "Name=";
            contentMessage += name + pendingFriendList[i];
        }

        for (int i = pendingFriendList.Count; i < 3; ++i)
        {
            string name = ",pending" + (i + 1) + "Name=";
            contentMessage += name + "NOTHING";
        }
        
        //Debug.Log(contentMessage);

        byte[] content = System.Text.Encoding.UTF8.GetBytes(contentMessage);
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, content, reliable, null);
    }

    private void UpdateFriendList()
    {
        GameObject friendListCanvas = DataPasser.Instance.friendListCanvas;

        for(int i = 0; i < friendList.Count; ++i)
        {
            string objName = "Friend" + (i + 1);
            GameObject friendObject = friendListCanvas.transform.Find("Friends").Find(objName).gameObject;

            friendObject.SetActive(true);
            friendObject.GetComponent<Text>().text = friendList[i];
        }

        for(int i = friendList.Count; i < 3; ++i)
        {
            string objName = "Friend" + (i + 1);
            GameObject friendObject = friendListCanvas.transform.Find("Friends").Find(objName).gameObject;

            friendObject.SetActive(false);
        }

        for (int i = 0; i < pendingFriendList.Count; ++i)
        {
            string objName = "Request" + (i + 1);
            GameObject pendingFriendObj = friendListCanvas.transform.Find("Pending").Find(objName).gameObject;

            pendingFriendObj.SetActive(true);
            pendingFriendObj.GetComponent<Text>().text = pendingFriendList[i];
        }

        for (int i = pendingFriendList.Count; i < 3; ++i)
        {
            string objName = "Request" + (i + 1);
            GameObject pendingFriendObj = friendListCanvas.transform.Find("Pending").Find(objName).gameObject;

            pendingFriendObj.SetActive(false);
        }
    }

    public void DeleteFriendRPC_Call(string targetUser)
    {
        GetComponent<PhotonView>().RPC("DeleteFriendRPC",
          PhotonTargets.All,
          new object[] { GetComponent<PhotonView>().owner.NickName, targetUser });
    }

    [PunRPC]
    public void SendFriendRequestRPC(string sender, string targetUser, PhotonMessageInfo info)
    {
        //Debug.Log(string.Format("Info: {0} {1} {2}", info.sender, info.photonView, info.timestamp));
        //string test = GetComponent<PhotonView>().owner.NickName;
        //Debug.Log(test + "TARGET:" + targetUser + "NAME:" + gameObject.name);
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().owner.NickName.Equals(targetUser))
            {
                if(player.GetComponent<PlayerData>().pendingFriendList.Count < 3)
                    player.GetComponent<PlayerData>().pendingFriendList.Add(sender);
            }
        }
    }

    public void SendFriendRequestRPC_Call(string targetUser)
    {
        GetComponent<PhotonView>().RPC("SendFriendRequestRPC",
          PhotonTargets.All,
          new object[] { GetComponent<PhotonView>().owner.NickName, targetUser });
    }

    [PunRPC]
    public void DeleteFriendRPC(string sender, string targetUser, PhotonMessageInfo info)
    {
        //Debug.Log(string.Format("Info: {0} {1} {2}", info.sender, info.photonView, info.timestamp));
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().owner.NickName.Equals(targetUser))
            {
                int removeIndex = -1;
                for (int i = 0; i < player.GetComponent<PlayerData>().friendList.Count; ++i)
                {
                    if (player.GetComponent<PlayerData>().friendList[i] == sender)
                    {
                        removeIndex = i;
                    }
                }

                if (removeIndex > -1)
                {
                    player.GetComponent<PlayerData>().friendList.RemoveAt(removeIndex);
                }
            }
        }
    }

    public void AcceptFriendRPC_Call(string targetUser)
    {
        GetComponent<PhotonView>().RPC("AcceptFriendRPC",
          PhotonTargets.All,
          new object[] { GetComponent<PhotonView>().owner.NickName, targetUser });
    }

    [PunRPC]
    public void AcceptFriendRPC(string sender, string targetUser, PhotonMessageInfo info)
    {
        //Debug.Log(string.Format("Info: {0} {1} {2}", info.sender, info.photonView, info.timestamp));
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PhotonView>().owner.NickName.Equals(targetUser))
            {
                if (player.GetComponent<PlayerData>().friendList.Count < 3)
                    player.GetComponent<PlayerData>().friendList.Add(sender);
            }
        }
    } 

    public void DeleteFriend(int index)
    {
        DeleteFriendRPC_Call(friendList[index]);
        friendList.RemoveAt(index);
    }

    public void AcceptFriend(int index)
    {
        friendList.Add(pendingFriendList[index]);
        AcceptFriendRPC_Call(pendingFriendList[index]);

        pendingFriendList.RemoveAt(index);
    }

    public void DeclinePending(int index)
    {
        pendingFriendList.RemoveAt(index);
    }
}
