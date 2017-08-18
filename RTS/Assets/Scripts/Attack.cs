using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int m_damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        // if the hit object is on the other team
        if (other.tag != gameObject.tag)
        {
            Unit data = other.gameObject.GetComponent<Unit>();

            // and the hit object can take damage
            if (data)
            {
                data.TakeDamage(m_damage);

                // destroy this attack
                Destroy(gameObject);
            }
        }
    }
}
