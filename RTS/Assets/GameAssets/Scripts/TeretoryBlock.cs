using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeretoryBlock : MonoBehaviour
{
    private Material m_mat;

    void Awake()
    {
        //m_mat = GetComponent<Renderer>().material;
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.isTrigger)
        {
            gameObject.tag = other.gameObject.tag;

            if (gameObject.tag == "Blue")
            {
                Debug.Log("Block now BLUE");
                //m_mat.color = new Color(0.220f, 0.2f, 0.7f, 0.5f);
            }
            else if (gameObject.tag == "Green")
            {
                Debug.Log("Block now GREEN");
                //m_mat.color = new Color(0.541f, 0.906f, 0.149f, 0.5f);
            }
        }
    }
}
