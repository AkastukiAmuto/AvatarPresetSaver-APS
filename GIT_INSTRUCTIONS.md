# GitHubへの公開手順 (VCC対応)

このプロジェクトをGitHubにアップロードし、VCC (VRChat Creator Companion) からインストールできるようにする手順です。

## 1. ローカルリポジトリの作成
まず、このフォルダをGitリポジトリとして初期化します。
ターミナルで以下のコマンドを順番に実行してください。

```powershell
# Gitの初期化
git init

# 全ファイルをステージング（作成した .gitignore に基づき不要なファイルは除外されます）
git add .

# 最初のコミット
git commit -m "Initial commit of Avatar Preset Saver v1.0.0"
```

## 2. GitHubリポジトリの作成
ブラウザでGitHubを開き、新しいリポジトリを作成します。

1.  GitHubにログインし、右上の「+」ボタンから「New repository」を選択。
2.  **Repository name** に `AvatarPresetSaver-APS` (または好きな名前) を入力。
3.  **Public** (公開) を選択 (VCCで配布するため)。
4.  "Add a README file", ".gitignore", "license" 等のチェックは **全て外す** (ローカルですでに作成済みのため)。
5.  「Create repository」をクリック。

## 3. リモートの追加とプッシュ
リポジトリ作成後に表示されるコマンドを使って、ローカルのコードをGitHubにアップロードします。
`[YOUR_USERNAME]` の部分はご自身のGitHubユーザー名に置き換えてください。

```powershell
# メインブランチの名前を main に変更（念のため）
git branch -M main

# リモートリポジトリ（GitHub）を登録
# ※ 下記URLは作成したリポジトリの画面でコピーできます
git remote add origin https://github.com/[YOUR_USERNAME]/AvatarPresetSaver-APS.git

# GitHubへプッシュ
git push -u origin main
```

## 4. VCCへの登録 (ユーザー側)
これでアップロードは完了です。ユーザーに配布する際は、以下のURLをインストール用リンクとして案内できます。
(VPMはリポジトリのURLをそのまま使用します)

`https://github.com/[YOUR_USERNAME]/AvatarPresetSaver-APS`

## 補足: 今後のアップデート手順
ファイルを修正してバージョンアップする際の手順です。

1.  `package.json` の `version` を書き換える (例: `"1.0.1"`).
2.  変更をコミットしてプッシュ:
    ```powershell
    git add .
    git commit -m "Update to v1.0.1: Fixed bugs..."
    git push
    ```
