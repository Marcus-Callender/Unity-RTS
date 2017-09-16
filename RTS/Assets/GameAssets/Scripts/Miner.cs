using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Miner : MonoBehaviour
{
    public GameObject m_toCreate;
    public string m_interactWithTag = "Mine";

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == m_interactWithTag)
        {
            Vector3 createPosition = other.transform.position;

            Instantiate(m_toCreate, createPosition, Quaternion.identity);

            Destroy(gameObject);
            Destroy(other.gameObject);
        }
    }
}
