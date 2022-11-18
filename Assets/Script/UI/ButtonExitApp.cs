using UnityEngine;
using System.Collections;

public class ButtonExitApp : MonoBehaviour
{
    public void ExitApp()
    {
        PrefManager.Instance.ResetGamePlayCount();
        Application.Quit();
    }
}
