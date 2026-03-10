#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class FixTMPFonts : MonoBehaviour
{
    [MenuItem("Tools/Fix TMP Font Materials")]
    static void FixFonts()
    {
        string[] guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TMP_FontAsset font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            
            if (font.material == null)
            {
                Material mat = new Material(Shader.Find("TextMeshPro/Mobile/Distance Field"));
                mat.name = font.name + " Material";
                AssetDatabase.CreateAsset(mat, "Assets/TextMesh Pro/Resources/Fonts & Materials/" + mat.name + ".mat");
                font.material = mat;
                EditorUtility.SetDirty(font);
                Debug.Log("Fixed: " + font.name);
            }
        }
        AssetDatabase.SaveAssets();
        Debug.Log("Done fixing TMP fonts!");
    }
}
#endif