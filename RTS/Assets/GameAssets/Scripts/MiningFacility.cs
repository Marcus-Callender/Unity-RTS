using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiningFacility : MonoBehaviour
{
    public GameObject m_leaveBehind;

    void Start()
    {

    }

    void Update()
    {

    }

    private void OnDestroy()
    {
        Instantiate(m_leaveBehind, gameObject.transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
