using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Unit
{
    public Sprite[] m_idle;
    public Sprite[] m_working;
    public Sprite[] m_appering;
    public Sprite[] m_damaged;

    private SpriteAnimation m_idleAnim;
    private SpriteAnimation m_workingAnim;
    private SpriteAnimation m_apperingAnim;
    private SpriteAnimation m_damagedAnim;

    //private Sprite[] m_currentAnim;
    private SpriteAnimation m_currentAnim;

    //private Timer m_animChange;
    private float m_animChangeTime = 0.16f;
    private int m_animIndex = 0;

    void Start()
    {

        m_healthBar = transform.GetChild(0).gameObject;
        m_healthBar.SetActive(false);
        m_healtharSize = m_healthBar.transform.localScale.x;

        m_render = GetComponent<SpriteRenderer>();
        m_data = GetComponent<UnitData>();
        m_health = m_maxHealth;

        Attack attack = GetComponentInChildren<Attack>();

        if (attack)
        {
            attack.m_id = m_id;
        }

        TakeDamage(0);

        //m_animChange = new Timer();
        //m_animChange.m_time = m_animChangeTime;
        //m_animChange.Play();

        //m_currentAnim = m_appering;
        //m_render.sprite = m_currentAnim[m_animIndex];

        m_idleAnim = new SpriteAnimation(m_idle, true);
        m_workingAnim = new SpriteAnimation(m_working, true);
        m_apperingAnim = new SpriteAnimation(m_appering);
        m_damagedAnim = new SpriteAnimation(m_damaged);

        m_currentAnim = m_apperingAnim;
        m_currentAnim.Play();
    }

    void Update()
    {
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

                //m_render.sprite = m_sprites[m_data.m_rotationIndex];
            }
        }
        else
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

                if (Mathf.Abs(transform.position.x - m_moveTo.x) > 0.33f)
                {
                    vel.x = transform.position.x > m_moveTo.x ? -1.0f : 1.0f;
                }

                if (Mathf.Abs(transform.position.y - m_moveTo.y) > 0.33f)
                {
                    vel.y = transform.position.y > m_moveTo.y ? -1.0f : 1.0f;
                }

                m_data.Rotate(m_data.Vec2ToIndex(vel));

                //m_render.sprite = m_sprites[m_data.m_rotationIndex];
            }
        }

        //m_animChange.Cycle();
        //
        //if (m_animChange.m_completed)
        //{
        //    m_animChange.Play();
        //
        //    m_animIndex++;
        //
        //    if (m_animIndex >= m_currentAnim.Length)
        //    {
        //        if (m_currentAnim == m_appering)
        //        {
        //            m_currentAnim = m_idle;
        //        }
        //
        //        m_animIndex = 0;
        //    }
        //
        //    m_render.sprite = m_currentAnim[m_animIndex];
        //}
        
        m_render.sprite = m_currentAnim.Cyce();
        
        if (m_currentAnim.m_completed)
        {
            if (m_currentAnim == m_apperingAnim || m_currentAnim == m_workingAnim)
            {
                m_currentAnim = m_idleAnim;
            }

            m_currentAnim.Play();
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
