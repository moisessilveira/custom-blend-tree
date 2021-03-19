using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlendTree3D)), CanEditMultipleObjects]
public class BlendTree3DEditor : Editor
{
    private int _selected;
    private BlendTree3D _target;
    private Transform _transform;

    private void OnEnable()
    {
        _selected = -1;
        _target = (BlendTree3D)target;
        _transform = _target.transform;
    }

    private void OnSceneGUI()
    {
        if (!_target.ShowGizmos)
            return;

        Handles.matrix = _transform.localToWorldMatrix;

        using (new Handles.DrawingScope(new Color(0, 0, 0, 0)))
        {
            for (int i = 0; i < _target.Points.Count; i++)
            {
                Vector3 position = _target.Points[i].Position;

                if (Handles.Button(position, Quaternion.identity, 0.1f, 0.1f, Handles.SphereHandleCap))
                {
                    _selected = i;

                    foreach (BlendTree3DPoint point in _target.Points)
                    {
                        point.HighLight = false;
                    }

                    _target.Points[i].HighLight = true;
                }
            }
        }

        if (_selected < 0)
        {
            return;
        }

        Vector3 selectedPoint = _target.Points[_selected].Position;

        using (EditorGUI.ChangeCheckScope scope = new EditorGUI.ChangeCheckScope())
        {
            if (Tools.current == Tool.Move)
            {
                Vector3 resultPosition = Handles.PositionHandle(selectedPoint, Quaternion.identity);

                if (!scope.changed)
                {
                    return;
                }

                Undo.RegisterCompleteObjectUndo(_target, $"{_target.name} Modification");

                _target.Points[_selected].Position = resultPosition;
                _target.UpdateSamples();
            }
            else if (Tools.current == Tool.Rotate)
            {
                Quaternion resultRotation = Handles.RotationHandle(_target.Points[_selected].Rotation, _target.Points[_selected].Position);

                if (!scope.changed)
                {
                    return;
                }

                Undo.RegisterCompleteObjectUndo(_target, $"{_target.name} Modification");
                _target.Points[_selected].Rotation = resultRotation;
                _target.UpdateSamples();
            }
        }
    }

    public override void OnInspectorGUI()
    {
        BlendTree3D _target = (BlendTree3D)target;
        serializedObject.Update();

        if (GUILayout.Button("Add point"))
        {
            _target.AddPoint();
        }

        if (GUILayout.Button("Delete selected point"))
        {
            _target.DeleteSelectedPoint();
        }

        if (GUILayout.Button("Deselect all points"))
        {
            _target.DeselectAllPoints();
        }

        if (GUILayout.Button("Delete all points"))
        {
            _target.DeleteAllPoints();
        }

        if (GUILayout.Button("Create grid of points"))
        {
            _target.CreateGridOfPoints();
        }

        EditorGUILayout.Separator();
        EditorGUILayout.BeginVertical("HelpBox");
        EditorGUILayout.LabelField("Selected point");
        EditorGUILayout.Separator();

        foreach (BlendTree3DPoint point in _target.Points)
        {
            if (point.HighLight)
            {
                point.Position = EditorGUILayout.Vector3Field("Position", point.Position);

                point.Rotation = Vector4ToQuaternion(EditorGUILayout.Vector4Field("Rotation", QuaternionToVector4(point.Rotation)));
            }
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        EditorGUILayout.EndVertical();
        DrawDefaultInspector();
    }

    static Vector4 QuaternionToVector4(Quaternion rot)
    {
        return new Vector4(rot.x, rot.y, rot.z, rot.w);
    }

    static Quaternion Vector4ToQuaternion(Vector4 rot)
    {
        return new Quaternion(rot.x, rot.y, rot.z, rot.w);
    }
}
