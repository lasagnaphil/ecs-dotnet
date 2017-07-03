using System;
using System.Collections.Generic;

namespace Ecs
{
    public class PooledListException : Exception
    {
        public PooledListException(string msg) : base(msg) { }
    }

    public class PooledList<T> where T : class
    {
        private const int initialPoolSize = 10;

        public int Count { get; private set; } = 0;

        private PooledListNode<T>[] nodes;
        private PooledListNode<T> firstAvailable;

        public PooledList()
        {
            nodes = new PooledListNode<T>[initialPoolSize];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new PooledListNode<T>();
            }
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                nodes[i].Next = nodes[i + 1];
            }
            nodes[nodes.Length - 1].Next = null;

            firstAvailable = nodes[0];
        }

        public void Add(T data)
        {
            // if the pool is full, double the pool size
            if (firstAvailable != null)
            {
                int prevPoolSize = nodes.Length;
                Array.Resize(ref nodes, prevPoolSize * 2);
                for (int i = prevPoolSize; i < nodes.Length; i++)
                {
                    nodes[i] = new PooledListNode<T>();
                }
                for (int i = prevPoolSize; i < nodes.Length - 1; i++)
                {
                    nodes[i].Next = nodes[i + 1];
                }
                nodes[nodes.Length - 1].Next = null;

                firstAvailable = nodes[prevPoolSize];
            }

            PooledListNode<T> newNode = firstAvailable;
            firstAvailable = newNode.Next;

            newNode.Data = data;
            Count++;
        }


        public bool TryGet(int index, out T result)
        {
            if (index < 0 || index >= nodes.Length)
            {
                result = null;
                return false;
            }
            result = nodes[index].Data;
            return true;
        }

        public T Get(int index)
        {
            if (index < 0 || index >= nodes.Length)
                throw new PooledListException($"Index out of bounds in method PooledList.Get(int), index = {index}");
            return nodes[index].Data;
        }

        public bool TryRemove(int index)
        {
            if (index < 0 || index >= nodes.Length) return false;

            nodes[index].Next = firstAvailable;
            firstAvailable = nodes[index];

            Count--;
            
            return true;
        }
        public void Remove(int index)
        {
            if (index < 0 || index >= nodes.Length)
                throw new PooledListException($"Index out of bounds in method PooledList.Remove(int), index = {index}");

            nodes[index].Next = firstAvailable;
            firstAvailable = nodes[index];

            Count--;

        }
    }

    class PooledListNode<T> where T : class
    {
        public T Data { get; set; }
        public PooledListNode<T> Next { get; set; }
        public bool Alive { get; set; }

        public PooledListNode()
        {
            Data = null;
        }
        public PooledListNode(T data)
        {
            Data = data;
        }
    }
}