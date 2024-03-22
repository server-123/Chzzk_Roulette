using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoteManager : MonoBehaviour
{
    public ChzzkChat chz;

    public GameObject Content;
    public GameObject VoteContent;
    public GameObject Item;
    public int SelectedItem;

    void Update()
    {
        //Vote Content Child ÀÎµ¦½º ºÎ¿©
        if (chz.vote)
        {
            int child = Content.transform.childCount;

            if(child > 0)
            {
                for(int i = 0; i < child; i++)
                {
                    GameObject u = Content.transform.GetChild(i).gameObject;
                    User user = u.GetComponent<User>();

                    u.SetActive(chz.choice[user.index] == SelectedItem);
                }
            }
        }
    }
}
