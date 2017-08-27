using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimation
{
    public Sprite[] m_sprites;
    public float m_frameTime = 0.16f;
    public bool m_reverseAfterFinishing = false;

    private Timer m_animTime;
    private bool m_reverse;
    private int m_animIndex = 0;

    public bool m_completed;

    public SpriteAnimation(Sprite[] sprites, bool reverseAfterFinishing = false)
    {
        m_animTime = new Timer();
        m_animTime.m_time = m_frameTime;

        m_sprites = sprites;
        m_reverseAfterFinishing = reverseAfterFinishing;
    }

    public void Play()
    {
        m_completed = false;
        m_animIndex = 0;
        m_reverse = false;
        m_animTime.Play();
    }

    public Sprite Cyce()
    {
        m_animTime.Cycle();

        if (m_animTime.m_completed)
        {
            m_animTime.Play();

            if (!m_reverse)
            {

                if (m_animIndex >= m_sprites.Length - 1)
                {
                    if (m_reverseAfterFinishing)
                    {
                        m_reverse = true;
                    }
                    else
                    {
                        m_completed = true;
                        m_animIndex = 0;
                    }
                }
                else
                {
                    m_animIndex++;

                }
            }
            else
            {

                if (m_animIndex <= 0)
                {
                    m_completed = true;
                    m_animIndex = 0;
                }
                else
                {
                    m_animIndex--;

                }
            }
        }

        return m_sprites[m_animIndex];
    }
}
