using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public static class CreationPrefabTool
{
    private static string PrefabEndsWith = ".prefab";
    /// <summary>
    /// 创建预制体
    /// </summary>
    /// <param name="选择的物体"></param>
    /// <param name="预制体存放路径"></param>
    /// <param name="预制体父物体路径"></param>
    public static void MyCreationPrefab(GameObject[] objs, string rootPath, string parent)
    {
        try
        {
            string path = rootPath + parent + "/";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            for (int i = 0; i < objs.Length; i++)
            {
                EditorUtility.DisplayProgressBar("制作预制体", "名字:" + objs[i].name, i * 1.0f / objs.Length);

#if UNITY_2018
                GameObject TempPrefab =  PrefabUtility.CreatePrefab(path + name + ".prefab", objs[i]);
#elif UNITY_2019
                GameObject TempPrefab = PrefabUtility.SaveAsPrefabAsset((GameObject)PrefabUtility.InstantiatePrefab(objs[i]), path + GetPrefabName(objs[i].name) + PrefabEndsWith);
#endif
            }
            EditorUtility.ClearProgressBar();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"创建预制体失败！{e}");
            EditorUtility.ClearProgressBar();
        }
    }
    /// <summary>
    /// 创建预制体重载
    /// </summary>
    /// <param name="modelPath"></param>
    /// <param name="paefabRoot"></param>
    /// <param name="是否给模型添加动画状态机"></param>
    /// <param name="动画状态机存放地址"></param>
    public static void MyCreationPrefab(List<string> modelPath, string paefabRoot, bool setAnim, string animPath)
    {
        try
        {
            for (int i = 0; i < modelPath.Count; i++)
            {
                if (modelPath[i] == "") continue;
                string path = paefabRoot + PathToPrefabParent(modelPath[i]) + "/";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                List<string> mPath = GetModels(modelPath[i]);
                for (int j = 0; j < mPath.Count; j++)
                {
                    EditorUtility.DisplayProgressBar("制作预制体", "模型路径:" + mPath[j], j * 1.0f / mPath.Count);
#if UNITY_2018
            PrefabUtility.CreatePrefab(path + name + ".prefab", objs[i]);
#elif UNITY_2019
                    GameObject TempPrefab = PrefabUtility.SaveAsPrefabAsset((GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(mPath[j])), path + PathToPrefabName(mPath[j]) + PrefabEndsWith);
#endif
                    if (setAnim)
                    {
                        SetAnimatorController(TempPrefab, mPath[j], animPath, PathToPrefabName(mPath[j]));
                    }
                }
                EditorUtility.ClearProgressBar();
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError($"创建预制体失败！{e}");
            EditorUtility.ClearProgressBar();
        }
    }
    /// <summary>
    /// 获取模型的名字
    /// </summary>
    /// <param name="pathList"></param>
    /// <returns></returns>
    public static List<string> GetModels(string pathList)
    {
        try
        {
            List<string> modelPath = new List<string>();
            string[] allStr = AssetDatabase.FindAssets("t:model", new string[] { pathList });
            for (int i = 0; i < allStr.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(allStr[i]);

                if (allStr.Length > 100)
                {
                    EditorUtility.DisplayProgressBar("查找模型", "ModelPath:" + path, i * 1.0f / allStr.Length);
                }
                modelPath.Add(path);
            }
            EditorUtility.ClearProgressBar();
            return modelPath;

        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            Debug.LogError($"查找模型失败{e}！");
            return null;

        }

    }
    /// <summary>
    /// 给预制体添加动画
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="modelPath"></param>
    /// <param name="animatorControllerPath"></param>
    /// <param name="name"></param>
    public static void SetAnimatorController(GameObject obj, string modelPath, string animatorControllerPath, string name)
    {
        Animator anim = obj.GetComponent<Animator>();
        if (anim)
        {
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(animatorControllerPath + name + ".controller");
            AnimatorController.SetAnimatorController(anim, controller);
            AnimatorControllerLayer layer = controller.layers[0];
            AnimatorStateMachine machine = layer.stateMachine;

            controller.AddParameter("e", AnimatorControllerParameterType.Bool);

            AnimatorState IdleState = machine.AddState("Idle", new Vector3(300, 0, 0));
            machine.defaultState = IdleState;
            AnimatorState behaviorState = machine.AddState("behavior", new Vector3(550, 0, 0));
            behaviorState.motion = GetAnimationClip(modelPath);

            AnimatorStateTransition behaviorTrue = IdleState.AddTransition(behaviorState);
            behaviorTrue.AddCondition(AnimatorConditionMode.If, 1f, "e");
            AnimatorStateTransition behaviorFlas = behaviorState.AddTransition(IdleState);
            behaviorFlas.AddCondition(AnimatorConditionMode.IfNot, 1F, "e");
        }
        else
        {
            Debug.Log(obj.name + "没有挂载动画状态机组件");
        }
    }

    /// <summary>
    /// 获取动画片段
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static AnimationClip GetAnimationClip(string path)
    {
        Object[] objects = AssetDatabase.LoadAllAssetsAtPath(path);
        for (int m = 0; m < objects.Length; m++)
        {
            if (objects[m].GetType() == typeof(AnimationClip))
            {
                AnimationClip clip = (AnimationClip)objects[m];
                return clip;
            }
        }
        return null;
    }
    /// <summary>
    /// 获取动画片段
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public static AnimationClip GetAnimationClip(GameObject go)
    {
        return go.GetComponent<Animation>().clip;
    }

    /// <summary>
    /// 给模型名字返回预制的名字
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    static string GetPrefabName(string name)
    {
        return name.Replace("_", "");
    }
    /// <summary>
    /// 得到模型的名字
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static string PathToPrefabName(string path)
    {
        string[] str = path.Split('/');
        string result = str[str.Length - 1];
        result = result.Replace("_", "");
        result = result.Replace(".fbx", "");
        return result;
    }
    /// <summary>
    /// 得到模型父物体名字  "Assets/Model"
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static string PathToPrefabParent(string path)
    {
        string[] str = path.Split('/');
        string result = str[str.Length - 1];
        return result;
    }

}
