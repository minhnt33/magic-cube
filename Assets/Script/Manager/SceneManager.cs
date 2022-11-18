using UnityEngine;
using System.Collections;

namespace MagicCubeManager
{
    public class SceneManager : MonoBehaviour
    {
        private static SceneManager _instance;

        public static SceneManager Instance
        {
            get
            {
                return _instance;
            }
        }

        [SerializeField]
        private SceneState _curSceneState;

        public delegate void OnSceneChangeEvent(SceneState newSceneState);
        public static event OnSceneChangeEvent OnSceneChange;

        // Use this for initialization
        void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        public void ChangeScene(SceneState newScene)
        {
            SetCurrentSceneState(newScene);

            string name = "";

            if (newScene == SceneState.MENU_SCENE)
                name = "MenuScene";
            else if (newScene == SceneState.LEVEL_SCENE)
                name = "LevelSelectionScene";
            else if (newScene == SceneState.GAME_SCENE)
                name = "GameScene";
            else if (newScene == SceneState.LOADING_SCENE)
                name = "LevelLoadingScene";

            Application.LoadLevel(name);
        }

        public void ChangeSceneAsync(SceneState newScene)
        {
            StartCoroutine(LoadingAsyncProcess(newScene));
        }

        IEnumerator LoadingAsyncProcess(SceneState newScene)
        {
            AsyncOperation async = Application.LoadLevelAsync(GetSceneIndex(newScene));
            async.allowSceneActivation = false;

            while (!async.isDone)
            {
                if (async.progress == 0.9f)
                {
                    SceneManager.Instance.SetCurrentSceneState(newScene);
                    async.allowSceneActivation = true;
                    yield break;
                }
                yield return null;
            }
        }

        public void SetCurrentSceneState(SceneState newState)
        {
            _curSceneState = newState;

            if (OnSceneChange != null)
                OnSceneChange(newState);
        }

        public SceneState GetCurrentSceneState()
        {
            return _curSceneState;
        }

        public int GetCurrentSceneIndex()
        {
            return (int)_curSceneState;
        }

        public int GetSceneIndex(SceneState state)
        {
            return (int)state;
        }
    }
}
