using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_rigb;

    [SerializeField]
    private float m_speed = 1.0f;

    public List<Vector3> m_path;

    void Start()
    {

    }

    void Update()
    {
        if (m_path.Count > 0)
        {
            Vector3 direction = m_path[0] - transform.position;
            direction.z = 0.0f;
            direction.Normalize();

            m_rigb.velocity = direction * m_speed;

            if (Mathf.Abs((transform.position - m_path[0]).magnitude) < 0.05f)
            {
                m_path.RemoveAt(0);
            }
        }
        else
        {
            m_rigb.velocity = Vector3.zero;
        }

        for (int z = 0; z < m_path.Count - 1; z++)
        {
            Debug.DrawLine(m_path[z], m_path[z + 1], Color.red);
        }
    }
}
