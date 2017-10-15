using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFade : MonoBehaviour
{
    public float m_fadeTime = 0.5f;
    private Timer m_fadeTimer;
    
    void Start()
    {
        m_fadeTimer = new Timer();
        m_fadeTimer.m_time = m_fadeTime;
        m_fadeTimer.Play();
    }
    
    void Update()
    {
        m_fadeTimer.Cycle();

        if (m_fadeTimer.m_completed)
        {
            Destroy(gameObject);
        }
    }
}
