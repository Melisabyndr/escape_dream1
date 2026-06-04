using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject infoPanel;

    public void ShowInfo()
    {
        Debug.Log("Buton çalıştı!");
        infoPanel.SetActive(true);
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
    }
}