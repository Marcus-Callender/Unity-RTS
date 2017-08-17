using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private Timer m_leftClickTimer;
    private Camera m_cam;

    private Unit m_selectedUnits;

    void Start()
    {
        m_leftClickTimer = new Timer();
        m_leftClickTimer.m_time = 0.2f;

        m_cam = GameObject.Find("Main Camera").GetComponent<Camera>();
    }

    void Update()
    {
        Vector3 ray = m_cam.ScreenToWorldPoint(Input.mousePosition);
        Debug.DrawRay(ray, Vector3.forward * 11, Color.yellow);

        if (Input.GetButtonDown("Fire1"))
        {
            m_leftClickTimer.Play();
        }

        m_leftClickTimer.Cycle();

        if (Input.GetButtonUp("Fire1"))
        {
            if (m_leftClickTimer.m_playing && !m_leftClickTimer.m_completed)
            {
                Debug.Log("Click");

                RaycastHit hit;

                if (Physics.Raycast(ray, Vector3.forward * 11, out hit))
                {
                    Unit _unit = hit.transform.gameObject.GetComponent<Unit>();
                    
                    if (_unit)
                    {
                        _unit.Select();
                    }
                }
            }
        }

        if (Input.GetButton("Fire1") && m_leftClickTimer.m_completed)
        {
            Debug.Log("Hold");
        }
    }
}
