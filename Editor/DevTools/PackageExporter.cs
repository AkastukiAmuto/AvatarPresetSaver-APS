using UnityEngine;
using UnityEditor;
using System.IO;

namespace Matsuki.AvatarPresetSaver.Editor.DevTools
{
    public static class PackageExporter
    {
        [MenuItem("Tools/Avatar Preset Saver/Dev/Export .unitypackage")]
        public static void Export()
        {
            // Try to find the package folder path dynamically
            // Assuming this script is at [PackageRoot]/Editor/DevTools/PackageExporter.cs
            string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(ScriptableObject.CreateInstance<AvatarPresetSaver>())); 
            // AvatarPresetSaver is main script, likely at [PackageRoot]/Editor/AvatarPresetSaver.cs
            
            if (string.IsNullOrEmpty(scriptPath))
            {
                Debug.LogError("Could not find package root.");
                return;
            }

            // Go up from Editor/AvatarPresetSaver.cs to PackageRoot
            string packageRoot = Path.GetDirectoryName(Path.GetDirectoryName(scriptPath)); 
            
            // If in Packages folder (VCC dev mode), it might be "Packages/com.matsuki..."
            // If in Assets/Plugins (Old style), it might be "Assets/Plugins/AvatarPresetSaver"
            
            string fileName = "AvatarPresetSaver_v1.0.0.unitypackage";
            string exportPath = fileName; // Save to project root by default

            Debug.Log($"Exporting package from: {packageRoot}");

            AssetDatabase.ExportPackage(packageRoot, exportPath, ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            
            EditorUtility.RevealInFinder(exportPath);
            Debug.Log($"Export complete: {Path.GetFullPath(exportPath)}");
        }
    }
}
