using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int index;
    public Text num;
    public Button btn;
    public InputField field;

    public GameObject User;
    public GameObject Per;
    public GameObject PerBar;
    public GameObject X;

    public ChzzkChat chz;
    public VoteManager vm;

    void Awake()
    {
        chz = GameObject.Find("Manager").GetComponent<ChzzkChat>();
        vm = GameObject.Find("Manager").GetComponent<VoteManager>();
    }

    void Update()
    {
        num.text = (index + 1).ToString();

        if (!chz.collecting)
        {
            btn.interactable = false;
            field.interactable = true;
            User.SetActive(false);
            Per.SetActive(false);
            PerBar.SetActive(false);
            X.SetActive(true);
        }
        else
        {
            btn.interactable = true;
            field.interactable = false;
            User.SetActive(true);
            Per.SetActive(true);
            PerBar.SetActive(true);
            X.SetActive(false);

            if (vm.Private)
            {
                User.GetComponent<Text>().text = "?��";
                Per.GetComponent<Text>().text = "?%";
                PerBar.GetComponent<Slider>().value = 0;
            }
            else
            {
                if (chz.choice.Count != 0)
                {
                    int amount = 0;

                    foreach (int i in chz.choice)
                    {
                        if (i == index + 1) amount++;
                    }

                    User.GetComponent<Text>().text = amount.ToString() + "��";
                    Per.GetComponent<Text>().text = (((float)amount/chz.choice.Count) * 100).ToString("F1") + "%";
                    PerBar.GetComponent<Slider>().value = (float)amount/chz.choice.Count;
                }
                else
                {
                    User.GetComponent<Text>().text = "0��";
                    Per.GetComponent<Text>().text = "0.0%";
                    PerBar.GetComponent<Slider>().value = 0;
                }
            }
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
