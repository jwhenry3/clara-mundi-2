using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections;
using ClaraMundi;

[CustomEditor(typeof(ButtonWithHybridNav))]
public class ButtonWithHybridNavEditor : UnityEditor.UI.ButtonEditor
{


  public override void OnInspectorGUI()
  {

    ButtonWithHybridNav component = (ButtonWithHybridNav)target;

    base.OnInspectorGUI();

    component.upSelectable = (Selectable)EditorGUILayout.ObjectField("Up Selectable", component.upSelectable, typeof(Selectable), true);
    component.fallbackUpSelectable = (Selectable)EditorGUILayout.ObjectField("Fallback Up Selectable", component.upSelectable, typeof(Selectable), true);

    component.downSelectable = (Selectable)EditorGUILayout.ObjectField("Down Selectable", component.upSelectable, typeof(Selectable), true);
    component.fallbackDownSelectable = (Selectable)EditorGUILayout.ObjectField("Fallback Down Selectable", component.upSelectable, typeof(Selectable), true);

    component.leftSelectable = (Selectable)EditorGUILayout.ObjectField("Left Selectable", component.upSelectable, typeof(Selectable), true);
    component.fallbackLeftSelectable = (Selectable)EditorGUILayout.ObjectField("Fallback Left Selectable", component.upSelectable, typeof(Selectable), true);

    component.rightSelectable = (Selectable)EditorGUILayout.ObjectField("Right Selectable", component.upSelectable, typeof(Selectable), true);
    component.fallbackRightSelectable = (Selectable)EditorGUILayout.ObjectField("Fallback Right Selectable", component.upSelectable, typeof(Selectable), true);


  }
}