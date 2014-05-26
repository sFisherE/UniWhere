using UnityEngine;
using System.Collections;
using UnityEditor;
//Select the dependencies of the found GameObject
public class DependenciesSelecter : EditorWindow
{
    public GameObject obj = null;
    [MenuItem("UniWhere/Select Dependencies")]
    public static void Init()
    {
        var window = GetWindow(typeof(DependenciesSelecter));
        window.position = new Rect(0, 0, 250, 80);
        window.Show();
    }
    public void OnInspectorUpdate()
    {
        Repaint();
    }
    public void OnGUI()
    {
        obj = EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20),
                "Find Dependency",
                obj,
                typeof(GameObject),
                false) as GameObject;

        if (obj)
        {
            if (GUI.Button(new Rect(3, 25, position.width - 6, 20), "Check Dependencies"))
                Selection.objects = EditorUtility.CollectDependencies(new Object[] { obj });
        }
        else
        {
            EditorGUI.LabelField(new Rect(3, 25, position.width - 6, 20),
                "Missing:",
                "Select an object first");
        }
    }
}
