using UnityEngine;
using UnityEngine.UI;

public class User : MonoBehaviour
{
    public int index;
    public Profile profile = new Profile();

    public Text text;

    public ChzzkChat chz;
    public VoteManager vm;

    void Awake()
    {
        chz = GameObject.Find("Manager").GetComponent<ChzzkChat>();
        vm = GameObject.Find("Manager").GetComponent<VoteManager>();
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
            if (chz.exclude[index]) chz.possible[index] = false;
            else
            {
                if (chz.subOnly)
                {
                    if (chz.sub[index])
                    {
                        if (chz.vote)
                        {
                            if (chz.choice[index] != vm.SelectedItem && vm.SelectedItem != 0) chz.possible[index] = false;
                            else chz.possible[index] = true;
                        }
                        else chz.possible[index] = true;
                    }
                    else chz.possible[index] = false;
                }
                else
                {
                    if (chz.vote)
                    {
                        if (chz.choice[index] != vm.SelectedItem && vm.SelectedItem != 0) chz.possible[index] = false;
                        else chz.possible[index] = true;
                    }
                    else chz.possible[index] = true;
                }
            }

            if (chz.possible[index]) text.color = new Color32(223, 226, 234, 255);
            else text.color = new Color32(63, 63, 63, 255);
        }
    }
}
