using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SFXManager))]
public class SFXManagerEditor : Editor
{

    #region SerializedProperties

    SerializedProperty MainSFXChart;
    SerializedProperty SFXChartInJson;
    SerializedProperty RegisteredSoundEffects;

    SerializedProperty CustomFilePath;
    SerializedProperty CustomSFXFolderPath;

    SerializedProperty CommandPointer;
    SerializedProperty CurrentTimePoint;

    SerializedProperty RunInEventMode;

    bool InspectionGroup = false;
    bool Admin = false;

    #endregion

    private void OnEnable()
    {
        MainSFXChart = serializedObject.FindProperty("MainSFXChart");
        SFXChartInJson = serializedObject.FindProperty("SFXChartInJson");
        RegisteredSoundEffects = serializedObject.FindProperty("RegisteredSoundEffects");

        CustomFilePath = serializedObject.FindProperty("CustomFilePath");
        CustomSFXFolderPath = serializedObject.FindProperty("CustomSFXFolderPath");

        CommandPointer = serializedObject.FindProperty("CommandPointer");
        CurrentTimePoint = serializedObject.FindProperty("CurrentTimePoint");

        RunInEventMode = serializedObject.FindProperty("RunInEventMode");
    }

    public override void OnInspectorGUI()
    {
        SFXManager ManagerInstance = (SFXManager)target;
        serializedObject.Update();

        EditorGUILayout.PropertyField(CustomSFXFolderPath);
        EditorGUI.indentLevel++;

        EditorGUILayout.LabelField("[The following configs will be disabled if run in EventMode]");
        EditorGUILayout.PropertyField(RunInEventMode);
        if (!ManagerInstance.RunInEventMode)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(SFXChartInJson);
            EditorGUILayout.PropertyField(CustomFilePath);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(MainSFXChart);
            EditorGUILayout.Space();

            InspectionGroup = EditorGUILayout.BeginFoldoutHeaderGroup(InspectionGroup, "MainChart Analysis");
            if(InspectionGroup)
            {
                Admin = EditorGUILayout.Toggle("Allow Modification", Admin);

                GUI.enabled = Admin;
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(CommandPointer);
                EditorGUILayout.PropertyField(CurrentTimePoint);
                EditorGUI.indentLevel--;
                GUI.enabled = true;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(RegisteredSoundEffects);
        GUI.enabled = true;

        serializedObject.ApplyModifiedProperties();
    }

}
