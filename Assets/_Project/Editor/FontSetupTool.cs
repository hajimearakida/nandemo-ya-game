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

        const string savePath = "Assets/_Project/Fonts/NotoSansJP_SDF.asset";

        // 既存アセットを削除して作り直す
        AssetDatabase.DeleteAsset(savePath);

        var fontAsset = TMP_FontAsset.CreateFontAsset(font);
        fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;

        // メインアセットを保存
        AssetDatabase.CreateAsset(fontAsset, savePath);

        // アトラステクスチャをサブアセットとして保存（これをしないと Play 時に破棄される）
        if (fontAsset.atlasTexture != null)
        {
            fontAsset.atlasTexture.name = "Atlas";
            AssetDatabase.AddObjectToAsset(fontAsset.atlasTexture, fontAsset);
        }

        // マテリアルをサブアセットとして保存
        if (fontAsset.material != null)
        {
            fontAsset.material.name = "Atlas Material";
            AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
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
                    Debug.LogWarning("[FontSetup] m_defaultFontAsset が見つかりません。Edit → Project Settings → TextMeshPro → Default Font Asset に手動で設定してください。");
                }
            }
        }
        else
        {
            Debug.LogWarning("[FontSetup] TMP_Settings が見つかりません。Edit → Project Settings → TextMeshPro → Default Font Asset に手動で設定してください。");
        }

        Selection.activeObject = fontAsset;
        EditorGUIUtility.PingObject(fontAsset);
        Debug.Log("[FontSetup] 完了: " + savePath);
    }
}
