using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    SpriteAnimation m_anim;
    public Sprite[] m_explosionSprites;

    protected SpriteRenderer m_render;

    private bool m_activated = false;
    
    void Start()
    {
        m_render = GetComponent<SpriteRenderer>();
        m_anim = new SpriteAnimation(m_explosionSprites);
    }

    void Update()
    {
        if (m_activated)
        {
            m_render.sprite = m_anim.Cyce();

            if (m_anim.m_completed)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Activate()
    {
        m_activated = true;
        m_anim.Play();
        m_render.sprite = m_explosionSprites[0];

        Rigidbody rigb = GetComponent<Rigidbody>();

        if (rigb)
        {
            rigb.velocity = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
