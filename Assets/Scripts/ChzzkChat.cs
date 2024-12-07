using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
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
    enum SslProtocolsHack
    {
        Tls = 192,
        Tls11 = 768,
        Tls12 = 3072
    }

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
    public VoteManager vm;
    public GameObject Content;
    public GameObject userBox;
    public List<Profile> p;
    public List<string> User;
    public List<bool> sub;
    public List<bool> exclude;
    public List<bool> possible;
    public List<int> choice;
    public bool collecting = false;
    public bool vote = false;
    public bool subOnly = false;
    int count;

    public InputField IdField;

    [Header("Chat Box")]
    public string winner = "";
    public bool chatOn = false;
    public List<string> chatMsg;
    public int msgCount = 0;
    public GameObject msg;
    public GameObject ChatContent;

    [Header("DR")]
    public bool DR = false;

    string heartbeatRequest = "{\"ver\":\"2\",\"cmd\":0}";
    string heartbeatResponse = "{\"ver\":\"2\",\"cmd\":10000}";

    void Awake()
    {
        if (PlayerPrefs.HasKey("Cid"))
        {
            channelId = PlayerPrefs.GetString("Cid");
            IdField.text = channelId;
            ChzzkConnect();
        }
    }

    void Update()
    {
        if (count < User.Count)
        {
            for (int i = User.Count - count; i > 0; i--)
            {
                userBox.GetComponent<User>().index = User.Count - i;
                userBox.GetComponent<User>().profile = p[User.Count - i];
                Instantiate(userBox, Content.transform);
            }

            count = User.Count;
        }

        if (msgCount < chatMsg.Count)
        {
            for (int i = chatMsg.Count - msgCount; i > 0; i--)
            {
                msg.GetComponent<Text>().text = chatMsg[chatMsg.Count - i];
                Instantiate(msg, ChatContent.transform);
            }

            msgCount = chatMsg.Count;
        }
    }

    void OnApplicationQuit()
    {
        if (!stopConnect) PlayerPrefs.SetString("Cid", channelId);
        else PlayerPrefs.DeleteKey("Cid");
        Disconncect();
    }

    public void SetRoulette()
    {
        vote = false;
        DR = false;
    }

    public void SetVote()
    {
        vote = true;
        DR = false;
    }

    public void SetDR()
    {
        vote = false;
        DR = true;
    }

    public void AddUser(Profile profile)
    {
        p.Add(profile);
        User.Add(profile.nickname);
        sub.Add(profile.streamingProperty.subscription.tier != 0);
        exclude.Add(false);
        possible.Add(false);
    }

    public void Home()
    {
        vm.SelectedItem = 0;
        vote = false;
        DR = false;
        collecting = false;
        InitializeUser();
        InitializeChat();
    }

    public void InitializeUser()
    {
        p = new List<Profile>();
        User = new List<string>();
        sub = new List<bool>();
        exclude = new List<bool>();
        possible = new List<bool>();
        choice = new List<int>();
        count = 0;
    }

    public void InitializeChat()
    {
        chatMsg = new List<string>();
        msgCount = 0;
        winner = "";
        chatOn = false;
    }

    public int Roulette()
    {
        if (!possible.Contains(true)) return -1;

        int i = 0;
        if (subOnly)
        {
            if (!possible.Contains(true)) return -1;
            else{
                i = UnityEngine.Random.Range(0, User.Count);
                while (!possible[i])
                {
                    i = UnityEngine.Random.Range(0, User.Count);
                }
             }
        }
        else
        {
            i = UnityEngine.Random.Range(0, User.Count);
            while (!possible[i])
            {
                i = UnityEngine.Random.Range(0, User.Count);
            }
        }
        return i;
    }

    public void ChzzkConnect()
    {
        channelName = "";
        InitializeUser();
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

                Connect();
            }
            else Debug.Log(www.error);
        }
    }

    public void Connect()
    {
        string msg = "{\"ver\":\"2\",\"cmd\":100,\"svcid\":\"game\",\"cid\":\"" + chatChannelId + "\",\"bdy\":{\"uid\":null,\"devType\":2001,\"accTkn\":\"" + accessToken + "\",\"auth\":\"READ\"},\"tid\":1}";

        ws = new WebSocket("wss://kr-ss1.chat.naver.com/chat");
        var sslProtocolHack = (System.Security.Authentication.SslProtocols)(SslProtocolsHack.Tls12 | SslProtocolsHack.Tls11 | SslProtocolsHack.Tls);
        ws.SslConfiguration.EnabledSslProtocols = sslProtocolHack;

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
                    string msg = d.bdy[i].msg;

                    if (collecting)
                    {
                        if(!User.Contains(profile.nickname) && !vote) AddUser(profile);
                        else if (vote)
                        {
                            if (msg.Contains(" ") && msg.Split(" ")[0] == "!투표")
                            {
                                if (!User.Contains(profile.nickname))
                                {
                                    try
                                    {
                                        choice.Add(int.Parse(msg.Split(" ")[1]));
                                        AddUser(profile);
                                    }
                                    catch (FormatException E)
                                    {
                                        Debug.LogError("FormatException: " + E.Message);
                                    }
                                    catch (OverflowException E)
                                    {
                                        Debug.LogError("OverflowException: " + E.Message);
                                    }
                                    catch (ArgumentNullException E)
                                    {
                                        Debug.LogError("ArgumentNullException: " + E.Message);
                                    }
                                    catch (Exception E)
                                    {
                                        Debug.LogError("Unexpected Exception: " + E.Message);
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        choice[User.IndexOf(profile.nickname)] = int.Parse(msg.Split(" ")[1]);
                                    }
                                    catch (FormatException E)
                                    {
                                        Debug.LogError("FormatException: " + E.Message);
                                    }
                                    catch (OverflowException E)
                                    {
                                        Debug.LogError("OverflowException: " + E.Message);
                                    }
                                    catch (ArgumentNullException E)
                                    {
                                        Debug.LogError("ArgumentNullException: " + E.Message);
                                    }
                                    catch (Exception E)
                                    {
                                        Debug.LogError("Unexpected Exception: " + E.Message);
                                    }
                                }
                            }
                        }
                    }

                    if (chatOn && profile.nickname == winner)
                    {
                        chatMsg.Add(d.bdy[i].msg);
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
        stopConnect = false;
        StartCoroutine(HeartBeat());
    }

    void ws_OnClose(object sender, CloseEventArgs e)
    {
        if (!stopConnect) Connect();
    }
}
