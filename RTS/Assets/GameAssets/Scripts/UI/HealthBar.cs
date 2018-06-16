using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    Image m_redBar;

    [SerializeField]
    Image m_greenBar;

    [SerializeField]
    private float m_offset = 0.5f;

    [SerializeField]
    private Image[] m_images;

    public bool m_active = false;

    private GameObject m_toFollow;

    HexUnit m_currentUnit;

    [SerializeField]
    float m_redHealthDelay = 0.25f;
    float m_redHealthTimer;

    [SerializeField]
    float m_redHealthSpeed = 3.0f;

    [SerializeField]
    private Animator m_animator;
    private int m_isVisableHash = Animator.StringToHash("IsVisable");

    private float m_redHealth;

    void Start()
    {
        m_redBar.fillAmount = 1.0f;
        m_greenBar.fillAmount = 1.0f;
    }

    void Update()
    {
        if (m_active)
        {
            transform.position = Camera.main.WorldToScreenPoint(m_toFollow.transform.position);
            transform.position += Vector3.up * m_offset;

            if (m_redHealthTimer > 0.0f)
            {
                m_redHealthTimer -= Time.deltaTime;
            }
            else
            {
                if (m_redHealth != m_currentUnit.m_health)
                {
                    m_redHealth -= m_redHealthSpeed * Time.deltaTime;

                    m_redHealth = Mathf.Max(m_redHealth, m_currentUnit.m_health, 0.0f);

                    m_redBar.fillAmount = m_redHealth / m_currentUnit.m_maxHealth;
                }
            }
        }
    }

    public void Register(HexUnit unit)
    {
        m_active = true;
        m_currentUnit = unit;
        m_toFollow = unit.gameObject;
        
        m_animator.SetBool(m_isVisableHash, true);

        m_currentUnit.del_OnBecameInvisible += DeRegister;
        m_currentUnit.del_OnHealthChanged += OnHealthChanged;

        m_redBar.fillAmount = m_currentUnit.m_health / m_currentUnit.m_maxHealth;
        m_greenBar.fillAmount = m_currentUnit.m_health / m_currentUnit.m_maxHealth;
    }

    private void DeRegister()
    {
        m_active = false;
        m_toFollow = null;
        
        m_animator.SetBool(m_isVisableHash, false);

        m_currentUnit.del_OnBecameInvisible -= DeRegister;
        m_currentUnit.del_OnHealthChanged -= OnHealthChanged;

        m_currentUnit = null;
    }

    private void OnHealthChanged(int health)
    {
        m_greenBar.fillAmount = (float)health / m_currentUnit.m_maxHealth;

        if (m_redHealthTimer <= 0.0f)
            m_redHealthTimer = m_redHealthDelay;
    }

    void OnDestroy()
    {
        if (m_currentUnit)
        {
            m_currentUnit.del_OnBecameInvisible -= DeRegister;
            m_currentUnit.del_OnHealthChanged -= OnHealthChanged;
        }
    }
}
