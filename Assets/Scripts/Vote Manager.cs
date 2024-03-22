using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static chat;

public class VoteManager : MonoBehaviour
{
    public ChzzkChat chz;

    public GameObject Content;
    public GameObject VoteContent;
    public GameObject Item;
    public int SelectedItem;

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

                    u.SetActive(chz.choice[user.index] == SelectedItem);
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

                if (items.field.text == "") return;
            }
        }

        GameObject item = Instantiate(Item);
        item.transform.SetParent(VoteContent.transform);
    }
}
