using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyNodeManager : MonoBehaviour
{
    public static MyNodeManager Instance;
    private List<Node> m_nodes = new List<Node>();
    [SerializeField] private GameObject nodePref;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }
        else if (Instance == null)
        {
            Instance = this;
        }
    }

    public void StartNodeManager()
    {
        PerlinNoise perliNoise = PerlinNoise.Instance;

        for(int x = 0; x < perliNoise.WorldSize; x++)
        {
            for (int z = 0; z < perliNoise.WorldSize; z++)
            {
                GameObject node = Instantiate(nodePref, new Vector3(x, MyWorld.Instance.GetHeight(x, z) + 1, z), Quaternion.identity);
                m_nodes.Add(node.GetComponent<Node>());
            }
        }

        for (int i = 0; i < m_nodes.Count; i++)
        {
            m_nodes[i].ConfigSurroundingNodes();
        }
    }
    public float GetDistanceBetween(Node a, Node b)
    {
        return (b.transform.position - a.transform.position).magnitude;
    }
    public List<Node> GetNodesInWorld()
    {
        return m_nodes;
    }
}
