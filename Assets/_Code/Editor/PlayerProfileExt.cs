#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace Profile {
    public static class PlayerProfileExt {
        [MenuItem("Tools/Clear Player Profile")]
        private static void ClearProfile() {
            string fullPath = PlayerProfile.FullPath;
            
            if (!File.Exists(fullPath)) {
                Debug.LogWarning("Cannot delete player data - no file found");
                return;
            }

            File.Delete(fullPath);
            
            Debug.Log("Deleted player data successfully");
        }
    }
}
#endif