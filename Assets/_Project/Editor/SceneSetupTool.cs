using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public static class SceneSetupTool
{
    // ══════════════════════════════════════════════════════════
    //  Main Scene (initial setup — run once on a fresh scene)
    // ══════════════════════════════════════════════════════════
    [MenuItem("NandemoYa/Setup Main Scene")]
    public static void SetupMainScene()
    {
        CreateManagers();
        CreateCanvas();
        CreateQuestEntryPrefab();
        Debug.Log("[SceneSetup] メインシーン初期化完了");
    }

    // ══════════════════════════════════════════════════════════
    //  Minigame Panels (run after Setup Main Scene)
    // ══════════════════════════════════════════════════════════
    [MenuItem("NandemoYa/Setup Minigame Panels")]
    public static void SetupMinigamePanels()
    {
        var canvas = GameObject.Find("Canvas");
        if (canvas == null)
        {
            Debug.Log("[SceneSetup] Canvas が見つかりません。Setup Main Scene を自動実行します...");
            SetupMainScene();
            canvas = GameObject.Find("Canvas");
            if (canvas == null) { Debug.LogError("[SceneSetup] Canvas の作成に失敗しました。"); return; }
        }

        SetupChoicePanel(canvas.transform.Find("Panel_Minigame_Choice")?.gameObject);
        SetupSearchPanel(canvas.transform.Find("Panel_Minigame_Search")?.gameObject);
        SetupPuzzlePanel(canvas.transform.Find("Panel_Minigame_Puzzle")?.gameObject);
        SetupTimingPanel(canvas.transform.Find("Panel_Minigame_Timing")?.gameObject);
        SetupSpecialPanel(canvas.transform.Find("Panel_Minigame_Special")?.gameObject);
        SetupDialoguePanel(canvas.transform.Find("Panel_Dialogue")?.gameObject);
        SetupGameOverPanel(canvas.transform.Find("Panel_GameOver")?.gameObject);
        SetupResultPanel(canvas.transform.Find("Panel_Result")?.gameObject);
        SetupDayStartPanel(canvas.transform.Find("Panel_DayStart")?.gameObject);
        SetupQuestBoardPanel(canvas.transform.Find("Panel_QuestBoard")?.gameObject);
        SetupShopPanel(canvas.transform.Find("Panel_Shop")?.gameObject);
        SetupEndingPanel(canvas.transform.Find("Panel_Ending")?.gameObject);
        WireMinigameManager();
        WireMainSceneController(canvas);
        WireGameBootstrap();
        SaveAsMainScene();

        EditorUtility.SetDirty(canvas);
        Debug.Log("[SceneSetup] ミニゲームパネル設定完了");
    }

    // ══════════════════════════════════════════════════════════
    //  Title Scene
    // ══════════════════════════════════════════════════════════
    [MenuItem("NandemoYa/Setup Title Scene")]
    public static void SetupTitleScene()
    {
        EnsureScenesDir();
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        EditorSceneManager.SetActiveScene(scene);

        // Camera
        var camGo = new GameObject("Main Camera");
        camGo.AddComponent<Camera>().backgroundColor = new Color(0.05f, 0.05f, 0.1f);
        camGo.tag = "MainCamera";

        // Canvas
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGo.AddComponent<GraphicRaycaster>();

        // Background
        var bg = CreatePanel("Background", canvasGo.transform, new Color(0.05f, 0.05f, 0.12f));
        bg.SetActive(true);

        // Title text
        var titleTxt = CreateLabel(canvasGo.transform, "TitleText",
            new Vector2(0.1f, 0.65f), new Vector2(0.9f, 0.85f), 52);
        titleTxt.color = new Color(1f, 0.92f, 0.6f);

        var subtitleTxt = CreateLabel(canvasGo.transform, "SubtitleText",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.65f), 24);
        subtitleTxt.color = new Color(0.85f, 0.85f, 0.85f);

        // Buttons
        var (newGameBtn, newGameLbl) = CreateButton(canvasGo.transform, "NewGameButton",
            new Vector2(0.3f, 0.38f), new Vector2(0.7f, 0.5f));
        newGameLbl.text = "はじめから";
        newGameLbl.fontSize = 24;

        var (continueBtn, continueLbl) = CreateButton(canvasGo.transform, "ContinueButton",
            new Vector2(0.3f, 0.24f), new Vector2(0.7f, 0.36f));
        continueLbl.text = "つづきから";
        continueLbl.fontSize = 24;

        // TitleUI component
        var titleUI = canvasGo.AddComponent<TitleUI>();
        var so = new SerializedObject(titleUI);
        so.FindProperty("newGameButton").objectReferenceValue = newGameBtn;
        so.FindProperty("continueButton").objectReferenceValue = continueBtn;
        so.FindProperty("titleText").objectReferenceValue = titleTxt;
        so.FindProperty("subtitleText").objectReferenceValue = subtitleTxt;
        so.ApplyModifiedPropertiesWithoutUndo();

        // EventSystem
        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();

        const string scenePath = "Assets/_Project/Scenes/TitleScene.unity";
        EditorSceneManager.SaveScene(scene, scenePath);
        EditorSceneManager.CloseScene(scene, true);
        AssetDatabase.Refresh();
        AddSceneToBuildSettings(scenePath);
        Debug.Log("[SceneSetup] TitleScene 作成完了: " + scenePath);
    }

    // ══════════════════════════════════════════════════════════
    //  Internal — Managers
    // ══════════════════════════════════════════════════════════
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
        root.AddComponent<GameBootstrap>();
        root.AddComponent<MainSceneController>();
        Undo.RegisterCreatedObjectUndo(root, "Create Managers");
    }

    private static void AddManager<T>(GameObject parent) where T : Component
    {
        var go = new GameObject(typeof(T).Name);
        go.transform.SetParent(parent.transform);
        go.AddComponent<T>();
    }

    // ══════════════════════════════════════════════════════════
    //  Internal — Canvas
    // ══════════════════════════════════════════════════════════
    private static void CreateCanvas()
    {
        var canvasGo = new GameObject("Canvas");
        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        canvasGo.AddComponent<GraphicRaycaster>();

        string[] panels =
        {
            "Panel_DayStart", "Panel_QuestBoard", "Panel_Dialogue", "Panel_Result",
            "Panel_Shop",     "Panel_Ending",     "Panel_GameOver",
            "Panel_Minigame_Choice", "Panel_Minigame_Search", "Panel_Minigame_Puzzle",
            "Panel_Minigame_Timing", "Panel_Minigame_Special"
        };
        foreach (var n in panels)
        {
            var p = CreatePanel(n, canvasGo.transform, new Color(0.1f, 0.1f, 0.15f, 0.95f));
            p.SetActive(false);
        }

        var es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();

        Undo.RegisterCreatedObjectUndo(canvasGo, "Create Canvas");
        Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
    }

    // ══════════════════════════════════════════════════════════
    //  Quest entry prefab
    // ══════════════════════════════════════════════════════════
    private static void CreateQuestEntryPrefab()
    {
        const string prefabDir = "Assets/_Project/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabDir))
            AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");

        const string prefabPath = prefabDir + "/QuestEntry.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath) != null) return;

        var go = new GameObject("QuestEntry");
        var rt = go.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 80);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.2f, 0.3f);
        go.AddComponent<Button>().targetGraphic = img;

        var textGo = new GameObject("Label");
        textGo.transform.SetParent(go.transform, false);
        var textRt = textGo.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(10, 4);
        textRt.offsetMax = new Vector2(-10, -4);
        var tmp = textGo.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 18;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.color = Color.white;

        PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
        Object.DestroyImmediate(go);
        AssetDatabase.Refresh(); // LoadAssetAtPath が即座に参照できるよう強制更新
        Debug.Log("[SceneSetup] QuestEntry プレハブ作成: " + prefabPath);
    }

    // ══════════════════════════════════════════════════════════
    //  Panel setup methods
    // ══════════════════════════════════════════════════════════
    private static void SetupChoicePanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Minigame_Choice が見つかりません"); return; }
        if (panel.GetComponent<ChoiceMinigame>() != null) { Debug.Log("[SceneSetup] ChoicePanel 設定済み"); return; }

        var cm = panel.AddComponent<ChoiceMinigame>();
        var qt  = CreateLabel(panel.transform, "QuestionText",
            new Vector2(0.05f, 0.62f), new Vector2(0.95f, 0.85f), 22);
        qt.alignment = TextAlignmentOptions.Center;

        const int N = 4;
        float[] yMins = { 0.44f, 0.30f, 0.16f, 0.02f };
        var btns = new Button[N];
        var lbls = new TextMeshProUGUI[N];
        for (int i = 0; i < N; i++)
        {
            var (b, l) = CreateButton(panel.transform, $"ChoiceBtn_{i}",
                new Vector2(0.08f, yMins[i]), new Vector2(0.92f, yMins[i] + 0.12f));
            btns[i] = b; lbls[i] = l;
        }

        var so = new SerializedObject(cm);
        so.FindProperty("questionText").objectReferenceValue = qt;
        SetArray(so, "choiceButtons", btns);
        SetArray(so, "choiceLabels",  lbls);
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(cm);
    }

    private static void SetupSearchPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Minigame_Search が見つかりません"); return; }
        if (panel.GetComponent<SearchMinigame>() != null) { Debug.Log("[SceneSetup] SearchPanel 設定済み"); return; }

        var sm = panel.AddComponent<SearchMinigame>();

        var desc = CreateLabel(panel.transform, "DescriptionText",
            new Vector2(0.05f, 0.74f), new Vector2(0.95f, 0.85f), 20);
        var rem  = CreateLabel(panel.transform, "RemainingText",
            new Vector2(0.65f, 0.62f), new Vector2(0.95f, 0.73f), 18);
        rem.alignment = TextAlignmentOptions.Right;
        var hint = CreateLabel(panel.transform, "HintText",
            new Vector2(0.05f, 0.03f), new Vector2(0.95f, 0.16f), 18);
        hint.alignment = TextAlignmentOptions.Left;

        const int N = 6;
        float col0 = 0.05f, col1 = 0.52f;
        float[] xs = { col0, col1, col0, col1, col0, col1 };
        float[] ys = { 0.62f, 0.62f, 0.44f, 0.44f, 0.26f, 0.26f };
        float w = 0.44f, h = 0.16f;
        var btns = new Button[N];
        var lbls = new TextMeshProUGUI[N];
        for (int i = 0; i < N; i++)
        {
            var (b, l) = CreateButton(panel.transform, $"LocBtn_{i}",
                new Vector2(xs[i], ys[i]), new Vector2(xs[i] + w, ys[i] + h));
            btns[i] = b; lbls[i] = l;
        }

        var so = new SerializedObject(sm);
        so.FindProperty("descriptionText").objectReferenceValue = desc;
        so.FindProperty("hintText").objectReferenceValue = hint;
        so.FindProperty("remainingText").objectReferenceValue = rem;
        SetArray(so, "locationButtons", btns);
        SetArray(so, "locationLabels",  lbls);
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(sm);
    }

    private static void SetupPuzzlePanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Minigame_Puzzle が見つかりません"); return; }
        if (panel.GetComponent<PuzzleMinigame>() != null) { Debug.Log("[SceneSetup] PuzzlePanel 設定済み"); return; }

        var pm = panel.AddComponent<PuzzleMinigame>();

        var instr = CreateLabel(panel.transform, "InstructionText",
            new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.85f), 20);
        var order = CreateLabel(panel.transform, "SelectedOrderText",
            new Vector2(0.05f, 0.18f), new Vector2(0.95f, 0.30f), 18);
        order.alignment = TextAlignmentOptions.Center;
        order.color = new Color(1f, 0.92f, 0.4f);

        const int N = 4;
        float startX = 0.05f, btnW = 0.215f, gap = 0.01f;
        var btns = new Button[N];
        var lbls = new TextMeshProUGUI[N];
        for (int i = 0; i < N; i++)
        {
            float x = startX + i * (btnW + gap);
            var (b, l) = CreateButton(panel.transform, $"ItemBtn_{i}",
                new Vector2(x, 0.45f), new Vector2(x + btnW, 0.65f));
            btns[i] = b; lbls[i] = l;
        }

        var (confirmBtn, confirmLbl) = CreateButton(panel.transform, "ConfirmButton",
            new Vector2(0.55f, 0.04f), new Vector2(0.92f, 0.16f), new Color(0.2f, 0.6f, 0.3f));
        confirmLbl.text = "決定";

        var (resetBtn, resetLbl) = CreateButton(panel.transform, "ResetButton",
            new Vector2(0.08f, 0.04f), new Vector2(0.45f, 0.16f), new Color(0.5f, 0.2f, 0.2f));
        resetLbl.text = "リセット";

        var so = new SerializedObject(pm);
        so.FindProperty("instructionText").objectReferenceValue = instr;
        so.FindProperty("selectedOrderText").objectReferenceValue = order;
        SetArray(so, "itemButtons", btns);
        SetArray(so, "itemLabels",  lbls);
        so.FindProperty("confirmButton").objectReferenceValue = confirmBtn;
        so.FindProperty("resetButton").objectReferenceValue = resetBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(pm);
    }

    private static void SetupTimingPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Minigame_Timing が見つかりません"); return; }
        if (panel.GetComponent<TimingMinigame>() != null) { Debug.Log("[SceneSetup] TimingPanel 設定済み"); return; }

        var tm = panel.AddComponent<TimingMinigame>();

        var instr = CreateLabel(panel.transform, "InstructionText",
            new Vector2(0.05f, 0.72f), new Vector2(0.95f, 0.85f), 20);
        var result = CreateLabel(panel.transform, "ResultText",
            new Vector2(0.3f, 0.16f), new Vector2(0.7f, 0.28f), 22);
        result.color = new Color(1f, 0.9f, 0.3f);

        // Slider
        var sliderGo = new GameObject("Gauge");
        sliderGo.transform.SetParent(panel.transform, false);
        var sliderRt = sliderGo.AddComponent<RectTransform>();
        sliderRt.anchorMin = new Vector2(0.05f, 0.48f);
        sliderRt.anchorMax = new Vector2(0.95f, 0.62f);
        sliderRt.offsetMin = Vector2.zero;
        sliderRt.offsetMax = Vector2.zero;
        var slider = sliderGo.AddComponent<Slider>();
        slider.interactable = false;

        // Slider background
        var bgGo = new GameObject("Background");
        bgGo.transform.SetParent(sliderGo.transform, false);
        var bgRt = bgGo.AddComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero; bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = Vector2.zero; bgRt.offsetMax = Vector2.zero;
        var bgImg = bgGo.AddComponent<Image>();
        bgImg.color = new Color(0.15f, 0.15f, 0.2f);
        slider.targetGraphic = bgImg;

        // Slider fill area
        var fillArea = new GameObject("Fill Area");
        fillArea.transform.SetParent(sliderGo.transform, false);
        var fillAreaRt = fillArea.AddComponent<RectTransform>();
        fillAreaRt.anchorMin = Vector2.zero; fillAreaRt.anchorMax = Vector2.one;
        fillAreaRt.offsetMin = Vector2.zero; fillAreaRt.offsetMax = Vector2.zero;
        var fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(fillArea.transform, false);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero; fillRt.anchorMax = new Vector2(0, 1);
        fillRt.offsetMin = Vector2.zero; fillRt.offsetMax = Vector2.zero;
        var fillImg = fillGo.AddComponent<Image>();
        fillImg.color = new Color(0.3f, 0.8f, 0.4f);
        slider.fillRect = fillRt;

        // Success zone
        var zoneGo = new GameObject("SuccessZone");
        zoneGo.transform.SetParent(sliderGo.transform, false);
        var zoneRt = zoneGo.AddComponent<RectTransform>();
        zoneRt.anchorMin = new Vector2(0.35f, 0f);
        zoneRt.anchorMax = new Vector2(0.65f, 1f);
        zoneRt.offsetMin = Vector2.zero; zoneRt.offsetMax = Vector2.zero;
        var zoneImg = zoneGo.AddComponent<Image>();
        zoneImg.color = new Color(1f, 0.9f, 0.2f, 0.35f);

        // Tap button
        var (tapBtn, tapLbl) = CreateButton(panel.transform, "TapButton",
            new Vector2(0.25f, 0.32f), new Vector2(0.75f, 0.46f), new Color(0.6f, 0.2f, 0.7f));
        tapLbl.text = "タップ！";
        tapLbl.fontSize = 26;

        var so = new SerializedObject(tm);
        so.FindProperty("gauge").objectReferenceValue = slider;
        so.FindProperty("successZoneIndicator").objectReferenceValue = zoneRt;
        so.FindProperty("instructionText").objectReferenceValue = instr;
        so.FindProperty("resultText").objectReferenceValue = result;
        so.FindProperty("tapButton").objectReferenceValue = tapBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(tm);
    }

    private static void SetupSpecialPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Minigame_Special が見つかりません"); return; }
        if (panel.GetComponent<SpecialMinigame>() != null) { Debug.Log("[SceneSetup] SpecialPanel 設定済み"); return; }

        var spm = panel.AddComponent<SpecialMinigame>();

        var narr = CreateLabel(panel.transform, "NarrativeText",
            new Vector2(0.05f, 0.38f), new Vector2(0.95f, 0.85f), 20);
        narr.alignment = TextAlignmentOptions.TopLeft;

        const int N = 3;
        float[] ys = { 0.22f, 0.09f, -0.04f };
        var btns = new Button[N];
        var lbls = new TextMeshProUGUI[N];
        for (int i = 0; i < N; i++)
        {
            var (b, l) = CreateButton(panel.transform, $"ActionBtn_{i}",
                new Vector2(0.08f, ys[i]), new Vector2(0.92f, ys[i] + 0.11f));
            btns[i] = b; lbls[i] = l;
        }

        var so = new SerializedObject(spm);
        so.FindProperty("narrativeText").objectReferenceValue = narr;
        SetArray(so, "actionButtons", btns);
        SetArray(so, "actionLabels",  lbls);
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(spm);
    }

    private static void SetupDialoguePanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Dialogue が見つかりません"); return; }
        if (panel.GetComponent<DialogueUI>() != null) { Debug.Log("[SceneSetup] DialoguePanel 設定済み"); return; }

        var ui = panel.AddComponent<DialogueUI>();

        var speaker = CreateLabel(panel.transform, "SpeakerText",
            new Vector2(0.04f, 0.72f), new Vector2(0.45f, 0.84f), 22);
        speaker.alignment = TextAlignmentOptions.Left;
        speaker.color = new Color(1f, 0.92f, 0.5f);

        var body = CreateLabel(panel.transform, "BodyText",
            new Vector2(0.04f, 0.34f), new Vector2(0.96f, 0.72f), 20);
        body.alignment = TextAlignmentOptions.TopLeft;

        var (nextBtn, nextLbl) = CreateButton(panel.transform, "NextButton",
            new Vector2(0.72f, 0.04f), new Vector2(0.96f, 0.17f));
        nextLbl.text = "▶ 次へ";

        // Choices area
        var choicesGo = new GameObject("ChoicesArea");
        choicesGo.transform.SetParent(panel.transform, false);
        var choicesRt = choicesGo.AddComponent<RectTransform>();
        choicesRt.anchorMin = new Vector2(0.04f, 0.04f);
        choicesRt.anchorMax = new Vector2(0.96f, 0.33f);
        choicesRt.offsetMin = Vector2.zero;
        choicesRt.offsetMax = Vector2.zero;
        choicesGo.AddComponent<Image>().color = new Color(0, 0, 0, 0);
        choicesGo.SetActive(false);

        float btnH = 0.44f;
        var cBtns = new Button[2];
        var cLbls = new TextMeshProUGUI[2];
        for (int i = 0; i < 2; i++)
        {
            float y = i == 0 ? 0.54f : 0.04f;
            var (b, l) = CreateButton(choicesGo.transform, $"ChoiceBtn_{i}",
                new Vector2(0.02f, y), new Vector2(0.98f, y + btnH));
            cBtns[i] = b; cLbls[i] = l;
        }

        var so = new SerializedObject(ui);
        so.FindProperty("speakerText").objectReferenceValue = speaker;
        so.FindProperty("bodyText").objectReferenceValue = body;
        so.FindProperty("nextButton").objectReferenceValue = nextBtn;
        so.FindProperty("choicesArea").objectReferenceValue = choicesGo;
        SetArray(so, "choiceButtons", cBtns);
        SetArray(so, "choiceLabels",  cLbls);
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ui);
    }

    private static void SetupGameOverPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_GameOver が見つかりません"); return; }
        if (panel.GetComponent<GameOverUI>() != null) { Debug.Log("[SceneSetup] GameOverPanel 設定済み"); return; }

        var ui = panel.AddComponent<GameOverUI>();

        var msg = CreateLabel(panel.transform, "MessageText",
            new Vector2(0.1f, 0.55f), new Vector2(0.9f, 0.80f), 26);

        var (retryBtn, retryLbl) = CreateButton(panel.transform, "RetryButton",
            new Vector2(0.15f, 0.32f), new Vector2(0.48f, 0.44f));
        retryLbl.text = "もう一度";

        var (titleBtn, titleLbl) = CreateButton(panel.transform, "TitleButton",
            new Vector2(0.52f, 0.32f), new Vector2(0.85f, 0.44f));
        titleLbl.text = "タイトルへ";

        var so = new SerializedObject(ui);
        so.FindProperty("messageText").objectReferenceValue = msg;
        so.FindProperty("retryButton").objectReferenceValue = retryBtn;
        so.FindProperty("titleButton").objectReferenceValue = titleBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ui);
    }

    private static void SetupResultPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Result が見つかりません"); return; }
        // ResultUI might already be on it from previous passes, just ensure refs
        var ui = panel.GetComponent<ResultUI>() ?? panel.AddComponent<ResultUI>();

        var result = panel.transform.Find("ResultText")?.GetComponent<TextMeshProUGUI>()
            ?? CreateLabel(panel.transform, "ResultText", new Vector2(0.1f, 0.58f), new Vector2(0.9f, 0.78f), 26);
        var reward = panel.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>()
            ?? CreateLabel(panel.transform, "RewardText", new Vector2(0.2f, 0.44f), new Vector2(0.8f, 0.56f), 22);
        reward.color = new Color(1f, 0.9f, 0.3f);

        Button continueBtn;
        var existing = panel.transform.Find("ContinueButton");
        if (existing != null)
            continueBtn = existing.GetComponent<Button>();
        else
        {
            TextMeshProUGUI lbl;
            continueBtn = CreateButton(panel.transform, "ContinueButton",
                new Vector2(0.3f, 0.24f), new Vector2(0.7f, 0.36f), out lbl);
            lbl.text = "つづける";
        }

        var so = new SerializedObject(ui);
        so.FindProperty("resultText").objectReferenceValue = result;
        so.FindProperty("rewardText").objectReferenceValue = reward;
        so.FindProperty("continueButton").objectReferenceValue = continueBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ui);
    }

    private static void SetupDayStartPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_DayStart が見つかりません"); return; }
        var ui = panel.GetComponent<DayStartUI>() ?? panel.AddComponent<DayStartUI>();

        var dayTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "DayText",
            () => CreateLabel(panel.transform, "DayText", new Vector2(0.1f, 0.6f), new Vector2(0.9f, 0.75f), 32));
        var goldTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "GoldText",
            () => CreateLabel(panel.transform, "GoldText", new Vector2(0.1f, 0.48f), new Vector2(0.9f, 0.60f), 22));
        var repTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "ReputationText",
            () => CreateLabel(panel.transform, "ReputationText", new Vector2(0.1f, 0.36f), new Vector2(0.9f, 0.48f), 22));

        Button startBtn;
        var eBtn = panel.transform.Find("StartButton");
        if (eBtn != null) startBtn = eBtn.GetComponent<Button>();
        else
        {
            TextMeshProUGUI l;
            startBtn = CreateButton(panel.transform, "StartButton",
                new Vector2(0.3f, 0.18f), new Vector2(0.7f, 0.31f), out l);
            l.text = "依頼ボードへ";
        }

        var so = new SerializedObject(ui);
        so.FindProperty("dayText").objectReferenceValue = dayTxt;
        so.FindProperty("goldText").objectReferenceValue = goldTxt;
        so.FindProperty("reputationText").objectReferenceValue = repTxt;
        so.FindProperty("startButton").objectReferenceValue = startBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ui);
    }

    private static void SetupQuestBoardPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_QuestBoard が見つかりません"); return; }
        var ui = panel.GetComponent<QuestBoardUI>() ?? panel.AddComponent<QuestBoardUI>();

        var dayTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "DayText",
            () => CreateLabel(panel.transform, "DayText", new Vector2(0.04f, 0.88f), new Vector2(0.5f, 0.98f), 20));
        var goldTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "GoldText",
            () => CreateLabel(panel.transform, "GoldText", new Vector2(0.5f, 0.88f), new Vector2(0.78f, 0.98f), 20));
        goldTxt.alignment = TextAlignmentOptions.Right;
        var repTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "ReputationText",
            () => CreateLabel(panel.transform, "ReputationText", new Vector2(0.78f, 0.88f), new Vector2(0.98f, 0.98f), 20));
        repTxt.alignment = TextAlignmentOptions.Right;

        // Scroll rect for quest list
        Transform listContainer;
        var existingScroll = panel.transform.Find("QuestScrollView");
        if (existingScroll != null)
        {
            listContainer = existingScroll.Find("Viewport/Content") ?? existingScroll;
        }
        else
        {
            var scrollGo = new GameObject("QuestScrollView");
            scrollGo.transform.SetParent(panel.transform, false);
            var scrollRt = scrollGo.AddComponent<RectTransform>();
            scrollRt.anchorMin = new Vector2(0.02f, 0.12f);
            scrollRt.anchorMax = new Vector2(0.98f, 0.87f);
            scrollRt.offsetMin = Vector2.zero; scrollRt.offsetMax = Vector2.zero;
            scrollGo.AddComponent<Image>().color = new Color(0, 0, 0, 0.2f);
            var sr = scrollGo.AddComponent<ScrollRect>();

            var vpGo = new GameObject("Viewport");
            vpGo.transform.SetParent(scrollGo.transform, false);
            var vpRt = vpGo.AddComponent<RectTransform>();
            vpRt.anchorMin = Vector2.zero; vpRt.anchorMax = Vector2.one;
            vpRt.offsetMin = Vector2.zero; vpRt.offsetMax = Vector2.zero;
            vpGo.AddComponent<Image>().color = new Color(0, 0, 0, 0);
            vpGo.AddComponent<Mask>().showMaskGraphic = false;

            var contentGo = new GameObject("Content");
            contentGo.transform.SetParent(vpGo.transform, false);
            var contentRt = contentGo.AddComponent<RectTransform>();
            contentRt.anchorMin = new Vector2(0, 1);
            contentRt.anchorMax = new Vector2(1, 1);
            contentRt.pivot = new Vector2(0.5f, 1f);
            contentRt.offsetMin = Vector2.zero; contentRt.offsetMax = Vector2.zero;
            var vlg = contentGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 6;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
            var csf = contentGo.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            sr.content = contentRt;
            sr.viewport = vpRt;
            sr.horizontal = false;
            sr.vertical = true;
            listContainer = contentRt;
        }

        Button endDayBtn;
        var eBtn = panel.transform.Find("EndDayButton");
        if (eBtn != null) endDayBtn = eBtn.GetComponent<Button>();
        else
        {
            TextMeshProUGUI l;
            endDayBtn = CreateButton(panel.transform, "EndDayButton",
                new Vector2(0.72f, 0.02f), new Vector2(0.98f, 0.11f), out l);
            l.text = "今日を終える";
        }

        // Find quest entry prefab
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/QuestEntry.prefab");

        var so = new SerializedObject(ui);
        so.FindProperty("questListContainer").objectReferenceValue = listContainer;
        so.FindProperty("questEntryPrefab").objectReferenceValue = prefab;
        so.FindProperty("dayText").objectReferenceValue = dayTxt;
        so.FindProperty("goldText").objectReferenceValue = goldTxt;
        so.FindProperty("reputationText").objectReferenceValue = repTxt;
        so.FindProperty("endDayButton").objectReferenceValue = endDayBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ui);
    }

    private static void SetupShopPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Shop が見つかりません"); return; }
        var ui = panel.GetComponent<ShopUI>() ?? panel.AddComponent<ShopUI>();

        // Minimal wiring — ShopUI needs itemContainer, itemEntryPrefab, goldText, closeButton
        var goldTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "GoldText",
            () => CreateLabel(panel.transform, "GoldText", new Vector2(0.04f, 0.88f), new Vector2(0.6f, 0.98f), 22));

        Transform itemContainer;
        var existingScroll = panel.transform.Find("ShopScrollView");
        if (existingScroll != null)
            itemContainer = existingScroll.Find("Viewport/Content") ?? existingScroll;
        else
        {
            var contentGo = new GameObject("ItemContainer");
            contentGo.transform.SetParent(panel.transform, false);
            var rt = contentGo.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.02f, 0.12f);
            rt.anchorMax = new Vector2(0.98f, 0.87f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            var vlg = contentGo.AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 6;
            vlg.childControlWidth = true;
            vlg.childForceExpandWidth = true;
            itemContainer = contentGo.transform;
        }

        Button closeBtn;
        var eBtn = panel.transform.Find("CloseButton");
        if (eBtn != null) closeBtn = eBtn.GetComponent<Button>();
        else
        {
            TextMeshProUGUI l;
            closeBtn = CreateButton(panel.transform, "CloseButton",
                new Vector2(0.72f, 0.02f), new Vector2(0.98f, 0.11f), out l);
            l.text = "閉じる";
        }

        // Shop item prefab — reuse QuestEntry or create minimal
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Project/Prefabs/QuestEntry.prefab");

        var so = new SerializedObject(ui);
        so.FindProperty("itemContainer").objectReferenceValue = itemContainer;
        so.FindProperty("itemEntryPrefab").objectReferenceValue = prefab;
        so.FindProperty("goldText").objectReferenceValue = goldTxt;
        so.FindProperty("closeButton").objectReferenceValue = closeBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ui);
    }

    private static void SetupEndingPanel(GameObject panel)
    {
        if (panel == null) { Debug.LogWarning("[SceneSetup] Panel_Ending が見つかりません"); return; }
        var ui = panel.GetComponent<EndingUI>() ?? panel.AddComponent<EndingUI>();

        var titleTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "EndingTitleText",
            () => { var t = CreateLabel(panel.transform, "EndingTitleText", new Vector2(0.1f, 0.70f), new Vector2(0.9f, 0.84f), 30); t.color = new Color(1f, 0.92f, 0.5f); return t; });
        var bodyTxt = FindOrCreate<TextMeshProUGUI>(panel.transform, "EndingBodyText",
            () => CreateLabel(panel.transform, "EndingBodyText", new Vector2(0.1f, 0.38f), new Vector2(0.9f, 0.70f), 20));

        Button retBtn;
        var eBtn = panel.transform.Find("TitleReturnButton");
        if (eBtn != null) retBtn = eBtn.GetComponent<Button>();
        else
        {
            TextMeshProUGUI l;
            retBtn = CreateButton(panel.transform, "TitleReturnButton",
                new Vector2(0.3f, 0.18f), new Vector2(0.7f, 0.31f), out l);
            l.text = "タイトルへ";
        }

        var so = new SerializedObject(ui);
        so.FindProperty("endingTitleText").objectReferenceValue = titleTxt;
        so.FindProperty("endingBodyText").objectReferenceValue = bodyTxt;
        so.FindProperty("titleReturnButton").objectReferenceValue = retBtn;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(ui);
    }

    // ══════════════════════════════════════════════════════════
    //  Wire MinigameManager refs to panel components
    // ══════════════════════════════════════════════════════════
    private static void WireMinigameManager()
    {
        var mm = Object.FindFirstObjectByType<MinigameManager>();
        if (mm == null) { Debug.LogWarning("[SceneSetup] MinigameManager が見つかりません"); return; }

        var canvas = GameObject.Find("Canvas");
        if (canvas == null) return;

        var choice  = canvas.transform.Find("Panel_Minigame_Choice")?.GetComponent<ChoiceMinigame>();
        var search  = canvas.transform.Find("Panel_Minigame_Search")?.GetComponent<SearchMinigame>();
        var puzzle  = canvas.transform.Find("Panel_Minigame_Puzzle")?.GetComponent<PuzzleMinigame>();
        var timing  = canvas.transform.Find("Panel_Minigame_Timing")?.GetComponent<TimingMinigame>();
        var special = canvas.transform.Find("Panel_Minigame_Special")?.GetComponent<SpecialMinigame>();

        var so = new SerializedObject(mm);
        so.FindProperty("choiceMinigame").objectReferenceValue  = choice;
        so.FindProperty("searchMinigame").objectReferenceValue  = search;
        so.FindProperty("puzzleMinigame").objectReferenceValue  = puzzle;
        so.FindProperty("timingMinigame").objectReferenceValue  = timing;
        so.FindProperty("specialMinigame").objectReferenceValue = special;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(mm);
        Debug.Log("[SceneSetup] MinigameManager 参照割り当て完了");
    }

    private static void WireMainSceneController(GameObject canvas)
    {
        var msc = Object.FindFirstObjectByType<MainSceneController>();
        if (msc == null) { Debug.LogWarning("[SceneSetup] MainSceneController が見つかりません"); return; }

        var t = canvas.transform;
        var so = new SerializedObject(msc);
        so.FindProperty("panelDayStart").objectReferenceValue   = t.Find("Panel_DayStart")?.gameObject;
        so.FindProperty("panelQuestBoard").objectReferenceValue = t.Find("Panel_QuestBoard")?.gameObject;
        so.FindProperty("panelDialogue").objectReferenceValue   = t.Find("Panel_Dialogue")?.gameObject;
        so.FindProperty("panelResult").objectReferenceValue     = t.Find("Panel_Result")?.gameObject;
        so.FindProperty("panelShop").objectReferenceValue       = t.Find("Panel_Shop")?.gameObject;
        so.FindProperty("panelEnding").objectReferenceValue     = t.Find("Panel_Ending")?.gameObject;
        so.FindProperty("panelGameOver").objectReferenceValue   = t.Find("Panel_GameOver")?.gameObject;
        so.FindProperty("panelChoice").objectReferenceValue     = t.Find("Panel_Minigame_Choice")?.gameObject;
        so.FindProperty("panelSearch").objectReferenceValue     = t.Find("Panel_Minigame_Search")?.gameObject;
        so.FindProperty("panelPuzzle").objectReferenceValue     = t.Find("Panel_Minigame_Puzzle")?.gameObject;
        so.FindProperty("panelTiming").objectReferenceValue     = t.Find("Panel_Minigame_Timing")?.gameObject;
        so.FindProperty("panelSpecial").objectReferenceValue    = t.Find("Panel_Minigame_Special")?.gameObject;
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(msc);
        Debug.Log("[SceneSetup] MainSceneController パネル参照割り当て完了");
    }

    private static void SaveAsMainScene()
    {
        EnsureScenesDir();
        const string mainPath = "Assets/_Project/Scenes/MainScene.unity";
        var activeScene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.SaveScene(activeScene, mainPath);
        AddSceneToBuildSettings(mainPath);
        Debug.Log("[SceneSetup] シーンを MainScene として保存: " + mainPath);
    }

    private static void WireGameBootstrap()
    {
        var gb = Object.FindFirstObjectByType<GameBootstrap>();
        if (gb == null) { Debug.LogWarning("[SceneSetup] GameBootstrap が見つかりません"); return; }

        var so = new SerializedObject(gb);
        so.FindProperty("gameManager").objectReferenceValue        = Object.FindFirstObjectByType<GameManager>();
        so.FindProperty("economyManager").objectReferenceValue     = Object.FindFirstObjectByType<EconomyManager>();
        so.FindProperty("questManager").objectReferenceValue       = Object.FindFirstObjectByType<QuestManager>();
        so.FindProperty("characterManager").objectReferenceValue   = Object.FindFirstObjectByType<CharacterManager>();
        so.FindProperty("minigameManager").objectReferenceValue    = Object.FindFirstObjectByType<MinigameManager>();
        so.FindProperty("dialogueSystem").objectReferenceValue     = Object.FindFirstObjectByType<DialogueSystem>();
        so.FindProperty("shopSystem").objectReferenceValue         = Object.FindFirstObjectByType<ShopSystem>();
        so.FindProperty("achievementManager").objectReferenceValue = Object.FindFirstObjectByType<AchievementManager>();
        so.FindProperty("endingManager").objectReferenceValue      = Object.FindFirstObjectByType<EndingManager>();
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(gb);
        Debug.Log("[SceneSetup] GameBootstrap 参照割り当て完了");
    }

    // ══════════════════════════════════════════════════════════
    //  Helpers
    // ══════════════════════════════════════════════════════════
    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        go.AddComponent<Image>().color = color;
        return go;
    }

    private static TextMeshProUGUI CreateLabel(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, int fontSize = 20)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = new Vector2(8, 4); rt.offsetMax = new Vector2(-8, -4);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return tmp;
    }

    private static (Button btn, TextMeshProUGUI label) CreateButton(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Color? bgColor = null)
    {
        TextMeshProUGUI label;
        var btn = CreateButton(parent, name, anchorMin, anchorMax, out label, bgColor);
        return (btn, label);
    }

    private static Button CreateButton(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax,
        out TextMeshProUGUI label, Color? bgColor = null)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = new Vector2(4, 4); rt.offsetMax = new Vector2(-4, -4);
        var img = go.AddComponent<Image>();
        img.color = bgColor ?? new Color(0.2f, 0.35f, 0.55f);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;

        var textGo = new GameObject("Text");
        textGo.transform.SetParent(go.transform, false);
        var textRt = textGo.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero; textRt.anchorMax = Vector2.one;
        textRt.offsetMin = new Vector2(6, 4); textRt.offsetMax = new Vector2(-6, -4);
        label = textGo.AddComponent<TextMeshProUGUI>();
        label.fontSize = 18;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;
        return btn;
    }

    private static void SetArray<T>(SerializedObject so, string fieldName, T[] items) where T : Object
    {
        var prop = so.FindProperty(fieldName);
        if (prop == null) return;
        prop.arraySize = items.Length;
        for (int i = 0; i < items.Length; i++)
            prop.GetArrayElementAtIndex(i).objectReferenceValue = items[i];
    }

    private static T FindOrCreate<T>(Transform parent, string name, System.Func<T> creator) where T : Component
    {
        var found = parent.Find(name)?.GetComponent<T>();
        return found != null ? found : creator();
    }

    private static void EnsureScenesDir()
    {
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Scenes"))
            AssetDatabase.CreateFolder("Assets/_Project", "Scenes");
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(
            EditorBuildSettings.scenes);
        bool exists = scenes.Exists(s => s.path == scenePath);
        if (!exists)
        {
            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
