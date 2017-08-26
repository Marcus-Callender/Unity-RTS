using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public GameObject m_projectile;
    private DetectObjectsInTrigger m_targates;
    private UnitData m_data;

    public float m_fireDelay = 1.0f;
    public float m_bulletSpeed = 3.0f;
    public bool m_targateFriendlyUnits = false;

    private Timer m_fireTimer;
    public int m_id;

    void Start()
    {
        m_fireTimer = new Timer();
        m_fireTimer.m_time = m_fireDelay;

        m_targates = GetComponent<DetectObjectsInTrigger>();
        m_data = GetComponentInParent<UnitData>();
    }

    void Update()
    {
        GameObject targate = null;

        m_targates.CheckRefrences();

        // runs the loop for each uint in range
        foreach (Unit tempTargate in m_targates.m_UnitsInTrigger)
        {
            // this ensures the target being looked at is the team this unit is supposed to be targeting
            if ((tempTargate.gameObject.tag == gameObject.tag) == m_targateFriendlyUnits)
            {
                // this enures medics will not heal uints at full health
                if (!(m_targateFriendlyUnits && (tempTargate.m_health == tempTargate.m_maxHealth)))
                {
                    // if the unit being looked at is the targate uint it will always be targeted
                    if (m_data.m_targateUnit == tempTargate.m_data)
                    {
                        targate = tempTargate.gameObject;
                        break;
                    }

                    if (targate && Vector3.Distance(tempTargate.transform.position, gameObject.transform.position) > Vector3.Distance(targate.transform.position, gameObject.transform.position))
                    {
                        // if there is already a unit targeted this will targate the closest unit
                        targate = tempTargate.gameObject;
                    }
                    else
                    {
                        // if there is currently no targate assignes this uint as the targate 
                        targate = tempTargate.gameObject;
                    }
                }
            }
        }

        //if (targate)
        //{
        //    //Vector2 angle = targate.gameObject.transform.position - transform.position;
        //    //angle.Normalize();
        //    //
        //    //if ((m_data.RotationVec2() - angle).magnitude <= 0.25f)
        //
        //    int angleIndex = m_data.Vec2ToIndex((targate.gameObject.transform.position - transform.position).normalized);
        //
        //    if (angleIndex != m_data.m_rotationIndex)
        //    {
        //        m_data.Rotate(angleIndex);
        //
        //        targate = null;
        //    }
        //}

        if (targate)
        {
            if (!m_fireTimer.m_playing)
            {
                m_fireTimer.Play();
            }

            m_fireTimer.Cycle();

            if (m_fireTimer.m_completed)
            {
                Debug.Log("Fired bullet");

                m_fireTimer.Stop();

                GameObject newProjectile = Instantiate(m_projectile, gameObject.transform.position, m_projectile.transform.rotation);

                Vector3 BulletVelocity = targate.transform.position - transform.position;
                BulletVelocity.Normalize();

                //Vector3 BulletVelocity = m_data.RotationVec2();

                BulletVelocity *= m_bulletSpeed;

                newProjectile.GetComponent<Rigidbody>().velocity = BulletVelocity;

                newProjectile.tag = gameObject.tag;

                Projectile proj = newProjectile.GetComponent<Projectile>();

                if (proj)
                {
                    proj.m_shooterid = m_id;
                }
            }
        }
        else
        {
            m_fireTimer.Stop();
        }
    }
}
