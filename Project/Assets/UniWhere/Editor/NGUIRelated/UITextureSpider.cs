using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;
class UITextureSpider : EditorWindow
{
    #region  WorkingPath
    string mPath;
    Object mFolder;
    void DrawSelectPath()
    {
        Object folder = EditorGUILayout.ObjectField("Select Path:", mFolder, typeof(Object), false) as Object;
        if (folder != mFolder)
        {
            mFolder = folder;
            mRelatedGameObjects.Clear();
        }
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
    #endregion
    List<GameObject> mRelatedGameObjects = new List<GameObject>();

    Texture mTexture;
    Vector2 mScroll = Vector2.zero;
    bool mShowSpecific = false;
    void OnGUI()
    {
        ComponentSelector.Draw<Texture>("Select", mTexture, obj =>
        {
            Texture tex = obj as Texture;
            //UIFont font = obj as UIFont;
            if (tex != mTexture)
                mTexture = tex;
        }, true);
        NGUIEditorTools.DrawSeparator();
        DrawSelectPath();
        NGUIEditorTools.DrawSeparator();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Detect", GUILayout.Width(200)))
        {
            mRelatedGameObjects.Clear();

            List<string> paths = UniWhereUtility.GetPrefabsRecursive(mPath);
            foreach (var p in paths)
            {
                Debug.Log(p);
                GameObject go = AssetDatabase.LoadAssetAtPath(p, typeof(GameObject)) as GameObject;
                if (go!=null)
                {
                    UITexture[] textures = go.GetComponentsInChildren<UITexture>(true);
                    bool find = false;
                    foreach (var t in textures)
                    {
                        if (t.mainTexture == mTexture)
                        {
                            find = true;
                            break;
                        }
                    }
                    if (find)
                    {
                        mRelatedGameObjects.Add(go);
                    }
                }
                else
                {
                    Debug.LogError("null gameobject");
                }
            }
        }
        mShowSpecific = EditorGUILayout.Toggle("Show Specific", mShowSpecific);
        GUILayout.EndHorizontal();
        DrawRelatedGameObject();
    }
    void DrawRelatedGameObject()
    {
        mScroll = GUILayout.BeginScrollView(mScroll);
        {
            foreach (var g in mRelatedGameObjects)
            {
                if (Selection.activeGameObject == g)
                    GUI.contentColor = Color.blue;
                else
                    GUI.contentColor = Color.black;
                if (GUILayout.Button(AssetDatabase.GetAssetPath(g), EditorStyles.whiteLabel, GUILayout.MinWidth(100f)))
                {
                    Selection.activeGameObject = g;
                }
                if (mShowSpecific)
                {
                    UITexture[] textures = g.GetComponentsInChildren<UITexture>(true);
                    textures = Array.FindAll(textures, p => p.mainTexture == mTexture);
                    //UILabel[] labels = g.GetComponentsInChildren<UILabel>(true);
                    //labels = Array.FindAll(labels, p => p.font == mFont);

                    foreach (var texture in textures)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(30f);
                        string path = UniWhereUtility.GetGameObjectPath(texture.gameObject);
                        if (Selection.activeGameObject == texture.gameObject)
                            GUI.contentColor = Color.blue;
                        else
                            GUI.contentColor = new Color32(70, 70, 70, 255);
                        if (GUILayout.Button(path, EditorStyles.whiteLabel, GUILayout.MinWidth(100f)))
                        {
                            Selection.activeGameObject = texture.gameObject;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
            }
        }
        GUILayout.EndScrollView();
    }


}