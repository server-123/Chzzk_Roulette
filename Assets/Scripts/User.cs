using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public Profile profile = new Profile();
    public bool sub = false;
    public bool exclude = false;

    public Text text;

    void Start()
    {
        text.text = profile.nickname;
        sub = (profile.streamingProperty.subscription.tier != 0);
    }

    void Update()
    {
        if (exclude)
        {
            text.color = new Color32(63, 63, 63, 255);
        }
        else
        {
            text.color = new Color32(223, 226, 234, 255);
        }
    }
}
