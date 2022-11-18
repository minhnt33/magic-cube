using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public ButtonToggle _toggler;

    private Button _button;

    // Use this for initialization
    void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnClick()
    {
        bool soundState = PrefManager.Instance.GetSoundSetting();

        soundState = !soundState;

        PrefManager.Instance.SaveSoundSetting(soundState);
        AudioManager.Instance.SetAudiosState(!soundState);
        _toggler.ChangeSprite(_button);
    }
}
