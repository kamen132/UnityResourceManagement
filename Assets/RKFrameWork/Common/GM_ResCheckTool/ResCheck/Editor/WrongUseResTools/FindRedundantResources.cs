using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace GMResChecker
{
    public class FindRedundantResources : ResCheckBaseSubWindowEditor
    {
        private static List<string> NOTextureList = new List<string>() {
            //"FX/UIParticle/Particle_Common"
        };
        private static string findBaseFonder = "/Product/Editor/Resources";
        private static string findSceneFonder = "/Product/Editor/Resources/Scene";
        private static string findSceneFonderCommon = "/Product/Editor/Resources/Scene/Common";
        private static string findSceneFonder1 = "/Product/Editor/Resources/Scene/highstreet01";
        private static string tempFindFonder = findBaseFonder;
        private class RedundancyData
        {
            public string rootPath;
            public List<GameObject> subObj = new List<GameObject>();
        }
        private static Dictionary<GameObject, RedundancyData> RedundancyObject = new Dictionary<GameObject, RedundancyData>();
        private static List<Material> RedundancyMaterials = new List<Material>();

        public static void AutoClearSceneData()
        {
            RedundancyObject.Clear();
            RedundancyMaterials.Clear();
            tempFindFonder = findSceneFonderCommon;

            CheckMatRedundancy();
            CheckPrefabRedundancy();
            tempFindFonder = findSceneFonder1;

            CheckMatRedundancy();
            CheckPrefabRedundancy();

            ClearSceneData();
        }
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找Controller冗余资源", GUILayout.Width(200), GUILayout.Height(20)))
            {
                CheckAnimatorOverrideController();
            }
            if (GUILayout.Button("查找Timeline冗余资源", GUILayout.Width(200), GUILayout.Height(20)))
            {
                CheckTimeRedundant();
            }
            if (GUILayout.Button("查找冗余资源", GUILayout.Width(200), GUILayout.Height(20)))
            {
                tempFindFonder = findBaseFonder;
                RedundancyObject.Clear();
                RedundancyMaterials.Clear();
                CheckMatRedundancy();
                CheckPrefabRedundancy();
            }
            GUILayout.Space(10);
            if (GUILayout.Button("查找场景冗余资源", GUILayout.Width(200), GUILayout.Height(20)))
            {
                tempFindFonder = findSceneFonder;
                RedundancyObject.Clear();
                RedundancyMaterials.Clear();
                CheckMatRedundancy();
                CheckPrefabRedundancy();
            }
            if (GUILayout.Button("查找副本1冗余资源", GUILayout.Width(200), GUILayout.Height(20)))
            {
                RedundancyObject.Clear();
                RedundancyMaterials.Clear();
                tempFindFonder = findSceneFonderCommon;
                
                CheckMatRedundancy();
                CheckPrefabRedundancy();
                tempFindFonder = findSceneFonder1;

                CheckMatRedundancy();
                CheckPrefabRedundancy();
            }
            GUILayout.Space(10);
            if (RedundancyMaterials.Count + RedundancyObject.Count > 0)
            {
                if (GUILayout.Button("删除全部冗余资源", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    foreach(var item in RedundancyMaterials)
                    {
                        DeleteMatRedundancy(item);
                    }
                    foreach (var item in RedundancyObject)
                    {
                        if (item.Value.rootPath.Contains("scene_root_node") == false)
                        {
                            DeleteObjRedundancy(item.Key);
                        }
                    }
                    RedundancyObject.Clear();
                    RedundancyMaterials.Clear();
                }
                if (GUILayout.Button("删除场景冗余资源", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    ClearSceneData();
                }
            }
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        static void CheckTimeRedundant()
        {

            string path = "Assets/Product/Editor/Resources/Role/Monster/dread02/SK/monster_dread02_changetoheart.playable";
            TimelineAsset asset = AssetDatabase.LoadAssetAtPath<TimelineAsset>(path);

            var item = asset.GetOutputTracks();
           
            var outputTracksEnumerator = asset.GetOutputTracks().GetEnumerator();
            while (outputTracksEnumerator.MoveNext())
            {
                TrackAsset track = outputTracksEnumerator.Current;
                PlayableAsset parent = track.parent;
                //Debug.Log("2222");
            }
            SerializedObject psSource = new SerializedObject(asset);
            int size = 0;
            var iter = psSource.GetIterator();//拿到迭代器
            while (iter.NextVisible(true))//如果有下一个属性
            {
                if (iter.propertyType != SerializedPropertyType.ObjectReference)
                {
                    continue;
                }
                //iter.FindPropertyRelativeuo
                Debug.Log("2222==" + iter.name);
            }
                //SerializedProperty monosProperty = psSource.FindProperty("MonoBehaviour");

                //Component[] cps = asset.g.GetComponentsInChildren<Component>(true);

                Debug.Log("2222==" + size);
            //var item2 = asset.GetRootTracks();
            //var allGrouos = asset.GetRootTracks().GetEnumerator();
            //while (allGrouos.MoveNext())
            //{
            //    GroupTrack track = allGrouos.Current as GroupTrack;

            //    Debug.Log("2222");
            //}
        }
        static void CheckAnimatorOverrideController()
        {
            var allfiles = Directory.GetFiles(Application.dataPath + tempFindFonder, "*.controller", SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                if (string.IsNullOrEmpty(file))
                    continue;

                string assetPath = ResCheckEditorUtil.GetAssetPath(file);
                AnimatorOverrideController overrideController = AssetDatabase.LoadAssetAtPath<AnimatorOverrideController>(assetPath);
                if(overrideController == null)
                {
                    continue;
                }
                var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>(overrideController.overridesCount);

                overrideController.GetOverrides(overrides);
                Dictionary<string, int> needOverrides = new Dictionary<string, int>();
                bool isChange = false;
                for (int i = overrides.Count - 1; i >= 0; i--)
                {
                    var item = overrides[i];
                    if (item.Value != null)
                    {
                        needOverrides[item.Key.name] = 1;
                    }
                }
                SerializedObject psSource = new SerializedObject(overrideController);
                SerializedProperty clipsProperty = psSource.FindProperty("m_Clips");
                for (int j = clipsProperty.arraySize - 1; j >= 0; j--)
                {
                    SerializedProperty item = clipsProperty.GetArrayElementAtIndex(j);
                    var m_OriginalClip = item.FindPropertyRelative("m_OriginalClip");
                    var m_OverrideClip = item.FindPropertyRelative("m_OverrideClip");
                    Object originalValue = m_OriginalClip.objectReferenceValue;
                    Object overrideValue = m_OverrideClip.objectReferenceValue;
                    if (originalValue != null && needOverrides.ContainsKey(originalValue.name) == false)
                    {
                        clipsProperty.DeleteArrayElementAtIndex(j);
                        isChange = true;
                    }
                }
                if (isChange)
                {
                    Debug.Log("name===" + assetPath);
                    psSource.ApplyModifiedProperties();
                    EditorUtility.SetDirty(overrideController);
                }

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            //string path = "Assets/Product/Editor/Resources/Role/Hero/nanzhu/A/hero_nanzhu_npc.controller";
            //string path = "Assets/Product/Editor/Resources/Role/Monster/dread01/A/monster_dread01_npc.controller";
            //string path = "Assets/Product/Editor/Resources/Role/Monster/dread02/A/monster_dread02_controller.controller";
         
        }
        static void ClearSceneData()
        {
            List<Material> dataMat = new List<Material>();
            foreach (var item in RedundancyMaterials)
            {
                string path = ResCheckJenkinsEntrance.GetAssetPath(item);
                if (path.Contains("Editor/Resources/Scene"))
                {
                    dataMat.Add(item);
                }
            }
            List<GameObject> dataObj = new List<GameObject>();
            foreach (var item in RedundancyObject)
            {
                if (item.Value.rootPath.Contains("Editor/Resources/Scene")
                    && item.Value.rootPath.Contains("scene_root_node") == false
                    )
                {
                    dataObj.Add(item.Key);
                }
            }
            foreach (var item in dataMat)
            {
                DeleteMatRedundancy(item);
                RedundancyMaterials.Remove(item);
            }
            foreach (var item in dataObj)
            {
                DeleteObjRedundancy(item);
                RedundancyObject.Remove(item);
            }
        }
        //[@MenuItem("Tools/GetRedundantResources")]
        public static string GetRedundantResources()
        {
            RedundancyObject.Clear();
            RedundancyMaterials.Clear();
            CheckMatRedundancy();
            CheckPrefabRedundancy();
            StringBuilder builder = new StringBuilder();
            builder.Append("=========GetRedundantResources:" + (RedundancyObject.Count + RedundancyMaterials.Count) + "=========\n");
            foreach (var item in RedundancyObject)
            {
                GameObject key = item.Key;
                string path = ResCheckJenkinsEntrance.GetAssetPath(key);
                foreach (var subItem in item.Value.subObj)
                {
                    builder.Append("prefab ");
                    builder.Append(path);
                    builder.Append("  ");
                    builder.Append(subItem.name);
                    builder.Append("\n");
                }
            }
            foreach (var item in RedundancyMaterials)
            {
                builder.Append("mat ");
                builder.Append(ResCheckJenkinsEntrance.GetAssetPath(item));
                builder.Append("\n");
            }
            builder.Append("\n\n");
            return builder.ToString();
        }

        private static void CheckMatRedundancy()
        {
            var files = Directory.GetFiles(Application.dataPath + tempFindFonder, "*.mat", SearchOption.AllDirectories);
            foreach (var path in files)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                string assetPath = ResCheckEditorUtil.GetAssetPath(path);
                //if(assetPath.Contains("fx_quest_jackpot_common"))
                //{
                //    Debug.Log("111111");
                //}
                Material mat = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as Material;
                if (mat)
                {
                    SerializedObject psSource = new SerializedObject(mat);
                    SerializedProperty emissionProperty = psSource.FindProperty("m_SavedProperties");
                    SerializedProperty texEnvs = emissionProperty.FindPropertyRelative("m_TexEnvs");
                    SerializedProperty floats = emissionProperty.FindPropertyRelative("m_Floats");
                    SerializedProperty colos = emissionProperty.FindPropertyRelative("m_Colors");

                    CheckMaterialSerializedProperty(texEnvs, mat);
                    CheckMaterialSerializedProperty(floats, mat);
                    CheckMaterialSerializedProperty(colos, mat);
                }
            }
        }

        static void CheckMaterialSerializedProperty(SerializedProperty property, Material mat)
        {
            if (RedundancyMaterials.Contains(mat))
            {
                return;
            }
            for (int j = property.arraySize - 1; j >= 0; j--)
            {
                var propertyName = property.GetArrayElementAtIndex(j).FindPropertyRelative("first").stringValue;
                if (mat.HasProperty(propertyName) == false)
                {
                    RedundancyMaterials.Add(mat);
                    return;
                }else
                {
                    if (propertyName.Equals("_MainTex") && NOTextureList.Contains(mat.shader.name))
                    {
                        RedundancyMaterials.Add(mat);
                        return;
                    }
                }
            }
        }

        private static void DeleteMatRedundancy(Material mat)
        {
            SerializedObject psSource = new SerializedObject(mat);
            SerializedProperty emissionProperty = psSource.FindProperty("m_SavedProperties");
            SerializedProperty texEnvs = emissionProperty.FindPropertyRelative("m_TexEnvs");
            SerializedProperty floats = emissionProperty.FindPropertyRelative("m_Floats");
            SerializedProperty colos = emissionProperty.FindPropertyRelative("m_Colors");

            DeleteDependencs(texEnvs, mat);
            DeleteDependencs(floats, mat);
            DeleteDependencs(colos, mat);
            psSource.ApplyModifiedProperties();
            EditorUtility.SetDirty(mat);
        }

        static void DeleteDependencs(SerializedProperty property, Material mat)
        {
            for (int j = property.arraySize - 1; j >= 0; j--)
            {
                var propertyName = property.GetArrayElementAtIndex(j).FindPropertyRelative("first").stringValue;
                if (mat.HasProperty(propertyName) == false)
                {
                    property.DeleteArrayElementAtIndex(j);
                }else
                {
                    if (propertyName.Equals("_MainTex") && NOTextureList.Contains(mat.shader.name))
                    {
                        property.DeleteArrayElementAtIndex(j);
                    }
                }
            }
        }

        public static void CheckPrefabRedundancy()
        {
            string[] files = Directory.GetFiles(Application.dataPath + tempFindFonder, "*.prefab", SearchOption.AllDirectories);

            foreach (var path in files)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                string assetPath = ResCheckEditorUtil.GetAssetPath(path);
                GameObject root = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                if (root)
                {
                    foreach (var item in root.GetComponentsInChildren<Transform>(true))
                    {
                        if (item.GetComponent<ParticleSystem>())
                        {
                            ParticleSystem particle = item.GetComponent<ParticleSystem>();
                            SerializedObject particlePS = new SerializedObject(particle);
                            SerializedProperty shapeModulePS = particlePS.FindProperty("ShapeModule");
                            //var shapeModuleActive = shapeModulePS.FindPropertyRelative("enabled").boolValue;
                            var shapeModuleType = shapeModulePS.FindPropertyRelative("type").intValue;
                            var shapeModuleMesh = shapeModulePS.FindPropertyRelative("m_Mesh").objectReferenceValue;
                            var shapeModuleMeshRenderer = shapeModulePS.FindPropertyRelative("m_MeshRenderer").objectReferenceValue;
                            var shapeModuleSkinMeshRenderer = shapeModulePS.FindPropertyRelative("m_SkinnedMeshRenderer").objectReferenceValue;

                            if (((shapeModuleType != 6 && shapeModuleMesh) ||
                                shapeModuleType != 13 && shapeModuleMeshRenderer ||
                                shapeModuleType != 14 && shapeModuleSkinMeshRenderer))
                            {
                                if (RedundancyObject.ContainsKey(root) == false)
                                {
                                    RedundancyObject[root] = new RedundancyData();
                                    RedundancyObject[root].rootPath = assetPath;
                                }
                                //Debug.Log("====   " + item.gameObject);
                                RedundancyObject[root].subObj.Add(item.gameObject);
                            }
                        }

                        if (item.GetComponent<ParticleSystemRenderer>())
                        {
                            ParticleSystemRenderer particleRenderer = item.GetComponent<ParticleSystemRenderer>();
                            SerializedObject particleRendererPS = new SerializedObject(particleRenderer);
                            //var particleRendererActive = particleRendererPS.FindProperty("m_Enabled").boolValue;
                            var particleRendererMode = particleRendererPS.FindProperty("m_RenderMode").intValue;
                            var particleRendererMesh = particleRendererPS.FindProperty("m_Mesh").objectReferenceValue;
                            var particleRendererMesh1 = particleRendererPS.FindProperty("m_Mesh1").objectReferenceValue;
                            var particleRendererMesh2 = particleRendererPS.FindProperty("m_Mesh2").objectReferenceValue;
                            var particleRendererMesh3 = particleRendererPS.FindProperty("m_Mesh3").objectReferenceValue;

                            
                            if (particleRendererMode != 4 && (particleRendererMesh || particleRendererMesh1 || particleRendererMesh2 || particleRendererMesh3)  )
                            {
                                if (RedundancyObject.ContainsKey(root) == false)
                                {
                                    RedundancyObject[root] = new RedundancyData();
                                    RedundancyObject[root].rootPath = assetPath;
                                }
                                RedundancyObject[root].subObj.Add(item.gameObject);
                                //Debug.Log("====   " + item.gameObject);
                            }
                        }
                    }
                }
            }
        }

        static void DeleteObjRedundancy(GameObject prefab)
        {
            if (prefab)
            {
                GameObject go = Instantiate(prefab);
                foreach (var o in go.GetComponentsInChildren<Transform>(true))
                {
                    if (o.GetComponent<ParticleSystem>())
                    {
                        ParticleSystem particle = o.GetComponent<ParticleSystem>();
                        SerializedObject particlePS = new SerializedObject(particle);
                        SerializedProperty shapeModulePS = particlePS.FindProperty("ShapeModule");
 
                        var shapeModuleType = shapeModulePS.FindPropertyRelative("type").intValue;
                        var shapeModuleMesh = shapeModulePS.FindPropertyRelative("m_Mesh").objectReferenceValue;
                        var shapeModuleMeshRenderer = shapeModulePS.FindPropertyRelative("m_MeshRenderer").objectReferenceValue;
                        var shapeModuleSkinMeshRenderer = shapeModulePS.FindPropertyRelative("m_SkinnedMeshRenderer").objectReferenceValue;
                      
                        if (shapeModuleType != 6 && shapeModuleMesh)
                        {
                            shapeModulePS.FindPropertyRelative("m_Mesh").DeleteCommand();
                        }
                        if (shapeModuleType != 13 && shapeModuleMeshRenderer)
                        {
                            shapeModulePS.FindPropertyRelative("m_MeshRenderer").DeleteCommand();
                        }
                        if (shapeModuleType != 14 && shapeModuleSkinMeshRenderer)
                        {
                            shapeModulePS.FindPropertyRelative("m_SkinnedMeshRenderer").DeleteCommand();
                        }
                        
                        particlePS.ApplyModifiedProperties();

                    }
                    if (o.GetComponent<ParticleSystemRenderer>())
                    {
                        ParticleSystemRenderer particleRenderer = o.GetComponent<ParticleSystemRenderer>();
                        SerializedObject particleRendererPS = new SerializedObject(particleRenderer);
                        //var particleRendererActive = particleRendererPS.FindProperty("m_Enabled").boolValue;
                        var particleRendererMode = particleRendererPS.FindProperty("m_RenderMode").intValue;

                        if ( particleRendererMode != 4)
                        {
                            particleRendererPS.FindProperty("m_Mesh").DeleteCommand();
                            particleRendererPS.FindProperty("m_Mesh1").DeleteCommand();
                            particleRendererPS.FindProperty("m_Mesh2").DeleteCommand();
                            particleRendererPS.FindProperty("m_Mesh3").DeleteCommand();
                        }
                        particleRendererPS.ApplyModifiedProperties();
                    }
                }
                PrefabUtility.SaveAsPrefabAsset(go, ResCheckJenkinsEntrance.GetAssetPath(prefab));
            }
        }

        private static Vector3 scrollPos = Vector3.zero;
        private void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            Material removemat = null;
            foreach (var item in RedundancyMaterials)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item, typeof(Object), true, GUILayout.Width(400));
                if (GUILayout.Button("清除冗余", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    removemat = item;
                }
                EditorGUILayout.EndHorizontal();
            }
            GameObject removeObj = null;
           
            foreach (var item in RedundancyObject)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, typeof(Object), true, GUILayout.Width(200));
                RedundancyData data = item.Value;
                EditorGUILayout.BeginVertical();
                foreach (var subItem in data.subObj)
                {
                    EditorGUILayout.ObjectField(subItem, typeof(Object), true, GUILayout.Width(200));
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("清除冗余", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    removeObj = item.Key;
                }
                EditorGUILayout.EndVertical();

            }
            
            if (removemat != null)
            {
                RedundancyMaterials.Remove(removemat);
                DeleteMatRedundancy(removemat);
            }
            if(removeObj != null)
            {
                RedundancyObject.Remove(removeObj);
                DeleteObjRedundancy(removeObj);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.EndScrollView();
        }
    }
}
