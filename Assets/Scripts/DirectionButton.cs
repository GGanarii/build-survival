using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class DirectionButton :
    MonoBehaviour,
    IPointerDownHandler,
    IPointerUpHandler,
    IPointerExitHandler
{
    [SerializeField] private PlayerController player;
    [SerializeField] private Vector2 direction;

    private static readonly HashSet<DirectionButton> PressedButtons = new();

    public void OnPointerDown(PointerEventData eventData)
    {
        PressedButtons.Add(this);
        ApplyCombinedInput();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Release();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Release();
    }

    private void OnDisable()
    {
        Release();
    }

    private void OnDestroy()
    {
        Release();
    }

    private void Release()
    {
        PressedButtons.Remove(this);
        ApplyCombinedInput();
    }

    private void ApplyCombinedInput()
    {
        if (player == null)
            return;

        Vector2 combined = Vector2.zero;

        foreach (DirectionButton button in PressedButtons)
        {
            if (button != null && button.player == player)
                combined += button.direction;
        }

        player.SetMobileInput(
            Vector2.ClampMagnitude(combined, 1f)
        );
    }
}
