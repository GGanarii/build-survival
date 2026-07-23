using UnityEngine;
using UnityEngine.UI;

public sealed class GameOverUI : MonoBehaviour
{
    private GameObject panel;
    private Text resultText;

    private void Start()
    {
        BuildUI();

        if (GameManager.Instance != null)
            GameManager.Instance.GameOverStarted += Show;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.GameOverStarted -= Show;
    }

    private void BuildUI()
    {
        panel = new GameObject(
            "GameOverPanel",
            typeof(RectTransform),
            typeof(Image)
        );

        panel.transform.SetParent(transform, false);

        RectTransform panelRect =
            panel.GetComponent<RectTransform>();

        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.78f);

        resultText = CreateText(
            panel.transform,
            "ResultText",
            new Vector2(0f, 120f),
            new Vector2(800f, 240f),
            72
        );

        Button retryButton = CreateButton(
            panel.transform,
            "RetryButton",
            "RETRY",
            new Vector2(0f, -140f)
        );

        retryButton.onClick.AddListener(Restart);
        panel.SetActive(false);
    }

    private void Show()
    {
        resultText.text =
            $"GAME OVER\n\nTIME  " +
            $"{GameManager.Instance.SurvivalTime:0.0}s";

        panel.SetActive(true);
    }

    private static Text CreateText(
        Transform parent,
        string objectName,
        Vector2 position,
        Vector2 size,
        int fontSize)
    {
        GameObject textObject = new(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text)
        );

        textObject.transform.SetParent(parent, false);

        RectTransform rect =
            textObject.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = size;

        Text text = textObject.GetComponent<Text>();
        text.font =
            Resources.GetBuiltinResource<Font>(
                "LegacyRuntime.ttf"
            );

        text.fontSize = fontSize;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;
        text.raycastTarget = false;

        return text;
    }

    private static Button CreateButton(
        Transform parent,
        string objectName,
        string label,
        Vector2 position)
    {
        GameObject buttonObject = new(
            objectName,
            typeof(RectTransform),
            typeof(Image),
            typeof(Button)
        );

        buttonObject.transform.SetParent(parent, false);

        RectTransform rect =
            buttonObject.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(520f, 150f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.05f, 0.9f, 1f, 0.9f);

        Button button = buttonObject.GetComponent<Button>();

        Text text = CreateText(
            buttonObject.transform,
            "Label",
            Vector2.zero,
            new Vector2(520f, 150f),
            54
        );

        text.text = label;
        text.color = Color.black;

        return button;
    }

    private static void Restart()
    {
        GameManager.Instance?.RestartRun();
    }
}
