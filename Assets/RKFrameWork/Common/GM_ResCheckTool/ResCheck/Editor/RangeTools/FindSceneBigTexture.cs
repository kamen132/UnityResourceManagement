using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindSceneBigTexture : ResCheckBaseSubWindowEditor
    {
        private enum SelectState
        {
            FindBigTexture = 1,
            FindPrefabData,
            FindMeshsData,
            FindEmptyMesh
        }
        private class FindPrefabDataAnotherData
        {
            public Texture2D t2D;
            public float size;
        }
        private class MeshData
        {
            public int vertexCount = 0;
            public int trisCount = 0;
        }
        private static SelectState _SelectState = SelectState.FindBigTexture;
        private static Dictionary<GameObject, List<GameObject>> emptyMeshDic = new Dictionary<GameObject, List<GameObject>>();

        private static Dictionary<Texture2D, float> _findBigTextureData = new Dictionary<Texture2D, float>();
        private static Dictionary<GameObject, float> _findPrefabData = new Dictionary<GameObject, float>();
        private static Dictionary<GameObject, MeshData> _findMeshData = new Dictionary<GameObject, MeshData>();
        private static Dictionary<GameObject, List<FindPrefabDataAnotherData>> _findPrefabDataAnother = new Dictionary<GameObject, List<FindPrefabDataAnotherData>>();
        private static GameObject selectObj;
        private static bool isInit = false;
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("检查场景大贴图", GUILayout.Width(200), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindBigTexture;
                FindPrefabData();
            }

            if (GUILayout.Button("检查每个场景贴图总大小", GUILayout.Width(200), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindPrefabData;
                FindPrefabData();
            }

            if (GUILayout.Button("检查每个场景顶点数和面数", GUILayout.Width(200), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindMeshsData;
                FindPrefabData();
            }
            if (GUILayout.Button("检查场景空Mesh的模型", GUILayout.Width(200), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindEmptyMesh;
                FindPrefabData();
            }

            if (GUILayout.Button("清除缓存", GUILayout.Width(100), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindBigTexture;
                _findPrefabData.Clear();
                _findBigTextureData.Clear();
                _findMeshData.Clear();
                emptyMeshDic.Clear();
                isInit = false;
            }

            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        static bool isContentSameTexture(List<FindPrefabDataAnotherData> list, Texture2D texture)
        {
            foreach (var item in list)
            {
                if (item.t2D == texture)
                {
                    return true;
                }
            }
            return false;
        }

        private static void FindPrefabData()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            string endSuffix = "*.prefab";
            string directoryPath = Application.dataPath + "/WorkAssets/scene_new";
            string[] sceneNames = Directory.GetFiles(directoryPath, endSuffix, SearchOption.AllDirectories);
            for (int nameIndex = 0; nameIndex < sceneNames.Length; nameIndex++)
            {
                string assetPath = sceneNames[nameIndex];
                assetPath = ResCheckEditorUtil.FormatPath(assetPath);
                assetPath = ResCheckEditorUtil.GetAssetPath(assetPath);
                EditorUtility.DisplayProgressBar("加载中", Path.GetFileName(assetPath), (nameIndex / sceneNames.Length));
                GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                GetAllPngData(go, assetPath);
                GetMeshsVertsAndTris(go);
            }
            EditorUtility.ClearProgressBar();
            _findBigTextureData = _findBigTextureData.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
            _findMeshData = _findMeshData.OrderByDescending(o => o.Value.vertexCount).ToDictionary(p => p.Key, o => o.Value);
            _findPrefabData = _findPrefabData.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
        }
        static void GetAllPngData(GameObject go, string assetPath)
        {
            float tempValue = 0;
            string[] deps = ResCheckJenkinsEntrance.GetDependencies(assetPath, true);
            foreach (string dep in deps)
            {
                if (ResCheckEditorUtil.IsTexture2D(dep))
                {
                    Texture2D texture = ResCheckJenkinsEntrance.LoadAssetAtPath(dep) as Texture2D;
                    if (texture == null)
                    {
                        Debug.Log("texture nul: " + dep);
                        continue;
                    }
                    float value = ResCheckEditorUtil.CalTextureSize(texture);
                    _findBigTextureData[texture] = value;
                    FindPrefabDataAnotherData item = new FindPrefabDataAnotherData();
                    item.t2D = texture;
                    item.size = value;
                    if (_findPrefabDataAnother.ContainsKey(go) == false)
                    {
                        _findPrefabDataAnother[go] = new List<FindPrefabDataAnotherData>();
                    }
                    _findPrefabDataAnother[go].Add(item);
                    tempValue += value;
                }
            }
            _findPrefabData[go] = tempValue;

        }

        //static void GetAnotherData1(GameObject go, string assetPath)
        //{
        //    if (go)
        //    {
        //        Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
        //        foreach (var render in renders)
        //        {
        //            foreach (var mat in render.sharedMaterials)
        //            {
        //                if (!mat) continue;
        //                for (int i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
        //                {
        //                    if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
        //                    {
        //                        string propertyname = ShaderUtil.GetPropertyName(mat.shader, i);
        //                        Texture2D texture = mat.GetTexture(propertyname) as Texture2D;

        //                        if (texture != null && _findBigTextureData.ContainsKey(texture) == false)
        //                        {
        //                            float value = ResCheckEditorUtil.CalTextureSize(texture);
        //                            _findBigTextureData[texture] = value;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}

        static void GetMeshsVertsAndTris(GameObject go)
        {
            if (go)
            {
                MeshData meshData = new MeshData();
                MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter filter in filters)
                {
                    if (filter.sharedMesh == null)
                    {
                        Debug.Log("miss mesh: " + go.name + "   __   " + filter.gameObject.name);
                        continue;
                    }
                    int vertexCount = filter.sharedMesh.vertexCount;
                    if (vertexCount == 0)
                    {
                        if (emptyMeshDic.ContainsKey(go) == false)
                        {
                            emptyMeshDic[go] = new List<GameObject>();
                        }
                        emptyMeshDic[go].Add(filter.gameObject);
                    }
                    meshData.vertexCount += vertexCount;
                    meshData.trisCount += filter.sharedMesh.triangles.Length / 3;
                    //Debug.Log("name==" + filter.sharedMesh.name + "  count:=" + filter.sharedMesh.vertexCount + "ccc===" + (filter.sharedMesh.triangles.Length / 3));
                }
                _findMeshData[go] = meshData;
            }
        }
        private static Vector3 scrollPos = Vector3.zero;
        private static void ShowFindBigTextrueResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));
            EditorGUILayout.LabelField("---------------------------检查场景大贴图-----------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in _findBigTextureData)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.LabelField(item.Value.ToString());
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(item.Value));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void ShowFindPrefabData()
        {
            EditorGUILayout.LabelField("--------------------------------检查场景贴图总大小------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(600), GUILayout.Height(600));

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in _findPrefabData)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(item.Value), GUILayout.Width(70));
                int len = 70;
                if (selectObj != null && selectObj == item.Key)
                {
                    len = 100;
                }
                if (GUILayout.Button("查看", GUILayout.Width(len), GUILayout.Height(20)))
                {
                    selectObj = item.Key;
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            ShowFindPrefabDataItem();
            EditorGUILayout.EndHorizontal();

        }
        private static Vector3 scrollPos2 = Vector3.zero;
        private static void ShowFindPrefabDataItem()
        {
            if (selectObj == null)
            {
                return;
            }
            GUILayout.Space(15);
            List<FindPrefabDataAnotherData> data = _findPrefabDataAnother[selectObj];
            data.Sort((a, b) => b.size.CompareTo(a.size));
            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, false, true, GUILayout.Width(500), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.ObjectField(selectObj, selectObj.GetType(), true, GUILayout.Width(400));
            EditorGUILayout.LabelField("贴图数量：" + data.Count);
            foreach (var item in data)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.t2D, item.t2D.GetType(), true, GUILayout.Width(400));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(item.size));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void ShowFindMeshsData()
        {
            EditorGUILayout.LabelField("---------------------------检查场景 Mesh 数据-----------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(400));
            EditorGUILayout.LabelField("顶点数", GUILayout.Width(100));
            EditorGUILayout.LabelField("面数", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in _findMeshData)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                MeshData meshData = item.Value;
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(meshData.vertexCount), GUILayout.Width(100));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(meshData.trisCount), GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void ShowEmptyMeshsData()
        {
            EditorGUILayout.LabelField("---------------------------检查场景 空 Mesh 数据-----------------------------------------------------");

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in emptyMeshDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                List<GameObject> meshsData = item.Value;
                EditorGUILayout.BeginVertical();
                foreach (var meshData in meshsData)
                {
                    EditorGUILayout.ObjectField(meshData, meshData.GetType(), true, GUILayout.Width(400));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        private static void ShowFindResult()
        {
            //dirs.Add(ResCheckJenkinsEntrance.LoadAssetAtPath(defaulutCheckPaths[i], typeof(Object)));
            switch (_SelectState)
            {
                case SelectState.FindBigTexture:
                    ShowFindBigTextrueResult();
                    break;
                case SelectState.FindPrefabData:
                    ShowFindPrefabData();
                    break;
                case SelectState.FindMeshsData:
                    ShowFindMeshsData();
                    break;
                case SelectState.FindEmptyMesh:
                    ShowEmptyMeshsData();
                    break;
            }
        }
    }
}
