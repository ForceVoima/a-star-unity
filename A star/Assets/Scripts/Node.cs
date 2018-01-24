using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool m_isBlocked;
    public bool overlapCapsule = false;
    public Vector3 m_vPosition;

    public int m_iGridX;
    public int m_iGridY;

    public int m_iGCost;
    public int m_iHCost;

    public int m_iFCost
    {
        get { return m_iHCost + m_iGCost; }
    }

    public Node m_Parent;

    public Node(bool bIsBlocked, Vector3 vPos, int x, int y)
    {
        m_isBlocked = bIsBlocked;
        m_vPosition = vPos;

        m_iGridX = x;
        m_iGridY = y;
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
