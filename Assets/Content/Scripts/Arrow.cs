using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 initiadirection, lastMousePosition;
    [SerializeField, Range(0, 10)] float draggingSmooth = 1f;

    private void Awake()
    {
        initiadirection = transform.eulerAngles;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            Vector3 rotation = transform.eulerAngles;
            if (rotation != initiadirection)
            {
                initiadirection = rotation;
                Ball.Instance.Launch();
            }
        }

        if (isDragging)
        {
            Vector3 deltaMousePosition = Input.mousePosition - lastMousePosition;
            float rotationSpeed = deltaMousePosition.x * Time.deltaTime * 1.0f; 
            transform.Rotate(Vector3.up, rotationSpeed);

            lastMousePosition = Input.mousePosition;
        }
    }
}