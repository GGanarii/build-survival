using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;

    [Header("Playfield")]
    [SerializeField] private Vector2 playfieldSize = new(9f, 16f);
    [SerializeField] private float boundaryMargin = 0.35f;

    private Vector2 mobileInput;

    private void Update()
    {
        Vector2 input = ReadKeyboardInput() + mobileInput;

        // 대각선 이동 시 속도가 더 빨라지는 것을 방지한다.
        input = Vector2.ClampMagnitude(input, 1f);

        Vector3 position = transform.position;
        position += (Vector3)(input * moveSpeed * Time.deltaTime);

        float maxX = playfieldSize.x * 0.5f - boundaryMargin;
        float maxY = playfieldSize.y * 0.5f - boundaryMargin;

        position.x = Mathf.Clamp(position.x, -maxX, maxX);
        position.y = Mathf.Clamp(position.y, -maxY, maxY);

        transform.position = position;
    }

    public void SetMobileInput(Vector2 input)
    {
        mobileInput = Vector2.ClampMagnitude(input, 1f);
    }

    private static Vector2 ReadKeyboardInput()
    {
        if (Keyboard.current == null)
            return Vector2.zero;

        Vector2 input = Vector2.zero;

        if (Keyboard.current.leftArrowKey.isPressed ||
            Keyboard.current.aKey.isPressed)
        {
            input.x -= 1f;
        }

        if (Keyboard.current.rightArrowKey.isPressed ||
            Keyboard.current.dKey.isPressed)
        {
            input.x += 1f;
        }

        if (Keyboard.current.upArrowKey.isPressed ||
            Keyboard.current.wKey.isPressed)
        {
            input.y += 1f;
        }

        if (Keyboard.current.downArrowKey.isPressed ||
            Keyboard.current.sKey.isPressed)
        {
            input.y -= 1f;
        }

        return input;
    }
}
    