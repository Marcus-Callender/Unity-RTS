using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameObject HealthBar;

    private bool m_moveing = false;
    private Vector2 m_moveTo;
    private Rigidbody m_rigb;

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
            transform.position = new Vector3(m_moveTo.x, m_moveTo.y, transform.position.z);

            //Vector3 vel = Vector3.zero;
            //
            //if (!Mathf.Approximately(transform.position.x, m_moveTo.x))
            //{
            //    vel.x = transform.position.x > m_moveTo.x ? -1.0f : 1.0f;
            //}
            //
            //if (!Mathf.Approximately(transform.position.y, m_moveTo.y))
            //{
            //    vel.y = transform.position.y > m_moveTo.y ? -1.0f : 1.0f;
            //}
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
