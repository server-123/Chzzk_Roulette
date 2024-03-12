using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public int index;
    public Profile profile = new Profile();

    public Text text;

    public ChzzkChat chz;

    void Awake()
    {
        chz = GameObject.Find("Manager").GetComponent<ChzzkChat>();
        text.text = profile.nickname;
    }

    void Update()
    {
        if (!chz.User.Contains(profile.nickname))
        {
            Destroy(this.gameObject);
        }
        else
        {
            if (chz.exclude[index])
            {
                chz.possible[index] = false;
            }
            else
            {
                if (chz.subOnly)
                {
                    if(chz.sub[index]) chz.possible[index] = true;
                    else chz.possible[index] = false;
                }
                else chz.possible[index] = true;
            }

            if (chz.possible[index]) text.color = new Color32(223, 226, 234, 255);
            else text.color = new Color32(63, 63, 63, 255);
        }
    }
}
