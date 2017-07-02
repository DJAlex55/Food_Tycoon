using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    private T[] items;
    private int currentItemsCount;

    public int Count { get { return currentItemsCount; } }


    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }


    public void Add(T item)
    {
        item.HeapIndex = currentItemsCount;
        items[currentItemsCount] = item;
        SortUp(item);

        currentItemsCount++;
    }

    public T RemoveFirst()
    {
        T firstItem = items[0];
        currentItemsCount--;

        items[0] = items[currentItemsCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }


    public void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;



        while (true)
        {
            T parentItem = items[parentIndex];
            

            if(item.CompareTo(parentItem) > 0)
                Swap(item, parentItem);
            else
                break;

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    public void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = (item.HeapIndex * 2) + 1;
            int childIndexRight = (item.HeapIndex * 2) + 2;
            int SwapIndex = 0;

            if(childIndexLeft < currentItemsCount)
            {
                SwapIndex = childIndexLeft;

                if(childIndexRight < currentItemsCount)
                {
                    if(items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        SwapIndex = childIndexRight;
                    }
                }

                if(item.CompareTo(items[SwapIndex]) < 0)
                {
                    Swap(item, items[SwapIndex]);
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




    public void Swap(T ItemA, T ItemB)
    {
        items[ItemA.HeapIndex] = ItemB;
        items[ItemB.HeapIndex] = ItemA;

        int ItemAIndex = ItemA.HeapIndex;
        ItemA.HeapIndex = ItemB.HeapIndex;
        ItemB.HeapIndex = ItemAIndex;
    }


    public void UpdateItem(T item)
    {
        SortUp(item);
    }


    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

}


public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex { get; set; }
}