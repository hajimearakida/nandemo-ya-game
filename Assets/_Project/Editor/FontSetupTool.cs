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

        var fontAsset = TMP_FontAsset.CreateFontAsset(font);
        fontAsset.atlasPopulationMode = AtlasPopulationMode.Dynamic;

        const string savePath = "Assets/_Project/Fonts/NotoSansJP_SDF.asset";
        AssetDatabase.CreateAsset(fontAsset, savePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = fontAsset;
        EditorGUIUtility.PingObject(fontAsset);

        Debug.Log("[FontSetup] 完了: " + savePath
            + "\n次のステップ: Window → TextMeshPro → Settings → Default Font Asset に NotoSansJP_SDF を設定してください。");
    }
}
