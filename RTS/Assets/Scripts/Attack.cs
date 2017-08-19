using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject m_projectile;
    private DetectObjectsInTrigger m_targates;

    public float m_fireDelay = 1.0f;
    public float m_bulletSpeed = 3.0f;

    private Timer m_fireTimer;

    void Start()
    {
        m_fireTimer = new Timer();
        m_fireTimer.m_time = m_fireDelay;

        m_targates = GetComponent<DetectObjectsInTrigger>();
    }

    void Update()
    {
        GameObject targate = null;

        m_targates.CheckRefrences();

        foreach (Unit tempTargate in m_targates.m_UnitsInTrigger)
        {
            if (tempTargate.gameObject.tag != gameObject.tag)
            {
                targate = tempTargate.gameObject;
                break;
            }
        }

        if (targate)
        {
            if (!m_fireTimer.m_playing)
            {
                m_fireTimer.Play();
            }

            m_fireTimer.Cycle();

            if (m_fireTimer.m_completed)
            {
                m_fireTimer.Stop();

                GameObject newProjectile = Instantiate(m_projectile, gameObject.transform.position, m_projectile.transform.rotation);

                Vector3 BulletVelocity = targate.transform.position - transform.position;

                BulletVelocity.Normalize();

                BulletVelocity *= m_bulletSpeed;

                newProjectile.GetComponent<Rigidbody>().velocity = BulletVelocity;

                newProjectile.tag = gameObject.tag;
            }
        }
        else
        {
            m_fireTimer.Stop();
        }
    }
}
