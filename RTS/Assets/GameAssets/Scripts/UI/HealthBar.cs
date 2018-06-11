using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    GameObject m_redBar;

    [SerializeField]
    GameObject m_greenBar;

    [SerializeField]
    private float m_offset = 0.5f;

    [SerializeField]
    private Image[] m_images;

    public bool m_active = false;

    private GameObject m_toFollow;

    HexUnit m_currentUnit;

    void Start()
    {
        for (int z = 0; z < m_images.Length; z++)
        {
            m_images[z].enabled = false;
        }
    }

    void Update()
    {
        if (m_active)
        {
            transform.position = Camera.main.WorldToScreenPoint(m_toFollow.transform.position);
            transform.position += Vector3.up * m_offset;
        }
    }

    public void Register(HexUnit unit)
    {
        m_active = true;
        m_currentUnit = unit;
        m_toFollow = unit.gameObject;

        for (int z = 0; z < m_images.Length; z++)
        {
            m_images[z].enabled = true;
        }

        m_currentUnit.del_OnBecameInvisible += DeRegister;
    }

    private void DeRegister()
    {
        m_active = false;
        m_toFollow = null;

        for (int z = 0; z < m_images.Length; z++)
        {
            m_images[z].enabled = false;
        }

        m_currentUnit.del_OnBecameInvisible -= DeRegister;

        m_currentUnit = null;
    }
}
