using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using MadLevelManager;
using MagicCubeManager;

public class GUILoadingScene : MonoBehaviour
{
    [SerializeField]
    private Image _loadingBar;
    [SerializeField]
    private Text _loadingText;

    public string[] _idiomArray;

    // Use this for initialization
    void Awake()
    {
        _loadingText.text = _idiomArray[Random.Range(0, _idiomArray.Length)];
    }

    void Start()
    {
        StartCoroutine(LoadingProcess());
    }

    IEnumerator LoadingProcess()
    {
        AsyncOperation async = Application.LoadLevelAsync("GameScene");

        async.allowSceneActivation = false;

        float fillProgress = 0f;

        while (!async.isDone)
        {
            fillProgress = async.progress + 0.1f;
            _loadingBar.fillAmount = fillProgress;

            if (async.progress == 0.9f)
            {
                SceneManager.Instance.SetCurrentSceneState(SceneState.GAME_SCENE);
                async.allowSceneActivation = true;
                yield break;
            }
            yield return null;
        }
    }
}
