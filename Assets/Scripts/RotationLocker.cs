using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLocker : MonoBehaviour
{
    private Quaternion childRotation;

    private void Awake()
    {
        childRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        transform.rotation = childRotation;
    }
}
