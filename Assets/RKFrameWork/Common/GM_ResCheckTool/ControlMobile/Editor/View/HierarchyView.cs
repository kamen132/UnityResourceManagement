
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Majic.CM
{
    public partial class HierarchyView : CMBaseWindowEditor
    {
        private List<GameObject> sceneList = new List<GameObject>();
        private Dictionary<int, GameObject> objectsDic = new Dictionary<int, GameObject>();
        Dictionary<int, GameObjectData> objectDatas = new Dictionary<int, GameObjectData>();
        private SceneDataList sceneDataList;
        private bool isLooking = false;
        private bool stopChecking = false;
        private bool isCheckVolumData = false;

        public override void OnGUI()
        {
            if (GUILayout.Button("Create HierarchyList", GUILayout.Width(300)))
            {
                stopChecking = true;
                isLooking = true;
                ClearSceneData();
                sceneDataList = MessageSender.GetHierarchyList();
                for (int i = 0; i < sceneDataList.List.Count; i++)
                {
                    SceneData scene = sceneDataList.List[i];
                    Transform parent = CreateSceneRoot(scene);
                    for (int j = 0; j < scene.objectList.Count; j++)
                    {
                        GameObjectData gameObjectData = scene.objectList[j];
                        objectDatas[gameObjectData.ID] = gameObjectData;
                        CreateObject(parent, gameObjectData);
                    }
                }
                stopChecking = false;
            }
            if (GUILayout.Button("Clear HierarchyList", GUILayout.Width(300)))
            {
                isLooking = false;
                ClearSceneData();
            }
            isCheckVolumData = GUILayout.Toggle(isCheckVolumData, "Check Volum", GUILayout.Width(300));
            OnUpdate();
        }

        public override void OnInit()
        {
            winName = "场景物体开关";
        }
        private void CreateObject(Transform scene, GameObjectData data)
        {
            GameObject gameObject = new GameObject();

            gameObject.name = data.Name;
            objectsDic[data.ID] = gameObject;
            if (data.ParentID == 0)
            {
                gameObject.transform.parent = scene;
            }
            else
            {
                gameObject.transform.parent = objectsDic[data.ParentID].transform;
            }
            gameObject.SetActive(data.ActiveSelf);
            gameObject.transform.localPosition = data.localPosition;
            gameObject.transform.localRotation = data.quaternion;
            gameObject.transform.localScale = data.LocalScale;
        }

        private Transform CreateSceneRoot(SceneData sceneData)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = "Scene_" + sceneData.Name;
            gameObject.transform.position = Vector3.zero;
            sceneList.Add(gameObject);
            GameObject root = GameObject.Find("HierarchyViewRoot");
            if (root == null)
            {
                root = new GameObject();
                root.name = "HierarchyViewRoot";
            }
            gameObject.transform.parent = root.transform;
            return gameObject.transform;
        }
        void ClearSceneData()
        {
            sceneList.Clear();
            objectsDic.Clear();
            DestroyImmediate(GameObject.Find("HierarchyViewRoot"));

        }
        public override void OnUpdate()
        {
            if (isLooking == false)
            {
                return;
            }
            CheckHierarchy();
        }
        private List<GameObjectData> result = new List<GameObjectData>();
        void CheckHierarchy()
        {
            if (objectsDic.Count > 0 && stopChecking == false)
            {
                result.Clear();
                foreach (var item in objectsDic)
                {
                    int ID = item.Key;
                    GameObject obj = item.Value;
                    GameObjectData gameObjectData = objectDatas[ID];

                    bool isChange = false;
                    bool activeSelf = obj.activeSelf;

                    if (gameObjectData.ActiveSelf != activeSelf)
                    {
                        gameObjectData.ActiveSelf = activeSelf;
                        isChange = true;
                    }
                  
                    Vector3 nowPos = obj.transform.localPosition;
                    if (gameObjectData.localPosition != nowPos)
                    {
                        gameObjectData.localPosition = nowPos;
                        isChange = true;
                    }

                    Quaternion nowLocalRotation = obj.transform.localRotation;
                    if (gameObjectData.quaternion != nowLocalRotation)
                    {
                        gameObjectData.quaternion = nowLocalRotation;
                        isChange = true;
                    }

                    Vector3 nowScale = obj.transform.localScale;
                    if (gameObjectData.LocalScale != nowScale)
                    {
                        gameObjectData.LocalScale = nowScale;
                        isChange = true;
                    }
                    if(isCheckVolumData)
                    {
                        bool result = CheckVolumData(obj, gameObjectData);
                        if (result)
                        {
                            isChange = true;
                        }
                    }
                    
                    if (isChange)
                    {
                        result.Add(gameObjectData);
                    }
                }
                if(result.Count > 0)
                {
                    GameObjectChageData realResult = new GameObjectChageData();
                    realResult.data = result;
                    MessageSender.ChangeHierarchy(JsonUtility.ToJson(realResult));
                }
            }
        }

        public override void Clear()
        {
            ClearSceneData();
        }
    }
}

