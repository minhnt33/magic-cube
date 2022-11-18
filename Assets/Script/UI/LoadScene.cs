using UnityEngine;
using System.Collections;
using MadLevelManager;
using UnityEngine.UI;
using MagicCubeManager;

public class LoadScene : MonoBehaviour
{
    public Image _fadeImage;
    public GameObject _decorationGroup;

    public void LoadSceneByName(string name)
    {
        if (name == "MenuScene")
            SceneManager.Instance.SetCurrentSceneState(SceneState.MENU_SCENE);
        else if (name == "LevelSelectionScene")
            SceneManager.Instance.SetCurrentSceneState(SceneState.LEVEL_SCENE);

        Application.LoadLevel(name);
    }

    public void LoadFadeInSceneByName(string name)
    {
        _fadeImage.enabled = true;

        if (_decorationGroup)
            _decorationGroup.SetActive(false);

        StartCoroutine(FadeCoroutine(name, 0f, 1f, 0.5f));
    }

    public void LoadFadeOutSceneByName(string name)
    {
        _fadeImage.enabled = true;
        if (_decorationGroup)
            _decorationGroup.SetActive(false);

        StartCoroutine(FadeCoroutine(name, 1f, 0f, 0.5f));
    }

    IEnumerator FadeCoroutine(string sceneName, float aFrom, float aTo, float duration)
    {
        float timer = duration;
        float progress = 0f;

        Color initColor = _fadeImage.color;
        initColor.a = aFrom;
        _fadeImage.color = initColor;
        Color newColor = initColor;
        newColor.a = aTo;

        while (true)
        {
            timer -= Time.deltaTime;
            progress = Mathf.InverseLerp(duration, 0f, timer);

            _fadeImage.color = Color.Lerp(initColor, newColor, progress);

            if (progress == 1)
            {
                Camera.main.cullingMask = 0;

                if (sceneName == "MenuScene")
                    SceneManager.Instance.SetCurrentSceneState(SceneState.MENU_SCENE);
                else if (sceneName == "LevelSelectionScene")
                    SceneManager.Instance.SetCurrentSceneState(SceneState.LEVEL_SCENE);

                Application.LoadLevel(sceneName);
                yield break;
            }

            yield return null;
        }
    }
}
