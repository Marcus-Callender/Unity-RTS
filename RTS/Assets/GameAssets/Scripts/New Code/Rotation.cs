using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{


    void Start()
    {
        for (int z = 0; z < 8; z++)
        {
            Vector2 vec = IndexToVec2(z);

            Debug.Log(z + " : " + vec);
            Debug.Log(z + " : " + Vec2ToIndex(vec));
        }
    }

    void Update()
    {

    }

    int Vec2ToIndex(Vector2 vec)
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

    Vector2 IndexToVec2(int index)
    {
        Vector2 returnVec = Vector2.zero;

        if (index >= 4)
        {
            index += 1;
        }

        returnVec.x = (index % 3) - 1;
        returnVec.y = ((index / 3) - 1) * -1;

        return returnVec;
    }
}
