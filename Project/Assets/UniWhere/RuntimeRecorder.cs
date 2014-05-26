#define RUNTIME_RECORD
#if UNITY_EDITOR
//using System;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEditor;
using Debug = UnityEngine.Debug;

/// <summary>
///   this class is design to record all the data need at runtime
/// </summary>
public class RuntimeRecorder : MonoBehaviour
{
    static RuntimeRecorder mInstance;
    public static RuntimeRecorder instance
    {
        get
        {
            if (!mInstance)
            {
                mInstance = FindObjectOfType(typeof(RuntimeRecorder)) as RuntimeRecorder;
                if (!mInstance)
                {
                    var obj = new GameObject("RuntimeRecorder");
                    mInstance = obj.AddComponent<RuntimeRecorder>();
                    DontDestroyOnLoad(obj);
                }
            }
            return mInstance;
        }
    }


    [Conditional("RUNTIME_RECORD")]
    void Awake()
    {
        spriteUsage = AssetDatabase.LoadAssetAtPath("Assets/UniWhere/SpriteUsage.asset",typeof(SpriteUsage)) as SpriteUsage;
        if (spriteUsage==null)
        {
            Debug.LogError("create SpriteUsage first!!");
        }
    }

    public SpriteUsage spriteUsage;
    [Conditional("RUNTIME_RECORD")]
    public void RecordSpriteUsage(UISprite sprite, string spriteName)
    {
        if (spriteUsage != null)
        {
            spriteUsage.ChangeSpriteName(sprite, spriteName);
        }
    }
}
#endif
