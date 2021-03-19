using System;
using UnityEngine;

[Serializable]
public class BlendTree3DPoint
{
    public Vector3 Position;
    public Quaternion Rotation;
    public bool HighLight;

    public BlendTree3DPoint(Vector3 Position, Quaternion Rotation, bool highLight = false)
    {
        this.Position = Position;
        this.Rotation = Rotation;
        HighLight = highLight;
    }
}
