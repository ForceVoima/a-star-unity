using System.Collections.Generic;
using UnityEngine;

public class A_Star : MonoBehaviour
{
    public Transform m_tSeeker;
    public Transform m_tTarget;

    PathGridManager m_Grid;
    
	void Awake ()
    {
        m_Grid = GetComponent<PathGridManager>();
	}
	
	void Update ()
    {
        FindPath(m_tSeeker.position, m_tTarget.position);
	}

    public int GetDistance(Node a, Node b)
    {
        int iDistX = Mathf.Abs(a.m_iGridX - b.m_iGridX);
        int iDistY = Mathf.Abs(a.m_iGridY - b.m_iGridY);

        if (iDistX > iDistY)
        {
            return 14 * iDistY + 10 * (iDistX - iDistY);
        }
        else
        {
            return 14 * iDistX + 10 * (iDistY - iDistX);
        }
    }

    public void FindPath(Vector3 start, Vector3 end)
    {
        Node startNode = m_Grid.NodeFromWorldPos(start);
        Node endNode = m_Grid.NodeFromWorldPos(end);

        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();

        // G-cost = distance from the starting node:
        startNode.m_iGCost = GetDistance(startNode, startNode);

        // H-cost = distance from the end node:
        startNode.m_iHCost = GetDistance(startNode, endNode);

        openSet.Add(startNode);

        Node currentNode = null;
        List<Node> neighbours = null;

        while (openSet.Count > 0)
        {
            currentNode = LowestFCost(openSet);
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Are we there yet?
            if (currentNode == endNode)
            {
                ReTracePath(startNode, endNode);
                return;
            }

            neighbours = m_Grid.GetNeighbours(currentNode);

            foreach (Node neighbour in neighbours)
            {
                if (neighbour.m_isBlocked)
                    continue;

                if (closedSet.Contains(neighbour))
                    continue;

                int newMovementCost = currentNode.m_iGCost + GetDistance(currentNode, neighbour);

                if (newMovementCost < neighbour.m_iGCost ||
                    !openSet.Contains(neighbour))
                {
                    neighbour.m_iGCost = newMovementCost;
                    neighbour.m_iHCost = GetDistance(neighbour, endNode);
                    neighbour.m_Parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        
        Debug.Log("No route was found!");
    }

    public void ReTracePath(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        // Are we there yet in reverse!
        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.m_Parent;
        }

        path.Reverse();

        m_Grid.ShowPath(path);
    }

    private Node LowestFCost(List<Node> openSet)
    {
        Node currentNode = openSet[0];

        for (int i = 1; i < openSet.Count; i++)
        {
            if ((openSet[i].m_ifCost < currentNode.m_ifCost) ||
                  openSet[i].m_ifCost == currentNode.m_ifCost && openSet[i].m_iHCost < currentNode.m_iHCost)
            {
                currentNode = openSet[i];
            }
        }

        return currentNode;
    }
}
