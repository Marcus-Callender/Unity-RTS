using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_TURN_TYPE
{
    FULL_SPEED,
    ONLY_WHILE_CLOSE,
    ONLY_WHILE_CORRECT,
    NORMALIZED
}

public class UnitMovement : MonoBehaviour
{
    public float m_speed = 2.0f;
    public E_TURN_TYPE m_turnType = E_TURN_TYPE.FULL_SPEED;
    public float m_turnSpeed = 2.0f;

    public float m_closeDistance = 0.1f;

    private Rigidbody m_rigb;

    private int m_currentDir;
    private Vector2 m_destination;

    void Start()
    {
        m_rigb = GetComponent<Rigidbody>();

        m_destination = m_rigb.position;
    }

    void Update()
    {
        if (findDistance(m_rigb.position, m_destination) > m_closeDistance)
        {

        }
    }

    void rotate()
    {

    }

    void move()
    {

    }

    float findDistance(Vector3 one, Vector3 two)
    {
        return Mathf.Abs(one.x - two.x) + Mathf.Abs(one.y - two.y) + Mathf.Abs(one.z - two.z);
    }
}
