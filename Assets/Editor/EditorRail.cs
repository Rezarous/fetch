using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rail)), CanEditMultipleObjects]
public class EditorRail : Editor
{
    protected virtual void OnSceneGUI()
    {
        Rail rail = (Rail)target;

        for(int i = 0; i < rail.points.Length; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 pos = rail.points[i];
            Vector3 newPos = Handles.PositionHandle(pos, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(rail, "change rail point");
                rail.points[i] = newPos;
            }
        }
    }
}