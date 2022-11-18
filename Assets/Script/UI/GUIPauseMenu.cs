using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GUIPauseMenu : MonoBehaviour
{
    public GameObject[] _childArray;

    private Image _image;

    void OnEnable()
    {
        GameStateManager.OnGameStateChange += OnGameStateChange;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateChange -= OnGameStateChange;
    }

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void OnGameStateChange(GAME_STATE newState)
    {
        if (newState == GAME_STATE.PAUSE)
            SetChildActive(true);
        else if (GameStateManager.Instance.GetPreviousState() == GAME_STATE.PAUSE)
            SetChildActive(false);
    }

    private void SetChildActive(bool isActive)
    {
        _image.enabled = isActive;
        for (int i = 0; i < _childArray.Length; i++)
        {
            _childArray[i].SetActive(isActive);
        }
    }
}
