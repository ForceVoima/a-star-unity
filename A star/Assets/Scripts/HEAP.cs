using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] aItems;
    int iCurrentItemCount = 0;

    public int Count { get { return iCurrentItemCount; } }


    /// <summary>
    /// Constructor for the Heap implementation.
    /// </summary>
    /// <param name="iHeapSize">Size of the Heap.</param>
    public Heap(int iHeapSize)
    {
        aItems = new T[iHeapSize];
    }

    public void Add(T item)
    {
        item.HeapIndex = iCurrentItemCount;
        aItems[iCurrentItemCount] = item;
        SortUp(item);
        iCurrentItemCount++;
    }

    private void SortUp(T item)
    {
        int iParentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = aItems[iParentIndex];

            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }
        }

        iParentIndex = (item.HeapIndex - 1) / 2;
    }

    private void SortDown(T item)
    {
        while (true)
        {
            int iLeft = (item.HeapIndex * 2) + 1;
            int iRight = (item.HeapIndex * 2) + 2;

            int iSwapIndex = 0;

            if (iLeft < iCurrentItemCount)
            {
                iSwapIndex = iLeft;

                if (iRight < iCurrentItemCount)
                {
                    if (aItems[iLeft].CompareTo(aItems[iRight]) < 0)
                    {
                        iSwapIndex = iRight;
                    }
                }

                if (item.CompareTo(aItems[iSwapIndex]) < 0)
                {
                    Swap(item, aItems[iSwapIndex]);
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    private void Swap(T itemA, T itemB)
    {
        aItems[itemA.HeapIndex] = itemB;
        aItems[itemB.HeapIndex] = itemA;

        int iTemp = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = iTemp;
    }

    public T RemoveFirst()
    {
        T first = aItems[0];

        iCurrentItemCount--;
        aItems[0] = aItems[iCurrentItemCount];
        aItems[0].HeapIndex = 0;
        SortDown(aItems[0]);
        return first;
    }

    public bool Contains(T item)
    {
        return Equals(aItems[item.HeapIndex], item);
    }
}

public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
