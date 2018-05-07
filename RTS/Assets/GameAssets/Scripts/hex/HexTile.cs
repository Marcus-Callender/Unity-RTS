using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer m_sprite;

    public void Selected()
    {
        m_sprite.color = Color.red;
    }

    public void DeSelect()
    {
        m_sprite.color = Color.white;
    }
}
