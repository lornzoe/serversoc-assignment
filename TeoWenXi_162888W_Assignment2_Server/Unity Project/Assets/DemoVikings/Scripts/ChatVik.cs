using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This simple chat example showcases the use of RPC targets and targetting certain players via RPCs.
/// </summary>
public class ChatVik : Photon.MonoBehaviour
{

    public static ChatVik SP;
    public List<string> messages = new List<string>();

    private int chatHeight = (int)140;
    private Vector2 scrollPos = Vector2.zero;
    private string chatInput = "";
    private float lastUnfocusTime = 0;

    void Awake()
    {
        SP = this;
    }

    void OnGUI()
    {        
        GUI.SetNextControlName("");

        GUILayout.BeginArea(new Rect(0, Screen.height - chatHeight, Screen.width, chatHeight));
        
        //Show scroll list of chat messages
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        GUI.color = Color.red;
        for (int i = messages.Count - 1; i >= 0; i--)
        {
            GUILayout.Label(messages[i]);
        }
        GUILayout.EndScrollView();
        GUI.color = Color.white;

        //Chat input
        GUILayout.BeginHorizontal(); 
        GUI.SetNextControlName("ChatField");
    chatInput = GUILayout.TextField(chatInput, GUILayout.MinWidth(200));
       
        if (Event.current.type == EventType.KeyDown && Event.current.character == '\n'){
            if (GUI.GetNameOfFocusedControl() == "ChatField")
            {                
                SendChat(PhotonTargets.All);
                lastUnfocusTime = Time.time;
                GUI.FocusControl("");
                GUI.UnfocusWindow();
            }
            else
            {
                if (lastUnfocusTime < Time.time - 0.1f)
                {
                    GUI.FocusControl("ChatField");
                }
            }
        }

        //if (GUILayout.Button("SEND", GUILayout.Height(17)))
         //   SendChat(PhotonTargets.All);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

    

        GUILayout.EndArea();
    }

    public static void AddMessage(string text)
    {
        SP.messages.Add(text);
        if (SP.messages.Count > 15)
            SP.messages.RemoveAt(0);
    }


    [PunRPC]
    void SendChatMessage(string text, PhotonMessageInfo info)
    {
        AddMessage("[" + info.sender + "] " + text);
    }

    [PunRPC]
    void FriendMessage(string text, PhotonMessageInfo info)
    {
        PlayerData playerObject = DataPasser.Instance.character.GetComponent<PlayerData>();

        bool isFriend = false;

        if (info.sender.NickName == playerObject.GetComponent<PhotonView>().owner.NickName)
            isFriend = true;
        else
        {
            for (int i = 0; i < playerObject.friendList.Count; ++i)
            {
                if (info.sender.NickName == playerObject.friendList[i])
                {
                    isFriend = true;
                    break;
                }
            }
        }
        

        if (isFriend)
        {
            string cutshorted = "";
            for(int i = 2; i < text.Length; ++i)
            {
                cutshorted += text[i];
            }
            AddMessage("[Friend][" + info.sender + "] " + cutshorted);
        }
    }

    void SendChat(PhotonTargets target)
    {
        if (chatInput != "" && chatInput[0] != '/')
        {
            photonView.RPC("SendChatMessage", target, chatInput);
            chatInput = "";
        }
        else if (chatInput[0] == '/')
        {
            if (chatInput[1] == 'f')
            {
                photonView.RPC("FriendMessage", target, chatInput);
                chatInput = "";
            }
            else
            {
                string player = GetSubstringFromString(chatInput, "/addFriend");
                DataPasser.Instance.character.GetComponent<PlayerData>().SendFriendRequestRPC_Call(player);
                chatInput = "";
            }
        }
    }

    void SendChat(PhotonPlayer target)
    {
        if (chatInput != "")
        {
            chatInput = "[PM] " + chatInput;
            photonView.RPC("SendChatMessage", target, chatInput);
            chatInput = "";
        }
    }

    void OnLeftRoom()
    {
        this.enabled = false;
    }

    void OnJoinedRoom()
    {
        this.enabled = true;
    }
    void OnCreatedRoom()
    {
        this.enabled = true;
    }

    public string GetSubstringFromString(string input, string tag)
    {
        //Get tag
        string outputString = "";
        for (int i = 0; i < input.Length; i++)
        {
            if (input.Substring(i, tag.Length) == tag)
            {
                int index = 0;
                while (i + tag.Length + 1 + index < input.Length && input[i + tag.Length + 1 + index] != ',')
                {
                    outputString += input[i + tag.Length + 1 + index];
                    index++;
                }
                break;
            }
        }

        return outputString;
    }
}
