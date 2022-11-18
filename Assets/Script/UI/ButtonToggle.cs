using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[System.Serializable]
public class ButtonToggle : System.Object {

    public Sprite _defaultSprite;
    public Sprite _toggleSprite;

    private bool _isDefault = true;

    public void ChangeSprite(Button button) {
        if (_isDefault) {
            button.image.sprite = _toggleSprite;
            _isDefault = false;
        }
        else {
            button.image.sprite = _defaultSprite;
            _isDefault = true;
        }
    }

    public bool IsDefaultImage()
    {
        return _isDefault;
    }
}
