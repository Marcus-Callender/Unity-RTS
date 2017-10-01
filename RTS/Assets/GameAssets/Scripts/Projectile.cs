using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int m_damage = 1;
    public Sprite[] m_sprites;
    public bool m_targateFriendlyUnits = false;

    public float m_lifeTime = 20.0f;
    private Timer m_lifeTimer = new Timer();

    private SpriteRenderer m_render;
    private Rigidbody m_rigb;

    public int m_shooterid = -1;

    Explosion m_explosion;

    private void Start()
    {
        m_rigb = GetComponent<Rigidbody>();
        m_render = GetComponent<SpriteRenderer>();

        m_lifeTimer.m_time = m_lifeTime;
        m_lifeTimer.Play();

        m_explosion = GetComponent<Explosion>();
    }

    private void Update()
    {
        // if there is no currently assigned sprite
        if (!m_render.sprite)
        {
            int spriteIndex = (1 + FloatAsIntDirection(m_rigb.velocity.x)) + ((1 + (FloatAsIntDirection(m_rigb.velocity.y)  * -1)) * 3);

            // this conpancates for the fact there is no sprite for no movement in eather direction
            if (spriteIndex > 4)
            {
                spriteIndex -= 1;
            }

            m_render.sprite = m_sprites[spriteIndex];
        }

        m_lifeTimer.Cycle();

        if (m_lifeTimer.m_completed)
        {
            if (m_explosion)
            {
                m_explosion.Activate();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if the hit object is on the other team
        if ((other.tag == gameObject.tag) == m_targateFriendlyUnits)
        {
            Unit data = other.gameObject.GetComponent<Unit>();

            // and the hit object can take damage
            if (data && (data.m_id != m_shooterid))
            {
                data.TakeDamage(m_damage);

                // destroy this attack
                if (m_explosion)
                {
                    m_explosion.Activate();
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private int FloatAsIntDirection(float x)
    {
        if (x > 0.33f)
        {
            return 1;
        }
        else if (x < -0.33f)
        {
            return -1;
        }

        return 0;
    }
}
