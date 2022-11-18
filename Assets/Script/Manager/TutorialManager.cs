using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{

    public Sprite[] _pageSprites;

    public Image _pageImage;
    public GameObject _rightHandle;
    public GameObject _leftHandle;
    public Text _pageIndexText;

    private int _currentPageIndex = 0;

    void Awake()
    {
        SetNewPage(0);
    }

    public void NextPage()
    {
        _currentPageIndex++;
        SetNewPage(_currentPageIndex);
    }

    public void PreviousPage()
    {
        _currentPageIndex--;
        SetNewPage(_currentPageIndex);
    }

    private void SetNewPage(int index)
    {
        if (index == _pageSprites.Length - 1)
            _rightHandle.SetActive(false);
        else if (index == 0)
            _leftHandle.SetActive(false);
        else
        {
            _rightHandle.SetActive(true);
            _leftHandle.SetActive(true);
        }

        _pageImage.sprite = _pageSprites[index];
        _pageIndexText.text = (_currentPageIndex + 1) + "/" + _pageSprites.Length;
    }
}
