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

    public float m_closeDistance = 0.1f;

    private Rigidbody m_rigb;

    public int m_currentRotation = 6;
    public Vector2 m_destination;

    private int[] m_clockwiseRotations;
    private int[] m_InverseRotations;

    public float m_rotationSpeed = 0.2f;
    private Timer m_rotationTimer;

    public UnitData m_data;
    public bool m_moveing = false;

    void Start()
    {
        m_rotationTimer = new Timer();
        m_rotationTimer.m_time = m_rotationSpeed;

        m_rigb = GetComponent<Rigidbody>();

        m_destination = m_rigb.position;

        m_clockwiseRotations = new int[] { 1, 2, 4, 7, 6, 5, 3, 0 };
        m_InverseRotations = new int[] { 7, 0, 1, 6, 2, 5, 4, 3 };

        m_data = GetComponent<UnitData>();
    }

    public void move()
    {
        if (m_data.m_targateUnit)
        {
            Debug.DrawRay(transform.position, m_data.m_targateUnit.transform.position - transform.position, Color.red);

            if (Vector3.Distance(transform.position, m_data.m_targateUnit.transform.position) > 2.0f)
            {
                Vector3 vel = Vector3.zero;

                if (Mathf.Abs(transform.position.x - m_data.m_targateUnit.transform.position.x) > 0.33f)
                {
                    vel.x = transform.position.x > m_data.m_targateUnit.transform.position.x ? -1.0f : 1.0f;
                }

                if (Mathf.Abs(transform.position.y - m_data.m_targateUnit.transform.position.y) > 0.33f)
                {
                    vel.y = transform.position.y > m_data.m_targateUnit.transform.position.y ? -1.0f : 1.0f;
                }

                Rotate(Vec2ToIndex(vel));

                if (Vec2ToIndex(vel) == m_currentRotation)
                {
                    m_rigb.velocity = RotationVec2() * m_speed;
                }
                else
                {
                    m_rigb.velocity = Vector3.zero;
                }
            }
        }
        else
        {
            if (m_moveing)
            {
                if (Mathf.Abs(transform.position.x - m_data.m_moveTo.x) < 0.05f && Mathf.Abs(transform.position.y - m_data.m_moveTo.y) < 0.05f)
                {
                    m_moveing = false;
                }
            }

            if (m_moveing)
            {
                Vector3 vel = Vector3.zero;

                if (Mathf.Abs(transform.position.x - m_data.m_moveTo.x) > 0.33f)
                {
                    vel.x = transform.position.x > m_data.m_moveTo.x ? -1.0f : 1.0f;
                }

                if (Mathf.Abs(transform.position.y - m_data.m_moveTo.y) > 0.33f)
                {
                    vel.y = transform.position.y > m_data.m_moveTo.y ? -1.0f : 1.0f;
                }

                Rotate(Vec2ToIndex(vel));

                if (Vec2ToIndex(vel) == m_currentRotation)
                {
                    m_rigb.velocity = RotationVec2() * m_speed;
                }
                else
                {
                    m_rigb.velocity = Vector3.zero;
                }
            }
            else
            {
                m_rigb.velocity = Vector3.zero;
            }
        }
    }

    public void Rotate(int newRotationIndex)
    {
        if (newRotationIndex != m_currentRotation)
        {
            if (!m_rotationTimer.m_playing)
            {
                m_rotationTimer.Play();
            }

            m_rotationTimer.Cycle();

            if (m_rotationTimer.m_completed)
            {
                int ClockwiseRotationDistance = 0;
                int AntiClockwiseRotationDistance = 0;

                if (m_InverseRotations[m_currentRotation] > m_InverseRotations[newRotationIndex])
                {
                    ClockwiseRotationDistance = m_InverseRotations[m_currentRotation] - m_InverseRotations[newRotationIndex];
                    AntiClockwiseRotationDistance = 8 - ClockwiseRotationDistance;
                }
                else
                {
                    AntiClockwiseRotationDistance = m_InverseRotations[newRotationIndex] - m_InverseRotations[m_currentRotation];
                    ClockwiseRotationDistance = 8 - AntiClockwiseRotationDistance;
                }

                int newRot = m_InverseRotations[m_currentRotation];

                if (ClockwiseRotationDistance > AntiClockwiseRotationDistance)
                {
                    newRot += 1;

                    if (newRot == 8)
                    {
                        newRot = 0;
                    }
                }
                else
                {
                    newRot -= 1;

                    if (newRot == -1)
                    {
                        newRot = 7;
                    }
                }

                m_currentRotation = m_clockwiseRotations[newRot];

                m_rotationTimer.Stop();
            }
        }
        else
        {
            m_rotationTimer.Stop();
        }
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

    public Vector2 IndexToVec2(int index)
    {
        Vector2 returnVec = Vector2.zero;

        if (index >= 4)
        {
            index += 1;
        }

        returnVec.x = (index % 3) - 1;
        returnVec.y = ((index / 3) - 1) * -1;

        returnVec.Normalize();

        return returnVec;
    }

    public Vector2 RotationVec2()
    {
        Vector2 returnVec = Vector2.zero;

        int index = m_currentRotation;

        if (index >= 4)
        {
            index += 1;
        }

        returnVec.x = (index % 3) - 1;
        returnVec.y = ((index / 3) - 1) * -1;

        returnVec.Normalize();

        return returnVec;
    }

    /*public void Cycle()
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
    }*/
}
