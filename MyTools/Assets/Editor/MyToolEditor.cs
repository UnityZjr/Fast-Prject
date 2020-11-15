using System;
using UnityEditor;
using UnityEngine;
using System.IO;

class MyToolEditor
{
    [MenuItem("Tools/创建预制体", false, 10)]
    public static void SelectionGameobjToPrefab()
    {
        CreationPrefabWindow window = (CreationPrefabWindow)EditorWindow.GetWindow(typeof(CreationPrefabWindow));
        window.titleContent = new GUIContent("创建预制体配置信息窗口");
    }

    [MenuItem("Tools/选中预制碎片化打包", false, 22)]
    public static void SelectGameObjBuildOne()
    {
        BundleTool.FragmentBuild();
    }


    //[MenuItem("Tools/测试", false, 11)]
    //public static void Test()
    //{
    //    TestWin win = (TestWin)EditorWindow.GetWindow(typeof(TestWin));
    //    win.titleContent = new GUIContent("测试窗口");
    //}


    [MenuItem("Tools/测试获取依赖文件", false, 12)]
    public static void Test1()
    {
        UnityEngine.Object[] selects = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
        UnityEngine.Object[] objs = EditorUtility.CollectDependencies(selects);
        for (int i = 0; i < selects.Length; i++)
        {
            string s = AssetDatabase.GetAssetPath(selects[i]);
            string[] str = AssetDatabase.GetDependencies(s);
        }

    }

    [MenuItem("Tools/测试加载", false, 13)]
    public static void Test3()
    {
        AssetBundle ab1 = AssetBundle.LoadFromFile(Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/jdk安装路径");
        AssetBundle ab = AssetBundle.LoadFromFile(Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/cube");
        GameObject gameobj = UnityEngine.Object.Instantiate(ab.LoadAsset<GameObject>("Cube"));
    }
    [MenuItem("Tools/测试加载1", false, 13)]
    public static void Test4()
    {
        AssetBundle ab = AssetBundle.LoadFromFile(Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/cube (1)");
        GameObject gameobj = UnityEngine.Object.Instantiate(ab.LoadAsset<GameObject>("Cube (1)"));
    }
    //public static void GetModels(string AssetsPath)
    //{
    //    try
    //    {
    //        ModelConfig modelConfig = AssetDatabase.LoadAssetAtPath<ModelConfig>(Paths.MODELCONFIG_PATH);
    //        modelConfig.m_AllModelName.Clear();
    //        DirectoryInfo directoryInfo = new DirectoryInfo(AssetsPath);
    //        FileInfo[] fileInfos = directoryInfo.GetFiles("*", SearchOption.AllDirectories);
    //        for (int i = 0; i < fileInfos.Length; i++)
    //        {
    //            if (fileInfos[i].Name.EndsWith(".fbx"))
    //            {
    //                modelConfig.m_AllModelName.Add(fileInfos[i].Name.Replace(".fbx", ""));
    //            }
    //        }
    //    }
    //    catch (System.Exception e)
    //    {
    //        Debug.LogError($"获取模型失败！{e}");
    //    }
    //}
}
