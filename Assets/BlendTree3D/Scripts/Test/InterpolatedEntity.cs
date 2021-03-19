using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InterpolatedEntity : MonoBehaviour
{
    [SerializeField] private BlendTree3D _blendTree3D;
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        Vector3 localPosition = new Vector3(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), _blendTree3D.InterpolateDepth(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")));
        _transform.localPosition = localPosition;
        Quaternion localRotation = _blendTree3D.InterpolateRotation(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), _blendTree3D.transform.forward, _blendTree3D.transform.up);
        _transform.localRotation = localRotation;
    }
}
