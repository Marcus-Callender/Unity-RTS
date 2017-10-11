using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    protected GameObject m_healthBar;
    protected float m_healtharSize;

    public bool m_moveing = false;
    protected Rigidbody m_rigb;
    protected SpriteRenderer m_render;
    public UnitData m_data;

    //public float m_speed = 1.0f;
    public int m_maxHealth = 5;
    public int m_health;

    public Sprite[] m_sprites;

    static int M_ID_COUNT = 0;
    public int m_id;

    UnitMovement m_movement;

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

        m_movement = GetComponent<UnitMovement>();
    }

    void Update()
    {
        //if (m_movement)
        //{
        //    m_movement.Cycle();
        //
        //    m_render.sprite = m_sprites[m_movement.m_currentRot];
        //}
        
        if (m_movement)
        {
            m_movement.move();

            m_render.sprite = m_sprites[m_movement.m_currentRotation];
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
        m_data.m_moveTo = MoveTo;

        m_movement.m_destination = MoveTo;
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
