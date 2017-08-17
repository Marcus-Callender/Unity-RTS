using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameObject HealthBar;

    private bool m_moveing = false;
    private Vector2 m_moveTo;

    void Start()
    {
        HealthBar = transform.GetChild(0).gameObject;
        HealthBar.SetActive(false);
    }
    
    void Update()
    {
        if (m_moveing)
        {
            transform.position = new Vector3(m_moveTo.x, m_moveTo.y, transform.position.z);
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
