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
    }

    public void SetActive()
    {
        m_active = true;
    }

    public bool isActive()
    {
        return m_active;
    }
}
