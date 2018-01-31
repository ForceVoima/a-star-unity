using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool m_isBlocked;
    public bool overlapCapsule = false;
    public Vector3 m_vPosition;

    public int m_iGridX;
    public int m_iGridY;

    public int m_iGCost;
    public int m_iHCost;

    int m_iHeapIndex;

    List<Node> neigbours;

    public int CompareTo(Node node)
    {
        int iComp = m_ifCost.CompareTo(node.m_ifCost);
        if (iComp == 0)
        {
            iComp = m_iHCost.CompareTo(node.m_iHCost);
        }
        return ~iComp;
    }

    public int m_ifCost
    {
        get { return m_iHCost + m_iGCost; }
    }

    public int HeapIndex
    {
        get
        {
            return m_iHeapIndex;
        }

        set
        {
            m_iHeapIndex = value;
        }
    }

    public Node m_Parent;

    public Node(bool bIsBlocked, Vector3 vPos, int x, int y)
    {
        m_isBlocked = bIsBlocked;
        m_vPosition = vPos;

        m_iGridX = x;
        m_iGridY = y;
    }

    public void BakeNeigbours(List<Node> neighbourList)
    {
        neigbours = neighbourList;
    }

    public List<Node> GetNeigbours()
    {
        return neigbours;
    }

    public Vector3 GetPosition()
    {
        return m_vPosition;
    }

    public bool IsBlocked()
    {
        return m_isBlocked;
    }

    public void SetOverlap(bool overlap)
    {
        overlapCapsule = overlap;
    }

    public bool IsOverlapping()
    {
        return overlapCapsule;
    }
}
