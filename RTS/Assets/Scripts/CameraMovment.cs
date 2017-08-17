using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovment : MonoBehaviour
{
    public float m_minX = -13.5f;
    public float m_maxX = 13.5f;

    public float m_minY = -10.0f;
    public float m_maxY = 10.0f;

    public float m_minZ = -12.0f;
    public float m_maxZ = -8.0f;

    void Start()
    {

    }

    void Update()
    {
        Debug.Log("Update");

        float newX = transform.position.x;
        float newY = transform.position.y;
        float newZ = transform.position.z;

        if (Input.GetButton("Fire3"))
        {
            Debug.Log("Fire3");
            newX -= Input.GetAxis("Mouse X");
            newY -= Input.GetAxis("Mouse Y");

             if (Input.GetAxis("Mouse X") != 0.0f)
            {
                Debug.Log("X axis");
            }
        }
        else
        {
            newZ += Input.GetAxis("Mouse ScrollWheel");
        }

        newX = Mathf.Clamp(newX, m_minX, m_maxX);
        newY = Mathf.Clamp(newY, m_minY, m_maxY);
        newZ = Mathf.Clamp(newZ, m_minZ, m_maxZ);

        transform.position = new Vector3(newX, newY, newZ);
    }
}
