using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;

[System.Serializable]
public class channel
{
    public int code;
    public string message;
    public channelContent content;

    [System.Serializable]
    public class channelContent
    {
        public string channelId;
        public string channelName;
        public string channelImageUrl;
        public bool verifiedMark;
        public string channelType;
        public string channelDescription;
        public int followerCount;
        public bool openLive;
        public bool subscriptionAvailability;
        public sub subscriptionPaymentAvailability;

        [System.Serializable]
        public class sub
        {
            public bool iapAvailability;
            public bool iabAvailability;
        }
    }
}

[System.Serializable]
public class live_status
{
    public int code;
    public string message;
    public status_content content;

    [System.Serializable]
    public class status_content
    {
        public string liveTitle;
        public string status;
        public int concurrentUserCount;
        public int accumulateCount;
        public bool paidPromotion;
        public bool adult;
        public string chatChannelId;
        public string categoryType;
        public string liveCategory;
        public string liveCategoryValue;
        public string livePollingStatusJson;
        public string faultStatus;
        public string userAdultStatus;
        public bool chatActive;
        public string chatAvailableGroup;
        public string chatAvailableCondition;
        public int minFollowerMinute;
    }
}

[System.Serializable]
public class chat
{
    public int code;
    public string message;
    public chat_content content;

    [System.Serializable]
    public class chat_content
    {
        public string accessToken;
        [Serializable]
        public class TemporaryRestrict
        {
            public bool temporaryRestrict;
            public int times;
            public int duration;
            public int createdTime;
        }
        public bool realNameAuth;
        public string extraToken;
    }
}

[System.Serializable]
public class receivedData
{
    public string svcid;
    public string ver;
    public Body[] bdy;
    public int cmd;
    public string tid;
    public string cid;

    [System.Serializable]
    public class Body
    {
        public string svcid;
        public string cid;
        public int mbrCnt;
        public string uid;
        public string profile;
        public string msg;
        public int msgTypeCode;
        public string msgStatusType;
        public string extras;
        public int ctime;
        public int utime;
        public string msgTid;
        public int msgTime;
    }
}

[System.Serializable]
public class Profile
{
    public string userIdHash;
    public string nickname;
    public string profileImageUrl;
    public string userRoleCode;
    public string badge;
    public string title;
    public bool verifiedMark;
    public Badge[] activityBadges;
    public StreamingProperty streamingProperty;

    [System.Serializable]
    public class Badge
    {
        public int badgeNo;
        public string badgeId;
        public string imageUrl;
        public string title;
        public string description;
        public bool activated;
    }

    [System.Serializable]
    public class StreamingProperty
    {
        public Subscription subscription;

        [System.Serializable]
        public class Subscription
        {
            public int accumulativeMonth;
            public int tier;
            public Badge badge;
        }
    }
}

public class ChzzkChat : MonoBehaviour
{
    public bool stopConnect = false;

    [Header("Channel Information")]
    public channel channel;
    public live_status status;
    public chat chat;
    public string channelName;

    public string channelId;
    public string chatChannelId;
    public string accessToken;

    WebSocket ws;

    [Header("Vote")]
    public GameObject userBox;
    public List<Profile> p;
    public List<string> User;
    public bool collecting = false;
    public bool roulette = false;
    public bool vote = false;
    public bool subOnly = false;
    int count;

    string heartbeatRequest = "{\"ver\":\"2\",\"cmd\":0}";
    string heartbeatResponse = "{\"ver\":\"2\",\"cmd\":10000}";
    
    public void SetRoulette()
    {
        roulette = true;
        vote = false;
    }

    public void SetVote()
    {
        roulette = false;
        vote = true;
    }

    public void Home()
    {
        roulette = false;
        vote = false;
        collecting = false;
    }

    public void InitializeUser()
    {
        p = new List<Profile>();
        User = new List<string>();
        count = 0;
    }

    void Update()
    {
        if(count < User.Count)
        {
            for (int i = User.Count - count; i > 0; i--)
            {
                userBox.GetComponent<User>().profile = p[User.Count - i];
                GameObject Box = Instantiate(userBox);
                Box.transform.SetParent(GameObject.Find("Content").transform);
            }

            count = User.Count;
        }
    }

    void OnApplicationQuit()
    {
        Disconncect();
    }

    public void ChzzkConnect()
    {
        channelName = "";
        InitializeUser();
        stopConnect = false;
        StartCoroutine(GetChannel());
    }

    IEnumerator HeartBeat()
    {
        while (!stopConnect)
        {
            yield return new WaitForSecondsRealtime(15f);

            if (ws != null && ws.IsAlive)
            {
                ws.Send(heartbeatRequest);
            }
        }
    }

    IEnumerator GetChannel()
    {
        string apiUrl = $"https://api.chzzk.naver.com/service/v1/channels/{channelId}";

        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                channel = JsonUtility.FromJson<channel>(www.downloadHandler.text);
                channelName = channel.content.channelName;
                yield return GetChat();
            }
            else Debug.Log(www.error);
        }
    }

    IEnumerator GetChat()
    {
        string apiUrl = $"https://api.chzzk.naver.com/polling/v2/channels/{channelId}/live-status";

        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                status = JsonUtility.FromJson<live_status>(www.downloadHandler.text);
                chatChannelId = status.content.chatChannelId;
                yield return GetAccessToken();
            }
            else Debug.Log(www.error);
        }
    }

    IEnumerator GetAccessToken()
    {
        string apiUrl = $"https://comm-api.game.naver.com/nng_main/v1/chats/access-token?channelId={chatChannelId}&chatType=STREAMING";

        using (UnityWebRequest www = UnityWebRequest.Get(apiUrl))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                chat = JsonUtility.FromJson<chat>(www.downloadHandler.text);
                accessToken = chat.content.accessToken;

                if (!stopConnect) Connect();
            }
            else Debug.Log(www.error);
        }
    }

    public void Connect()
    {
        string msg = "{\"ver\":\"2\",\"cmd\":100,\"svcid\":\"game\",\"cid\":\"" + chatChannelId + "\",\"bdy\":{\"uid\":null,\"devType\":2001,\"accTkn\":\"" + accessToken + "\",\"auth\":\"READ\"},\"tid\":1}";

        ws = new WebSocket("wss://kr-ss1.chat.naver.com/chat");
        ws.OnMessage += ws_OnMessage;
        ws.OnOpen += ws_OnOpen;
        ws.OnClose += ws_OnClose;
        ws.Connect();
        ws.Send(msg);
    }

    public void Disconncect()
    {
        stopConnect = true;
        try
        {
            if (ws == null) return;
            if (ws.IsAlive)
            {
                Debug.Log("close");
                ws.Close();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    void ws_OnMessage(object sender, MessageEventArgs e)
    {
        receivedData d = JsonUtility.FromJson<receivedData>(e.Data);

        switch (d.cmd)
        {
            case 0:
                ws.Send(heartbeatResponse);
                break;
            case 93101:
                for (int i = 0; i < d.bdy.Length; i++)
                {
                    Profile profile = JsonUtility.FromJson<Profile>(d.bdy[i].profile);

                    if (!User.Contains(profile.nickname) && collecting && roulette)
                    {
                        p.Add(profile);
                        User.Add(profile.nickname);
                    }
                }
                break;
            case 93102:
            default:
                break;
        }
    }

    void ws_OnOpen(object sender, EventArgs e)
    {
        Debug.Log("open");
        StartCoroutine(HeartBeat());
    }

    void ws_OnClose(object sender, CloseEventArgs e)
    {
        if (!stopConnect) Connect();
    }
}