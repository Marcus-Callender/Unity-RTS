using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour
{
    [SerializeField]
    private Rigidbody m_rigb;

    [SerializeField]
    private SpriteRenderer m_renderer;

    [SerializeField]
    private float m_speed = 1.0f;

    public List<Vector3> m_path;

    public Sprite[] m_sprites;

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
            
            {
                SetSpriteFromDir(direction);
            }

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

    //     0 1 2
    //     - - -
    // 2 | 6 7 8
    // 1 | 3 4 5
    // 0 | 0 1 2
    private void SetSpriteFromDir(Vector3 dir)
    {
        Debug.Log("Direction: " + dir.x.ToString("F2") + ", " + dir.y.ToString("F2"));
        int yIndex = dir.y > 0.333f ? 2 : (dir.y < -0.333f ? 0 : 1);
        int xIndex = dir.x > 0.333f ? 2 : (dir.x < -0.333f ? 0 : 1);
        m_renderer.sprite = m_sprites[(yIndex * 3) + xIndex];
    }

    private void SetSpriteFromDir()
    {
        Vector3 dir = m_path[1] - transform.position;
        dir.z = 0.0f;
        dir.Normalize();

        Debug.Log("Direction: " + dir.x.ToString("F2") + ", " + dir.y.ToString("F2"));
        int xIndex = dir.x > 0.333f ? 2 : (dir.x < -0.333f ? 0 : 1);
        int yIndex = dir.y > 0.333f ? 2 : (dir.y < -0.333f ? 0 : 1);
        m_renderer.sprite = m_sprites[(yIndex * 3) + xIndex];
    }
}
