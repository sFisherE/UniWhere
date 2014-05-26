using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpriteUsage))]
class SpriteUsageInspector : Editor
{
    SpriteUsage mSpriteUsage;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        mSpriteUsage = target as SpriteUsage;
        GUILayout.BeginHorizontal();
        //if (GUILayout.Button("SetDirty"))
        //    EditorUtility.SetDirty(mSpriteUsage);
        if (GUILayout.Button("Clear"))
            mSpriteUsage.data.Clear();
        GUILayout.EndHorizontal();

        foreach (var v in mSpriteUsage.data)
        {
            GUILayout.Label(v.atlasName);
            //string path = AssetDatabase.GetAssetPath(v.atlasInstanceId);
            //Debug.Log(v.atlasInstanceId.ToString());
            //GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            //UIAtlas atlas = go.GetComponent<UIAtlas>();
            //if (atlas!=null)
            //{
                foreach (var item in v.spriteNames)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(30);
                    //UISpriteData sd = atlas.spriteList.Find(p => p.name == item);
                    //if (sd != null)
                        GUILayout.Label(item);
                    //else
                    //    GUILayout.Label(item + " not exist");

                    GUI.contentColor = Color.white;
                    GUILayout.EndHorizontal();
                }
            //}
        }

    }
}
