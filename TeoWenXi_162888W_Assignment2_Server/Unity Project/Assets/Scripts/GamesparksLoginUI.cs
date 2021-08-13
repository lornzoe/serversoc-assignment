using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSparks.Core;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;

public class GamesparksLoginUI : MonoBehaviour
{
    public GameObject CanvasObject;
    public Text userNameField, connectionInfoField;
    public InputField userNameInput, passwordInput;
    public bool connected, loggedIn;

    void Start()
    {
        DataPasser.Instance.gamesparksObject = gameObject;
        connected = false;
        loggedIn = false;
        GS.GameSparksAvailable += OnGameSparksConnected;
    }

    private void OnGameSparksConnected(bool _isConnected)
    {
        if (_isConnected) 
        {
            connectionInfoField.text = "GameSparks Connected";
            connected = true;
        }
    else{
            connectionInfoField.text = "GameSparks Not Connected";
            connected = false;
        }
    }

    private void Update()
    {
        CanvasObject = DataPasser.Instance.gamesparksCanvas;
        userNameField = CanvasObject.transform.Find("UsernamePanel").Find("UsernameText").GetComponent<Text>();
        connectionInfoField = CanvasObject.transform.Find("ConnectedField").Find("ConnectedText").GetComponent<Text>();
        userNameInput = CanvasObject.transform.Find("UsernameInput").GetComponent<InputField>();
        passwordInput = CanvasObject.transform.Find("PasswordInput").GetComponent<InputField>();

        if (loggedIn)
        {
            CanvasObject.transform.Find("UsernameInput").gameObject.SetActive(false);
            CanvasObject.transform.Find("PasswordInput").gameObject.SetActive(false);
            CanvasObject.transform.Find("Login").gameObject.SetActive(false);
            CanvasObject.transform.Find("Device Login").gameObject.SetActive(false);
            CanvasObject.transform.Find("FacebookLogin").gameObject.SetActive(false);
            CanvasObject.transform.Find("Kills").gameObject.SetActive(true);
            CanvasObject.transform.Find("Kills").GetComponent<Text>().text = "Player Kills: " + DataPasser.Instance.character.GetComponent<PlayerData>().kills;

            CanvasObject.transform.Find("AchievementKills5").gameObject.SetActive(true);
            CanvasObject.transform.Find("AchievementJump5").gameObject.SetActive(true);
            CanvasObject.transform.Find("AchievementGetItem").gameObject.SetActive(true);

            if(DataPasser.Instance.character.GetComponent<PlayerData>().achievementKills5 == true)
                CanvasObject.transform.Find("AchievementKills5").GetComponent<Text>().text = "Achivement Kills 5: Achieved";
            else
                CanvasObject.transform.Find("AchievementKills5").GetComponent<Text>().text = "Achivement Kills 5: Not Achieved";

            if (DataPasser.Instance.character.GetComponent<PlayerData>().achievementJump5 == true)
                CanvasObject.transform.Find("AchievementJump5").GetComponent<Text>().text = "Achivement Jump 5: Achieved";
            else
                CanvasObject.transform.Find("AchievementJump5").GetComponent<Text>().text = "Achivement Jump 5: Not Achieved";

            if (DataPasser.Instance.character.GetComponent<PlayerData>().achievementGetItem == true)
                CanvasObject.transform.Find("AchievementGetItem").GetComponent<Text>().text = "Achivement Get Item: Achieved";
            else
                CanvasObject.transform.Find("AchievementGetItem").GetComponent<Text>().text = "Achivement Get Item: Not Achieved";
        }
        else
        {
            CanvasObject.transform.Find("UsernameInput").gameObject.SetActive(true);
            CanvasObject.transform.Find("PasswordInput").gameObject.SetActive(true);
            CanvasObject.transform.Find("Login").gameObject.SetActive(true);
            CanvasObject.transform.Find("Device Login").gameObject.SetActive(true);
            CanvasObject.transform.Find("FacebookLogin").gameObject.SetActive(true);
            CanvasObject.transform.Find("Kills").gameObject.SetActive(false);
            CanvasObject.transform.Find("AchievementKills5").gameObject.SetActive(false);
            CanvasObject.transform.Find("AchievementJump5").gameObject.SetActive(false);
            CanvasObject.transform.Find("AchievementGetItem").gameObject.SetActive(false);
        }
    }

    public void GetKillsLeaderboard()
    {
        int count = 0;
        GameObject leaderboards = CanvasObject.transform.Find("Leaderboards").gameObject;
        new GameSparks.Api.Requests.LeaderboardDataRequest().SetLeaderboardShortCode("KILLS_LEADERBOARD").SetEntryCount(100).Send((response) => {
            if (!response.HasErrors)
            {
                Debug.Log("Found Leaderboard Data...");
                foreach (GameSparks.Api.Responses.LeaderboardDataResponse._LeaderboardData entry in response.Data)
                {
                    int rank = (int)entry.Rank;
                    string playerName = entry.UserName;
                    string score = entry.JSONData["KILLS"].ToString();
                    //Debug.Log("Rank:" + rank + " Name:" + playerName + " \n Score:" + score);

                    leaderboards.transform.Find("" + (count + 1)).gameObject.SetActive(true);
                    leaderboards.transform.Find("" + (count + 1)).GetComponent<Text>().text = "Rank " + rank + ": " + playerName + ", Kills: " + score;

                    count++;
                    if (count > 2)
                        break;
                }
            }
            else
            {
                Debug.Log("Error Retrieving Leaderboard Data...");
            }
        });
    }

    public void UserAuthenticate(string Username, string Password)
    {
        Debug.Log("Attempting User Login...");
        //print out the username and password here just to check they are correct //
        Debug.Log("User Name:" + Username);
        Debug.Log("Password:" + Password);
        new GameSparks.Api.Requests.AuthenticationRequest()
            .SetUserName(Username)//set the username for login
            .SetPassword(Password)//set the password for login
            .Send((auth_response) => { //send the authentication request
                if (!auth_response.HasErrors)
                { // for the next part, check to see if we have any errors i.e. Authentication failed
                    connectionInfoField.text = "GameSparks Authenticated";
                    userNameField.text = auth_response.DisplayName;
                    loggedIn = true;
                }
                else
                {
                    Debug.LogWarning(auth_response.Errors.JSON); // if we have errors, print them out
                    if (auth_response.Errors.GetString("DETAILS") == "UNRECOGNISED")
                    { // if we get this error it means we are not registered, so let's register the user instead
                        Debug.Log("User Doesn't Exists, Registering User");
                        new GameSparks.Api.Requests.RegistrationRequest()
                            .SetDisplayName(Username)
                            .SetUserName(Username)
                            .SetPassword(Password)
                            .Send((reg_response) => {
                                if (!reg_response.HasErrors)
                                {
                                    connectionInfoField.text = "GameSparks Authenticated";
                                    userNameField.text = reg_response.DisplayName;
                                    loggedIn = true;
                                }
                                else
                                {
                                    Debug.LogWarning(auth_response.Errors.JSON); // if we have errors, print them out
                                }
                            });
                    }
                }
            });
    }

    public void UserAuthentication_Bttn()
    {
        Debug.Log("Attempting User Login...");
        //print out the username and password here just to check they are correct //
        Debug.Log("User Name:" + userNameInput.text);
        Debug.Log("Password:" + passwordInput.text);
        new GameSparks.Api.Requests.AuthenticationRequest()
            .SetUserName(userNameInput.text)//set the username for login
            .SetPassword(passwordInput.text)//set the password for login
            .Send((auth_response) => { //send the authentication request
            if (!auth_response.HasErrors)
                { // for the next part, check to see if we have any errors i.e. Authentication failed
                connectionInfoField.text = "GameSparks Authenticated";
                    userNameField.text = auth_response.DisplayName;
                }
                else
                {
                    Debug.LogWarning(auth_response.Errors.JSON); // if we have errors, print them out
                if (auth_response.Errors.GetString("DETAILS") == "UNRECOGNISED")
                    { // if we get this error it means we are not registered, so let's register the user instead
                    Debug.Log("User Doesn't Exists, Registering User");
                        new GameSparks.Api.Requests.RegistrationRequest()
                            .SetDisplayName(userNameInput.text)
                            .SetUserName(userNameInput.text)
                            .SetPassword(passwordInput.text)
                            .Send((reg_response) => {
                                if(!reg_response.HasErrors)
                                {
                                    connectionInfoField.text = "GameSparks Authenticated";
                                    userNameField.text = reg_response.DisplayName;
                                }
                        else{
                                    Debug.LogWarning(auth_response.Errors.JSON); // if we have errors, print them out
                            }
                            });
                    }
                }
            });
    }

    public void DeviceAuthentication_bttn()
    {
        Debug.Log("Attempting Device Authentication");
        new GameSparks.Api.Requests.DeviceAuthenticationRequest()
            .SetDisplayName("Player 2")
            .Send((auth_response) => {
                if (!auth_response.HasErrors)
                { // for the next part, check to see if we have any errors i.e. Authentication failed
                connectionInfoField.text = "GameSparks Authenticated";
                    userNameField.text = auth_response.DisplayName;
                }
                else
                {
                    Debug.LogWarning(auth_response.Errors.JSON); // if we have errors, print them out
            }
            });
    }

    public void GetFacebookPic()
    {
        new AccountDetailsRequest().Send((response) =>
        {
            string fbID;
            fbID = response.ExternalIds.GetString("FB");
            Debug.Log(fbID);

            var www = new WWW("http://graph.facebook.com/" + fbID + "/picture?width=100&height=100");

            while (!www.isDone)
            {

                //Debug.Log("Waiting Download to finish..."+ "LINK : http://graph.facebook.com/"+fbID+"/picture?width=100&height=100");

            }

            Debug.Log("Download Finished");
            Image userAvatar = GameObject.FindGameObjectWithTag("profilePic").GetComponent<Image>();
            userAvatar.sprite = Sprite.Create(www.texture, new Rect(0, 0, 100, 100), new Vector2(0, 0));

        });
    }
}
