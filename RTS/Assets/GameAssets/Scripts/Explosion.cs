using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    //[SerializeField]
    //private SpriteAnimation m_anim;

    [SerializeField]
    private Sprite[] m_explosionSprites;

    [SerializeField]
    private SpriteRenderer m_render;

    [SerializeField]
    private Rigidbody m_rigidbody;

    [SerializeField]
    private float m_frameDisplayTime = 0.032f;
    private float m_frameDisplayTimer = 0.0f;

    private int m_frameIndex = 0;

    private bool m_activated = false;
    
    void Update()
    {
        if (m_activated)
        {
            /*m_render.sprite = m_anim.Cyce();

            if (m_anim.m_completed)
            {
                Destroy(gameObject);
            }*/

            m_frameDisplayTimer -= Time.deltaTime;
            if (m_frameDisplayTimer <= 0.0f)
            {
                m_frameIndex++;

                if (m_frameIndex == m_explosionSprites.Length)
                {
                    Destroy(gameObject);
                }
                else
                {
                    m_frameDisplayTimer = m_frameDisplayTime;
                    m_render.sprite = m_explosionSprites[m_frameIndex];
                }
            }
        }
    }

    public void Activate()
    {
        if (!m_activated)
        {
            m_activated = true;
            ///m_anim.Play();
            m_render.sprite = m_explosionSprites[0];
            m_frameDisplayTimer = m_frameDisplayTime;
    
            m_rigidbody.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
