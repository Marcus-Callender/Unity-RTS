using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameReferee : MonoBehaviour
{
    public float m_timeModifier = 9.1f;
    public Text m_timeDisplay;
    private Timer m_gameTimer;
    public int m_gameStartTime = 99;

    void Start()
    {
        m_timeDisplay = GameObject.Find("Game Timer").GetComponent<Text>();
        m_gameTimer = new Timer();
        m_gameTimer.m_time = m_gameStartTime * m_timeModifier;
        
        m_gameTimer.Play(true);
    }

    void Update()
    {
        m_gameTimer.Cycle();
        
        m_timeDisplay.text = "" + Mathf.Ceil(m_gameTimer.GetLerp() * (float)m_gameStartTime);
    }
}
