using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private GameObject m_healthBar;

    public bool m_moveing = false;
    public Vector2 m_moveTo;
    private Rigidbody m_rigb;
    private SpriteRenderer m_render;

    public float m_speed = 1.0f;
    public int m_maxHealth = 5;
    private int m_health;

    public Sprite[] m_sprites;

    void Start()
    {
        m_healthBar = transform.GetChild(0).gameObject;
        m_healthBar.SetActive(false);

        m_rigb = GetComponent<Rigidbody>();
        m_render = GetComponent<SpriteRenderer>();
        m_health = m_maxHealth;

        TakeDamage(0);
    }
    
    void Update()
    {
        if (m_moveing)
        {
            if (Mathf.Abs(transform.position.x - m_moveTo.x) < 0.05f && Mathf.Abs(transform.position.y - m_moveTo.y) < 0.05f)
            {
                m_moveing = false;
            }
        }

        if (m_moveing)
        {
            Vector3 vel = Vector3.zero;
            
            if (Mathf.Abs(transform.position.x - m_moveTo.x) > 0.05f)
            {
                vel.x = transform.position.x > m_moveTo.x ? -1.0f : 1.0f;
            }
            
            if (Mathf.Abs(transform.position.y - m_moveTo.y) > 0.05f)
            {
                vel.y = transform.position.y > m_moveTo.y ? -1.0f : 1.0f;
            }

            int spriteIndex = (1 + (int)vel.x) + ((1 + ((int)vel.y * -1)) * 3);

            // this conpancates for the fact there is no sprite for no movement in eather direction
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
        m_healthBar.SetActive(true);
    }

    public void DeSelect()
    {
        Debug.Log("DeSelected");
        m_healthBar.SetActive(false);
    }

    public void Move(Vector2 MoveTo)
    {
        // tells this uint it needs to move and where it needs to move to
        m_moveing = true;
        m_moveTo = MoveTo;
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;

        m_health = Mathf.Min(m_health, m_maxHealth);

        // if this unit has no health remaining
        if (m_health <= 0)
        {
            ParticleSystem paint = GetComponentInChildren<ParticleSystem>();

            if (paint)
            {
                paint.transform.SetParent(null);
            }

            // destroy the unit
            Destroy(gameObject);
        }

        m_healthBar.transform.localScale = new Vector3((float)m_health / (float)m_maxHealth, m_healthBar.transform.localScale.y, m_healthBar.transform.localScale.z);
        m_healthBar.transform.localPosition = new Vector3((-1.0f + ((float)m_health / (float)m_maxHealth)) * 0.5f, m_healthBar.transform.localPosition.y, m_healthBar.transform.localPosition.z);
    }
}
