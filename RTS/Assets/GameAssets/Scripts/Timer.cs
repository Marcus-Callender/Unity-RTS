using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer
{
    public bool m_completed = false;
    public bool m_reversed = false;
    public bool m_playing = false;

    public float m_time = 0.0f;
    private float m_timeCount = 0.0f;

    // Timer dose not inherit from MonoBehaviour to allow multiple Timers to be attached to one object
    // because of this Cycle in Timer must be called each frame to update
    public void Cycle()
    {
        if (!m_completed && m_playing)
        {
            if (!m_reversed)
            {
                // increses the time
                m_timeCount += Time.deltaTime;

                // if the timeCount is above or the same as the specified time the timer is completed
                if (m_time <= m_timeCount)
                {
                    m_completed = true;
                }
            }
            else
            {
                // decreses the time
                m_timeCount -= Time.deltaTime;

                // if the timeCount is 0 or less the timer is completed
                if (m_timeCount <= 0.0f)
                {
                    m_completed = true;
                }
            }
        }
    }

    // an alternate Cycle that takes in the ammount of time to be updated
    public void Cycle(float dt)
    {
        // the same as regular Cycle but uses the dt paramiter insted of Time.deltaTime
        if (!m_completed && m_playing)
        {
            if (!m_reversed)
            {
                m_timeCount += dt;

                if (m_time <= m_timeCount)
                {
                    m_completed = true;
                }
            }
            if (m_reversed)
            {
                m_timeCount -= dt;

                if (m_timeCount <= 0.0f)
                {
                    m_completed = true;
                }
            }
        }
    }

    // defaults reverse to false when it is not specified
    public void Play(bool reverse = false)
    {
        m_reversed = reverse;
        m_playing = true;
        m_completed = false;

        // sets the timeCount the the other end of where it will end when the tmer is completed
        if (reverse)
        {
            m_timeCount = m_time;
        }
        else
        {
            m_timeCount = 0.0f;
        }
    }

    public void Stop()
    {
        m_completed = false;
        m_playing = false;

        m_timeCount = 0.0f;
    }

    // returns how far allong the timer is to being completed
    // 0 = farthest away
    // 1 = completed
    public float GetLerp()
    {
        return m_timeCount / m_time;
    }
}
