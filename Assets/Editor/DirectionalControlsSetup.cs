#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class DirectionalControlsSetup
{
    private const string CanvasName = "GameCanvas";
    private const string PlayerName = "Player";

    [MenuItem("Tools/Graze Overdrive/Create Directional Controls")]
    public static void CreateDirectionalControls()
    {
        PlayerController player = FindPlayer();

        if (player == null)
        {
            Debug.LogError(
                $"'{PlayerName}' 오브젝트에서 PlayerController를 찾지 못했습니다.\n" +
                "먼저 Player 오브젝트에 PlayerController를 추가하세요."
            );

            return;
        }

        RemoveExistingCanvas();

        Canvas canvas = CreateCanvas();
        RectTransform safeArea = CreateSafeArea(canvas.transform);
        RectTransform controlsRoot = CreateControlsRoot(safeArea);

        CreateDirectionButton(
            controlsRoot,
            "UpButton",
            "▲",
            new Vector2(0f, 140f),
            Vector2.up,
            player
        );

        CreateDirectionButton(
            controlsRoot,
            "DownButton",
            "▼",
            new Vector2(0f, -140f),
            Vector2.down,
            player
        );

        CreateDirectionButton(
            controlsRoot,
            "LeftButton",
            "◀",
            new Vector2(-140f, 0f),
            Vector2.left,
            player
        );

        CreateDirectionButton(
            controlsRoot,
            "RightButton",
            "▶",
            new Vector2(140f, 0f),
            Vector2.right,
            player
        );

        EnsureEventSystem();
        MarkSceneDirty();

        Selection.activeGameObject = canvas.gameObject;

        Debug.Log(
            "방향 조작 UI를 생성했습니다. " +
            "GameCanvas > SafeArea > ControlsRoot에서 위치와 크기를 조정할 수 있습니다."
        );
    }

    private static PlayerController FindPlayer()
    {
        GameObject playerObject = GameObject.Find(PlayerName);

        if (playerObject != null &&
            playerObject.TryGetComponent(out PlayerController player))
        {
            return player;
        }

        return Object.FindFirstObjectByType<PlayerController>();
    }

    private static void RemoveExistingCanvas()
    {
        GameObject existing = GameObject.Find(CanvasName);

        if (existing == null)
            return;

        bool replace = EditorUtility.DisplayDialog(
            "기존 UI 발견",
            $"{CanvasName} 오브젝트가 이미 있습니다.\n삭제하고 새로 생성할까요?",
            "교체",
            "취소"
        );

        if (!replace)
            throw new System.OperationCanceledException();

        Undo.DestroyObjectImmediate(existing);
    }

    private static Canvas CreateCanvas()
    {
        GameObject canvasObject = new(
            CanvasName,
            typeof(RectTransform),
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        Undo.RegisterCreatedObjectUndo(canvasObject, "Create directional controls");

        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        return canvas;
    }

    private static RectTransform CreateSafeArea(Transform parent)
    {
        GameObject safeAreaObject = new(
            "SafeArea",
            typeof(RectTransform),
            typeof(SafeAreaFitter)
        );

        Undo.RegisterCreatedObjectUndo(safeAreaObject, "Create safe area");
        safeAreaObject.transform.SetParent(parent, false);

        RectTransform rect = safeAreaObject.GetComponent<RectTransform>();
        StretchToParent(rect);

        return rect;
    }

    private static RectTransform CreateControlsRoot(Transform parent)
    {
        GameObject controlsObject = new(
            "ControlsRoot",
            typeof(RectTransform)
        );

        Undo.RegisterCreatedObjectUndo(controlsObject, "Create controls root");
        controlsObject.transform.SetParent(parent, false);

        RectTransform rect = controlsObject.GetComponent<RectTransform>();

        // 왼쪽 아래 기준
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.zero;
        rect.pivot = Vector2.zero;

        // 가장자리에서 조금 띄운 위치
        rect.anchoredPosition = new Vector2(70f, 90f);
        rect.sizeDelta = new Vector2(500f, 500f);

        return rect;
    }

    private static void CreateDirectionButton(
        Transform parent,
        string objectName,
        string symbol,
        Vector2 position,
        Vector2 direction,
        PlayerController player)
    {
        GameObject buttonObject = new(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Image),
            typeof(Button),
            typeof(DirectionButton)
        );

        Undo.RegisterCreatedObjectUndo(buttonObject, $"Create {objectName}");
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = position;
        rect.sizeDelta = new Vector2(130f, 130f);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(0.05f, 0.95f, 1f, 0.40f);

        Button button = buttonObject.GetComponent<Button>();
        button.transition = Selectable.Transition.ColorTint;

        ColorBlock colors = button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(1f, 1f, 1f, 0.85f);
        colors.pressedColor = new Color(0.55f, 1f, 1f, 1f);
        colors.selectedColor = Color.white;
        colors.disabledColor = new Color(1f, 1f, 1f, 0.25f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.05f;
        button.colors = colors;

        DirectionButton directionButton =
            buttonObject.GetComponent<DirectionButton>();

        SerializedObject serializedButton =
            new SerializedObject(directionButton);

        serializedButton.FindProperty("player").objectReferenceValue = player;
        serializedButton.FindProperty("direction").vector2Value = direction;
        serializedButton.ApplyModifiedPropertiesWithoutUndo();

        CreateButtonLabel(buttonObject.transform, symbol);
    }

    private static void CreateButtonLabel(
        Transform parent,
        string symbol)
    {
        GameObject labelObject = new(
            "Label",
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text)
        );

        Undo.RegisterCreatedObjectUndo(labelObject, "Create button label");
        labelObject.transform.SetParent(parent, false);

        RectTransform rect = labelObject.GetComponent<RectTransform>();
        StretchToParent(rect);

        Text text = labelObject.GetComponent<Text>();
        text.text = symbol;
        text.alignment = TextAnchor.MiddleCenter;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 58;
        text.fontStyle = FontStyle.Bold;
        text.color = Color.white;
        text.raycastTarget = false;
    }

    private static void EnsureEventSystem()
    {
        EventSystem existing =
            Object.FindFirstObjectByType<EventSystem>();

        if (existing != null)
            return;

        GameObject eventSystemObject = new(
            "EventSystem",
            typeof(EventSystem)
        );

        Undo.RegisterCreatedObjectUndo(
            eventSystemObject,
            "Create EventSystem"
        );

        // Input System UI 모듈이 설치되어 있으면 우선 사용한다.
        System.Type inputSystemModuleType = System.Type.GetType(
            "UnityEngine.InputSystem.UI.InputSystemUIInputModule, " +
            "Unity.InputSystem"
        );

        if (inputSystemModuleType != null)
        {
            eventSystemObject.AddComponent(inputSystemModuleType);
        }
        else
        {
            eventSystemObject.AddComponent<StandaloneInputModule>();
        }
    }

    private static void StretchToParent(RectTransform rect)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;
        rect.sizeDelta = Vector2.zero;
    }

    private static void MarkSceneDirty()
    {
        EditorSceneManager.MarkSceneDirty(
            EditorSceneManager.GetActiveScene()
        );
    }
}

#endif
    