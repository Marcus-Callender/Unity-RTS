using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    protected GameObject m_healthBar;
    protected float m_healtharSize;

    public bool m_moveing = false;
    public Vector2 m_moveTo;
    protected Rigidbody m_rigb;
    protected SpriteRenderer m_render;
    public UnitData m_data;

    public float m_speed = 1.0f;
    public int m_maxHealth = 5;
    public int m_health;

    public Sprite[] m_sprites;

    static int M_ID_COUNT = 0;
    public int m_id;

    public Vector2[] m_path;

    void Start()
    {
        m_id = M_ID_COUNT;
        M_ID_COUNT += 1;

        m_healthBar = transform.GetChild(0).gameObject;
        m_healthBar.SetActive(false);
        m_healtharSize = m_healthBar.transform.localScale.x;

        m_rigb = GetComponent<Rigidbody>();
        m_render = GetComponent<SpriteRenderer>();
        m_data = GetComponent<UnitData>();
        m_health = m_maxHealth;

        Attack attack = GetComponentInChildren<Attack>();

        if (attack)
        {
            attack.m_id = m_id;
        }

        TakeDamage(0);
    }

    void Update()
    {
        if (m_path.Length > 0 && !m_moveing)
        {
            m_moveTo = m_path[m_path.Length -1];
            m_path[m_path.Length - 1] = Vector2.zero;
            m_moveing = true;
        }

        if (m_data.m_targateUnit)
        {
            Debug.DrawRay(transform.position, m_data.m_targateUnit.transform.position - transform.position, Color.red);

            if (Vector3.Distance(transform.position, m_data.m_targateUnit.transform.position) > 2.0f)
            {
                Vector3 vel = Vector3.zero;

                if (Mathf.Abs(transform.position.x - m_data.m_targateUnit.transform.position.x) > 0.33f)
                {
                    vel.x = transform.position.x > m_data.m_targateUnit.transform.position.x ? -1.0f : 1.0f;
                }

                if (Mathf.Abs(transform.position.y - m_data.m_targateUnit.transform.position.y) > 0.33f)
                {
                    vel.y = transform.position.y > m_data.m_targateUnit.transform.position.y ? -1.0f : 1.0f;
                }

                m_data.Rotate(m_data.Vec2ToIndex(vel));

                m_render.sprite = m_sprites[m_data.m_rotationIndex];

                if (m_data.Vec2ToIndex(vel) == m_data.m_rotationIndex)
                {
                    m_rigb.velocity = m_data.RotationVec2() * m_speed;
                }
                else
                {
                    m_rigb.velocity = Vector3.zero;
                }
            }
        }
        else
        {
            if (m_moveing)
            {
                if (Mathf.Abs(transform.position.x - m_moveTo.x) < 0.1f && Mathf.Abs(transform.position.y - m_moveTo.y) < 0.1f)
                {
                    m_moveing = false;
                }
            }

            if (m_moveing)
            {
                Vector3 vel = Vector3.zero;

                if (Mathf.Abs(transform.position.x - m_moveTo.x) > 0.33f)
                {
                    vel.x = transform.position.x > m_moveTo.x ? -1.0f : 1.0f;
                }

                if (Mathf.Abs(transform.position.y - m_moveTo.y) > 0.33f)
                {
                    vel.y = transform.position.y > m_moveTo.y ? -1.0f : 1.0f;
                }

                Debug.DrawRay(transform.position, (Vector3)m_moveTo - transform.position, Color.yellow);

                m_data.Rotate(m_data.Vec2ToIndex(vel));

                m_render.sprite = m_sprites[m_data.m_rotationIndex];

                if (m_data.Vec2ToIndex(vel) == m_data.m_rotationIndex)
                {
                    m_rigb.velocity = m_data.RotationVec2() * m_speed;
                }
                else
                {
                    m_rigb.velocity = Vector3.zero;
                }
            }
            else
            {
                m_rigb.velocity = Vector3.zero;
            }
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

        m_healthBar.transform.localScale = new Vector3(((float)m_health / (float)m_maxHealth) * m_healtharSize, m_healthBar.transform.localScale.y, m_healthBar.transform.localScale.z);
        m_healthBar.transform.localPosition = new Vector3((-1.0f + ((float)m_health / (float)m_maxHealth)) * 0.5f, m_healthBar.transform.localPosition.y, m_healthBar.transform.localPosition.z);
    }
}
