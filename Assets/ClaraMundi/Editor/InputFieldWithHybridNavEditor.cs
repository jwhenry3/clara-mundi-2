using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using ClaraMundi;
using UnityEngine.Events;

[CustomEditor(typeof(InputFieldWithHybridNav))]
public class InputFieldWithHybridNavEditor : TMPro.EditorUtilities.TMP_InputFieldEditor
{


  public override void OnInspectorGUI()
  {

    InputFieldWithHybridNav component = (InputFieldWithHybridNav)target;

    base.OnInspectorGUI();


    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("upSelectable"), true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("fallbackUpSelectable"), true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("downSelectable"), true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("fallbackDownSelectable"), true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("leftSelectable"), true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("fallbackLeftSelectable"), true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("rightSelectable"), true);
    EditorGUILayout.PropertyField(this.serializedObject.FindProperty("fallbackRightSelectable"), true);

  }
}