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

    public int m_currentRot = 6;
    public Vector2 m_destination;

    private int[] m_clockwiseRotations;
    private int[] m_InverseRotations;

    public float m_rotationSpeed = 0.2f;
    private Timer m_rotationTimer;

    void Start()
    {
        m_rotationTimer = new Timer();
        m_rotationTimer.m_time = m_rotationSpeed;

        m_rigb = GetComponent<Rigidbody>();

        m_destination = m_rigb.position;

        m_clockwiseRotations = new int[] { 1, 2, 4, 7, 6, 5, 3, 0 };

        m_InverseRotations = new int[] { 7, 0, 1, 6, 2, 5, 4, 3 };
    }

    public void Cycle()
    {
        Debug.Log(findDistance(m_rigb.position, m_destination) + " dist");

        if (findDistance(m_rigb.position, m_destination) > m_closeDistance)
        {
            Debug.Log("Moveing");

            rotate();

            move();
        }
    }

    void rotate()
    {
        if (directionToDestination() != m_currentRot)
        {
            if (!m_rotationTimer.m_playing)
            {
                m_rotationTimer.Play();
            }

            m_rotationTimer.Cycle();

            if (m_rotationTimer.m_completed)
            {
                if (FindClockwiseRotation(directionToDestination()) <= FindAnitClockwiseRotation(directionToDestination()))
                {
                    RotateClockwise();
                }
                else
                {
                    RotateAntiClockwise();
                }

                m_rotationTimer.Stop();
            }
        }
        else
        {
            m_rotationTimer.Stop();
        }
    }

    void move()
    {
        if (m_turnType == E_TURN_TYPE.FULL_SPEED)
        {
            m_rigb.velocity = RotationVec2() * m_speed;
        }
        else if (m_turnType == E_TURN_TYPE.NORMALIZED)
        {
            int rot = Mathf.Min(FindClockwiseRotation(directionToDestination()), FindClockwiseRotation(directionToDestination()));

            float normal = 1.0f - (rot / 4.0f);

            m_rigb.velocity = RotationVec2() * (m_speed * normal);
        }
        else if (m_turnType == E_TURN_TYPE.ONLY_WHILE_CLOSE)
        {
            int rot = Mathf.Min(FindClockwiseRotation(directionToDestination()), FindClockwiseRotation(directionToDestination()));

            if (rot < 2)
            {
                m_rigb.velocity = RotationVec2() * m_speed;
            }
        }
        else if (m_turnType == E_TURN_TYPE.ONLY_WHILE_CORRECT)
        {
            if (m_currentRot == directionToDestination())
            {
                m_rigb.velocity = RotationVec2() * m_speed;
            }
        }
    }

    int FindClockwiseRotation(int newRot)
    {
        if (m_InverseRotations[m_currentRot] > m_InverseRotations[newRot])
        {
            return m_InverseRotations[m_currentRot] - m_InverseRotations[newRot];
        }

        return 8 - (m_InverseRotations[newRot] - m_InverseRotations[m_currentRot]);
    }

    int FindAnitClockwiseRotation(int newRot)
    {
        if (m_InverseRotations[m_currentRot] > m_InverseRotations[newRot])
        {
            return m_InverseRotations[newRot] - m_InverseRotations[m_currentRot];
        }

        return 8 - (m_InverseRotations[m_currentRot] - m_InverseRotations[newRot]);
    }

    void RotateClockwise()
    {
        Debug.Log("Clockwise turn");

        m_currentRot = m_clockwiseRotations[m_InverseRotations[m_currentRot] + 1];

        if (m_InverseRotations[m_currentRot] == 8)
        {
            m_currentRot = m_clockwiseRotations[0];
        }
    }

    void RotateAntiClockwise()
    {
        Debug.Log("anti-clockwise turn");

        m_currentRot = m_clockwiseRotations[m_InverseRotations[m_currentRot] - 1];

        if (m_InverseRotations[m_currentRot] == -1)
        {
            m_currentRot = m_clockwiseRotations[7];
        }
    }

    public Vector2 RotationVec2()
    {
        Vector2 returnVec = Vector2.zero;

        int index = m_currentRot;

        if (index >= 4)
        {
            index += 1;
        }

        returnVec.x = (index % 3) - 1;
        returnVec.y = ((index / 3) - 1) * -1;

        returnVec.Normalize();

        return returnVec;
    }

    float findDistance(Vector3 one, Vector3 two)
    {
        return Mathf.Abs(one.x - two.x) + Mathf.Abs(one.y - two.y) + Mathf.Abs(one.z - two.z);
    }

    public int Vec2ToIndex(Vector2 vec)
    {
        int returnIndex = 0;

        returnIndex += (int)(vec.x + 1.0f);
        returnIndex += (int)(((vec.y * -1.0f) + 1.0f) * 3.0f);

        if (returnIndex > 4)
        {
            returnIndex -= 1;
        }

        return returnIndex;
    }

    int directionToDestination()
    {
        Vector3 vel = Vector3.zero;

        if (Mathf.Abs(transform.position.x - m_destination.x) > 0.33f)
        {
            vel.x = transform.position.x > m_destination.x ? -1.0f : 1.0f;
        }

        if (Mathf.Abs(transform.position.y - m_destination.y) > 0.33f)
        {
            vel.y = transform.position.y > m_destination.y ? -1.0f : 1.0f;
        }

        return Vec2ToIndex(vel);
    }
}
