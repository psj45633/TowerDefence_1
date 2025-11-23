using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private GameObject gameoverPanel;
    [SerializeField] private GameObject menu;

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
        if(menu != null)
        {
            Time.timeScale = 0f;
            menu.SetActive(true);
        }
    }

    public void Resume()
    {
        if(menu != null)
        {
            Time.timeScale = 1.0f;
            menu.SetActive(false);
        }
        
    }
    
    public void SoundSwitch()
    {

    }
}
