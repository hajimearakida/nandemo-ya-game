using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class AssetGeneratorTool
{
    private const string Root = "Assets/_Project/Data";

    // ────────────────────────────────────────────
    //  Entry point
    // ────────────────────────────────────────────
    [MenuItem("NandemoYa/Generate All Assets")]
    public static void GenerateAllAssets()
    {
        EnsureDirs();
        var characters = GenerateCharacters();
        AssetDatabase.SaveAssets();
        var quests = GenerateQuests();
        AssetDatabase.SaveAssets();
        var shop = GenerateShopItems();
        AssetDatabase.SaveAssets();
        GenerateDialogues(quests);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        AssignToScene(characters, quests, shop);
        Debug.Log($"[Generator] 完了 — キャラ:{characters.Length} クエスト:{quests.Length} ショップ:{shop.Length}");
    }

    // ────────────────────────────────────────────
    //  Directories
    // ────────────────────────────────────────────
    private static void EnsureDirs()
    {
        EnsureDir(Root);
        EnsureDir($"{Root}/Characters");
        EnsureDir($"{Root}/Quests");
        EnsureDir($"{Root}/Shop");
        EnsureDir($"{Root}/Dialogues");
    }

    private static void EnsureDir(string path)
    {
        if (!AssetDatabase.IsValidFolder(path))
        {
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            AssetDatabase.CreateFolder(parent, Path.GetFileName(path));
        }
    }

    // ────────────────────────────────────────────
    //  Characters
    // ────────────────────────────────────────────
    private static CharacterData[] GenerateCharacters()
    {
        var defs = new (string id, string name, string intro)[]
        {
            ("tama",       "タマ",         "謎めいた老齢の黒猫。言葉を話せるが、普段は喋らないふりをしている。あなたにだけ話しかけてくる。"),
            ("belta",      "ベルタ",        "タヌキ獣人の雑貨屋店主。噂好きで口が達者。本当に困っている人には優しい。"),
            ("gran",       "グラン爺さん",   "40年前に魔王を倒した伝説の元勇者。今は家に引きこもっている偏屈な隠居人。"),
            ("riri",       "リリ",          "人間の言葉を練習中の妖精族の子供。単語を間違えて使うことが多い純粋な子。"),
            ("dog",        "ドグ",          "口は悪いが腕は本物のドワーフの鍛冶屋。頼んだ仕事は絶対やり遂げる職人気質。"),
            ("sei",        "セイ",          "植物に詳しいエルフの花屋。穏やかで笑顔が絶えないが、秘密を抱えている。"),
            ("garo",       "ガロ",          "明るくてお節介な人間の宿屋主人。旅人の情報を一番持っている。"),
            ("pico",       "ピコ",          "飛べるのに方向音痴という致命的な欠点を持つ鳥人族の配達員。"),
            ("nui_komugi", "ヌイ＆コムギ",   "町中を走り回る人間の姉弟。大人が知らない情報を持っている情報屋コンビ。"),
            ("rokka",      "ロッカ",        "常に仮面をつけた謎の種族。素顔を知る者はいない。終盤の鍵を握る存在。"),
        };

        var list = new CharacterData[defs.Length];
        for (int i = 0; i < defs.Length; i++)
        {
            var (id, name, intro) = defs[i];
            var path = $"{Root}/Characters/{id}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<CharacterData>(path);
            if (existing != null) { list[i] = existing; continue; }

            var cd = ScriptableObject.CreateInstance<CharacterData>();
            cd.characterId       = id;
            cd.characterName     = name;
            cd.introductionText  = intro;
            AssetDatabase.CreateAsset(cd, path);
            list[i] = cd;
        }
        return list;
    }

    // ────────────────────────────────────────────
    //  Quests (all 50)
    // ────────────────────────────────────────────
    private static QuestData[] GenerateQuests()
    {
        // (id, name, desc, charId, type, diff, unlockQuestId, unlockCharId, unlockRelation, unlockDay)
        var defs = new (string id, string name, string desc, string charId,
                        MinigameType type, int diff,
                        string unlockQuestId, string unlockCharId, int unlockRel, int unlockDay)[]
        {
            // ── ベルタ ──────────────────────────────────
            ("quest_01","お届け物、お願い！",       "荷物を指定の住民に届ける",                                 "belta", MinigameType.Search, 1, null, null,  0, 0),
            ("quest_02","仕入れ品の仕分け",         "大量の商品を種類別に整理する",                             "belta", MinigameType.Puzzle, 1, null, null,  0, 0),
            ("quest_03","ドグさんと仲直りしたい",   "言い合いになった鍛冶屋との仲裁役を務める",                 "belta", MinigameType.Choice, 2, null, null,  0, 0),
            ("quest_04","看板をなおして",           "壊れた看板を修繕する",                                     "belta", MinigameType.Puzzle, 2, null, null,  0, 0),
            ("quest_05","内緒で届けてほしいの",     "差出人不明の贈り物を届ける",                               "belta", MinigameType.Search, 2, null,"belta", 40, 0),
            // ── グラン爺さん ────────────────────────────
            ("quest_06","倉庫を片付けてくれ",       "雑然とした倉庫を整理する。昔の遺品が出てくる",             "gran",  MinigameType.Puzzle, 1, null, null,  0, 0),
            ("quest_07","剣がなくなった",           "若い頃の剣を町のどこかで探す",                             "gran",  MinigameType.Search, 2, null, null,  0, 0),
            ("quest_08","墓参りの代わりに行け",     "旧仲間の墓に花を供えてくる",                               "gran",  MinigameType.Search, 2, null, null,  0, 0),
            ("quest_09","手紙を書いてくれ",         "40年ぶりの旧友への代筆。何を書くか選ぶ",                   "gran",  MinigameType.Choice, 2, null, null,  0, 0),
            ("quest_10","お前に頼みたいことがある", "「勇者だった頃の自分」と向き合う最後の依頼",               "gran",  MinigameType.Choice, 3, null,"gran",  40, 0),
            // ── リリ ────────────────────────────────────
            ("quest_11","てがみ、かいて！",         "リリの気持ちを人間語の手紙にする",                         "riri",  MinigameType.Choice, 1, null, null,  0, 0),
            ("quest_12","みちが、わからない",       "迷子になったリリを町の中で見つける",                       "riri",  MinigameType.Search, 1, null, null,  0, 0),
            ("quest_13","おたんじょうびに、プレゼント","友達へのプレゼントを一緒に選ぶ",                        "riri",  MinigameType.Search, 1, null, null,  0, 0),
            ("quest_14","おまじないの練習、てつだって","魔法の詠唱タイミングを合わせる",                        "riri",  MinigameType.Timing, 2, null, null,  0, 0),
            ("quest_15","りりのひみつ、おしえる",   "リリがこの町に来た理由を打ち明ける",                       "riri",  MinigameType.Choice, 2, null,"riri",  40, 0),
            // ── ドグ ────────────────────────────────────
            ("quest_16","素材が足りん",             "鍛冶素材を町の周辺で調達してくる",                         "dog",   MinigameType.Search, 1, null, null,  0, 0),
            ("quest_17","工具が壊れた",             "特殊な工具のパーツを組み直す",                             "dog",   MinigameType.Puzzle, 2, null, null,  0, 0),
            ("quest_18","弟子が逃げた",             "弟子を探し、辞めたい理由を聞いて説得する",                 "dog",   MinigameType.Search, 2, null, null,  0, 0),
            ("quest_19","昔の剣の持ち主を探したい", "40年前に作った剣の受取人を探す",                           "dog",   MinigameType.Search, 3, null, null,  0, 0),
            ("quest_20","本当のことを話す",         "ドグが隠していた過去の失敗を打ち明ける",                   "dog",   MinigameType.Choice, 3, null,"dog",   40, 0),
            // ── セイ ────────────────────────────────────
            ("quest_21","珍しい花を探してほしい",   "町外れに咲く薬草を採取してくる",                           "sei",   MinigameType.Search, 1, null, null,  0, 0),
            ("quest_22","花を大切に届けてほしい",   "揺らさずに指定の場所へ運ぶ",                               "sei",   MinigameType.Timing, 2, null, null,  0, 0),
            ("quest_23","花言葉を調べてほしい",     "住民から手がかりを集めて花言葉を特定する",                 "sei",   MinigameType.Search, 1, null, null,  0, 0),
            ("quest_24","枯れかけた鉢植えをなおして","水・日光・土のバランスを調整する",                        "sei",   MinigameType.Puzzle, 2, null, null,  0, 0),
            ("quest_25","あの場所には、行けなくて", "セイがこの町に来た理由に関わる依頼",                       "sei",   MinigameType.Choice, 3, null,"sei",   40, 0),
            // ── ガロ ────────────────────────────────────
            ("quest_26","棚が壊れちゃって",         "宿の棚と窓枠を修繕する",                                   "garo",  MinigameType.Puzzle, 1, null, null,  0, 0),
            ("quest_27","旅人さんの相談に乗ってやって","宿泊客の悩みを聞いて助言する",                         "garo",  MinigameType.Choice, 1, null, null,  0, 0),
            ("quest_28","迷子の子供が…",           "宿に保護した子供の親を探して届ける",                       "garo",  MinigameType.Search, 2, null, null,  0, 0),
            ("quest_29","食材が急に足りなくなった", "時間内に食材を調達して回る",                               "garo",  MinigameType.Timing, 2, null, null,  0, 0),
            ("quest_30","幽霊が出るんだよね、実は", "宿の一室に現れる存在と対話する",                           "garo",  MinigameType.Special,3, null,"garo",  40, 0),
            // ── ピコ ────────────────────────────────────
            ("quest_31","また誤配してしまいました…","届いた手紙の正しい受取人を探す",                          "pico",  MinigameType.Search, 1, null, null,  0, 0),
            ("quest_32","急ぎの配達をお願いします", "時間内に複数の家へ配達する",                               "pico",  MinigameType.Timing, 2, null, null,  0, 0),
            ("quest_33","大切な手紙を失くして",     "風で飛んだ手紙を町中で探す",                               "pico",  MinigameType.Search, 2, null, null,  0, 0),
            ("quest_34","羽根を怪我してしまって",   "ピコの全配達を一日代行する",                               "pico",  MinigameType.Timing, 3, null,"pico",  40, 0),
            // ── ヌイ＆コムギ ────────────────────────────
            ("quest_35","秘密基地に変な人がいる",   "居座っている謎の旅人を交渉で追い出す",                     "nui_komugi", MinigameType.Choice, 1, null, null,  0, 0),
            ("quest_36","この卵、なんの卵？",       "手がかりを集めて卵の種族を特定する",                       "nui_komugi", MinigameType.Puzzle, 2, null, null,  0, 0),
            ("quest_37","大人には言えない秘密の相談","コムギが抱えた学校の悩みを聞く",                          "nui_komugi", MinigameType.Choice, 2, null, null,  0, 0),
            ("quest_38","コムギが帰ってこない",     "探検に出たコムギを探しに行く",                             "nui_komugi", MinigameType.Search, 2, null, null,  0, 0),
            ("quest_39","グラン爺さんに会いに行きたい","二人をグラン爺さんに引き合わせる",                      "nui_komugi", MinigameType.Choice, 3, null,"nui_komugi", 40, 0),
            // ── タマ（全件隠し） ─────────────────────────
            ("quest_40","ある場所に花を供えてきてほしい","タマが指定する場所を探して花を供える",                "tama",  MinigameType.Search, 2, null, null,  0, 15),
            ("quest_41","飼い主を探してほしい",     "手がかりをたどり、消えた飼い主の真相へ",                   "tama",  MinigameType.Search, 3,"quest_40", null,  0, 0),
            ("quest_48","あの子のそばにいてやってくれ","タマがリリの様子を心配して見守りを頼む",               "tama",  MinigameType.Search, 2,"quest_41", null,  0, 0),
            ("quest_49","俺のことを、覚えておいてくれ","タマの記憶が薄れていることが明かされる",               "tama",  MinigameType.Choice, 3,"quest_48", null,  0, 0),
            ("quest_42","タマの本当のお願い",       "タマが喋れる理由と正体が明かされる",                       "tama",  MinigameType.Special,3,"quest_49", null,  0, 0),
            // ── ロッカ（全件超隠し） ─────────────────────
            ("quest_43","……見えますか",            "ロッカが初めて声をかけてくる",                             "rokka", MinigameType.Special,3, null, null,  0, 30),
            ("quest_44","記憶を、集めてほしい",     "住民たちの「忘れていること」を集める",                     "rokka", MinigameType.Search, 3,"quest_43", null,  0, 0),
            ("quest_45","最後のお願い",             "ロッカの正体と町の秘密が明かされる",                       "rokka", MinigameType.Special,3,"quest_44", null,  0, 0),
            ("quest_50","あなたに、この町を見せたい","ロッカが深夜の町を案内する。エンディングへの布石",        "rokka", MinigameType.Special,3,"quest_45", null,  0, 0),
            // ── ピコ追加 ─────────────────────────────────
            ("quest_47","手紙が読めなくて…",       "雨で滲んだ手紙の届け先を住民に聞いて特定する",             "pico",  MinigameType.Search, 2, null, null,  0, 0),
        };

        var list = new QuestData[defs.Length];
        for (int i = 0; i < defs.Length; i++)
        {
            var d = defs[i];
            var path = $"{Root}/Quests/{d.id}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<QuestData>(path);
            QuestData qd;
            if (existing != null)
            {
                qd = existing;
            }
            else
            {
                qd = ScriptableObject.CreateInstance<QuestData>();
                AssetDatabase.CreateAsset(qd, path);
            }

            qd.questId         = d.id;
            qd.questName       = d.name;
            qd.description     = d.desc;
            qd.characterId     = d.charId;
            qd.minigameType    = d.type;
            qd.difficulty      = d.diff;
            qd.rewardGold      = d.diff == 1 ? 50 : d.diff == 2 ? 100 : 200;
            qd.rewardReputation= d.diff == 1 ?  3 : d.diff == 2 ?   5 :  10;
            qd.rewardRelation  = d.diff == 1 ?  5 : d.diff == 2 ?   8 :  15;

            qd.unlockCondition = new UnlockCondition
            {
                requiredQuestId    = d.unlockQuestId ?? "",
                characterId        = d.unlockCharId  ?? "",
                relationThreshold  = d.unlockRel,
                dayThreshold       = d.unlockDay
            };

            EditorUtility.SetDirty(qd);
            list[i] = qd;
        }
        return list;
    }

    // ────────────────────────────────────────────
    //  Shop items
    // ────────────────────────────────────────────
    private static ShopItemData[] GenerateShopItems()
    {
        var defs = new (string id, string name, string desc, int cost, ShopEffectType type, float val)[]
        {
            ("pocket_watch",  "ポケットウォッチ",  "「パズル型」の制限時間が10秒延長される",          80, ShopEffectType.ExtendPuzzleTime,  10f),
            ("magnifier",     "虫眼鏡",            "「探索型」でヒントが1つ多く見られる",              60, ShopEffectType.AddSearchHints,     1f),
            ("lucky_charm",   "幸運のお守り",       "「選択肢型」で選択肢が1つ増え、正解も増える",     100, ShopEffectType.AddChoiceOption,     1f),
            ("rhythm_stone",  "リズムの石",         "「タイミング型」で成功ゾーンが少し広くなる",       80, ShopEffectType.LooseTimingJudge,   0.08f),
        };

        var list = new ShopItemData[defs.Length];
        for (int i = 0; i < defs.Length; i++)
        {
            var d = defs[i];
            var path = $"{Root}/Shop/{d.id}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<ShopItemData>(path);
            ShopItemData sd;
            if (existing != null) { sd = existing; }
            else
            {
                sd = ScriptableObject.CreateInstance<ShopItemData>();
                AssetDatabase.CreateAsset(sd, path);
            }
            sd.itemId      = d.id;
            sd.itemName    = d.name;
            sd.description = d.desc;
            sd.cost        = d.cost;
            sd.effectType  = d.type;
            sd.effectValue = d.val;
            EditorUtility.SetDirty(sd);
            list[i] = sd;
        }
        return list;
    }

    // ────────────────────────────────────────────
    //  Dialogues (one per quest)
    // ────────────────────────────────────────────
    private static void GenerateDialogues(QuestData[] quests)
    {
        foreach (var quest in quests)
        {
            var path = $"{Root}/Dialogues/dlg_{quest.questId}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<DialogueData>(path);
            DialogueData dlg;
            if (existing != null)
            {
                dlg = existing;
            }
            else
            {
                dlg = ScriptableObject.CreateInstance<DialogueData>();
                AssetDatabase.CreateAsset(dlg, path);
            }

            dlg.dialogueId = $"dlg_{quest.questId}";
            dlg.lines = new DialogueLine[]
            {
                new DialogueLine
                {
                    characterId = quest.characterId,
                    text        = $"【{quest.questName}】\n{quest.description}",
                    choices     = null
                },
                new DialogueLine
                {
                    characterId = quest.characterId,
                    text        = "……引き受けてもらえますか？",
                    choices     = new DialogueChoice[]
                    {
                        new DialogueChoice { label = "引き受ける", resultFlag = "accept",  reputationDelta = 0, relationDelta = 2 },
                        new DialogueChoice { label = "断る",       resultFlag = "decline", reputationDelta = 0, relationDelta = 0 }
                    }
                }
            };

            quest.dialogueData = dlg;
            quest.dialogueId   = dlg.dialogueId;
            EditorUtility.SetDirty(dlg);
            EditorUtility.SetDirty(quest);
        }
    }

    // ────────────────────────────────────────────
    //  Auto-assign to scene managers
    // ────────────────────────────────────────────
    private static void AssignToScene(CharacterData[] chars, QuestData[] quests, ShopItemData[] shop)
    {
        AssignArray<QuestManager,     QuestData>    ("allQuests",      quests);
        AssignArray<CharacterManager, CharacterData>("allCharacters",   chars);
        AssignArray<ShopSystem,       ShopItemData> ("allItems",        shop);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("[Generator] シーンへの割り当て完了 — Ctrl+S でシーンを保存してください");
    }

    private static void AssignArray<TComponent, TAsset>(string fieldName, TAsset[] assets)
        where TComponent : Component
        where TAsset     : Object
    {
        var comp = Object.FindFirstObjectByType<TComponent>();
        if (comp == null)
        {
            Debug.LogWarning($"[Generator] {typeof(TComponent).Name} がシーンに見つかりません。手動で割り当ててください。");
            return;
        }
        var so   = new SerializedObject(comp);
        var prop = so.FindProperty(fieldName);
        if (prop == null)
        {
            Debug.LogWarning($"[Generator] {typeof(TComponent).Name}.{fieldName} が見つかりません。");
            return;
        }
        prop.arraySize = assets.Length;
        for (int i = 0; i < assets.Length; i++)
            prop.GetArrayElementAtIndex(i).objectReferenceValue = assets[i];
        so.ApplyModifiedPropertiesWithoutUndo();
        EditorUtility.SetDirty(comp);
        Debug.Log($"[Generator] {typeof(TComponent).Name}.{fieldName} に {assets.Length} 件割り当て");
    }
}
