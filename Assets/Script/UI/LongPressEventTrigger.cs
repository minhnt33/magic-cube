using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class LongPressEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    public float durationThreshold = 1.0f;

    public UnityEvent onLongPress = new UnityEvent();
    public UnityEvent onUnpress = new UnityEvent();

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;

    private Coroutine _updateCor;

    private IEnumerator UpdateEvent()
    {
        while (true)
        {
            if (isPointerDown && !longPressTriggered)
            {
                if (Time.time - timePressStarted > durationThreshold)
                {
                    longPressTriggered = true;
                    onLongPress.Invoke();
                }
            }
            else
                yield break;

            yield return null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;

        _updateCor = StartCoroutine(UpdateEvent());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
        onUnpress.Invoke();
        StopCoroutine(_updateCor);
        _updateCor = null;
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerDown = false;
    }
}