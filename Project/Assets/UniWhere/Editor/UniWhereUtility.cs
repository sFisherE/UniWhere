using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Reflection;

using Object = UnityEngine.Object;
class UniWhereUtility
{
    /// <summary>
    ///   加载文件夹下的所有指定类型的资源
    /// </summary>
    public static List<T> GetAssetsInFolder<T>(string folderPath) where T : Object
    {
        List<T> list = new List<T>();
        string[] files = Directory.GetFiles(folderPath);
        foreach (var item in files)
        {
            //filter out the meta file
            if (Path.GetExtension(item) != ".meta")
            {
                T obj = AssetDatabase.LoadAssetAtPath(item, typeof(T)) as T;
                if (!list.Contains(obj))
                    list.Add(obj);
            }
        }
        return list;
    }

    public static List<T> GetAssetsInFolderRecursive<T>(string folderPath) where T : Object
    {
        List<T> list = new List<T>();
        list.AddRange(GetAssetsInFolder<T>(folderPath));
        foreach (string d in Directory.GetDirectories(folderPath))
        {
            list.AddRange(GetAssetsInFolder<T>(folderPath));
        }
        return list;
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        string path = "/" + obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = "/" + obj.name + path;
        }
        return path;
    }
    //public static GameObject InstantiatePrefab(string assetPath)
    //{
    //    GameObject prefab = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
    //    GameObject go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
    //    return go;
    //}

    public static List<string> GetPrefabsRecursive(string path)
    {
        return GetAssetsRecursive(path, "*.prefab");
    }

    public static List<string> GetAnimationClipsRecursive(string path)
    {
        return GetAssetsRecursive(path, "*.anim");
    }

    public static List<string> GetAssetsRecursive(string path, string searchPattern)
    {
        List<string> paths = new List<string>();
        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("path is null");
            return paths;
        }
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        if (dirInfo != null)
        {
            GetAssetsRecursive(dirInfo, paths, searchPattern);
        }
        return paths;
    }
    static void GetAssetsRecursive(FileSystemInfo info, List<string> paths, string searchPattern)
    {
        DirectoryInfo dir = info as DirectoryInfo;
        if (dir == null) return;//不是目录 
        FileSystemInfo[] files = dir.GetFileSystemInfos();
        FileInfo[] prefabs = dir.GetFiles(searchPattern);
        foreach (FileInfo f in prefabs)
        {
            string fullPath = f.ToString();
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("Assets");
            System.Text.RegularExpressions.Match match = regex.Match(fullPath);
            string path = fullPath.Substring(match.Index);
            paths.Add(path);
        }
        for (int i = 0; i < files.Length; i++)
        {
            FileInfo file = files[i] as FileInfo;
            if (file == null)//对于子目录，进行递归调用 
                GetAssetsRecursive(files[i], paths, searchPattern);
        }
    }

    public static ArrayList GetAssetsRecursivelyAtPath(string folderPath, Type theType, ArrayList list)
    {
        string[] files = Directory.GetFiles(folderPath);
        foreach (string file in files)
        {
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(file);
            list.AddRange(Array.FindAll(objs, o => o.GetType() == theType));
        }

        foreach (string directory in Directory.GetDirectories(folderPath))
        {
            GetAssetsRecursivelyAtPath(directory, theType, list);
        }

        return list;
    }
    public static ArrayList GetAssetsInFolder(string folderPath, Type searchType)
    {
        string[] files = Directory.GetFiles(folderPath);

        ArrayList list = new ArrayList();
        foreach (string path in files)
        {
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(path);
            objs = Array.FindAll(objs, x => { return x.GetType() == searchType; });
            list.AddRange(objs);
        }
        return list;
    }


    /// <summary>          
    /// Copy文件夹          
    /// </summary>          
    public static string CopyFolder(string sPath, string dPath)
    {
        string flag = "success";
        try
        {
            if (!Directory.Exists(dPath))
                Directory.CreateDirectory(dPath);
            DirectoryInfo sDir = new DirectoryInfo(sPath);
            FileInfo[] fileArray = sDir.GetFiles();
            foreach (FileInfo file in fileArray)
                file.CopyTo(dPath + "/" + file.Name, true);
            DirectoryInfo dDir = new DirectoryInfo(dPath);
            DirectoryInfo[] subDirArray = sDir.GetDirectories();
            foreach (DirectoryInfo subDir in subDirArray)
            {
                CopyFolder(subDir.FullName, dPath + "/" + subDir.Name);
            }
        }
        catch (Exception ex)
        {
            flag = ex.ToString();
        }
        return flag;
    }



    public static bool IsDirectory(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        // get the file attributes for file or directory
        FileAttributes attr = File.GetAttributes(path);
        //detect whether its a directory or file
        if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            return true;
        else
            return false;
    }

    public static bool IsScript(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return false;
        }
        FileAttributes attr = File.GetAttributes(path);
        FileInfo fi = new FileInfo(path);
        Debug.Log(fi.Extension);
        if (fi.Extension == ".cs" || fi.Extension == ".js" || fi.Extension == ".boo")
            return true;
        else
            return false;
    }
    //public static Object GetObject(string name)
    //{
    //    int assetID = EditorPrefs.GetInt(name, -1);
    //    return (assetID != -1) ? EditorUtility.InstanceIDToObject(assetID) : null;
    //}


    #region GetType
    public static List<Type> GetTypeList()
    {
        List<Type> types = new List<Type>();
        AppDomain domain = AppDomain.CurrentDomain;
        Type ComponentType = typeof(Component);
        foreach (Assembly asm in domain.GetAssemblies())
        {
            Assembly currentAssembly = null;
            //	add UnityEngine.dll component types
            if (asm.FullName == "UnityEngine")
                currentAssembly = asm;
            //	check only for temporary assemblies (i.e. d6a5e78fb39c28ds27a1ec4f9g1 )
            if (ContainsNumbers(asm.FullName))
                currentAssembly = asm;
            if (currentAssembly != null)
            {
                foreach (Type t in currentAssembly.GetExportedTypes())
                {
                    if (ComponentType.IsAssignableFrom(t))
                    {
                        types.Add(t);
                    }
                }
            }
        }
        return types;
    }
    static bool ContainsNumbers(String text)
    {
        int i = 0;
        foreach (char c in text)
        {
            if (int.TryParse(c.ToString(), out i))
                return true;
        }
        return false;
    }
    #endregion


    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }


    public static List<string> GetTextures(string path)
    {
        List<string> paths = new List<string>();
        DirectoryInfo dirInfo = new DirectoryInfo(path);
        FileInfo[] textures = dirInfo.GetFiles("*.png");
        foreach (FileInfo f in textures)
        {
            string fullPath = f.ToString();
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("Assets");
            System.Text.RegularExpressions.Match match = regex.Match(fullPath);
            paths.Add(fullPath.Substring(match.Index));
        }
        return paths;
    }

    /// <summary>
    ///   保存为最高配置
    /// </summary>
    public static void MakeTextureTrue(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
        //if (ti == null) return false;

        TextureImporterSettings settings = new TextureImporterSettings();
        ti.ReadTextureSettings(settings);

        settings.mipmapEnabled = false;
        settings.readable = false;
        settings.maxTextureSize = 4096;
        settings.textureFormat = TextureImporterFormat.RGBA32;
        settings.filterMode = FilterMode.Trilinear;
        settings.aniso = 4;
        settings.wrapMode = TextureWrapMode.Clamp;
        settings.npotScale = TextureImporterNPOTScale.None;

        ti.SetTextureSettings(settings);
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate | ImportAssetOptions.ForceSynchronousImport);
    }

}
