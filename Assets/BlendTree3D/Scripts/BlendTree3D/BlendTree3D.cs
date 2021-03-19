using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class BlendTree3D : MonoBehaviour
{
    public List<BlendTree3DPoint> Points;
    public Vector4 Corners = new Vector4(-1, -1, 2, 2);
    public bool ShowGizmos = true;
    public Mesh meshForRotation;

    public int GridSubdivision = 2;

    private Transform _transform;

    private PolarGradientBandInterpolator _polarGradientBandInterpolator;
    private float[][] _pointsCoordinates;

    private void Awake()
    {
        UpdateSamples();
    }

    public void UpdateSamples()
    {
        _pointsCoordinates = new float[Points.Count][];

        for (int i = 0; i < Points.Count; i++)
        {
            _pointsCoordinates[i] = new[] { Points[i].Position.x, Points[i].Position.y };
        }

        _polarGradientBandInterpolator = new PolarGradientBandInterpolator(_pointsCoordinates);
    }

    public float InterpolateDepth(float x, float y)
    {
        float[] interpolated = _polarGradientBandInterpolator.Interpolate(new[] { x, y }, true);

        float z = 0;

        for (int i = 0; i < interpolated.Length; i++)
        {
            if (interpolated[i] != 0)
            {
                z += Points[i].Position.z * interpolated[i];
            }
        }

        return z;
    }

    public Quaternion InterpolateRotation(float x, float y, Vector3 forward, Vector3 up)
    {
        float[] interpolated = _polarGradientBandInterpolator.Interpolate(new[] { x, y }, true);

        Vector3 localForward = Vector3.zero;
        Vector3 localUp = Vector3.zero;

        for (int i = 0; i < interpolated.Length; i++)
        {
            localForward += Points[i].Rotation * forward * interpolated[i];
            localUp += Points[i].Rotation * up * interpolated[i];
        }

        return Quaternion.LookRotation(localForward.normalized, localUp.normalized);
    }

    private void Reset()
    {
        Points = new List<BlendTree3DPoint>();
    }

    private void OnValidate()
    {
        _transform = transform;
    }

    private void OnDrawGizmos()
    {
        if (ShowGizmos)
        {
            Gizmos.matrix = _transform.localToWorldMatrix;
            DrawGizmoSquare(Corners.x, Corners.y, Corners.z, Corners.w);

            foreach (BlendTree3DPoint point in Points)
            {
                Gizmos.color = new Color(0 + point.Position.z, 0 + point.Position.z, +point.Position.z, 1);

                if (point.HighLight)
                {
                    Gizmos.color = Color.red;
                }

                Gizmos.DrawSphere(point.Position, 0.05f);

                Gizmos.color = Color.green;

                Gizmos.DrawMesh(meshForRotation, point.Position, point.Rotation, new Vector3(0.05f, 0.05f, 0.05f));
            }
        }
    }

    private void DrawGizmoSquare(float startingPointX, float startingPointY, float xLenght, float yLenght)
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawLine(new Vector3(startingPointX, startingPointY, 0), new Vector3(startingPointX, startingPointY + yLenght, 0));

        Gizmos.DrawLine(new Vector3(startingPointX, startingPointY + yLenght, 0), new Vector3(startingPointX + xLenght, startingPointY + yLenght, 0));

        Gizmos.DrawLine(new Vector3(startingPointX + xLenght, startingPointY + yLenght, 0), new Vector3(startingPointX + xLenght, startingPointY, 0));

        Gizmos.DrawLine(new Vector3(startingPointX + xLenght, startingPointY, 0), new Vector3(startingPointX, startingPointY, 0));
    }

    [ContextMenu("AddPoint")]
    public void AddPoint()
    {
        Points.Add(new BlendTree3DPoint(new Vector3(Corners.x, Corners.y, 0), Quaternion.identity));
    }

    [ContextMenu("DeselectAllPoints")]
    public void DeselectAllPoints()
    {
        foreach (BlendTree3DPoint point in Points)
        {
            point.HighLight = false;
        }
    }

    [ContextMenu("CreateGridOfPoints")]
    public void CreateGridOfPoints()
    {
        Points.Clear();

        for (int i = -GridSubdivision; i <= GridSubdivision; i++)
        {
            for (int j = -GridSubdivision; j <= GridSubdivision; j++)
            {
                Vector3 position = new Vector3(i / (float)GridSubdivision, j / (float)GridSubdivision, (((-i * i) + (-j * j)) / (float)GridSubdivision / 10) + 1);
                Quaternion rotation = Quaternion.LookRotation(position);
                Points.Add(new BlendTree3DPoint(position, rotation));
            }
        }
    }

    [ContextMenu("DeleteAllPoints")]
    public void DeleteAllPoints()
    {
        Points.Clear();
    }

    [ContextMenu("DeleteSelectedPoint")]
    public void DeleteSelectedPoint()
    {
        foreach (BlendTree3DPoint point in FindSelectedPoints())
        {
            Points.Remove(point);
        }
    }

    private List<BlendTree3DPoint> FindSelectedPoints()
    {
        List<BlendTree3DPoint> selectedPoints = new List<BlendTree3DPoint>();

        foreach (BlendTree3DPoint point in Points)
        {
            if (point.HighLight)
            {
                selectedPoints.Add(point);
            }
        }

        return selectedPoints;
    }
}
