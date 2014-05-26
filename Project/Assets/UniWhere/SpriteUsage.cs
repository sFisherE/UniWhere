#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///   record the sprite usage 
/// </summary>
public class SpriteUsage : ScriptableObject
{
    [System.Serializable]
    public class UsageEntry
    {
        public string atlasName;
        //public int atlasInstanceId;
        public List<string> spriteNames;
    }

    [SerializeField]
    public List<UsageEntry> data = new List<UsageEntry>();


    /// <summary>
    ///   record all the usage of every sprite,so we can know which one is not used with AtlasSpider.
    ///   you should rewrite the setter of "spriteName" in advance 
    /// </summary>
    public void ChangeSpriteName(UISprite sprite, string spriteName)
    {
        if (sprite == null)
            return;

        UsageEntry ue = data.Find(p => p.atlasName == sprite.atlas.name);
        UISpriteData sd = sprite.atlas.spriteList.Find(p => p.name == spriteName);
        if (sd == null)
        {
            Debug.LogError("the spriteName " + spriteName + " is not existed");
            return;
        }
        else
        {
            if (ue == null)
            {
                ue = new UsageEntry();
                ue.atlasName = sprite.atlas.name;
                //ue.atlasInstanceId = sprite.atlas.GetInstanceID();
                ue.spriteNames = new List<string>();
                data.Add(ue);
            }
            if (!ue.spriteNames.Contains(spriteName))
            {
                ue.spriteNames.Add(spriteName);
            }
        }

    }
}
#endif