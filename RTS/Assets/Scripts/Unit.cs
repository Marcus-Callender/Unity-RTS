using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameObject HealthBar;

    private bool m_moveing = false;
    private Vector2 m_moveTo;
    private Rigidbody m_rigb;

    public float m_speed = 1.0f;

    void Start()
    {
        HealthBar = transform.GetChild(0).gameObject;
        HealthBar.SetActive(false);

        m_rigb = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (m_moveing)
        {
            if (Mathf.Abs(transform.position.x - m_moveTo.x) < 0.01f && Mathf.Abs(transform.position.y - m_moveTo.y) < 0.01f)
            {
                m_moveing = false;
            }
        }

        if (m_moveing)
        {
            Vector3 vel = Vector3.zero;
            
            if (Mathf.Abs(transform.position.x - m_moveTo.x) > 0.01f)
            {
                vel.x = transform.position.x > m_moveTo.x ? -1.0f : 1.0f;
            }
            
            if (Mathf.Abs(transform.position.y - m_moveTo.y) > 0.01f)
            {
                vel.y = transform.position.y > m_moveTo.y ? -1.0f : 1.0f;
            }

            vel.Normalize();

            vel *= m_speed;

            m_rigb.velocity = vel;
        }
        else
        {
            m_rigb.velocity = Vector3.zero;
        }
    }

    public void Select()
    {
        Debug.Log("Selected");
        HealthBar.SetActive(true);
    }

    public void DeSelect()
    {
        Debug.Log("DeSelected");
        HealthBar.SetActive(false);
    }

    public void Move(Vector2 MoveTo)
    {
        m_moveing = true;
        m_moveTo = MoveTo;
    }
}
