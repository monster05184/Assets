
using UnityEditor;
using UnityEngine;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine.Rendering.Universal.Internal;

public class AnimClipVexRenderer : OdinEditorWindow
{
    private const string OutPutDir = "Assets/RoleAssets/clip";
    public static bool UseMsg = true;
    [MenuItem("Tools/动画顶点位置渲染到纹理")]
    private static void OpenWindow()
    {
        GetWindow<AnimClipVexRenderer>().Show();
        if (!Directory.Exists(OutPutDir))
        {
            Directory.CreateDirectory(OutPutDir);
        }
    }

    [Title("预渲染蒙皮")] public SkinnedMeshRenderer skinnedMeshRenderer;
    [Title("动画播放组件")] public Animation animation;
    [Title("播放的动画")] public AnimationClip clip;
    [Title("预渲染尺寸")] [Range(64, 4096)] public int width;
    [Title("信息")] [ReadOnly] public int vertexCount;
    [ReadOnly] public int animFrameCount;
    [ReadOnly] public int height;
    protected override void OnGUI()
    {
        base.OnGUI();
        if (clip != null && animation != null && skinnedMeshRenderer != null)
        {
            vertexCount = skinnedMeshRenderer.sharedMesh.vertexCount;
            animFrameCount = (int)(clip.length * clip.frameRate);
            height = Mathf.CeilToInt((float)vertexCount * animFrameCount / width);
        }
    }
    protected override void Initialize()
    {
        base.Initialize();
    }

    public static bool CreateAnimTex(Animation animation,SkinnedMeshRenderer skinnedMeshRenderer,AnimationClip clip,
        int width,int vertexCount,int animFramCount,float maxSize,out Texture2D animTex)
    {
        if (vertexCount == 0 || animFramCount == 0)
        {
            animTex = null;
            return false;
        }
        //对动画进行采样
        if (animation.GetClip(clip.name) != null)
            animation.RemoveClip(clip.name);
        animation.AddClip(clip,clip.name);
        animation.Play(clip.name);
        int lines = Mathf.CeilToInt((float)vertexCount * animFramCount / width);
        Texture2D result = new Texture2D(width, lines, TextureFormat.RGBAHalf, false);
        result.filterMode = FilterMode.Point;
        result.wrapMode = TextureWrapMode.Clamp;
        Color[] colors = new Color[width * lines];
        for (int i = 0; i < animFramCount; i++)
        {
            float time = (float)i / (animFramCount - 1);
            animation[clip.name].normalizedTime = time;
            animation.Sample();
            Mesh mesh = new Mesh();
            skinnedMeshRenderer.BakeMesh(mesh);
            var vertices = mesh.vertices;
            for (int j = 0; j < vertexCount; j++)
            {
                Color color = new Color();
                var v = vertices[j];
                color.r = v.x / maxSize * 0.5f + 0.5f;
                color.g = v.y / maxSize * 0.5f + 0.5f;
                color.b = v.z / maxSize * 0.5f + 0.5f;
                color.a = 1;
                colors[i * vertexCount + j] = color;
            }
        }
        result.SetPixels(colors);
        result.Apply();
        animTex = result;
        return true;
        
    }
}
