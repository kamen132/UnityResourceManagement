using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class AboutScene : ResCheckBaseSubWindowEditor
    {
        private enum SelectState
        {
            FindBigTexture = 1,
            FindPrefabData,
            FindMeshsData,
            FindShaderData,
            FindEmptyMesh,//空mesh
            FindMissingMesh,
            FindCullOffData,
        }
        private class PrefabTexutreSizeData
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
        private static Dictionary<GameObject, float> _prefabTotlePngSizeData = new Dictionary<GameObject, float>();
        private static Dictionary<GameObject, MeshData> _findMeshData = new Dictionary<GameObject, MeshData>();
        private static Dictionary<GameObject, Dictionary<Object, MeshData>> _findMeshObjInfoData = new Dictionary<GameObject, Dictionary<Object, MeshData>>();
        private static Dictionary<GameObject, List<Shader>> _findObjShaderInfoData = new Dictionary<GameObject, List<Shader>>();
        private static Dictionary<GameObject, List<PrefabTexutreSizeData>> _prefabTexutreSizeData = new Dictionary<GameObject, List<PrefabTexutreSizeData>>();
        private static GameObject selectObj;
        private static Dictionary<GameObject, List<string>> missingMeshDic = new Dictionary<GameObject, List<string>>();
        private static bool isInit = false;
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            float width = 160;
            if (GUILayout.Button("查看场景大贴图", GUILayout.Width(width), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindBigTexture;
                FindPrefabData();
            }

            if (GUILayout.Button("查看每个场景贴图总大小", GUILayout.Width(width), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindPrefabData;
                FindPrefabData();
            }

            if (GUILayout.Button("检查场景 Mesh 数据", GUILayout.Width(width), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindMeshsData;
                FindPrefabData();
            }
            if (GUILayout.Button("检查场景 Shader 数据", GUILayout.Width(width), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindShaderData;
                FindPrefabData();
            }

            if (GUILayout.Button("查看场景空Mesh的模型", GUILayout.Width(width), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindEmptyMesh;
                FindPrefabData();
            }
            if (GUILayout.Button("查看场景MissingMesh的模型", GUILayout.Width(width), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindMissingMesh;
                FindPrefabData();
            }
            if (GUILayout.Button("查看场景Cull off的模型", GUILayout.Width(width), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindCullOffData;
                FindPrefabData();
            }
            if (GUILayout.Button("清除缓存", GUILayout.Width(100), GUILayout.Height(20)))
            {
                _SelectState = SelectState.FindBigTexture;
                _prefabTotlePngSizeData.Clear();
                _findBigTextureData.Clear();
                _findMeshData.Clear();
                emptyMeshDic.Clear();
                matIsCullOffDIc.Clear();

                _findMeshObjInfoData.Clear();
                _findObjShaderInfoData.Clear();
                _prefabTexutreSizeData.Clear();
                missingMeshDic.Clear();

                isInit = false;
            }

            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        void FindPrefabData()
        {
            if (isInit)
            {
                return;
            }
            isInit = true;
            string endSuffix = "*.prefab";
            string directoryPath = Application.dataPath + "/Product/Editor/Resources/SceneOutput";
            string[] sceneNames = Directory.GetFiles(directoryPath, endSuffix, SearchOption.AllDirectories);
            for (int nameIndex = 0; nameIndex < sceneNames.Length; nameIndex++)
            {
                string assetPath = sceneNames[nameIndex];
                assetPath = ResCheckEditorUtil.GetAssetPath(assetPath);
                EditorUtility.DisplayProgressBar("加载中", Path.GetFileName(assetPath), nameIndex / sceneNames.Length);
                GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                GetAllPngData(go, assetPath);
                _findMeshData[go] = GetMeshsVertsAndTris(go);
                GetAllMeshInfo(go, assetPath);
                GetShaderInfo(go, assetPath);
                GetMatCullOffInfo(go);
            }
            EditorUtility.ClearProgressBar();
            _findBigTextureData = _findBigTextureData.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);
            _findMeshData = _findMeshData.OrderByDescending(o => o.Value.vertexCount).ToDictionary(p => p.Key, o => o.Value);
            _prefabTotlePngSizeData = _prefabTotlePngSizeData.OrderByDescending(o => o.Value).ToDictionary(p => p.Key, o => o.Value);

            Dictionary<GameObject, Dictionary<Object, MeshData>> tempData = new Dictionary<GameObject, Dictionary<Object, MeshData>>();
            foreach (var item in _findMeshObjInfoData)
            {
                var valueData = item.Value.OrderByDescending(o => o.Value.vertexCount).ToDictionary(p => p.Key, o => o.Value);
                tempData[item.Key] = valueData;
            }
            _findMeshObjInfoData = tempData;

            foreach (var item in _prefabTexutreSizeData)
            {
                item.Value.Sort((a, b) => b.size.CompareTo(a.size));
            }
        }
        void GetAllMeshInfo(GameObject scenePrefab, string assetPath)
        {
            string[] allDeps = ResCheckJenkinsEntrance.GetDependencies(assetPath);
            Dictionary<Object, MeshData> data = new Dictionary<Object, MeshData>();
            foreach (string dep in allDeps)
            {
                if (dep.ToLower().EndsWith(".fbx"))
                {
                    GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(dep) as GameObject;
                    if (go == null)
                        return;
                    MeshData meshData = GetMeshsVertsAndTris(go);
                    data[go] = meshData;
                }
            }
            _findMeshObjInfoData[scenePrefab] = data;
        }
        void GetShaderInfo(GameObject scenePrefab, string assetPath)
        {
            string[] allDeps = ResCheckJenkinsEntrance.GetDependencies(assetPath);

            foreach (string dep in allDeps)
            {
                if (dep.ToLower().EndsWith(".shader"))
                {
                    Shader shader = ResCheckJenkinsEntrance.LoadAssetAtPath(dep) as Shader;
                    if (_findObjShaderInfoData.ContainsKey(scenePrefab) == false)
                    {
                        _findObjShaderInfoData[scenePrefab] = new List<Shader>();
                    }
                    _findObjShaderInfoData[scenePrefab].Add(shader);
                }
            }
        }
        void GetAllPngData(GameObject go, string assetPath)
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
                    PrefabTexutreSizeData item = new PrefabTexutreSizeData();
                    item.t2D = texture;
                    item.size = value;
                    if (_prefabTexutreSizeData.ContainsKey(go) == false)
                    {
                        _prefabTexutreSizeData[go] = new List<PrefabTexutreSizeData>();
                    }
                    _prefabTexutreSizeData[go].Add(item);
                    tempValue += value;
                }
            }
            _prefabTotlePngSizeData[go] = tempValue;

        }

        private static Dictionary<GameObject, Dictionary<GameObject, Material>> matCullOfDic = new Dictionary<GameObject, Dictionary<GameObject, Material>>();
        private static Dictionary<Material, bool> matIsCullOffDIc = new Dictionary<Material, bool>();
        void GetMatCullOffInfo(GameObject go)
        {
            
            Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
            foreach (Renderer render in renders)
            {
                Material mat = render.sharedMaterial;
                if(mat == null)
                {
                    //Debug.Log("mat is null:" + go + "--" + render.gameObject);
                    continue;
                }
                if (matIsCullOffDIc.ContainsKey(mat) == false)
                {
                    matIsCullOffDIc[mat] = IsMatCullOff(mat);
                }
                if(matIsCullOffDIc[mat])
                {
                    if (matCullOfDic.ContainsKey(go) == false)
                    {
                        matCullOfDic[go] = new Dictionary<GameObject, Material>();
                    }
                    matCullOfDic[go][render.gameObject] = mat;
                }
            }
        }
       
        static bool IsMatCullOff(Material mat)
        {
            SerializedObject psSource = new SerializedObject(mat);
            SerializedProperty emissionProperty = psSource.FindProperty("m_SavedProperties");

            SerializedProperty floats = emissionProperty.FindPropertyRelative("m_Floats");
            for (int j = floats.arraySize - 1; j >= 0; j--)
            {
                var propertyName = floats.GetArrayElementAtIndex(j).FindPropertyRelative("first").stringValue;
                if(propertyName.Equals("_Cull"))
                {
                    float value = floats.GetArrayElementAtIndex(j).FindPropertyRelative("second").floatValue;
                    return value == 0;
                }
            }
            return false;
        }
        MeshData GetMeshsVertsAndTris(GameObject go)
        {
            if (go)
            {
                MeshData meshData = new MeshData();
                MeshFilter[] filters = go.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter filter in filters)
                {
                    if (filter.sharedMesh == null)
                    {
                        //Debug.Log("miss mesh: " + go.name + "   __   " + filter.gameObject.name);
                        if (missingMeshDic.ContainsKey(go) == false)
                        {
                            missingMeshDic[go] = new List<string>();
                        }
                        missingMeshDic[go].Add(filter.gameObject.name);
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
                return meshData;
            }
            return null;
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
            foreach (var item in _prefabTotlePngSizeData)
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
            GUILayout.Space(7);
            List<PrefabTexutreSizeData> data = _prefabTexutreSizeData[selectObj];
            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, false, true, GUILayout.Width(600), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            EditorGUILayout.ObjectField(selectObj, selectObj.GetType(), true, GUILayout.Width(400));
            EditorGUILayout.LabelField("贴图数量：" + data.Count);
            float width = 90;
            for (int i = 0; i < data.Count; i++)
            {
                PrefabTexutreSizeData item = data[i];
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.t2D, item.t2D.GetType(), true, GUILayout.Width(300));
                EditorGUILayout.LabelField(item.t2D.width + "x" + item.t2D.width, GUILayout.Width(width));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(item.size), GUILayout.Width(width));
                EditorGUILayout.LabelField(item.t2D.format.ToString(), GUILayout.Width(width));
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void ShowFindMeshsData()
        {
            EditorGUILayout.LabelField("---------------------------检查场景 Mesh 数据-----------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(400));
            EditorGUILayout.LabelField("vertex", GUILayout.Width(100));
            EditorGUILayout.LabelField("tris", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(750), GUILayout.Height(600));

            GUILayout.Space(5);
            //EditorGUILayout.BeginVertical();
            foreach (var item in _findMeshData)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                MeshData meshData = item.Value;
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(meshData.vertexCount), GUILayout.Width(100));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(meshData.trisCount), GUILayout.Width(100));
                if (GUILayout.Button("查看", GUILayout.Width(100), GUILayout.Height(20)))
                {
                    selectObj = item.Key;
                }
                EditorGUILayout.EndHorizontal();
            }
            //EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            ShowFindMeshObjItem();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
        private static void ShowFindMeshObjItem()
        {
            if (selectObj == null)
            {
                return;
            }

            Dictionary<Object, MeshData> data = _findMeshObjInfoData[selectObj];

            EditorGUILayout.ObjectField(selectObj, selectObj.GetType(), true, GUILayout.Width(400));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(300));
            EditorGUILayout.LabelField("vertex", GUILayout.Width(100));
            EditorGUILayout.LabelField("tris", GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            float maxVertext = 0;
            float maxTris = 0;
            foreach (var item in data)
            {
                maxVertext += item.Value.vertexCount;
                maxTris += item.Value.trisCount;
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("", GUILayout.Width(300));
            EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(maxVertext), GUILayout.Width(100));
            EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(maxTris), GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();

            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, false, true, GUILayout.Width(600), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();

            foreach (var item in data)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(300));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(item.Value.vertexCount), GUILayout.Width(100));
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(item.Value.trisCount), GUILayout.Width(500));
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
        private static void ShowMissingMeshsData()
        {
            EditorGUILayout.LabelField("---------------------------检查场景 Mssing Mesh 数据-----------------------------------------------------");
            GUILayout.Space(5);
            if (GUILayout.Button("导出excle", GUILayout.Width(200), GUILayout.Height(20)))
            {
                string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
                List<string> title = new List<string>();
                title.Add("PrefabName");
                title.Add("SubObjectName");
                List<List<string>> data = new List<List<string>>();
                foreach (var item in missingMeshDic)
                {
                    foreach (string name in item.Value)
                    {
                        List<string> itemData = new List<string>();
                        itemData.Add(item.Key.name);
                        itemData.Add(name);
                        data.Add(itemData);
                    }
                }
                ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "SceneMissingMeshData", "sheet", title, data);

            }
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in missingMeshDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                List<string> meshsData = item.Value;
                EditorGUILayout.BeginVertical();
                foreach (var meshData in meshsData)
                {
                    EditorGUILayout.LabelField(meshData, GUILayout.Width(400));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void ShowShaderData()
        {
            EditorGUILayout.LabelField("---------------------------检查场景 shader 数据-----------------------------------------------------");
            GUILayout.Space(5);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in _findObjShaderInfoData)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(400));
                List<Shader> data = item.Value;
                EditorGUILayout.BeginVertical();
                foreach (Shader shaderData in data)
                {
                    EditorGUILayout.ObjectField(shaderData, shaderData.GetType(), true, GUILayout.Width(400));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
        private static void ShowCullOffData()
        {
            EditorGUILayout.LabelField("---------------------------检查场景 CullOff 数据-----------------------------------------------------");
            GUILayout.Space(5);
            if(matCullOfDic.Count > 0)
            {
                if (GUILayout.Button("导出excle", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
                    List<string> title = new List<string>();
                    title.Add("PrefabName");
                    title.Add("ObjectName");
                    title.Add("ObjectName");
                    title.Add("ObjectName");
                    List<List<string>> data = new List<List<string>>();
                    foreach (var item in matCullOfDic)
                    {
                        foreach (var tiem2 in item.Value)
                        {
                            List<string> itemData = new List<string>();
                            itemData.Add(item.Key.name);
                            itemData.Add(tiem2.Key.name);
                            itemData.Add(tiem2.Value.name);
                            itemData.Add(tiem2.Value.shader.name);
                            data.Add(itemData);
                        }
                    }
                    ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "SceneCullofData", "sheet", title, data);

                }
            }
           

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));

            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            foreach (var item in matCullOfDic)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(200));
               
                EditorGUILayout.BeginVertical();
                foreach (var item2 in item.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(item2.Key, item2.Key.GetType(), true, GUILayout.Width(200));
                    EditorGUILayout.ObjectField(item2.Value, item2.Value.GetType(), true, GUILayout.Width(200));
                    EditorGUILayout.ObjectField(item2.Value.shader, item2.Value.shader.GetType(), true, GUILayout.Width(200));
                    EditorGUILayout.EndHorizontal();
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
                case SelectState.FindMissingMesh:
                    ShowMissingMeshsData();
                    break;
                case SelectState.FindShaderData:
                    ShowShaderData();
                    break;
                case SelectState.FindCullOffData:
                    ShowCullOffData();
                    break;
                    

            }
        }
    }
}

