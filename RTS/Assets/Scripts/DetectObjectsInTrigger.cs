using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectObjectsInTrigger : MonoBehaviour
{
    public List<GameObject> m_InTrigger;
    public List<Unit> m_UnitsInTrigger;
    
    private void FixedUpdate()
    {
        m_InTrigger.Clear();
        m_UnitsInTrigger.Clear();
    }

    void OnEnable()
    {
        m_InTrigger.Clear();
        m_UnitsInTrigger.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        m_InTrigger.Add(other.gameObject);

        Unit data = other.gameObject.GetComponent<Unit>();

        if (data)
        {
            m_UnitsInTrigger.Add(data);
        }
    }
}
