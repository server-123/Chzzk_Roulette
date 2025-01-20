using UnityEngine;

public class Manager : MonoBehaviour
{
    ChzzkChat chz;

    void Start()
    {
        //Display.displays[1].Activate();
        chz = GetComponent<ChzzkChat>();
    }

    public void Quit()
    {
        chz.Disconncect();
        Application.Quit();
    }
}
