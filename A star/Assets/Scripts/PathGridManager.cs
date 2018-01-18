using UnityEngine;

public class PathGridManager : MonoBehaviour
{
    public LayerMask m_ObstacleMask;
    public LayerMask m_CapsuleMask;

    public Vector2 m_vGridSize = new Vector2(10.0f, 10.0f);
    public float m_fHalfNodeWidth = 0.5f;

    private Node[,] m_aGrid;

    public GameObject capsule;
    private int[] lastCapsuleNode;

    public bool repopulate = false;

    private void OnDrawGizmos()
    {
        Vector3 cube = new Vector3(1.9f * m_fHalfNodeWidth, 1.0f, 1.8f * m_fHalfNodeWidth);

        // Sanity check not to draw non-existent Nodes
        if (m_aGrid != null && m_aGrid.Length > 0)
        {
            for (int x = 0; x < m_aGrid.GetLength(0); x++)
            {
                for (int z = 0; z < m_aGrid.GetLength(1); z++)
                {
                    if (m_aGrid[x, z].IsOverlapping())
                        Gizmos.color = Color.blue;
                    else if (m_aGrid[x, z].IsBlocked())
                        Gizmos.color = Color.red;
                    else
                        Gizmos.color = Color.white;

                    Gizmos.DrawWireCube(m_aGrid[x, z].GetPosition(), cube);
                }
            }
        }
    }

    private void Start()
    {
        PopulateGrid();

        lastCapsuleNode = FindObject(capsule.transform.position);
        m_aGrid[lastCapsuleNode[0], lastCapsuleNode[1]].SetOverlap(true);
    }

    private void Update()
    {
        if (CapsuleInNode(capsule.transform.position, m_aGrid[lastCapsuleNode[0], lastCapsuleNode[1]]))
        {
            return;
        }
        else
        {
            m_aGrid[lastCapsuleNode[0], lastCapsuleNode[1]].SetOverlap(false);
            lastCapsuleNode = FindObject(capsule.transform.position);
            m_aGrid[lastCapsuleNode[0], lastCapsuleNode[1]].SetOverlap(true);
        }

        if (repopulate)
        {
            repopulate = false;
            PopulateGrid();
        }
    }

    private void PopulateGrid()
    {
        int width = (int)(m_vGridSize.x / (m_fHalfNodeWidth * 2f));
        int length = (int)(m_vGridSize.y / (m_fHalfNodeWidth * 2f));

        Debug.Log("Length: " + length + " | Width: " + width);
        Debug.Log("Press F5 to repopulate!");

        m_aGrid = new Node[width, length];

        Vector3 position = transform.position +
            (m_vGridSize.x / 2f - m_fHalfNodeWidth) * Vector3.left +
            (m_vGridSize.y / 2f - m_fHalfNodeWidth) * Vector3.back;

        bool blocked;

        for (int x = 0; x < width; x++)
        {
            position.z = (m_vGridSize.y / 2f - m_fHalfNodeWidth) * -1f;

            for (int z = 0; z < length; z++)
            {
                blocked = Physics.CheckSphere(position, m_fHalfNodeWidth, m_ObstacleMask);
                m_aGrid[x, z] = new Node(blocked, position);
                position += Vector3.forward * 2f * m_fHalfNodeWidth;
            }

            position += Vector3.right * 2f * m_fHalfNodeWidth;
        }
    }

    private bool CapsuleInNode(Vector3 position, Node node)
    {
        float sensitivity = m_fHalfNodeWidth;

        if (
            position.x <= node.GetPosition().x + sensitivity &&
            position.x >= node.GetPosition().x - sensitivity &&
            position.z <= node.GetPosition().z + sensitivity &&
            position.z >= node.GetPosition().z - sensitivity
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private int[] FindObject(Vector3 position)
    {
        int[] result = new int[2];

        if (m_aGrid != null && m_aGrid.Length > 0)
        {
            for (int x = 0; x < m_aGrid.GetLength(0); x++)
            {
                for (int z = 0; z < m_aGrid.GetLength(1); z++)
                {
                    if (CapsuleInNode(position, m_aGrid[x,z]))
                    {
                        result[0] = x;
                        result[1] = z;
                        return result;
                    }
                }
            }
        }

        Debug.LogError("Object not found!");
        return result;
    }
}
