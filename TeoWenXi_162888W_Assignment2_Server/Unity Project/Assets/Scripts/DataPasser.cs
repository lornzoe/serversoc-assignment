using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPasser : MonoBehaviour
{
    public static DataPasser Instance { get; private set; }

    public GameObject codeObject;
    public GameObject character;
    public GameObject pet;
    public string MainMenuLoginOK;
    public GameObject inventoryCanvas;
    public GameObject friendListCanvas;
    public GameObject gamesparksCanvas;
    public GameObject gamesparksObject;
    public string playerPassword;

    public GameObject minimapCamera;
    public GameObject minimapObject;
    public float duration = 0.5f;
    public float timer = 0;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Init()
    {
        MainMenuLoginOK = "";
        duration = 0.5f;
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > duration)
        {
            byte evCode = 4;
            PhotonNetwork.RaiseEvent(evCode, "RunThread", true, null);
            timer = 0;
        }

        if (!codeObject)
            codeObject = GameObject.FindGameObjectWithTag("Code");

        if(MainMenuLoginOK == "WrongPassword")
        {
            codeObject.GetComponent<MainMenuVik>().loginOk = MainMenuLoginOK;
        }

        if(character)
        {
            minimapCamera.transform.parent = character.transform;
            minimapObject.SetActive(true);
        }
        else
        {
            minimapObject.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.M))
        {
            if (minimapObject.transform.Find("Minimap").Find("FullMap").gameObject.activeSelf)
                minimapObject.transform.Find("Minimap").Find("FullMap").gameObject.SetActive(false);
            else
                minimapObject.transform.Find("Minimap").Find("FullMap").gameObject.SetActive(true);
        }
    }

    public void AcceptFriend(int index)
    {
        character.GetComponent<PlayerData>().AcceptFriend(index);
    }

    public void DeleteFriend(int index)
    {
        character.GetComponent<PlayerData>().DeleteFriend(index);
    }

    public void DeclinePending(int index)
    {
        character.GetComponent<PlayerData>().DeclinePending(index);
    }
}
