using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager :Singleton<ObjectManager>
{
   protected Dictionary<Type,object> m_ClassPoolDic=new Dictionary<Type, object>();
   /// <summary>
   /// 创建类对象池，创建完成以后外面可以保存ClassObjectPool<T>,然后调用Spawn和Recycle来创建和回收类对象
   /// </summary>
   /// <param name="maxCount"></param>
   /// <typeparam name="T"></typeparam>
   /// <returns></returns>
   public ClassObjectPool<T> GetOrCreatClassPool<T>(int maxCount) where T : class, new()
   {
      Type type = typeof(T);
      object obj = null;
      if (!m_ClassPoolDic.TryGetValue(type,out obj)||obj==null)
      {
         ClassObjectPool<T> newPool=new ClassObjectPool<T>(maxCount);
         m_ClassPoolDic.Add(type, newPool);
         return newPool;
      }
      return obj as ClassObjectPool<T>;
   }
}
