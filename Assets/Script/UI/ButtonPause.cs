using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MagicCubeManager;

public class ButtonPause : MonoBehaviour
{
    public void PauseGame()
    {
        GameStateManager.Instance.SetGameState(GAME_STATE.PAUSE);
        AudioManager.Instance.PauseAllSound();
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        AudioManager.Instance.UnpauseAllSound();
        GameStateManager.Instance.SetGameState(GameStateManager.Instance.GetPreviousState());
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.Instance.ChangeScene(SceneState.LOADING_SCENE);
    }
}