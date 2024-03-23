using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class VoteManager : MonoBehaviour
{
    public ChzzkChat chz;

    public GameObject Content;
    public GameObject VoteContent;
    public Button Start;

    public GameObject Item;
    public int SelectedItem;
    public bool Private = false;

    void Update()
    {
        if (chz.vote)
        {
            int child = Content.transform.childCount;
            int voteChild = VoteContent.transform.childCount;

            if(child > 0)
            {
                for(int i = 0; i < child; i++)
                {
                    GameObject u = Content.transform.GetChild(i).gameObject;
                    User user = u.GetComponent<User>();

                    if (!chz.User.Contains(user.profile.nickname)) Destroy(u);
                    else if (SelectedItem == 0) u.SetActive(true);
                    else u.SetActive(chz.choice[user.index] == SelectedItem);
                }
            }

            if (voteChild > 0)
            {
                for (int i = 0; i < voteChild; i++)
                {
                    GameObject it = VoteContent.transform.GetChild(i).gameObject;
                    Item item = it.GetComponent<Item>();

                    item.index = i;
                }
            }
            else if (voteChild == 0) NewItem();

            if(voteChild == 1) Start.interactable = false;
            else
            {
                Start.interactable = true;
            }

            for (int i = 0; i < chz.choice.Count; i++)
            {
                if (chz.choice[i] > voteChild || chz.choice[i] < 1)
                {
                    chz.choice.RemoveAt(i);
                    chz.User.RemoveAt(i);
                    chz.sub.RemoveAt(i);
                    chz.exclude.RemoveAt(i);
                    chz.possible.RemoveAt(i);
                    i--;
                }
            }
        }
        else
        {
            Start.interactable = true;

            int voteChild = VoteContent.transform.childCount;

            if (voteChild > 0)
            {
                for (int i = 0; i < voteChild; i++)
                {
                    Destroy(VoteContent.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    public void NewItem()
    {
        int voteChild = VoteContent.transform.childCount;

        if (voteChild > 0)
        {
            for (int i = 0; i < voteChild; i++)
            {
                GameObject it = VoteContent.transform.GetChild(i).gameObject;
                Item items = it.GetComponent<Item>();

                if (items.field.text == "")
                {
                    if (items.index + 1 < voteChild) Destroy(it);
                    return;
                }
            }
        }

        GameObject item = Instantiate(Item);
        item.transform.SetParent(VoteContent.transform);
    }
}
