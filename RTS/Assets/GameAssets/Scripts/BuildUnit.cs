using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildUnit : MonoBehaviour
{
    public GameObject[] m_canBuild;

    void Start()
    {

    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("BUILD TRIGGER: " + other.tag);
        Debug.Log("BUILD NAME: " + other.gameObject.name);

        if (other.tag == "UI")
        {
            Debug.Log("UI TRIGGER");

            Draggable data = other.gameObject.GetComponent<Draggable>();
            UnitOrder order = other.gameObject.GetComponent<UnitOrder>();

            if (order)
            {
                Destroy(data.placeholder);
                Destroy(other.gameObject);
            }
        }
    }
}
