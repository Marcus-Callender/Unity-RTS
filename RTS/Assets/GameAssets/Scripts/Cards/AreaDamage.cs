using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaDamage : MonoBehaviour
{
    public SphereCollider m_radius;
    public int m_damage;
    public Sprite[] m_sprites;

    private SpriteRenderer m_sprite;
    private SpriteAnimation m_anim;

    public bool m_reverseAnim = false;

    void Start()
    {
        m_sprite = GetComponent<SpriteRenderer>();

        m_radius = GetComponent<SphereCollider>();

        m_anim = new SpriteAnimation(m_sprites);
        m_anim.m_reverseAfterFinishing = m_reverseAnim;
        m_anim.Play();
    }

    void Update()
    {
        m_sprite.sprite = m_anim.Cyce();

        if (m_anim.m_completed)
        {
            Destroy(gameObject);
        }
    }

    public void Activate()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        Unit data = other.GetComponent<Unit>();

        if (data)
        {
            data.TakeDamage(m_damage);
        }
    }
}
