using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
public static class UniWhereMenu
{
    [MenuItem("UniWhere/NGUI/Create SpriteUsage")]
    public static SpriteUsage CreateSpriteUsage()
    {
        const string Path = "Assets/UniWhere/SpriteUsage.asset";
        var so = AssetDatabase.LoadMainAssetAtPath(Path) as SpriteUsage;
        if (so)
            return so;
        so = ScriptableObject.CreateInstance<SpriteUsage>();
        DirectoryInfo di = new DirectoryInfo(Application.dataPath + "/UniWhere");
        if (!di.Exists)
            di.Create();

        AssetDatabase.CreateAsset(so, Path);
        AssetDatabase.SaveAssets();
        return so;
    }
    [MenuItem("UniWhere/Component Spider")]
    static void Init()
    {
        ComponentSpider window = (ComponentSpider)EditorWindow.GetWindow(typeof(ComponentSpider), false, "Component Spider");
    }
    [MenuItem("UniWhere/Missing MonoBehaviour Spider")]
    static void CreateMissingMonoBehaviourSpiderWizard()
    {
        MissingMonoBehaviourSpider window = (MissingMonoBehaviourSpider)EditorWindow.GetWindow(typeof(MissingMonoBehaviourSpider), false, "Missing MonoBehaviour Spider");
    }

    [MenuItem("UniWhere/NGUI/Atlas Spider")]
    static void CreateAtlasSpiderWizard()
    {
        UIAtlasSpider window = (UIAtlasSpider)EditorWindow.GetWindow(typeof(UIAtlasSpider), false, "Atlas Spider");
    }
    [MenuItem("UniWhere/NGUI/Font Spider")]
    static void CreateFontSpiderWizard()
    {
        UIFontSpider window = (UIFontSpider)EditorWindow.GetWindow(typeof(UIFontSpider), false, "Font Spider");
    }

    [MenuItem("UniWhere/AnimationClip Spider")]
    static void CreateAnimationClipSpider()
    {
        AnimationClipSpider window = (AnimationClipSpider)EditorWindow.GetWindow(typeof(AnimationClipSpider), false, "Animation Spider");
    }


    [MenuItem("UniWhere/Texture In ParticleSystem Spider")]
    static void CreateTextureInPSSpider()
    {
        TextureInPSSpider window = (TextureInPSSpider)EditorWindow.GetWindow(typeof(TextureInPSSpider), false, "Texture In ParticleSystem Spider");
    }
}

