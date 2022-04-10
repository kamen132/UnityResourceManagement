using System.Collections.Generic;
using UnityEngine;
namespace Majic.CM
{
    public partial class MobileServer
    {
        void GetObjectDicData(List<GameObjectData> objectList, Transform trans, Transform parent)
        {
            GameObjectData gameObjectData = new GameObjectData();
            int ID = trans.gameObject.GetInstanceID();
            gameObjectData.ID = ID;
            gameObjectData.ParentID = (parent == null ? 0 : parent.gameObject.GetInstanceID());
            gameObjectData.Name = trans.name;
            gameObjectData.ActiveSelf = trans.gameObject.activeSelf;
            gameObjectData.localPosition = trans.localPosition;
            gameObjectData.quaternion = trans.localRotation;
            gameObjectData.LocalScale = trans.localScale;
            objectList.Add(gameObjectData);
            foreach (Transform item in trans)
            {
                GetObjectDicData(objectList, item, trans);
            }
        }

        void ChangeGameObject(ThreadTask task)
        {
            string value = task.Parame["value"];
            GameObjectChageData gameObjectChageData = JsonUtility.FromJson<GameObjectChageData> (value);
            foreach(GameObjectData gameObjectData in gameObjectChageData.data)
            {
                if (objectsDic.ContainsKey(gameObjectData.ID) == false)
                {
                    continue;
                }
                GameObject obj = objectsDic[gameObjectData.ID];

                obj.SetActive(gameObjectData.ActiveSelf);
                obj.transform.localPosition = gameObjectData.localPosition;
                obj.transform.localRotation = gameObjectData.quaternion;
                obj.transform.localScale = gameObjectData.LocalScale;
            }
        }
      
    }
}
