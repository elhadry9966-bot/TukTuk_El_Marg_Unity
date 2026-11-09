using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles on-screen UI buttons that can be held down (Gas, Brake, Steer Left, Steer Right).
/// Supports multi-touch and mouse clicks.
/// </summary>
public class HoldButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public string controlName;
    public bool isPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPressed = false;
    }

    private void OnDisable()
    {
        isPressed = false;
    }
}
