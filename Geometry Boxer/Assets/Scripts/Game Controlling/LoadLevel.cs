using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadLevel : MonoBehaviour
{
    public enum LevelsBuildNum
    {
        MainMenuScene = 0,
        ChooseFactionScene = 1,
        LevelSelectScene = 2,
        CubeLevelScene1 = 3
    };
    /// <summary>
    /// When called, loads in the scene at the given build order
    /// number.  If scene doesn't exist, the main menu will be loaded.
    /// </summary>
    /// <param name="level">The value that determines which scene is loaded.</param>
    public void LoadALevel(int level)
    {
        if(level < 0) //check other potential problems
        {
            level = (int)LevelsBuildNum.MainMenuScene;
        }
        SceneManager.LoadScene(level); //maybe use async version?
        Time.timeScale = 1;
    }

    /// <summary>
    /// Function to quit the game when called. Quits as soon as possible.
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Function to move to the next scene in the current
    /// build order.
    /// </summary>
    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
