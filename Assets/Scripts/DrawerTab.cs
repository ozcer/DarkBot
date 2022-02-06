using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DrawerTab : MonoBehaviour, IPointerClickHandler
{
    public RectTransform content;
    public float openDuration = 1f;
    public float closeDuration = 1f;
    public LeanTweenType openEase;
    public LeanTweenType closeEase;
    bool _isOpen = true;

    public UnityEvent onOpen = new UnityEvent();
    public UnityEvent onClose = new UnityEvent();
    
    void Start()
    {
        Close(instant: true);
    }

    public void Toggle()
    {
        if (_isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }
    }

    void Open(bool instant=false)
    {
        // TODO get y programatically
        float y = 624;
        float time = instant ? 0f : openDuration;
        LeanTween.moveY(
            content.gameObject,
            y,
            time).setEase(openEase);
       
        _isOpen = true;
        onOpen.Invoke();
    }

    void Close(bool instant=false)
    {
        // TODO get y programatically
        float y = 1150;
        float time = instant ? 0f : closeDuration;
        LeanTween.moveY(
            content.gameObject,
            y,
            time).setEase(closeEase);
        
        _isOpen = false;
        onClose.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Toggle();
    }
}
