﻿using System.Collections;
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

    private SpriteAnimation m_currentAnim;

    private float m_animChangeTime = 0.16f;
    private int m_animIndex = 0;

    public GameObject m_uintToBuild = null;

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

        m_idleAnim = new SpriteAnimation(m_idle, true);
        m_workingAnim = new SpriteAnimation(m_working, true);
        m_apperingAnim = new SpriteAnimation(m_appering);
        m_damagedAnim = new SpriteAnimation(m_damaged);

        m_currentAnim = m_apperingAnim;
        m_currentAnim.Play();
    }

    void Update()
    {
        if (m_currentAnim == m_workingAnim && m_uintToBuild && m_currentAnim.m_reverse)
        {
            Instantiate(m_uintToBuild, transform.position + new Vector3(0.0f, -1.5f, 0.0f), Quaternion.identity);
            m_uintToBuild = null;
        }
        
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

    public void BuildUnit(GameObject unitToBuild)
    {
        m_uintToBuild = unitToBuild;

        m_currentAnim = m_workingAnim;
        m_currentAnim.Play();
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
