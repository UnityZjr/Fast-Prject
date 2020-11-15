using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class BundleTool
{
    private static string m_BuildTarget = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();
    private static string m_BundleParentFfileName;

    /// <summary>
    /// 把传入文件夹内的预制体打出ab包
    /// </summary>
    /// <param name="prefabPath"></param>
    public static void Build(string[] prefabPath)
    {
        try
        {
            string[] allStr = AssetDatabase.FindAssets("t:Prefab", prefabPath);
            for (int i = 0; i < allStr.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
                EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, i * 1.0f / allStr.Length);
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] allDepend = AssetDatabase.GetDependencies(path);
                List<string> allDependPath = new List<string>();
                for (int j = 0; j < allDepend.Length; j++)
                {
                    if (!allDepend[j].EndsWith(".cs"))
                    {
                        allDependPath.Add(allDepend[j]);
                    }
                }
                SetABName(obj.name, allDependPath);
                BunildAssetBundle();

                allDependPath.Clear();
                string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
                for (int j = 0; j < oldABNames.Length; j++)
                {
                    AssetDatabase.RemoveAssetBundleName(oldABNames[j], true);
                    EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + oldABNames[j], j * 1.0f / oldABNames.Length);
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"打包失败！报错为：{e}");
        }

    }

    /// <summary>
    /// 把传入文件夹内的预制体打出ab包，并且按文件夹名存放
    /// </summary>
    /// <param name="prefabPath"></param>
    public static void FileBuild(string[] prefabPath)
    {
        try
        {
            for (int i = 0; i < prefabPath.Length; i++)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(prefabPath[i]);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
                for (int j = 0; j < fileInfos.Length; j++)
                {
                    if (fileInfos[j].Name.EndsWith(".prefab"))
                    {
                        string path = prefabPath[i] + "/" + fileInfos[j].Name;
                        EditorUtility.DisplayProgressBar("查找Prefab", "Prefab:" + path, j * 1.0f / fileInfos.Length);
                        GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                        string[] allDepend = AssetDatabase.GetDependencies(path);
                        List<string> allDependPath = new List<string>();
                        for (int k = 0; k < allDepend.Length; k++)
                        {
                            if (!allDepend[k].EndsWith(".cs"))
                            {
                                allDependPath.Add(allDepend[k]);
                            }
                        }
                        SetABName(obj.name, allDependPath);
                        string tempStr = prefabPath[i];

                        string[] str = tempStr.Split('/');
                        m_BundleParentFfileName = str[str.Length - 1];
                        BunildAssetBundle(m_BundleParentFfileName);

                        allDependPath.Clear();
                        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();
                        for (int k = 0; k < oldABNames.Length; k++)
                        {
                            AssetDatabase.RemoveAssetBundleName(oldABNames[k], true);
                            EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + oldABNames[k], k * 1.0f / oldABNames.Length);
                        }
                    }
                }
                DeleteMainfest(m_BundleParentFfileName);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.ClearProgressBar();
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError(e);
        }

    }
    /// <summary>
    /// 删除老的ab包
    /// </summary>
    /// <param name="bunleTargetPath"></param>
    static void DeleteAB()
    {
        string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo direction = new DirectoryInfo(m_BuildTarget);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories);
        for (int i = 0; i < files.Length; i++)
        {
            if (ConatinABName(files[i].Name, allBundlesName) || files[i].Name.EndsWith(".meta") || files[i].Name.EndsWith(".manifest") || files[i].Name.EndsWith("assetbundleconfig"))
            {
                continue;
            }
            else
            {
                Debug.Log("此AB包已经被删或者改名了：" + files[i].Name);
                if (File.Exists(files[i].FullName))
                {
                    File.Delete(files[i].FullName);
                }
                if (File.Exists(files[i].FullName + ".manifest"))
                {
                    File.Delete(files[i].FullName + ".manifest");
                }
            }
        }
    }

    /// <summary>
    /// 通过名字校验是否为老的ab包
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    static bool ConatinABName(string name, string[] strs)
    {
        for (int i = 0; i < strs.Length; i++)
        {
            if (name == strs[i])
                return true;
        }
        return false;
    }

    /// <summary>
    /// 设置assbundleName
    /// </summary>
    /// <param name="name"></param>
    /// <param name="paths"></param>
    static void SetABName(string name, List<string> paths)
    {
        for (int i = 0; i < paths.Count; i++)
        {
            SetABName(name, paths[i]);
        }
    }
    static void SetABName(string name, string path)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter == null)
        {
            Debug.LogError("不存在此路径文件：" + path);
        }
        else
        {
            assetImporter.assetBundleName = name;
            assetImporter.assetBundleVariant = "ab";
        }
    }
    static void DeleteMainfest(string fileName = null)
    {
        if (fileName != null) m_BuildTarget = m_BuildTarget + "/" + fileName;
        try
        {
            if (!Directory.Exists(m_BuildTarget))
            {
                Debug.LogError("正常逻辑不会出这个错误，请排查！");
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(m_BuildTarget);
            FileInfo[] files = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".manifest"))
                {
                    File.Delete(files[i].FullName);
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"删除mainfest失败！报错信息是{e}");
        }

    }

    private static void BunildAssetBundle(string fileName = null)
    {
        string[] allBundles = AssetDatabase.GetAllAssetBundleNames();
        //key为全路径，value为包名
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();
        for (int i = 0; i < allBundles.Length; i++)
        {
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundles[i]);
            for (int j = 0; j < allBundlePath.Length; j++)
            {
                if (allBundlePath[j].EndsWith(".cs"))
                    continue;

                resPathDic.Add(allBundlePath[j], allBundles[i]);
            }
        }

        if (!Directory.Exists(m_BuildTarget))
        {
            Directory.CreateDirectory(m_BuildTarget);
        }

        DeleteAB();
        //BuildPipeline.SetAssetBundleEncryptKey("0123456789abcdef");//加密ab包
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_BuildTarget, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        //AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(m_BunleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression |BuildAssetBundleOptions.EnableProtection, EditorUserBuildSettings.activeBuildTarget);//加密ab包需要的打包方式
        if (manifest == null)
        {
            Debug.LogError("AssetBundle 打包失败！");
        }
        else
        {
            Debug.Log("AssetBundle 打包完毕");
        }
        DeleteMainfest();
    }

    #region 碎片化打包方案，所有物体单独一个ab包，加载预制体之前需要先加载依赖文件，不能有同名文件
    public static void FragmentBuild()
    {
        if (!Directory.Exists(Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString()))
        {
            Directory.CreateDirectory(Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString());
        }

        BuildPipeline.BuildAssetBundles(Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString(),
            BundleTool.GetAssetBundleBuild(Selection.gameObjects), BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        DeleteMainfest();
    }

    public static AssetBundleBuild[] GetAssetBundleBuild(GameObject[] objs)
    {
        try
        {
            List<string> all = GetAllBuildGamobj(objs);

            AssetBundleBuild[] builds = new AssetBundleBuild[all.Count];
            for (int i = 0; i < all.Count; i++)
            {
                if (all[i].EndsWith(".mat")) continue;
                AssetBundleBuild build = new AssetBundleBuild();
                string[] str = all[i].Split('/');
                string name = str[str.Length - 1];
                string[] str1 = name.Split('.');
                string result = str1[0];
                build.assetBundleName = result;
                build.assetNames = new string[] { all[i] };
                build.assetBundleVariant = "ab";
                builds[i] = build;
            }
            return builds;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"获取AssetBundleBuild失败，报错信息为：{e}");
            return null;
        }
       
    }
    static List<string> GetAllBuildGamobj(GameObject[] objs)
    {
        List<string> all = new List<string>();
        for (int i = 0; i < objs.Length; i++)
        {
            if (!Check(all, AssetDatabase.GetAssetPath(objs[i])))
                all.Add(AssetDatabase.GetAssetPath(objs[i]));
            string[] depend = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(objs[i]));
            for (int j = 0; j < depend.Length; j++)
            {
                if (!Check(all, depend[j]))
                    all.Add(depend[j]);
            }
        }
        return all;
    }
    private static bool Check(List<string> list, string target)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == target)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
}

