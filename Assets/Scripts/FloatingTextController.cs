using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float destroyTime = 2f;

    void Start()
    {
        Destroy(gameObject, destroyTime);
        transform.position += offset;
    }
}
