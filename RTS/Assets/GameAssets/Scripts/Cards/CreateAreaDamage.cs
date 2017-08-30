using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateAreaDamage : MonoBehaviour
{
    private Camera m_cam;
    public GameObject m_radius;

    void Start()
    {
        m_cam = GameObject.Find("Main Camera").GetComponent<Camera>();

    }

    void Update()
    {

    }

    public Color ColourSprite(bool canAfford)
    {
        if (canAfford)
        {
            return Color.green;
        }

        return Color.red;
    }

    public void Activate()
    {
        Vector3 mousePos = m_cam.ScreenToWorldPoint(Input.mousePosition);
        Instantiate(m_radius, new Vector3(mousePos.x, mousePos.y, 0.0f), Quaternion.identity);

        Destroy(gameObject);
    }
}
