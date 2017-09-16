using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningFacility : MonoBehaviour
{
    public GameObject m_leaveBehind;

    private PlayerControl m_player = null;

    public float m_mineSpeed = 15.0f;
    private Timer m_mineTimer;

    void Start()
    {
        GameObject playerObj = GameObject.Find("Player");

        if (playerObj && (playerObj.tag == gameObject.tag))
        {
            PlayerControl playerData = playerObj.GetComponent<PlayerControl>();

            if (playerData)
            {
                m_player = playerData;
                m_mineTimer = new Timer();
                m_mineTimer.m_time = m_mineSpeed;
                m_mineTimer.Play();
            }
        }
    }

    void Update()
    {
        if (m_player)
        {
            m_mineTimer.Cycle();

            if (m_mineTimer.m_completed)
            {
                m_mineTimer.Play();
                m_player.m_silver++;
            }
        }
    }

    private void OnDestroy()
    {
        Instantiate(m_leaveBehind, gameObject.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
