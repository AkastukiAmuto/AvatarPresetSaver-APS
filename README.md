# Avatar Preset Saver

[English](#english) | [日本語](#japanese)

<a name="english"></a>
## English

**Avatar Preset Saver** is a Unity Editor extension tool designed to save your VRChat avatar "base" or any GameObject as a prefab with a single click. It also manages a local library of your saved presets, allowing you to easily reuse them across different projects.

### Features
*   **One-Click Prefab Save**: Easily save your avatar or object as a prefab in the `Assets/AvatarPresets` folder.
*   **Thumbnail Generation**: Automatically generates a thumbnail centered on the avatar's head and sets it as the prefab icon.
*   **Global Library**: Saves your presets as `.unitypackage` files in a global folder (outside the project), making them accessible from any other project using this tool.
*   **Easy Import**: View and import your saved presets from the "Library" tab with a visual grid interface.

### Requirements
*   Unity 2019.4 or later (Tested on 2019.4.31f1 for VRChat SDK)
*   Windows OS (for standard file path handling)

### Installation
1.  Download the latest release or clone this repository.
2.  Place the `AvatarPresetSaver` folder (containing `package.json` and `Editor` folder) into your Unity project's `Packages` directory.
    *   Alternatively, you can add it via VCC if a package listing is provided.

### Usage

Open the tool via the menu bar: `Tools > Avatar Preset Saver`.

#### 1. Saver Tab
This is the main tab for saving your current work.
1.  **Target**: Select the GameObject you want to save (defaults to current selection).
2.  **Save Name**: Enter a name for the prefab (defaults to the object name).
3.  **Options**:
    *   **Generate Thumbnail**: If checked, a camera will automatically capture the avatar's head for the icon.
    *   **Add to Library**: If checked, the saved prefab will also be exported to your global library folder as a `.unitypackage`.
4.  **Button**: Click "素体を保存する" (Save Avatar) to execute. The prefab will be created in `Assets/AvatarPresets`.

#### 2. Library Tab
Browse and import presets from your global collection.
*   **Refresh**: Reloads the list of packages from the library folder.
*   **Grid View**: Click on any thumbnail to import that package into your current project.

#### 3. Settings Tab
Configure user preferences.
*   **Library Path**: Change the location where your global library is stored. Default is `Documents/AvatarPresetSaver_Library`.

---

<a name="japanese"></a>
## 日本語

**Avatar Preset Saver** は、VRChatアバターの「素体」や任意のGameObjectをワンクリックでプレハブとして保存・管理できるUnityエディタ拡張ツールです。
ローカルなライブラリ機能も備えており、保存したプリセットを別のプロジェクトから簡単に呼び出して再利用することができます。

### 機能
*   **ワンクリック保存**: アバターやオブジェクトを `Assets/AvatarPresets` にプレハブとして保存します。
*   **サムネイル自動生成**: アバターの頭部を中心に撮影した画像を自動生成し、プレハブのアイコンとして設定します。
*   **グローバルライブラリ**: 保存したプレハブを `.unitypackage` としてＰＣ内の共通フォルダに保存し、プロジェクトを跨いで共有できます。
*   **簡単インポート**: 「Library」タブから、保存済みのプリセットをサムネイル付きの一覧で確認し、クリックひとつでインポートできます。

### 動作環境
*   Unity 2019.4 以降 (VRChat SDK標準の 2019.4.31f1 で動作確認済み)
*   Windows OS

### インストール
1.  リリースページからダウンロード、またはリポジトリをクローンします。
2.  `AvatarPresetSaver` フォルダ（`package.json` や `Editor` フォルダが含まれるもの）を、Unityプロジェクトの `Packages` フォルダ内に配置してください。
    *   または、VCCなどに登録して追加してください。

### 使い方

メニューバーの `Tools > Avatar Preset Saver` からウィンドウを開きます。

#### 1. Saver (保存) タブ
現在のアバターやオブジェクトを保存するためのメイン画面です。
1.  **対象**: 保存したいGameObjectを選択します（ヒエラルキーで選択していれば自動セットされます）。
2.  **保存名**: プレハブの名前を入力します（デフォルトはオブジェクト名）。
3.  **オプション**:
    *   **サムネイルを生成**: チェックを入れると、保存時に自動でサムネイル撮影を行いアイコンに設定します。
    *   **ライブラリに追加**: チェックを入れると、PC内の共通ライブラリフォルダにもバックアップ（`.unitypackage`）を作成します。
4.  **実行**: 「素体を保存する」ボタンを押すと保存が実行されます。

#### 2. Library (ライブラリ) タブ
保存済みのプリセット一覧を表示・インポートします。
*   **Refresh**: ライブラリフォルダを再スキャンして一覧を更新します。
*   **一覧表示**: サムネイルをクリックすると、そのパッケージを現在のプロジェクトにインポートします。

#### 3. Settings (設定) タブ
ツールの設定を行います。
*   **Library Path**: 共通ライブラリの保存先フォルダを変更できます。デフォルトは `Documents/AvatarPresetSaver_Library` です。

## License
MIT License
