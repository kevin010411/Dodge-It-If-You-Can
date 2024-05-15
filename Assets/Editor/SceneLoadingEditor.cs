using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SceneLoadingController))]
[CanEditMultipleObjects]
public class SceneLoadingEditor : Editor
{

    #region SerializedProperties

    SerializedProperty PlayOnStart;
    SerializedProperty DontLoadScene;
    SerializedProperty TriggerByParameter;
    SerializedProperty TransitionSFX;

    SerializedProperty TargetTransition;
    SerializedProperty TargetSceneToLoad;
    SerializedProperty OptionalBuilderIndex;

    SerializedProperty TriggerParameter;
    SerializedProperty TriggerStateName;

    SerializedProperty Duration;
    SerializedProperty SFXName;

    bool PlayOnStartConfigurations = false;

    #endregion

    private void OnEnable()
    {
        PlayOnStart = serializedObject.FindProperty("PlayOnStart");
        DontLoadScene = serializedObject.FindProperty("DontLoadScene");
        TriggerByParameter = serializedObject.FindProperty("TriggerByParameter");
        TransitionSFX = serializedObject.FindProperty("TransitionSFX");

        TargetTransition = serializedObject.FindProperty("TargetTransition");
        TargetSceneToLoad = serializedObject.FindProperty("TargetSceneToLoad");
        OptionalBuilderIndex = serializedObject.FindProperty("OptionalBuilderIndex");

        TriggerParameter = serializedObject.FindProperty("TriggerParameter");
        TriggerStateName = serializedObject.FindProperty("TriggerStateName");

        Duration = serializedObject.FindProperty("Duration");
        SFXName = serializedObject.FindProperty("SFXName");
    }

    public override void OnInspectorGUI()
    {
        SceneLoadingController ControllerInstance = (SceneLoadingController)target;

        serializedObject.Update();

        EditorGUILayout.LabelField("[Check this if a transition need to be played OnStart]");
        EditorGUILayout.PropertyField(PlayOnStart);
        if (ControllerInstance.PlayOnStart)
        {
            PlayOnStartConfigurations = 
                EditorGUILayout.BeginFoldoutHeaderGroup(PlayOnStartConfigurations, "PlayOnStartConfigurations");
            if (PlayOnStartConfigurations)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(DontLoadScene);
                EditorGUILayout.LabelField("[Animator Trigger Mode]");

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(TriggerByParameter);
                if (ControllerInstance.TriggerByParameter)
                {
                    EditorGUILayout.PropertyField(TriggerParameter);
                }
                else
                {
                    EditorGUILayout.PropertyField(TriggerStateName);
                }
                EditorGUI.indentLevel--;

                EditorGUILayout.PropertyField(TargetTransition);
                EditorGUILayout.LabelField("[Is transition sound effect needed.]");

                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(TransitionSFX);
                if(ControllerInstance.TransitionSFX)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(SFXName);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
                
                EditorGUILayout.LabelField("[Sub-configurations, closed if DontLoadScene]");
                if (ControllerInstance.DontLoadScene == false)
                {
                    EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(TargetSceneToLoad);
                    EditorGUILayout.PropertyField(OptionalBuilderIndex);
                    EditorGUILayout.PropertyField(Duration);
                    EditorGUI.indentLevel--;
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        serializedObject.ApplyModifiedProperties();
    }

}
