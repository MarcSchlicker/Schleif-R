using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Transform))]
public class WorldCoordinatesDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Transform transform = (Transform)target;

        EditorGUILayout.LabelField("World Coordinates", transform.position.ToString());
    }
}