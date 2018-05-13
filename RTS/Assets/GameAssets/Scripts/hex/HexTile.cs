using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer m_sprite;

    [SerializeField]
    TextMesh m_text;

    private bool m_active = true;

    public void Selected()
    {
        m_sprite.color = Color.red;
    }

    public void StartPath()
    {
        m_sprite.color = Color.red;
    }

    public void OnPath()
    {
        m_sprite.color = Color.yellow;
    }

    public void EndPath()
    {
        m_sprite.color = Color.green;
    }

    public void Selected(float r, float g, float b)
    {
        m_sprite.color = new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), 0.5f);
    }

    public void DeSelect()
    {
        m_sprite.color = Color.white;
    }

    public void SetInactive()
    {
        m_active = false;
        m_sprite.color = Color.black;
    }

    public void SetActive()
    {
        m_active = true;
        m_sprite.color = Color.white;
    }

    public bool isActive()
    {
        return m_active;
    }

    public void SetText(string text)
    {
        m_text.text = text;
    }
}
