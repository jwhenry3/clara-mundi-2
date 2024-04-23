using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using ClaraMundi;
using UnityEditor.SceneManagement;

using UnityEngine.UI;
using UnityEditor.UI;

namespace ClaraMundi.EditorUtilities
{
  [CustomEditor(typeof(ButtonWithHybridNav), true)]
  [CanEditMultipleObjects]
  /// <summary>
  ///   Custom Editor for the Button Component.
  ///   Extend this class to write a custom editor for a component derived from Button.
  /// </summary>
  public class ButtonEditor : SelectableEditor
  {
    SerializedProperty m_OnClickProperty;

    protected override void OnEnable()
    {
      base.OnEnable();
      m_OnClickProperty = serializedObject.FindProperty("m_OnClick");
    }

    public override void OnInspectorGUI()
    {
      base.OnInspectorGUI();
      EditorGUILayout.Space();

      serializedObject.Update();
      EditorGUILayout.PropertyField(m_OnClickProperty);

      EditorGUILayout.PropertyField(serializedObject.FindProperty("upSelectable"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("fallbackUpSelectable"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("downSelectable"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("fallbackDownSelectable"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("leftSelectable"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("fallbackLeftSelectable"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("rightSelectable"));
      EditorGUILayout.PropertyField(serializedObject.FindProperty("fallbackRightSelectable"));

      serializedObject.ApplyModifiedProperties();
    }
  }
}