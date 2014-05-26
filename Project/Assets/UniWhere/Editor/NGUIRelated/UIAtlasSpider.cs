﻿//using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;

/// <summary>
///   不仅要检查prefab，scene也要检查
/// </summary>
public class UIAtlasSpider : EditorWindow
{
    UIAtlas mSelectAtlas;
    void DrawSelectAtlas()
    {
        ComponentSelector.Draw<UIAtlas>("Select", mSelectAtlas, obj =>
            {
                UIAtlas atlas = obj as UIAtlas;
                if (mSelectAtlas != atlas)
                {
                    mSelectAtlas = atlas;
                    spriteUseStates.Clear();
                    if (mSelectAtlas != null)
                    {
                        foreach (var s in mSelectAtlas.spriteList)
                        {
                            spriteUseStates.Add(s.name, new SpriteEntry(s.name, false));
                        }
                    }
                    NGUISettings.atlas = atlas;
                }
                mRuntimeUsage = null;
                Repaint();
            },true);

        if (NGUISettings.atlas==null)
            NGUISettings.atlas = mSelectAtlas;

        GUILayout.BeginHorizontal();
        {
            //编辑器刷新之后可能刷掉数据了，所以当没有数据的时候强制再去获取一次
            if (spriteUseStates.Count == 0)
            {
                if (mSelectAtlas != null)
                {
                    spriteUseStates.Clear();
                    foreach (var s in mSelectAtlas.spriteList)
                    {
                        spriteUseStates.Add(s.name, new SpriteEntry(s.name, false));
                    }
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    string mPath;
    Object mFolder;
    void DrawSelectPath()
    {
        mFolder = EditorGUILayout.ObjectField("Select Path:", mFolder, typeof(Object), false) as Object;
        if (mFolder != null)
        {
            string path = AssetDatabase.GetAssetPath(mFolder);
            if (UniWhereUtility.IsDirectory(path))
            {
                mPath = path;
                GUILayout.Label("you select a path:    " + mPath);
            }
            else
            {
                mPath = string.Empty;
                EditorGUILayout.HelpBox("please select a folder", MessageType.Warning);
            }
        }
    }


    List<string> mPrefabNames = new List<string>();

    Vector2 mScroll = Vector2.zero;
    GUIStyle mStyle = new GUIStyle();
    bool mShowPrefabs = false;

    class SpriteEntry
    {
        public string name;
        public bool useState;
        public int useTimes;
        public List<GameObject> relatedGos = new List<GameObject>();
        public SpriteEntry(string name, bool state)
        {
            this.name = name;
            useState = state;
        }
    }
    Dictionary<string, SpriteEntry> spriteUseStates = new Dictionary<string, SpriteEntry>();
    bool mShowRelatedSprites = false;
    List<string> mRuntimeUsage;
    List<string> runtimeUsage
    {
        get
        {
            if (mRuntimeUsage == null)
            {
                //SpriteUsage spriteUsage = Resources.Load("SpriteUsage") as SpriteUsage;
                SpriteUsage spriteUsage = AssetDatabase.LoadAssetAtPath("Assets/UniWhere/SpriteUsage.asset", typeof(SpriteUsage)) as SpriteUsage;
                if (mSelectAtlas != null)
                {
                    SpriteUsage.UsageEntry ue = spriteUsage.data.Find(p => p.atlasName == mSelectAtlas.name);
                    if (ue != null)
                        mRuntimeUsage = ue.spriteNames;
                }
            }
            return mRuntimeUsage;
        }
    }
    List<string> mDelNames = new List<string>();
    void DrawWidgets()
    {
        if (mSelectAtlas != null)
        {
            bool delete = false;
            mScroll = GUILayout.BeginScrollView(mScroll);
            {
                GUILayout.BeginVertical();
                {
                    foreach (UISpriteData s in mSelectAtlas.spriteList)
                    {
                        if (spriteUseStates.ContainsKey(s.name))
                        {
                            GUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(20f));
                                {

                                    //    selection = iter.Key;

                                    bool useInRuntime = false;
                                    if (runtimeUsage != null)
                                    {
                                        if (runtimeUsage.Contains(s.name))
                                            useInRuntime = true;
                                        //Debug.Log("useInRuntime:" + useInRuntime);
                                    }
                                    GUI.backgroundColor = spriteUseStates[s.name].useState ? new Color(0.4f, 1f, 0f, 1f) : new Color(1, 0.4f, 0.4f, 1);
                                    if (useInRuntime)
                                        GUI.backgroundColor = new Color(0.4f, 1f, 0f, 1f);// NGUIHelperSettings.Green;

                                    GUILayout.Label(s.name, "HelpBox", GUILayout.Width(150), GUILayout.Height(18f));

                                    //GUI.backgroundColor = Color.white;
                                    //绘制一个点击区域
                                    bool highlight = (UIAtlasInspector.instance != null) && (NGUISettings.selectedSprite == s.name);
                                    GUI.backgroundColor = highlight ? Color.white : new Color(0.8f, 0.8f, 0.8f);
                                    if (GUILayout.Button("Select", GUILayout.Height(20f), GUILayout.Width(100)))
                                    {
                                        if (NGUISettings.atlas != null && !string.IsNullOrEmpty(s.name))
                                            NGUIEditorTools.SelectSprite(s.name);
                                    }

                                    GUI.backgroundColor = Color.white;
                                    //GUILayout.Label(spriteUseStates[s.name].ToString(), GUILayout.Width(150));
                                    //if (GUILayout.Button("Sprite", "DropDownButton", GUILayout.Width(76f)))
                                    //{
                                    //    //SpriteSelector.Show(mSelectAtlas, s.name, null);
                                    //    NGUISettings.atlas = mSelectAtlas;
                                    //    NGUISettings.selectedSprite = s.name;
                                    //    SpriteSelector.Show(null);
                                    //}
                                    GUILayout.Label(spriteUseStates[s.name].useTimes.ToString() + (useInRuntime ? " / runtime" : string.Empty));

                                    if (mDelNames.Contains(s.name))
                                    {
                                        GUI.backgroundColor = Color.red;
                                        if (GUILayout.Button("Delete", GUILayout.Width(60f)))
                                        {
                                            delete = true;
                                        }
                                        GUI.backgroundColor = Color.green;
                                        if (GUILayout.Button("X", GUILayout.Width(22f)))
                                        {
                                            mDelNames.Remove(s.name);
                                            delete = false;
                                        }
                                        GUI.backgroundColor = Color.white;
                                    }
                                    else
                                    {
                                        // If we have not yet selected a sprite for deletion, show a small "X" button
                                        if (GUILayout.Button("X", GUILayout.Width(22f))) mDelNames.Add(s.name);
                                    }
                                }
                                GUILayout.EndHorizontal();
                                if (mShowRelatedSprites)
                                {
                                    if (spriteUseStates[s.name].useState)
                                    {
                                        foreach (GameObject go in spriteUseStates[s.name].relatedGos)
                                        {
                                            if (go != null)
                                            {
                                                GUILayout.BeginHorizontal();
                                                GUILayout.Space(30f);
                                                string path = UniWhereUtility.GetGameObjectPath(go);
                                                //path = path.Substring(("/" + RootName).Length);
                                                GUI.contentColor = Color.black;

                                                GUIStyle style = EditorStyles.whiteLabel;
                                                if (Selection.activeGameObject == go)
                                                {
                                                    GUI.contentColor = Color.blue;
                                                }
                                                if (GUILayout.Button(path, style, GUILayout.MinWidth(100f)))
                                                {
                                                    Selection.activeGameObject = go;
                                                }
                                                GUILayout.EndHorizontal();
                                            }
                                        }
                                    }
                                }
                                GUILayout.Space(5);
                        }
                    }
                }
                GUILayout.EndVertical();
                if (delete)
                {
                    //List<AtlasUtility.SpriteEntry> sprites = new List<AtlasUtility.SpriteEntry>();
                    List<UIAtlasMaker.SpriteEntry> sprites = new List<UIAtlasMaker.SpriteEntry>();
                   //AtlasUtility.ExtractSprites(mSelectAtlas, sprites);
                   UIAtlasMaker.ExtractSprites(mSelectAtlas, sprites);

                    for (int i = sprites.Count; i > 0; )
                    {
                        UIAtlasMaker.SpriteEntry ent = sprites[--i];
                        if (mDelNames.Contains(ent.tex.name))
                            sprites.RemoveAt(i);
                    }
                    UIAtlasMaker.UpdateAtlas(mSelectAtlas, sprites);
                    mDelNames.Clear();
                }
            }
            GUILayout.EndScrollView();


        }
    }
    void OnGUI()
    {
        DrawSelectAtlas();
        NGUIEditorTools.DrawSeparator();
        DrawSelectPath();
        NGUIEditorTools.DrawSeparator();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Detect", GUILayout.Width(200)))
        {
            GetSpriteUseState();
        }
        mShowRelatedSprites = EditorGUILayout.Toggle("Show Related Sprite", mShowRelatedSprites);
        GUILayout.EndHorizontal();
        DrawWidgets();
    }

    void GetSpriteUseState()
    {
        mPrefabNames = UniWhereUtility.GetPrefabsRecursive(mPath);
        foreach (var path in mPrefabNames)
        {
            GameObject go = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
            if (go != null)
            {
                UISprite[] sprites = go.GetComponentsInChildren<UISprite>(true);
                foreach (var s in sprites)
                {
                    if (s.atlas == mSelectAtlas)
                    {
                        string key = s.spriteName;
                        spriteUseStates[key].useState = true;
                        List<GameObject> gos = spriteUseStates[key].relatedGos;
                        if (!gos.Contains(s.gameObject))
                        {
                            spriteUseStates[key].useTimes++;//使用次数+1
                            gos.Add(s.gameObject);
                        }
                    }
                }
            }
        }
    }
}
