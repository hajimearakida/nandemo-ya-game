using UnityEngine;
using UnityEditor;
using TMPro;

public static class FontSetupTool
{
    [MenuItem("NandemoYa/Create Japanese Font Asset")]
    public static void CreateJapaneseFontAsset()
    {
        var guids = AssetDatabase.FindAssets("NotoSansJP-Regular t:Font");
        if (guids.Length == 0)
        {
            Debug.LogError("[FontSetup] NotoSansJP-Regular.ttf が見つかりません。Assets/_Project/Fonts/ にインポートしてください。");
            return;
        }

        var fontPath = AssetDatabase.GUIDToAssetPath(guids[0]);
        var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);

        // 既存アセットがあれば再利用
        const string savePath = "Assets/_Project/Fonts/NotoSansJP_SDF.asset";
        var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(savePath);
        if (fontAsset == null)
        {
            fontAsset = TMP_FontAsset.CreateFontAsset(font);
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            AssetDatabase.CreateAsset(fontAsset, savePath);
        }
        else
        {
            fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;
            EditorUtility.SetDirty(fontAsset);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // TMP Settings のデフォルトフォントを設定
        var settingsGuids = AssetDatabase.FindAssets("t:TMP_Settings");
        if (settingsGuids.Length > 0)
        {
            var settingsPath = AssetDatabase.GUIDToAssetPath(settingsGuids[0]);
            var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>(settingsPath);
            if (settings != null)
            {
                var so = new SerializedObject(settings);
                var prop = so.FindProperty("m_defaultFontAsset");
                if (prop != null)
                {
                    prop.objectReferenceValue = fontAsset;
                    so.ApplyModifiedPropertiesWithoutUndo();
                    EditorUtility.SetDirty(settings);
                    AssetDatabase.SaveAssets();
                    Debug.Log("[FontSetup] TMP デフォルトフォント → NotoSansJP_SDF に設定完了");
                }
                else
                {
                    Debug.LogWarning("[FontSetup] TMP_Settings の m_defaultFontAsset プロパティが見つかりません。手動で設定してください。");
                }
            }
        }
        else
        {
            Debug.LogWarning("[FontSetup] TMP_Settings アセットが見つかりません。Edit → Project Settings → TextMeshPro → Default Font Asset に NotoSansJP_SDF を手動で設定してください。");
        }

        Selection.activeObject = fontAsset;
        EditorGUIUtility.PingObject(fontAsset);
        Debug.Log("[FontSetup] フォントアセット作成完了: " + savePath);
    }
}
