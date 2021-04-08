using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    private bool isPointerDown;
    private float timer =0;
    private int tapCount = 0;

    public float doubletapThreshold;
    public UnityEvent onHoldEvent;
    public UnityEvent onDoubleTap;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GetComponent<Button>().interactable)
        {
            tapCount++;
            isPointerDown = true;
            if (timer > 0 && timer < doubletapThreshold && tapCount > 1)
            {
                onDoubleTap.Invoke();
                tapCount = 0;
            }
            timer = 0;
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (isPointerDown)
        {
            if(onHoldEvent != null)
            {
                onHoldEvent.Invoke();
            }
        }
    }
}
