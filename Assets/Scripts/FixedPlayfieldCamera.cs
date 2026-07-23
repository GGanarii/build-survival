using UnityEngine;

[RequireComponent(typeof(Camera))]
public sealed class FixedPlayfieldCamera : MonoBehaviour
{
    [SerializeField] private float playfieldWidth = 9f;
    [SerializeField] private float playfieldHeight = 16f;

    private Camera targetCamera;
    private int previousWidth;
    private int previousHeight;

    private void Awake()
    {
        targetCamera = GetComponent<Camera>();
        targetCamera.orthographic = true;

        ApplyViewport();
    }

    private void Update()
    {
        if (Screen.width == previousWidth && Screen.height == previousHeight)
            return;

        ApplyViewport();
    }

    private void ApplyViewport()
    {
        previousWidth = Screen.width;
        previousHeight = Screen.height;

        float screenAspect = (float)Screen.width / Screen.height;
        float targetAspect = playfieldWidth / playfieldHeight;

        targetCamera.orthographicSize = playfieldHeight * 0.5f;

        if (screenAspect > targetAspect)
        {
            // 태블릿처럼 목표 화면보다 넓을 때 좌우를 비운다.
            float viewportWidth = targetAspect / screenAspect;
            float viewportX = (1f - viewportWidth) * 0.5f;

            targetCamera.rect = new Rect(viewportX, 0f, viewportWidth, 1f);
        }
        else
        {
            // 아주 긴 휴대폰에서는 위아래를 비운다.
            float viewportHeight = screenAspect / targetAspect;
            float viewportY = (1f - viewportHeight) * 0.5f;

            targetCamera.rect = new Rect(0f, viewportY, 1f, viewportHeight);
        }
    }
}
