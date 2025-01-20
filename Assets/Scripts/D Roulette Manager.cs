using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DRouletteManager : MonoBehaviour
{
    public ChzzkChat chz;

    public GameObject DContent;
    public Button start;

    public GameObject Item;
    public int SelectedItem;
    public bool Private = false;

    public int price = 2000;
    public bool isRolling;
    public int currentIndex = 0;

    void Update()
    {
        donationProccess();

        if (chz.DR)
        {
            int Child = DContent.transform.childCount;

            if (Child > 0)
            {
                for (int i = 0; i < Child; i++)
                {
                    GameObject it = DContent.transform.GetChild(i).gameObject;
                    Item item = it.GetComponent<Item>();

                    item.index = i;
                }
            }
            else if (Child == 0) NewItem();

            if (Child <= 2) start.interactable = false;
            else
            {
                start.interactable = true;
            }
        }
        else
        {
            int Child = DContent.transform.childCount;

            if (Child > 0)
            {
                for (int i = 0; i < Child; i++)
                {
                    Destroy(DContent.transform.GetChild(i).gameObject);
                }
            }
        }
    }

    public void NewItem()
    {
        int Child = DContent.transform.childCount;

        if (Child > 0)
        {
            for (int i = 0; i < Child; i++)
            {
                GameObject it = DContent.transform.GetChild(i).gameObject;
                Item items = it.GetComponent<Item>();

                if (items.field.text == "")
                {
                    if (items.index + 1 < Child) Destroy(it);
                    return;
                }
            }
        }

        Instantiate(Item, DContent.transform);
    }

    void donationProccess()
    {
        if (!isRolling)
        {
            Donation donation;
            if (currentIndex < chz.donation.Count) donation = chz.donation[currentIndex];
            else donation = null;

            if (donation != null)
            {
                string nickname = "익명의 후원자";
                if (donation.nickname != null) nickname = donation.nickname;

                Debug.Log($"{nickname} 님이 {string.Format("{0:n0}", price)}원 후원\n");
                currentIndex++;
            }
        }
    }
}
