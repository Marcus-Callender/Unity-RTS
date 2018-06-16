﻿using System.Collections;
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

    [SerializeField]
    private HexUnit m_targate;

    [SerializeField]
    private E_playerColours m_colour;

    public List<Vector3> m_path;
    public Sprite[] m_sprites;

    [SerializeField]
    public int m_maxHealth = 5;

    [SerializeField]
    private Animator m_animator;
    private int m_ShowCircleHash = Animator.StringToHash("ShowCircle");

    private int m_healthInternal;

    public int m_health {
        get { return m_healthInternal; }
        set
        {
            if (value != m_healthInternal)
            {
                if (del_OnHealthChanged != null)
                    del_OnHealthChanged(value);

                m_healthInternal = value;
            }
        }
    }

    public delegate void BecameInvisible();
    public BecameInvisible del_OnBecameInvisible;

    public delegate void HealthChanged(int val);
    public HealthChanged del_OnHealthChanged;

    void Awake()
    {
        m_health = m_maxHealth;
        PlayerManager.m_instance.RegisterUnit(this, m_colour);
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
                SquareGridManager.m_instance.FreeHex(m_path[0]);

                m_path.RemoveAt(0);

                SquareGridManager.m_instance.BlockHex(transform.position);
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
        //Debug.Log("Direction: " + dir.x.ToString("F2") + ", " + dir.y.ToString("F2"));
        int yIndex = dir.y > 0.333f ? 2 : (dir.y < -0.333f ? 0 : 1);
        int xIndex = dir.x > 0.333f ? 2 : (dir.x < -0.333f ? 0 : 1);
        m_renderer.sprite = m_sprites[(yIndex * 3) + xIndex];
    }

    private void SetSpriteFromDir()
    {
        Vector3 dir = m_path[1] - transform.position;
        dir.z = 0.0f;
        dir.Normalize();

        //Debug.Log("Direction: " + dir.x.ToString("F2") + ", " + dir.y.ToString("F2"));
        int xIndex = dir.x > 0.333f ? 2 : (dir.x < -0.333f ? 0 : 1);
        int yIndex = dir.y > 0.333f ? 2 : (dir.y < -0.333f ? 0 : 1);
        m_renderer.sprite = m_sprites[(yIndex * 3) + xIndex];
    }

    private void OnBecameVisible()
    {
        Debug.Log("Now visable.");

        HealthBarManager.m_instance.Register(this);
    }

    private void OnBecameInvisible()
    {
        Debug.Log("Now invisable.");

        if (del_OnBecameInvisible != null)
        {
            del_OnBecameInvisible();
        }
    }

    public void TakeDamage(int damage)
    {
        m_health -= damage;

        m_health = Mathf.Min(m_health, m_maxHealth);

        if (m_health <= 0)
        {
            Destroy(gameObject);

            SquareGridManager.m_instance.FreeHex(transform.position);

            if (del_OnBecameInvisible != null)
                del_OnBecameInvisible();
        }
    }

    public void Select()
    {
        m_animator.SetBool(m_ShowCircleHash, true);
    }

    public void DeSelect()
    {
        m_animator.SetBool(m_ShowCircleHash, false);
    }

    void OnDestroy()
    {
        del_OnBecameInvisible = null;
        del_OnHealthChanged = null;
    }
}
