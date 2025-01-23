using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DRouletteManager : MonoBehaviour
{
    public ChzzkChat chz;

    public GameObject DContent;
    public Button start;
    public GameObject Item;

    public List<DonationRecord> donation;
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
                    Dtem item = it.GetComponent<Dtem>();

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
                Dtem items = it.GetComponent<Dtem>();

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
            if (currentIndex < donation.Count)
            {
                if(donation[currentIndex].reward == -1) donation[currentIndex].reward = Random.Range(0, DContent.transform.childCount);
                Debug.Log($"{donation[currentIndex].nickName} ´ÔÀÌ {string.Format("{0:n0}", price)}¿ø ÈÄ¿ø\n");
                currentIndex++;
            }
        }
    }
}
