using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System;
using System.Collections.Generic;

public class CreationPrefabWindow : EditorWindow
{
    AnimBool m_SelectionGameobjectWinWin;
    private string m_PrefabsRootPath = null;
    private string m_PrafabsParent = null;
    private string m_AnimatorControllerPath = null;
    private bool m_SetAnimatorController = false;
    AnimBool m_Win;
    private List<string> list = new List<string>();
    void OnEnable()
    {
        m_Win = new AnimBool(true);
        m_Win.valueChanged.AddListener(Repaint);

        m_SelectionGameobjectWinWin = new AnimBool(true);
        m_SelectionGameobjectWinWin.valueChanged.AddListener(Repaint);
    }


    void OnGUI()
    {
        m_SelectionGameobjectWinWin.target = EditorGUILayout.ToggleLeft("选择模型制作为预制体", m_SelectionGameobjectWinWin.target);

        if (EditorGUILayout.BeginFadeGroup(m_SelectionGameobjectWinWin.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel("预制体存放根路径");
            m_PrefabsRootPath = EditorGUILayout.TextField(m_PrefabsRootPath ?? "Assets/Prefabs/");
            EditorGUILayout.PrefixLabel("预制体存放文件夹名称");
            m_PrafabsParent = EditorGUILayout.TextField(m_PrafabsParent ?? "Other");
            m_SetAnimatorController = EditorGUILayout.Toggle("为该模型添加动画控制", m_SetAnimatorController);
            if (EditorGUILayout.BeginFadeGroup(m_SetAnimatorController == true ? 1.0f : 0))
            {
                EditorGUILayout.PrefixLabel("动画状态机存放文件夹名称");
                m_AnimatorControllerPath = EditorGUILayout.TextField(m_AnimatorControllerPath ?? "Assets/AnimatorControllers");
            }
            EditorGUILayout.EndFadeGroup();
            if (GUILayout.Button("把选择的模型制作为预制体", GUILayout.Width(200)))
            {
                if (Selection.gameObjects.Length > 0)
                {
                    Debug.LogError("根据CreationPrefabTool.GetAnimationClip()适配");
                    Close();
                }
                else
                {
                    Debug.LogError("请选择至少大于一个模型进行预制体制作！");
                    Close();
                }
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();

        m_Win.target = EditorGUILayout.ToggleLeft("批量模型制作为预制体", m_Win.target);

        if (EditorGUILayout.BeginFadeGroup(m_Win.faded))
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PrefixLabel("预制体存放根路径");
            m_PrefabsRootPath = EditorGUILayout.TextField(m_PrefabsRootPath ?? "Assets/Prefabs/");
            EditorGUILayout.PrefixLabel("模型存放根路径");
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal("box");
                m_SetAnimatorController = EditorGUILayout.Toggle("为该模型添加动画控制", m_SetAnimatorController);
                if (EditorGUILayout.BeginFadeGroup(m_SetAnimatorController == true ? 1.0f : 0))
                {
                    EditorGUILayout.PrefixLabel("动画状态机存放文件夹名称");
                    m_AnimatorControllerPath = EditorGUILayout.TextField(m_AnimatorControllerPath ?? "Assets/AnimatorControllers");
                }
                EditorGUILayout.EndFadeGroup();
                list[i] = EditorGUILayout.TextField(list[i]);
                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                {
                    list.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("Add", GUILayout.Width(200)))
            {
                list.Add("");
            }
            if (GUILayout.Button("批量制作预制体", GUILayout.Width(200)))
            {
                CreationPrefabTool.MyCreationPrefab(list, m_PrefabsRootPath, m_SetAnimatorController, m_AnimatorControllerPath);
            }
            EditorGUI.indentLevel--;
        }
        EditorGUILayout.EndFadeGroup();
    }
}
