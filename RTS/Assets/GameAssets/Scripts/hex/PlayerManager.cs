using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_playerColours
{
    GREEN,
    BLUE
}

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager m_instance;

    public Player[] m_players;

    void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Debug.LogError("Two player managers detected.");
            Destroy(gameObject);
        }
    }

    public void RegisterUnit(HexUnit unit, E_playerColours colour)
    {
        m_players[(int)colour].AddUnit(unit);
    }
}
