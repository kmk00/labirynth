using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void MenuScene()
    {
        SceneManager.LoadScene(0);
    }
    public void DFSScene()
    {
        SceneManager.LoadScene(1);
    }
    public void BFSScene()
    {
        SceneManager.LoadScene(2);
    }
    public void Q_LearningScene()
    {
        SceneManager.LoadScene(3);
    }
    public void DFSVsPlayerScene()
    {
        SceneManager.LoadScene(4);
    }
    public void BFSVsPlayerScene()
    {
        SceneManager.LoadScene(5);
    }
    public void PlayerVsQ_LearningScene()
    {
        SceneManager.LoadScene(6);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
