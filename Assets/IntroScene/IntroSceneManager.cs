using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroSceneManager : MonoBehaviour
{
    public VideoPlayer vp;

    void Start()
    {
        vp.Play();
        vp.loopPointReached += VideoBitti;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("escape dreamm");
        }
    }

    void VideoBitti(VideoPlayer player)
    {
        SceneManager.LoadScene("escape dreamm");
    }
}