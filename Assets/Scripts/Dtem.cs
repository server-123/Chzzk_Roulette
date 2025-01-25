using System;
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
        try
        {
            percent = float.Parse(Per.text);
        }
        catch (FormatException E)
        {
            Debug.LogError("FormatException: " + E.Message);
            Per.text = "0";
        }
        catch (OverflowException E)
        {
            Debug.LogError("OverflowException: " + E.Message);
            Per.text = "0";
        }
        catch (ArgumentNullException E)
        {
            Debug.LogError("ArgumentNullException: " + E.Message);
            Per.text = "0";
        }
        catch (Exception E)
        {
            Debug.LogError("Unexpected Exception: " + E.Message);
            Per.text = "0";
        }

        if (chz.collecting)
        {
            field.interactable = false;
            Per.interactable = false;
            X.SetActive(false);
        }
        else
        {
            num.text = (index + 1).ToString();

            if(index != dm.ChildCount() - 1)
            {
                field.interactable = true;
                Per.interactable = true;
                X.SetActive(true);
            }
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
