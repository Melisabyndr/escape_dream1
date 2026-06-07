using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UIManager : MonoBehaviour
{
    public GameObject infoPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("IntroScene");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void ShowInfo()
    {
        infoPanel.SetActive(true);
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
    }
}