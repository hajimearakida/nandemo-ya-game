# システム設計

## フォルダ構成

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Managers/       # ゲーム全体を管理するクラス群
│   │   ├── Systems/        # 個別システム
│   │   ├── UI/             # UI制御
│   │   ├── Minigames/      # ミニゲーム本体
│   │   ├── Data/           # ScriptableObjectの定義
│   │   └── Utils/          # 汎用ヘルパー
│   ├── ScriptableObjects/
│   │   ├── Quests/         # 依頼データ（50件）
│   │   ├── Characters/     # キャラクターデータ
│   │   ├── Dialogues/      # 会話・選択肢データ
│   │   └── ShopItems/      # 強化アイテムデータ
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Art/
│   │   ├── UI/
│   │   ├── Characters/     # キャラアイコン
│   │   └── Backgrounds/
│   └── Audio/
│       ├── BGM/
│       └── SE/
```

---

## シーン構成

```
TitleScene        タイトル・セーブデータ選択
MainScene         ゲーム本体（ほぼここで完結）
  └ MinigameScene ミニゲーム実行時にAdditiveロード
```

### MainScene内のUI層

```
[Canvas]
├── DayStartUI      今日の日付・資金・評判表示
├── QuestBoardUI    依頼一覧・選択
├── DialogueUI      会話・選択肢
├── MinigameUI      ミニゲーム結果オーバーレイ
├── ShopUI          強化ショップ
└── SystemUI        セーブ・設定・終了
```

---

## コアシステム

### GameManager
ゲーム全体のステートマシン。

```csharp
public enum GameState
{
    DayStart,   // 朝：日数・資金・評判確認
    QuestBoard, // 依頼ボードを見る
    Dialogue,   // キャラクターと会話
    Minigame,   // ミニゲーム実行中
    Result,     // 依頼結果表示
    Shop,       // 強化ショップ
    DayEnd,     // 就寝・翌日へ
    GameOver,   // 廃業エンド
    Ending      // エンディング分岐
}
```

### QuestManager
依頼の管理・解放・完了追跡。

```
依頼の状態: Locked → Available → Active → Completed / Failed

解放条件チェック:
  - 関係値が閾値以上
  - 特定の依頼を完了済み
  - 特定の日数以降
```

### CharacterManager
住民の関係値・解放管理。

```
各キャラクター:
  - 関係値（0〜100）
  - 解放済み隠し依頼リスト
  - 表示する会話バリエーション（関係値帯で変化）
```

### EconomyManager
資金・評判・ゲームオーバー判定。

```
資金:
  - 依頼完了で増加
  - ショップ購入で減少
  - 0になっても即ゲームオーバーではない

評判値（0〜100）:
  - 依頼成功で+
  - 依頼失敗・断りすぎで-
  - 一定以下で廃業エンドへ
```

### DialogueSystem
会話テキストの表示・選択肢処理。

```
DialogueData（ScriptableObject）
  - キャラクターID
  - セリフ配列
  - 選択肢 → 分岐先DialogueID or 結果フラグ
```

### SaveSystem
JSON形式でローカル保存。

```
SaveData:
  - 現在の日数
  - 資金・評判値
  - 各キャラクターの関係値
  - 依頼の完了状態（50件分のフラグ）
  - 解放済み実績
  - 購入済みショップアイテム
```

---

## データ構造（ScriptableObject）

### QuestData

```csharp
public class QuestData : ScriptableObject
{
    public string questId;
    public string questName;
    public string characterId;
    public MinigameType minigameType;
    public int difficulty;          // 1〜3
    public int rewardGold;
    public int rewardReputation;
    public int rewardRelation;
    public UnlockCondition unlockCondition;
    public string dialogueId;
}
```

### CharacterData

```csharp
public class CharacterData : ScriptableObject
{
    public string characterId;
    public string characterName;
    public Sprite icon;
    public int[] relationThresholds; // 隠し依頼の解放閾値
}
```

---

## 開発フェーズ

```
Phase 1: 土台
  ① GameManager・シーン遷移
  ② SaveSystem
  ③ DialogueSystem

Phase 2: ゲームループ
  ④ QuestBoard UI
  ⑤ QuestManager（依頼解放・完了）
  ⑥ EconomyManager（資金・評判・ゲームオーバー）

Phase 3: ミニゲーム
  ⑦ BaseMinigame + ChoiceMinigame（最もシンプル）
  ⑧ SearchMinigame
  ⑨ PuzzleMinigame
  ⑩ TimingMinigame
  ⑪ SpecialMinigame

Phase 4: 肉付け
  ⑫ CharacterManager（関係値・隠し依頼）
  ⑬ ShopSystem
  ⑭ Steam実績連携（Steamworks.NET）
  ⑮ エンディング分岐
```
