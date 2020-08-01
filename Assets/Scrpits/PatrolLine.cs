﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class PatrolLine : MonoBehaviour
{

    private GameObject[] nodes;
    private Vector3 distance;

    private bool getDistance = true;
    private bool backAndForth = true;

    private int distanceCount;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotateSpeed = 5f;

    void Start()
    {
        nodes = new GameObject[transform.childCount];
        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = transform.GetChild(0).gameObject;
            nodes[i].transform.SetParent(transform.parent);
        }
    }

    void FixedUpdate()
    {
        transform.Rotate(0, 0, rotateSpeed);
        goToNodes();
    }

    void goToNodes()
    {
        if (getDistance)
        {
            distance = (nodes[distanceCount].transform.position - transform.position).normalized;
            getDistance = false;
        }

        float mesafe = Vector3.Distance(transform.position, nodes[distanceCount].transform.position);
        transform.position += distance * Time.deltaTime * speed;

        if (mesafe < 0.5f)
        {
            getDistance = true;
            if (distanceCount == nodes.Length - 1)
            {
                backAndForth = false;
            }
            else if (distanceCount == 0)
            {
                backAndForth = true;
            }

            if (backAndForth)
            {
                distanceCount++;
            }
            else
            {
                distanceCount--;
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.GetChild(i).transform.position, 1);
        }
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.GetChild(i).transform.position, transform.GetChild(i + 1).transform.position);
        }
    }
#endif

}

#if UNITY_EDITOR
[CustomEditor(typeof(PatrolLine))]
[System.Serializable]

class partolLineEditor : Editor
{
    public override void OnInspectorGUI()
    {
        PatrolLine script = (PatrolLine)target;
        if (GUILayout.Button("Create", GUILayout.MinWidth(100), GUILayout.Width(100)))
        {
            GameObject newObject = new GameObject();
            newObject.transform.parent = script.transform;
            newObject.transform.position = script.transform.position;
            newObject.name = script.transform.childCount.ToString();
        }
        EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rotateSpeed"));
        serializedObject.ApplyModifiedProperties();
        serializedObject.Update();
    }
}
#endif

