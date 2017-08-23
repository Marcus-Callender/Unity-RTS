using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public int m_rotationIndex = 6;

    public float m_rotationSpeed = 0.2f;
    private Timer m_rotationTimer;

    private int[] m_clockwiseRotations;
    private int[] m_InverseRotations;

    void Start()
    {
        m_rotationTimer = new Timer();
        m_rotationTimer.m_time = m_rotationSpeed;

        m_clockwiseRotations = new int[] { 1, 2, 4, 7, 6, 5, 3, 0 };

        m_InverseRotations = new int[] { 7, 0, 1, 6, 2, 5, 4, 3 };
    }
    
    public void Rotate(int newRotationIndex)
    {
        if (newRotationIndex != m_rotationIndex)
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

                if (m_InverseRotations[m_rotationIndex] > m_InverseRotations[newRotationIndex])
                {
                    ClockwiseRotationDistance = m_InverseRotations[m_rotationIndex] - m_InverseRotations[newRotationIndex];
                    AntiClockwiseRotationDistance = 8 - ClockwiseRotationDistance;
                }
                else
                {
                    AntiClockwiseRotationDistance = m_InverseRotations[newRotationIndex] - m_InverseRotations[m_rotationIndex];
                    ClockwiseRotationDistance = 8 - AntiClockwiseRotationDistance;
                }

                int newRot = m_InverseRotations[m_rotationIndex];

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

                m_rotationIndex = m_clockwiseRotations[newRot];

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

        int index = m_rotationIndex;

        if (index >= 4)
        {
            index += 1;
        }

        returnVec.x = (index % 3) - 1;
        returnVec.y = ((index / 3) - 1) * -1;

        returnVec.Normalize();

        return returnVec;
    }
}
