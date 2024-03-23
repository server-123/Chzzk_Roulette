using UnityEngine;
using UnityEngine.UI;

public class Message : MonoBehaviour
{
    ChzzkChat chz;
    Text text;

    void Awake()
    {
        chz = GameObject.Find("Manager").GetComponent<ChzzkChat>();
        text = GetComponent<Text>();
    }

    void Update()
    {
        if (!chz.chatMsg.Contains(text.text)) Destroy(this.gameObject);
    }
}

