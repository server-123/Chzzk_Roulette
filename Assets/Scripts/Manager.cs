using UnityEngine;

public class Manager : MonoBehaviour
{
    ChzzkChat chz;

    void Start()
    {
        chz = GetComponent<ChzzkChat>();
    }

    public void Quit()
    {
        chz.Disconncect();
        Application.Quit();
    }
}
