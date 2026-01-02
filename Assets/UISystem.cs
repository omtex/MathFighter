using UnityEngine;
using UnityEngine.UI;

public class UISystem : MonoBehaviour
{
    public GameObject PausedPanel;
    public AudioSource a_source;
    public AudioClip ButtonClickSound;
    public AudioClip PauseSound;
    public AudioClip UnpauseSound;

    void Start()
    {
        a_source = GetComponent<AudioSource>();
    }

   
    #region Pause Game
    public void PauseAndResumeGame()
    {
        GameManager.Instance.IsGamePaused = !GameManager.Instance.IsGamePaused;

        bool isPause = GameManager.Instance.IsGamePaused;
        //a_source.PlayOneShot(ButtonClickSound);
        PausedPanel.SetActive(isPause);

        if (isPause)
        {
            Time.timeScale = 0f;
            GameManager.Instance.PauseGame();
            a_source.PlayOneShot(PauseSound);
        }
        else
        {
            Time.timeScale = 1f;
            GameManager.Instance.ResumeGame();
            a_source.PlayOneShot(UnpauseSound);
        }
    }

    public void ExitMenu()
    {
        GameManager.Instance.ResetGame(false);
        GameManager.Instance.EndGame(false);
        PausedPanel.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    #endregion
}
