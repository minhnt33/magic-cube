using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class HoldEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityEvent onHold = new UnityEvent();
    public UnityEvent onUnhold = new UnityEvent();

    private bool isPointerDown = false;
    private Coroutine _updateCor;

    public void OnPointerDown(PointerEventData eventData)
    {
        isPointerDown = true;
        _updateCor = StartCoroutine(UpdateEvent());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        onUnhold.Invoke();

        if (_updateCor != null)
            StopCoroutine(_updateCor);

        _updateCor = null;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    private IEnumerator UpdateEvent()
    {
        while (true)
        {
            onHold.Invoke();

            if (!isPointerDown)
                yield break;

            yield return null;
        }
    }
}