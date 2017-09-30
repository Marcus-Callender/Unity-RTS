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

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != gameObject.tag)
        {
            Projectile projData = other.GetComponent<Projectile>();

            if (projData && m_data)
            {
                m_data.TakeDamage(projData.m_damage);

                Destroy(other.gameObject);
            }
        }
    }
}
