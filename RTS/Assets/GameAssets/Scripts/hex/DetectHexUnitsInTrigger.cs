﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectHexUnitsInTrigger : MonoBehaviour
{
    public List<GameObject> m_InTrigger;
    public List<HexUnit> m_UnitsInTrigger;

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("#### " + other.gameObject + " entered in trigger");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("#### " + other.gameObject + " exited in trigger");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("## " + other.gameObject + " stay in trigger");

        m_InTrigger.Add(other.gameObject);

        HexUnit data = other.gameObject.GetComponent<HexUnit>();

        if (data)
        {
            m_UnitsInTrigger.Add(data);
        }
    }

    public void CheckRefrences()
    {
        for (int z = 0; z < m_InTrigger.Count; z++)
        {
            if (!m_InTrigger[z])
            {
                m_InTrigger.Remove(m_InTrigger[z]);
            }
        }

        for (int z = 0; z < m_UnitsInTrigger.Count; z++)
        {
            if (!m_UnitsInTrigger[z])
            {
                m_UnitsInTrigger.Remove(m_UnitsInTrigger[z]);
            }
        }
    }
}