
using UnityEditor;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
public class AnimClipVexRenderer : OdinEditorWindow
{
    private const string OutPutDir = "Assets/RoleAssets/clip";
    public static bool UseMsg = true;
    [MenuItem("Tools/动画顶点位置渲染到纹理")]
    private static void OpenWindow()
    {
        
    }

    private static void Msg(string info)
    {
        
    }

    public static bool CreateAnimTex(Animation animation)
    {
        return true;
    }
}
