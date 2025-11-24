using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject menuPanel;

    public void GameOver()
    {
        if(gameoverPanel !=null) gameoverPanel.SetActive(true);
    }

    public void Restart()
    {
        Debug.Log("Restart");
        SceneManager.LoadScene("InGame");
    }

    public void TItle()
    {
        Debug.Log("Title");
    }

    public void Pause()
    {
        if(menuPanel != null)
        {
            Time.timeScale = 0f;
            menuPanel.SetActive(true);
        }
    }

    public void Resume()
    {
        if(menuPanel != null)
        {
            Time.timeScale = 1.0f;
            menuPanel.SetActive(false);
        }
        
    }
    
    public void SoundSwitch()
    {

    }
}
