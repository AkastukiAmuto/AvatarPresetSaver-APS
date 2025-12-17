using UnityEngine;
using UnityEditor;
using System.IO;
using System;
using System.Linq;

namespace Matsuki.AvatarPresetSaver.Editor
{
    public class AvatarPresetSaver : EditorWindow
    {
        // Enums
        private enum Tab { Saver, Library, Settings }
        
        // State
        private Tab currentTab = Tab.Saver;
        private GameObject avatarToSave;
        private string saveName;
        private bool generateThumbnail = true;
        private bool addToLibrary = true;
        
        // Library State
        private string[] cachedPackages;
        private Vector2 scrollPos;
        private string searchText = "";
        
        // Settings State
        private const string PREF_LIB_PATH = "AvatarPresetSaver_LibraryPath";
        private string libraryPath;

        [MenuItem("Tools/Avatar Preset Saver")]
        public static void ShowWindow()
        {
            GetWindow<AvatarPresetSaver>("Avatar Preset Saver");
        }

        private void OnEnable()
        {
            // Initialize Settings
            libraryPath = EditorPrefs.GetString(PREF_LIB_PATH, 
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "AvatarPresetSaver_Library"));

            if (Selection.activeGameObject != null)
            {
                avatarToSave = Selection.activeGameObject;
                saveName = avatarToSave.name;
            }
            RefreshLibrary();
        }

        private void OnGUI()
        {
            // Tab Header
            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            if (GUILayout.Toggle(currentTab == Tab.Saver, "Saver", EditorStyles.toolbarButton)) currentTab = Tab.Saver;
            if (GUILayout.Toggle(currentTab == Tab.Library, "Library", EditorStyles.toolbarButton)) 
            {
                if (currentTab != Tab.Library) RefreshLibrary();
                currentTab = Tab.Library;
            }
            if (GUILayout.Toggle(currentTab == Tab.Settings, "Settings", EditorStyles.toolbarButton)) currentTab = Tab.Settings;
            GUILayout.EndHorizontal();

            // Content
            GUILayout.Space(10);
            
            switch (currentTab)
            {
                case Tab.Saver:
                    DrawSaverTab();
                    break;
                case Tab.Library:
                    DrawLibraryTab();
                    break;
                case Tab.Settings:
                    DrawSettingsTab();
                    break;
            }
        }

        // --- Tabs ---

        private void DrawSaverTab()
        {
            GUILayout.Label("Current Project Saver", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            avatarToSave = (GameObject)EditorGUILayout.ObjectField("対象", avatarToSave, typeof(GameObject), true);
            if (EditorGUI.EndChangeCheck() && avatarToSave != null)
            {
                saveName = avatarToSave.name;
            }

            saveName = EditorGUILayout.TextField("保存名", saveName);

            GUILayout.Space(5);
            generateThumbnail = EditorGUILayout.Toggle("サムネイルを生成", generateThumbnail);
            addToLibrary = EditorGUILayout.Toggle("ライブラリに追加", addToLibrary);

            GUILayout.Space(10);
            if (GUILayout.Button("素体を保存する", GUILayout.Height(40)))
            {
                SaveAvatarAsPrefab();
            }
        }

        private void DrawLibraryTab()
        {
            GUILayout.Label("Global Avatar Library", EditorStyles.boldLabel);
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Refresh", GUILayout.Width(80))) RefreshLibrary();
            GUILayout.Space(10);
            
            // Search Bar
            GUILayout.Label("Search:", GUILayout.Width(50));
            searchText = EditorGUILayout.TextField(searchText);
            if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.Width(40)))
            {
                searchText = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (cachedPackages == null || cachedPackages.Length == 0)
            {
                GUILayout.Label("No presets found in library.");
                if (GUILayout.Button("Open Folder")) OpenLibraryFolder();
                return;
            }

            // Filter
            var filteredPackages = cachedPackages.Where(p => 
                string.IsNullOrEmpty(searchText) || 
                Path.GetFileNameWithoutExtension(p).IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToArray();

            if (filteredPackages.Length == 0)
            {
                GUILayout.Label("No matching presets found.");
                return;
            }

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            
            // Grid Layout
            float windowWidth = position.width;
            int columns = Mathf.Max(1, (int)(windowWidth / 110));
            int rows = Mathf.CeilToInt((float)filteredPackages.Length / columns);

            for (int r = 0; r < rows; r++)
            {
                GUILayout.BeginHorizontal();
                for (int c = 0; c < columns; c++)
                {
                    int index = r * columns + c;
                    if (index >= filteredPackages.Length) break;

                    string pkgPath = filteredPackages[index];
                    string pkgName = Path.GetFileNameWithoutExtension(pkgPath);
                    string thumbPath = Path.ChangeExtension(pkgPath, ".png");

                    GUILayout.BeginVertical(GUILayout.Width(100));
                    
                    // Thumbnail
                    Texture2D thumb = EditorGUIUtility.IconContent("Prefab Icon").image as Texture2D; // Default
                    if (File.Exists(thumbPath))
                    {
                        // Load texture (simple way, better to cache in real app)
                        byte[] fileData = File.ReadAllBytes(thumbPath);
                        Texture2D tex = new Texture2D(2, 2);
                        tex.LoadImage(fileData);
                        thumb = tex;
                    }

                    if (GUILayout.Button(thumb, GUILayout.Width(100), GUILayout.Height(100)))
                    {
                        ImportPackage(pkgPath);
                    }
                    GUILayout.Label(pkgName, EditorStyles.miniLabel, GUILayout.Width(100));
                    
                    GUILayout.EndVertical();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
        }

        private void DrawSettingsTab()
        {
            GUILayout.Label("Preferences", EditorStyles.boldLabel);

            GUILayout.Label("Library Path:");
            GUILayout.BeginHorizontal();
            GUILayout.TextField(libraryPath);
            if (GUILayout.Button("Browse", GUILayout.Width(60)))
            {
                string path = EditorUtility.OpenFolderPanel("Select Library Folder", libraryPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    libraryPath = path;
                    EditorPrefs.SetString(PREF_LIB_PATH, libraryPath);
                    RefreshLibrary();
                }
            }
            GUILayout.EndHorizontal();
            
            GUILayout.Space(5);
            if (GUILayout.Button("Open Library Folder"))
            {
                OpenLibraryFolder();
            }
            
            GUILayout.Space(20);
            GUILayout.Label($"Current Location: {libraryPath}", EditorStyles.miniLabel);
        }

        // --- Logic ---

        private void RefreshLibrary()
        {
            if (!Directory.Exists(libraryPath))
            {
                cachedPackages = new string[0];
                return;
            }
            cachedPackages = Directory.GetFiles(libraryPath, "*.unitypackage");
        }
        
        private void OpenLibraryFolder()
        {
            if (!Directory.Exists(libraryPath)) Directory.CreateDirectory(libraryPath);
            System.Diagnostics.Process.Start(libraryPath);
        }

        private void SaveAvatarAsPrefab()
        {
            if (avatarToSave == null)
            {
                EditorUtility.DisplayDialog("Error", "保存するアバター(GameObject)を選択してください。", "OK");
                return;
            }

            // 1. Local Save
            string baseFolder = "Assets/AvatarPresets";
            if (!AssetDatabase.IsValidFolder(baseFolder)) AssetDatabase.CreateFolder("Assets", "AvatarPresets");

            string fileName = !string.IsNullOrEmpty(saveName) ? saveName : avatarToSave.name;
            string localPath = $"{baseFolder}/{fileName}.prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(avatarToSave, localPath, InteractionMode.UserAction);
            
            Texture2D thumbnail = null;
            if (generateThumbnail && savedPrefab != null)
            {
                thumbnail = GenerateAndSetThumbnail(savedPrefab, avatarToSave);
            }

            AssetDatabase.Refresh();
            EditorGUIUtility.PingObject(savedPrefab);

            // 2. Library Export
            if (addToLibrary)
            {
                if (!Directory.Exists(libraryPath)) Directory.CreateDirectory(libraryPath);

                string pkgName = Path.GetFileNameWithoutExtension(localPath); // Matches unique name
                string exportSubPath = Path.Combine(libraryPath, pkgName + ".unitypackage");
                
                // Export Package
                AssetDatabase.ExportPackage(localPath, exportSubPath, ExportPackageOptions.IncludeDependencies);

                // Export Thumbnail
                if (thumbnail != null)
                {
                    string thumbPath = Path.Combine(libraryPath, pkgName + ".png");
                    File.WriteAllBytes(thumbPath, thumbnail.EncodeToPNG());
                }
            }

            EditorUtility.DisplayDialog("Success", $"保存に成功しました:\n{localPath}" + (addToLibrary ? $"\nライブラリにも追加されました" : ""), "OK");
        }

        private Texture2D GenerateAndSetThumbnail(GameObject prefab, GameObject sourceSource)
        {
            Animator animator = sourceSource.GetComponent<Animator>();
            Transform head = animator ? animator.GetBoneTransform(HumanBodyBones.Head) : sourceSource.transform;
            if (head == null) head = sourceSource.transform;

            GameObject camObj = new GameObject("ThumbnailCamera");
            Camera cam = camObj.AddComponent<Camera>();
            
            Vector3 targetPos = head.position;
            Vector3 camPos = targetPos + (sourceSource.transform.forward * 0.5f) + (Vector3.up * 0.1f);
            
            cam.transform.position = camPos;
            cam.transform.LookAt(targetPos);
            cam.nearClipPlane = 0.01f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1f);

            int res = 256;
            RenderTexture rt = RenderTexture.GetTemporary(res, res, 24);
            cam.targetTexture = rt;
            cam.Render();

            RenderTexture.active = rt;
            Texture2D thumb = new Texture2D(res, res, TextureFormat.RGBA32, false);
            thumb.ReadPixels(new Rect(0, 0, res, res), 0, 0);
            thumb.Apply();

            cam.targetTexture = null;
            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);
            DestroyImmediate(camObj);

            thumb.name = "Thumbnail";
            AssetDatabase.AddObjectToAsset(thumb, prefab);
            EditorGUIUtility.SetIconForObject(prefab, thumb);

            return thumb; // Return for export
        }

        private void ImportPackage(string path)
        {
            if (EditorUtility.DisplayDialog("Import", $"このプリセットをインポートしますか？\n{Path.GetFileName(path)}", "Import", "Cancel"))
            {
                 AssetDatabase.ImportPackage(path, true);
            }
        }
    }
}
