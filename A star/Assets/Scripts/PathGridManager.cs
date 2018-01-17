using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGridManager : MonoBehaviour
{
    public LayerMask m_ObstacleMask;
    public LayerMask m_CapsuleMask;
    public Vector2 m_vGridSize;
    public float m_fHalfNodeWidth = 0.5f;

    public const float GRID_WIDTH = 10.0f;
    public const float GRID_LENGTH = 10.0f;

    Node[,] m_aGrid;

    public GameObject capsule;

    private void OnDrawGizmos()
    {
        Vector3 cube = new Vector3(1.9f * m_fHalfNodeWidth, 1.0f, 1.9f * m_fHalfNodeWidth);

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

        FindCapsule();
    }

    private void Update()
    {
        FindCapsule();
    }

    private void PopulateGrid()
    {
        int width = (int)(GRID_WIDTH / m_vGridSize.x);
        int length = (int)(GRID_LENGTH / m_vGridSize.y);

        Debug.Log("Length: " + length + " | Width: " + width);

        m_aGrid = new Node[length, width];

        Vector3 position = transform.position +
            (GRID_LENGTH / 2f - m_fHalfNodeWidth) * Vector3.left +
            (GRID_WIDTH / 2f - m_fHalfNodeWidth) * Vector3.back;

        bool blocked;

        for (int x = 0; x < length; x++)
        {
            position.z = (GRID_WIDTH / 2f - m_fHalfNodeWidth) * -1f;

            for (int z = 0; z < width; z++)
            {
                blocked = Physics.CheckSphere(position, m_fHalfNodeWidth, m_ObstacleMask);
                m_aGrid[x, z] = new Node(blocked, position);
                position += Vector3.forward * 2f * m_fHalfNodeWidth;
            }

            position += Vector3.right * 2f * m_fHalfNodeWidth;
        }
    }

    private void FindCapsule()
    {
        float sensitivity = m_fHalfNodeWidth;
        bool triggered = false;

        if (m_aGrid != null && m_aGrid.Length > 0)
        {
            for (int x = 0; x < m_aGrid.GetLength(0); x++)
            {
                for (int z = 0; z < m_aGrid.GetLength(1); z++)
                {
                    if (
                        !triggered &&
                        capsule.transform.position.x <= m_aGrid[x, z].GetPosition().x + sensitivity &&
                        capsule.transform.position.x >= m_aGrid[x, z].GetPosition().x - sensitivity &&
                        capsule.transform.position.z <= m_aGrid[x, z].GetPosition().z + sensitivity &&
                        capsule.transform.position.z >= m_aGrid[x, z].GetPosition().z - sensitivity
                        )
                    {
                        m_aGrid[x, z].SetOverlap(true);
                        triggered = true;
                    }
                    else
                    {
                        m_aGrid[x, z].SetOverlap(false);
                    }
                }
            }
        }
    }
}
