using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
///   texture used in particleSystem
///   find all the reference in my project
/// </summary>
/// 
using Object = UnityEngine.Object;
class TextureInPSSpider : EditorWindow
{
    class Entry
    {
        public Texture tex;
        public List<GameObject> gos;
    }
    List<Entry> mEntrys = new List<Entry>();


    void GetTextures(GameObject go)
    {
        Object[] objs = EditorUtility.CollectDependencies(new GameObject[1]{go});
        foreach (var obj in objs)
        {
            if (obj is Texture)
            {
                //Debug.Log(obj.name);
                Texture texture = obj as Texture;
                Entry en = mEntrys.Find(p => p.tex == texture);
                if (en != null)
                {
                    if (!en.gos.Contains(go))
                    en.gos.Add(go);
                }
            }
        }
    }

    #region  WorkingPath
    string mTexturePath;
    Object mTextureFolder;
    void DrawSelectTexturePath()
    {
        Object folder = EditorGUILayout.ObjectField("Select Texture Path:", mTextureFolder, typeof(Object), false) as Object;
        if (folder != mTextureFolder)
        {
            mTextureFolder = folder;
            mEntrys.Clear();
            mTexturePath = GetPath(mTextureFolder);
            mTextures = UniWhereUtility.GetAssetsInFolderRecursive<Texture>(mTexturePath);
            Debug.Log(mTextures.Count);
            foreach (var item in mTextures)
            {
                mEntrys.Add(new Entry { tex = item, gos = new List<GameObject>() });
            }
        }
        if (string.IsNullOrEmpty(mTexturePath))
        {
            mTexturePath = GetPath(mTextureFolder);
        }

        if (!string.IsNullOrEmpty(mTexturePath))
            GUILayout.Label("you select a path:    " + mTexturePath);
        else
            EditorGUILayout.HelpBox("please select the particle textures folder", MessageType.Warning);

        //get again!
        if (!string.IsNullOrEmpty(mTexturePath) && mTextures.Count==0)
        {
            mTextures = UniWhereUtility.GetAssetsInFolderRecursive<Texture>(mTexturePath);
        }
        if (mTextures!=null)
        {
            if (mTextures.Count > 0 && mEntrys.Count == 0)
            {
                foreach (var item in mTextures)
                {
                    if (mEntrys.Find(p => p.tex == item) == null)
                        mEntrys.Add(new Entry { tex = item, gos = new List<GameObject>() });
                }
            }
        }

    }
    string GetPath(Object folder)
    {
        string ret="";
        if (folder != null)
        {
            string path = AssetDatabase.GetAssetPath(folder);
            if (UniWhereUtility.IsDirectory(path))
                ret = path;
            else
                ret = string.Empty;
        }
        return ret;
    }
    string mPrefabPath;
    Object mPrefabFolder;
    void DrawSelectPSPrefabPath()
    {
        Object folder = EditorGUILayout.ObjectField("Select Prefab Path:", mPrefabFolder, typeof(Object), false) as Object;
        if (folder != mPrefabFolder)
        {
            mPrefabFolder = folder;
            mEntrys.Clear();
            mPrefabPath = GetPath(mPrefabFolder);
            //mTextures = EditorCommonUtility.GetAssetsInFolderRecursively<Texture>(mTexturePath);
            mGameObjects = UniWhereUtility.GetAssetsInFolderRecursive<GameObject>(mPrefabPath);
        }
        if (!string.IsNullOrEmpty(mPrefabPath))
        {
            mPrefabPath = GetPath(mPrefabFolder);
            mGameObjects = UniWhereUtility.GetAssetsInFolderRecursive<GameObject>(mPrefabPath);
        }

        if (!string.IsNullOrEmpty(mPrefabPath))
            GUILayout.Label("you select a path:    " + mPrefabPath);
        else
            EditorGUILayout.HelpBox("please select the particle prefabs folder", MessageType.Warning);

    }
    #endregion
    Vector2 mScroll = Vector2.zero;
    List<Texture> mTextures = null;
    List<GameObject> mGameObjects = null;
    bool mShowRelated;
    void OnGUI()
    {
        DrawSelectTexturePath();
        DrawSelectPSPrefabPath();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Find"))
        {
            foreach (var go in mGameObjects)
            {
                GetTextures(go);
            }
        }
        mShowRelated = EditorGUILayout.Toggle("Show Related Prefab", mShowRelated);
        GUILayout.EndHorizontal();
        GUILayout.Label("Texture Count:" + mTextures.Count);
        mScroll = GUILayout.BeginScrollView(mScroll);
        {
            if (mEntrys != null)
            {
                foreach (var item in mEntrys)
                {
                    if (item != null)
                    {

                        if (item.tex!=null)
                        {
                            if (Selection.activeObject == item.tex)
                                GUI.contentColor = Color.blue;
                            else
                                GUI.contentColor = new Color32(30, 30, 30, 255);
                            if (item.gos.Count == 0)
                            {
                                GUI.contentColor = Color.red;
                            }
                            if (GUILayout.Button(item.tex.name, EditorStyles.whiteLabel))
                            {
                                Selection.activeObject = item.tex;
                            }
                            GUI.contentColor = Color.white;

                            if (mShowRelated)
                            {
                                foreach (var go in item.gos)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(30f);
                                    if (Selection.activeGameObject == go)
                                        GUI.contentColor = Color.blue;
                                    else
                                        GUI.contentColor = new Color32(30, 30, 30, 255);
                                    if (GUILayout.Button(go.name, EditorStyles.whiteLabel))
                                    {
                                        Selection.activeObject = go;
                                    }
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                }
            }
        }
        GUILayout.EndScrollView();
    }
}
