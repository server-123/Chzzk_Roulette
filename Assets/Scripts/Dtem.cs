using UnityEngine;
using UnityEngine.UI;

public class Dtem : MonoBehaviour
{
    public int index;
    public float percent;

    public Text num;
    public InputField field;
    public InputField Per;
    public GameObject X;

    public ChzzkChat chz;
    public DRouletteManager dm;

    void Awake()
    {
        chz = GameObject.Find("Manager").GetComponent<ChzzkChat>();
        dm = GameObject.Find("Manager").GetComponent<DRouletteManager>();

        field.interactable = true;
        Per.interactable = true;
    }

    void Update()
    {
        num.text = (index + 1).ToString();

        if (chz.collecting)
        {
            field.interactable = false;
            Per.interactable = false;
            X.SetActive(false);
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
