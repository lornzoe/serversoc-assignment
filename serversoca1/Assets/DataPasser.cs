using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataPasser : MonoBehaviour {
    public static DataPasser Instance { get; private set; }

    public GameObject codeObject;
    public GameObject character;

    public string mainMenuPasser;

    // Use this for initialization
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Init();
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Init()
    {
        mainMenuPasser = "";
    }

    // Update is called once per frame
    void Update () {
        if (!codeObject)
            codeObject = GameObject.FindGameObjectWithTag("Code");

        if (mainMenuPasser == "wrongPassword")
        {
            codeObject.GetComponent<MainMenuVik>().loginCheck = mainMenuPasser;
        }
    }
}
