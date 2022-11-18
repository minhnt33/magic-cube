using UnityEngine;
using System.Collections;

public class ShowingResult : MonoBehaviour
{

    public GameObject[] _starArray;


    public void ShowingStarEvent()
    {
        int star = GameModeManager.Instance.ScoreManager.NumberOfStar;

        for (int i = 0; i < star; i++)
        {
            _starArray[i].SetActive(true);
        }
    }
}
