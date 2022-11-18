using UnityEngine;
using System.Collections;
using MadLevelManager;
using MagicCubeManager;

public class ButtonChangeScene : MonoBehaviour
{

    public void ChangeSceneByName(string levelName)
    {
        Application.LoadLevel(levelName);
    }

    public void ChangeNextLevel()
    {
        Time.timeScale = 1f;
        MadLevel.LoadNext(MadLevel.Type.Level);
    }

    public void ChangeToLoadingScene()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.ChangeScene(SceneState.LOADING_SCENE);
    }

    public void ChangeToLevelScene()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.ChangeScene(SceneState.LEVEL_SCENE);
    }

    public void ChangeToGameScene()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.ChangeScene(SceneState.GAME_SCENE);
    }

    public void ChangeToMenuScene()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.ChangeScene(SceneState.MENU_SCENE);
    }
}
