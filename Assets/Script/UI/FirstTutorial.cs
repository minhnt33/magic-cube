using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FirstTutorial : MonoBehaviour
{
    private Animator _anim;
    public GameObject _cover;

    void Start()
    {
        if (PrefManager.Instance.IsFirstOpeningGame)
        {
            _anim = GetComponent<Animator>();
            _anim.enabled = true;
            _cover.SetActive(true);
        }
    }

    public void OnButtonClick()
    {
        if (_anim != null)
        {
            _anim.enabled = false;
            _cover.SetActive(false);
        }
    }
}
