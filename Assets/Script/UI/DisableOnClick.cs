using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisableOnClick : MonoBehaviour {
    private Image _buttonImage;

    // Use this for initialization
    void Awake() {
        _buttonImage = GetComponent<Image>();
    }

    public void OnClick() {
        _buttonImage.enabled = false;
    }
}
