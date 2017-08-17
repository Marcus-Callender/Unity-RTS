using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionBox : MonoBehaviour
{
    public List<Unit> m_detected;

    private void FixedUpdate()
    {
        //m_detected.Clear();
    }


    //void OnDisable()
    //{
    //    Debug.Log("PrintOnDisable: script was disabled");
    //}

    void OnEnable()
    {
        m_detected.Clear();
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Trigger");

        Unit data = other.gameObject.GetComponent<Unit>();

        if (data)
        {
            Debug.Log("Data found");
            m_detected.Add(data);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        Debug.Log("Trigger");

        Unit data = collision.gameObject.GetComponent<Unit>();

        if (data && !m_detected.Contains(data))
        {
            Debug.Log("Data found");
            m_detected.Add(data);
        }
    }
}
