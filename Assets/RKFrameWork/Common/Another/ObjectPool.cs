﻿
/*
 * 对象池
 */

using System;
using System.Collections.Generic;
namespace Majic.CM
{
    public class ObjectPool<T> where T : class, new()
    {
        public delegate T CreateFunc();
        public ObjectPool(int poolSize, CreateFunc createFunc = null, Action<T> resetAction = null)
        {
            Init(poolSize, createFunc, resetAction);
        }
        public T GetObject()
        {
            lock (this)
            {
                if (m_objStack.Count > 0)
                {
                    T t = m_objStack.Pop();
                    return t;
                }
            }
            return new T();
        }

        public void ReleaseObject(T obj)
        {
            if (obj == null)
                return;
            lock (this)
            {
                m_objStack.Push(obj);
            }
        }

        public void Init(int poolSize, CreateFunc createFunc = null, Action<T> resetAction = null)
        {
            m_objStack = new Stack<T>(poolSize);
            for (int i = 0; i < poolSize; i++)
            {
                T item = new T();
                m_objStack.Push(item);
            }
        }



        // 少用，调用这个池的作用就没有了
        public void Clear()
        {
            if (m_objStack != null)
                m_objStack.Clear();
        }

        public int Count
        {
            get
            {
                if (m_objStack == null)
                    return 0;
                return m_objStack.Count;
            }
        }

        public Stack<T>.Enumerator GetIter()
        {
            if (m_objStack == null)
                return new Stack<T>.Enumerator();
            return m_objStack.GetEnumerator();
        }

        private Stack<T> m_objStack = null;
    }
}


