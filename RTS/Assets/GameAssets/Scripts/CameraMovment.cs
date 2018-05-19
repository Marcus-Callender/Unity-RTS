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

    public float m_scrollSpeed = 1.0f;

    private Camera m_cam;

    void Start()
    {
        m_cam = GetComponent<Camera>();
    }

    void Update()
    {
        float newX = transform.position.x;
        float newY = transform.position.y;

        if (Input.GetButton("Fire3"))
        {
            //Debug.Log("Fire3");
            newX -= Input.GetAxis("Mouse X") * 0.5f;
            newY -= Input.GetAxis("Mouse Y") * 0.5f;

            /*if (Input.GetAxis("Mouse X") != 0.0f)
            {
                Debug.Log("X axis");
            }*/
        }
        else
        {
            m_cam.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * m_scrollSpeed;

            m_cam.orthographicSize = Mathf.Clamp(m_cam.orthographicSize, m_minZ, m_maxZ);
        }

        newX = Mathf.Clamp(newX, m_minX - ((1.0f - m_cam.orthographicSize) * 1.775f), m_maxX + ((1.0f - m_cam.orthographicSize) * 1.775f));
        newY = Mathf.Clamp(newY, m_minY - (1.0f - m_cam.orthographicSize), m_maxY + (1.0f - m_cam.orthographicSize));

        transform.position = new Vector3(newX, newY, transform.position.z);
    }
}
