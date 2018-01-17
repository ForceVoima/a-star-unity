﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool m_isBlocked;
    public Vector3 m_vPosition;

    public Node(bool bIsBlocked, Vector3 vPos)
    {
        m_isBlocked = bIsBlocked;
        m_vPosition = vPos;
    }

    public Vector3 GetPosition()
    {
        return m_vPosition;
    }

    public bool IsBlocked()
    {
        return m_isBlocked;
    }
}
