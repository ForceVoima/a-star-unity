using UnityEngine;

public class PathGridManager : MonoBehaviour
{
    public LayerMask m_ObstacleMask;
    public LayerMask m_CapsuleMask;

    public Vector2 m_vGridSize = new Vector2(10.0f, 10.0f);
    public float m_fHalfNodeWidth = 0.5f;

    private Vector3 cube;

    private Node[,] m_aGrid;

    public GameObject capsule;
    private int[] lastCapsuleNode;

    private bool complained = false;
    private int width;
    private int length;

    private void OnDrawGizmos()
    {
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

    private void OnValidate()
    {
        if (m_fHalfNodeWidth < 0f)
            m_fHalfNodeWidth *= -1f;

        PopulateGrid();

        cube = new Vector3(1.9f * m_fHalfNodeWidth, 1.0f, 1.8f * m_fHalfNodeWidth);

        if (width > 0 && length > 0)
        {
            lastCapsuleNode = FindObject(capsule.transform.position);
            m_aGrid[lastCapsuleNode[0], lastCapsuleNode[1]].SetOverlap(true);
        }
    }

    private void Update()
    {
        if (m_aGrid == null ||
            m_aGrid.GetLength(0) == 0 ||
            m_aGrid.GetLength(1) == 0)
            return;

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
    }

    private void PopulateGrid()
    {
        if (m_fHalfNodeWidth <= 0.00001f)
        {
            Debug.LogError("Division by zero!");
            return;
        }

        width = (int)((m_vGridSize.x + 0.000001f) / (m_fHalfNodeWidth * 2f));
        length = (int)((m_vGridSize.y + 0.000001f) / (m_fHalfNodeWidth * 2f));

        if (width < 1 || length < 1)
        {
            m_aGrid = null;
            return;
        }

        m_aGrid = new Node[width, length];

        Vector3 position = transform.position +
            (width * m_fHalfNodeWidth - m_fHalfNodeWidth) * Vector3.left +
            (length * m_fHalfNodeWidth - m_fHalfNodeWidth) * Vector3.back;

        bool blocked;

        for (int x = 0; x < width; x++)
        {
            position.x = transform.position.x + (2 * x + 1 - width) * m_fHalfNodeWidth;

            for (int z = 0; z < length; z++)
            {
                position.z = transform.position.z + (2 * z + 1 - length) * m_fHalfNodeWidth;
                blocked = Physics.CheckSphere(position, m_fHalfNodeWidth, m_ObstacleMask);
                m_aGrid[x, z] = new Node(blocked, position);
            }
        }
    }

    private int[] FindObject(Vector3 position)
    {
        float leftMostX = transform.position.x + (1 - width) * m_fHalfNodeWidth;
        float deltaX = position.x - leftMostX;
        int indexX = Mathf.RoundToInt(deltaX / (2f * m_fHalfNodeWidth));

        float bottomZ = transform.position.z + (1 - length) * m_fHalfNodeWidth;
        float deltaZ = position.z - bottomZ;
        int indexZ = Mathf.RoundToInt(deltaZ / (2f * m_fHalfNodeWidth));

        if (indexX >= width)
            indexX = width - 1;
        else if (indexX < 0)
            indexX = 0;

        if (indexZ >= length)
            indexZ = length - 1;
        else if (indexZ < 0)
            indexZ = 0;

        return new int[2] { indexX, indexZ };
    }

    private bool CapsuleInNode(Vector3 position, Node node)
    {
        float sensitivity = m_fHalfNodeWidth + 0.00001f;
        Vector3 nodePosition = node.GetPosition();

        if (Mathf.Abs(nodePosition.x - position.x) < sensitivity &&
            Mathf.Abs(nodePosition.z - position.z) < sensitivity)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Less efficient old way of picking the correct node
    private int[] FindObject2(Vector3 position)
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
                        complained = false;
                        return result;
                    }
                }
            }
        }

        if (!complained)
        {
            complained = true;
            Debug.LogError(Time.fixedTime + "Object not found!");
        }
        return result;
    }
}
