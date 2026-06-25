using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public static class SceneSetupTool
{
    [MenuItem("NandemoYa/Setup Main Scene")]
    public static void SetupMainScene()
    {
        CreateManagers();
        CreateCanvas();
        Debug.Log("[SceneSetup] 完了。Inspectorで各スクリプトの参照を設定してください。");
    }

    private static void CreateManagers()
    {
        var root = new GameObject("--- MANAGERS ---");

        AddManager<GameManager>(root);
        AddManager<EconomyManager>(root);
        AddManager<QuestManager>(root);
        AddManager<CharacterManager>(root);
        AddManager<MinigameManager>(root);
        AddManager<DialogueSystem>(root);
        AddManager<ShopSystem>(root);
        AddManager<AchievementManager>(root);
        AddManager<EndingManager>(root);

        var bootstrap = root.AddComponent<GameBootstrap>();
        var controller = root.AddComponent<MainSceneController>();

        Undo.RegisterCreatedObjectUndo(root, "Create Managers");
    }

    private static void AddManager<T>(GameObject parent) where T : Component
    {
        var go = new GameObject(typeof(T).Name);
        go.transform.SetParent(parent.transform);
        go.AddComponent<T>();
    }

    private static void CreateCanvas()
    {
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasGo.AddComponent<CanvasScaler>();
        canvasGo.AddComponent<GraphicRaycaster>();

        var scaler = canvasGo.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);

        string[] panels =
        {
            "Panel_DayStart",
            "Panel_QuestBoard",
            "Panel_Dialogue",
            "Panel_Result",
            "Panel_Shop",
            "Panel_Ending",
            "Panel_GameOver",
            "Panel_Minigame_Choice",
            "Panel_Minigame_Search",
            "Panel_Minigame_Puzzle",
            "Panel_Minigame_Timing",
            "Panel_Minigame_Special"
        };

        foreach (var name in panels)
        {
            var panel = CreatePanel(name, canvasGo.transform);
            panel.SetActive(false);
        }

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        Undo.RegisterCreatedObjectUndo(canvasGo, "Create Canvas");
        Undo.RegisterCreatedObjectUndo(eventSystem, "Create EventSystem");
    }

    private static GameObject CreatePanel(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        var img = go.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        AddLabel(go.transform, name.Replace("Panel_", ""), new Vector2(0, 0.85f), new Vector2(1, 1));

        return go;
    }

    private static void AddLabel(Transform parent, string text, Vector2 anchorMin, Vector2 anchorMax)
    {
        var go = new GameObject("Label_" + text);
        go.transform.SetParent(parent);

        var rect = go.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = new Vector2(20, 0);
        rect.offsetMax = new Vector2(-20, 0);

        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 28;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }
}
