using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Rail)), CanEditMultipleObjects]
public class EditorRail : Editor
{
    protected virtual void OnSceneGUI()
    {
        Rail rail = (Rail)target;

        for(int i = 0; i < rail.localPoints.Length; i++)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 pos = rail.transform.TransformPoint(rail.localPoints[i]);
            Vector3 newPos = Handles.PositionHandle(pos, Quaternion.identity);
            Vector3 newLocalPos = rail.transform.InverseTransformPoint(newPos);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(rail, "change rail point");
                rail.localPoints[i] = newLocalPos;
            }
        }
    }
}