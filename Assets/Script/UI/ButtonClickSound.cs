using UnityEngine;
using System.Collections;

public class ButtonClickSound : MonoBehaviour
{
    public void PlaySound(AudioClip clip)
    {
        AudioManager.Instance.PlayOne(clip);
    }
}
