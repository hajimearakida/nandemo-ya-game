# Git ブランチ運用ルール

## ブランチ構成

```
main
  ├── staging
  └── feature/xxx
```

| ブランチ | 役割 |
|----------|------|
| `main` | リリース用。Steamに出すビルドはここから作る |
| `staging` | 検証環境。複数機能をまとめて動作確認する |
| `feature/xxx` | 機能単位の作業ブランチ。**mainから切って**、stagingで検証後にmainにマージする |

## 機能ブランチの命名規則

```
feature/機能名
```

### 例

| ブランチ名 | 対応する作業 |
|------------|-------------|
| `feature/game-manager` | GameManager・シーン遷移 |
| `feature/save-system` | セーブ・ロード機能 |
| `feature/dialogue-system` | 会話・選択肢システム |
| `feature/quest-board-ui` | 依頼ボードUI |
| `feature/quest-manager` | 依頼管理システム |
| `feature/economy-manager` | 資金・評判システム |
| `feature/minigame-choice` | 選択肢ミニゲーム |
| `feature/minigame-search` | 探索ミニゲーム |
| `feature/minigame-puzzle` | パズルミニゲーム |
| `feature/minigame-timing` | タイミングミニゲーム |
| `feature/minigame-special` | 特殊ミニゲーム |
| `feature/character-manager` | 関係値・隠し依頼管理 |
| `feature/shop-system` | ショップ・強化システム |
| `feature/steam-achievements` | Steam実績連携 |
| `feature/endings` | エンディング分岐 |

## 作業の流れ

```
# 1. mainから作業ブランチを切る
git checkout main
git pull
git checkout -b feature/xxx

# 2. 作業・コミット
git add <ファイル>
git commit -m "説明"

# 3. stagingにマージして動作確認
git checkout staging
git merge feature/xxx
git push

# 4. 検証OKならmainにマージ
git checkout main
git merge feature/xxx
git push
```

## コミットメッセージの書き方

```
<種別>: <内容>
```

| 種別 | 使いどころ |
|------|-----------|
| `feat` | 新機能追加 |
| `fix` | バグ修正 |
| `refactor` | 動作を変えないコード整理 |
| `docs` | ドキュメント変更 |
| `assets` | 画像・音声などアセットの追加・変更 |

### 例
```
feat: GameManagerのステートマシン実装
fix: セーブデータが上書きされないバグを修正
assets: ベルタのアイコン画像を追加
```
