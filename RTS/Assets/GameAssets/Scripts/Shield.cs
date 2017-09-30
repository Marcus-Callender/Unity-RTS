using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    Unit m_data;

    void Start()
    {
        Unit data = GetComponentInParent<Unit>();

        if (data)
        {
            m_data = data;
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != gameObject.tag)
        {
            Projectile projData = collision.gameObject.GetComponent<Projectile>();

            if (projData && m_data)
            {
                m_data.TakeDamage(projData.m_damage);

                Destroy(collision.gameObject);
            }
        }
    }
}
