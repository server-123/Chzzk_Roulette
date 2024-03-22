using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int index;
    public Text num;
    public Button btn;
    public InputField field;

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
        }
        else
        {
            btn.interactable = true;
            field.interactable = false;
        }
    }
}
