using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquareTile : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer m_sprite;

    private bool m_active = true;

    

    public void SetInactive()
    {
        m_active = false;
        //Color col = m_sprite.color;
        //col.a = 0.5f;
        //m_sprite.color = col;
    }

    public void SetActive()
    {
        m_active = true;
        //Color col = m_sprite.color;
        //col.a = 0.0f;
        //m_sprite.color = col;
    }

    public bool isActive()
    {
        return m_active;
    }
}
