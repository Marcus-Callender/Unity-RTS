using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_healthbarPrefab;

    [SerializeField]
    private int m_healthbarPoolSize = 2;
    
    private HealthBar[] m_instantiatedHealthbars;

    public static HealthBarManager m_instance;

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        else
        {
            Debug.LogError("Two health bar managers detected in scene.");
            Destroy(gameObject);
        }

        m_instantiatedHealthbars = new HealthBar[m_healthbarPoolSize];

        for (int z = 0; z < m_healthbarPoolSize; z++)
        {
            GameObject go = Instantiate(m_healthbarPrefab, transform);
            m_instantiatedHealthbars[z] = go.GetComponent<HealthBar>();
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void Register(HexUnit unit)
    {
        for(int z = 0; z < m_instantiatedHealthbars.Length; z++)
        {
            if (!m_instantiatedHealthbars[z].m_active)
            {
                m_instantiatedHealthbars[z].Register(unit);

                break;
            }
        }
    }
}
