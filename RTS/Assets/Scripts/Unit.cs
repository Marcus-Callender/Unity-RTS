using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameObject HealthBar;

    public bool m_moveing = false;
    public Vector2 m_moveTo;
    private Rigidbody m_rigb;
    private SpriteRenderer m_render;

    public float m_speed = 1.0f;

    public Sprite[] m_sprites;

    void Start()
    {
        HealthBar = transform.GetChild(0).gameObject;
        HealthBar.SetActive(false);

        m_rigb = GetComponent<Rigidbody>();
        m_render = GetComponent<SpriteRenderer>();
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

            int spriteIndex = (1 + (int)vel.x) + ((1 + ((int)vel.y * -1)) * 3);

            if (spriteIndex > 4)
            {
                spriteIndex -= 1;
            }

            m_render.sprite = m_sprites[spriteIndex];

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
