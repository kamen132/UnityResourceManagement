
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class GetRoleInfo : ResCheckBaseSubWindowEditor
    {
        private class RoleInfo
        {
            public float textureTotleSize = 0;
            
            public Dictionary<Texture, float> textures = new Dictionary<Texture, float>();
            public Dictionary<GameObject, RolePrefabInfo> rolePrefabInfos = new Dictionary<GameObject, RolePrefabInfo>();

            public bool isShowTImeLineInfo = false;
            public Dictionary<Object, TimeLineInfo> timeLineInfos = new Dictionary<Object, TimeLineInfo>();
            public List<Texture2D> textureList = new List<Texture2D>();
        }
        private class RolePrefabInfo
        {
            public int bones = 0;
            public int vertexCount = 0;
            public int trisCount = 0;
            public float textureTotleSize = 0;
            public Dictionary<Texture2D, float> textures = new Dictionary<Texture2D, float>();
            public List<Texture2D> textureList = new List<Texture2D>();
        }
        private class TimeLineInfo
        {
            public float textureTotleSize = 0;
            public Dictionary<Texture2D, float> textures = new Dictionary<Texture2D, float>();
            public List<Texture2D> textureList = new List<Texture2D>();
        }
        private Dictionary<string, RoleInfo> findResult = new Dictionary<string, RoleInfo>();
        private List<string> findResultList = new List<string>();

        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("查找角色信息", GUILayout.Width(400), GUILayout.Height(20)))
            {
                findResult.Clear();
                findResultList.Clear();
                FindPrefab();
                EditorUtility.ClearProgressBar();
            }
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }
        
        void FindPrefab()
        {
            string directoryPath = Application.dataPath + "/Product/Editor/Resources/Role";
            string[]  dirs = Directory.GetDirectories(directoryPath);
            foreach(string dir in dirs)
            {
                string fonderName = PathUtil.GetFolderName(dir);
                if (fonderName.Contains("Common"))
                {
                    continue;
                }
                string[] subDirs = Directory.GetDirectories(dir);
                foreach (string rolePath in subDirs)
                {
                    string roleName = PathUtil.GetFolderName(rolePath);
                    RoleInfo roleInfo = new RoleInfo();
                    string[] prefabPaths = Directory.GetFiles(rolePath, "*.prefab", SearchOption.AllDirectories);
                    if (prefabPaths.Length == 0)//里面没有prefab
                    {
                        continue;
                    }
                    foreach (string prefabPath in prefabPaths)
                    {
                        DelOnePrefab(prefabPath, roleInfo);
                    }
                    findResult[roleName] = roleInfo;
                    findResultList.Add(roleName);
                }
            }
            findResultList.Sort((a, b) => findResult[b].textureTotleSize.CompareTo(findResult[a].textureTotleSize));
            foreach (var item in findResult)
            {
                RoleInfo roleInfo = item.Value;
                roleInfo.textureList.Sort((a, b) => roleInfo.textures[b].CompareTo(roleInfo.textures[a]));
                foreach (var item2 in roleInfo.rolePrefabInfos)
                {
                    var info = item2.Value;
                    info.textureList.Sort((a, b) => info.textures[b].CompareTo(info.textures[a]));
                }
                foreach (var item2 in roleInfo.timeLineInfos)
                {
                    var info = item2.Value;
                    info.textureList.Sort((a, b) => info.textures[b].CompareTo(info.textures[a]));
                }
            }
        }
        void DelOnePrefab(string prefabPath, RoleInfo roleInfo)
        {
            string prefabName = PathUtil.GetFileNameWithoutExtension(prefabPath);
           
            
            string prefabAssetPath = PathUtil.GetAssetPath(prefabPath);

            string title = string.Format("加载prefab {0} 中", Path.GetFileName(prefabAssetPath));
            EditorUtility.DisplayProgressBar(title, Path.GetFileName(prefabAssetPath), 1);
            if (prefabName.Trim().ToLower().Contains("timeline"))
            {
                TimeLineInfo timeLineInfo = new TimeLineInfo();
                GetTImeLineData(prefabAssetPath, timeLineInfo, roleInfo);
                Object prefab = ResCheckJenkinsEntrance.LoadAssetAtPath(prefabAssetPath);
                roleInfo.timeLineInfos[prefab] = timeLineInfo;
            }
            else
            {
                RolePrefabInfo rolePrefabInfo = new RolePrefabInfo();
                GameObject prefab = ResCheckJenkinsEntrance.LoadAssetAtPath(prefabAssetPath) as GameObject;
                GetPrefabMeshData(prefab, rolePrefabInfo);
                GetPrefabTexData(prefabAssetPath, rolePrefabInfo, roleInfo);
                GetPrefabBonesData(prefab, rolePrefabInfo);
                roleInfo.rolePrefabInfos[prefab] = rolePrefabInfo;
            }
        }
        void GetTImeLineData(string prefabAssetPath, TimeLineInfo timeLineInfo, RoleInfo roleInfo)
        {
            string[] prefabDeps = ResCheckJenkinsEntrance.GetDependencies(prefabAssetPath);
            foreach (string dep in prefabDeps)
            {
                string depAssetPath = PathUtil.GetAssetPath(dep);
                if (ResCheckEditorUtil.IsTexture2D(depAssetPath))
                {
                    Texture2D tex = ResCheckJenkinsEntrance.LoadAssetAtPath(depAssetPath) as Texture2D;
                    Object tex2 = ResCheckJenkinsEntrance.LoadAssetAtPath(depAssetPath);
                    if (tex != null)
                    {
                        float value = ResCheckEditorUtil.CalTextureSize(tex);
                        if (timeLineInfo.textures.ContainsKey(tex) == false)
                        {
                            timeLineInfo.textureTotleSize += value;
                            timeLineInfo.textures[tex] = value;
                            timeLineInfo.textureList.Add(tex);
                        }
                        if (roleInfo.textures.ContainsKey(tex) == false)
                        {
                            roleInfo.textureTotleSize += value;
                            roleInfo.textures[tex] = value;
                            roleInfo.textureList.Add(tex);
                        }
                    }
                }
            }
        }
        void GetPrefabTexData(string prefabAssetPath, RolePrefabInfo rolePrefabInfo, RoleInfo roleInfo)
        {
            string[] prefabDeps = ResCheckJenkinsEntrance.GetDependencies(prefabAssetPath);
            foreach (string dep in prefabDeps)
            {
                string depAssetPath = PathUtil.GetAssetPath(dep);
                if (ResCheckEditorUtil.IsTexture2D(depAssetPath))
                {
                    Texture2D tex = ResCheckJenkinsEntrance.LoadAssetAtPath(depAssetPath) as Texture2D;
                    Object tex2 = ResCheckJenkinsEntrance.LoadAssetAtPath(depAssetPath);
                    if(tex != null)
                    {
                        float value = ResCheckEditorUtil.CalTextureSize(tex);
                        if (rolePrefabInfo.textures.ContainsKey(tex) == false)
                        {
                            rolePrefabInfo.textureTotleSize += value;
                            rolePrefabInfo.textures[tex] = value;
                            rolePrefabInfo.textureList.Add(tex);
                        }
                        if (roleInfo.textures.ContainsKey(tex) == false)
                        {
                            roleInfo.textureTotleSize += value;
                            roleInfo.textures[tex] = value;
                            roleInfo.textureList.Add(tex);
                        }
                    }
                }
            }
        }
      
        void GetPrefabBonesData(GameObject prefab, RolePrefabInfo rolePrefabInfo)
        {
            Transform RootBones = prefab.transform.Find("root/Bip001");
            rolePrefabInfo .bones = ResCheckEditorUtil.GetTransformChildrenNum(RootBones);
        }
        void GetPrefabMeshData(GameObject prefab, RolePrefabInfo rolePrefabInfo)
        {
            SkinnedMeshRenderer[] renders = prefab.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (SkinnedMeshRenderer render in renders)
            {
                if (render.sharedMesh == null)
                {
                    Debug.Log("miss mesh: " + prefab.name + "   __   " + render.gameObject.name);
                    continue;
                }
                int vertexCount = render.sharedMesh.vertexCount;

                if (vertexCount == 0)
                {
                    Debug.Log("empty mesh: " + prefab.name + "   __   " + render.gameObject.name);
                    continue;
                }
                rolePrefabInfo.vertexCount += vertexCount;
                rolePrefabInfo.trisCount += render.sharedMesh.triangles.Length / 3;
            }
        }
        private Vector3 scrollPos = Vector3.zero;
        public void ShowFindResult()
        {
            EditorGUILayout.BeginHorizontal();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.Width(900), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            EditorGUILayout.BeginVertical();
            int len = 70;
            foreach (var name in findResultList)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(name, GUILayout.Width(100));
                RoleInfo roleInfo = findResult[name];
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(roleInfo.textureTotleSize), GUILayout.Width(100));
                EditorGUILayout.EndHorizontal();

                foreach(var rolePrefabInfo in roleInfo.rolePrefabInfos)
                {
                    EditorGUILayout.BeginHorizontal();
                    GameObject obj = rolePrefabInfo.Key;
                    RolePrefabInfo prefabInfo = rolePrefabInfo.Value;
                    EditorGUILayout.ObjectField(obj, obj.GetType(), true, GUILayout.Width(300));
                    float width = 90;
                    EditorGUILayout.LabelField("bones:" + ResCheckEditorUtil.HumanReadableNum(prefabInfo.bones), GUILayout.Width(width));
                    EditorGUILayout.LabelField("vertex:" + ResCheckEditorUtil.HumanReadableNum(prefabInfo.vertexCount), GUILayout.Width(width));
                    EditorGUILayout.LabelField("tris:" + ResCheckEditorUtil.HumanReadableNum(prefabInfo.trisCount), GUILayout.Width(width));
                    EditorGUILayout.LabelField("texSize:" + ResCheckEditorUtil.HumanReadableFilesize(prefabInfo.textureTotleSize), GUILayout.Width(width));

                    
                    if (selectObj != null && selectObj == obj)
                    {
                        len = 100;
                    }else
                    {
                        len = 70;
                    }
                    if (GUILayout.Button("查看", GUILayout.Width(len), GUILayout.Height(20)))
                    {
                        lookPrefab = true;
                        selectObj = obj;
                        selectName = name;
                    }
                    //public Dictionary<Texture, bool> textures = new Dictionary<Texture, bool>();
                    EditorGUILayout.EndHorizontal();
                }
              
                
                if(roleInfo.isShowTImeLineInfo)
                {
                    if (GUILayout.Button("HideTimeLineInfo", GUILayout.Width(400), GUILayout.Height(20)))
                    {
                        roleInfo.isShowTImeLineInfo = false;
                    }
                    EditorGUILayout.LabelField("---------------------------------timelines-----------------------------------------------");
                    foreach (var timeLineInfo in roleInfo.timeLineInfos)
                    {
                        EditorGUILayout.BeginHorizontal();
                        TimeLineInfo data = timeLineInfo.Value;
                        EditorGUILayout.ObjectField(timeLineInfo.Key, timeLineInfo.Key.GetType(), true, GUILayout.Width(300));
                        EditorGUILayout.LabelField("texSize:" + ResCheckEditorUtil.HumanReadableFilesize(data.textureTotleSize), GUILayout.Width(200));
                        if (selectObj != null && selectObj == timeLineInfo.Key)
                        {
                            len = 100;
                        }
                        else
                        {
                            len = 70;
                        }
                        if (GUILayout.Button("查看", GUILayout.Width(len), GUILayout.Height(20)))
                        {
                            lookPrefab = false;
                            selectObj = timeLineInfo.Key;
                            selectName = name;
                        }

                        EditorGUILayout.EndHorizontal();
                    }
                }else
                {
                    if (GUILayout.Button("ShowTimeLineInfo", GUILayout.Width(400), GUILayout.Height(20)))
                    {
                        roleInfo.isShowTImeLineInfo = true;
                    }
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.EndScrollView();
            ShowFindPrefabDataItem();
            EditorGUILayout.EndHorizontal();
        }
        private Object selectObj;
        private string selectName;
        private Vector3 scrollPos2 = Vector3.zero;
        private bool lookPrefab = true;
        private void ShowFindPrefabDataItem()
        {
            if (selectObj == null)
            {
                return;
            }
            if(findResult.ContainsKey(selectName) == false)
            {
                return;
            }
            scrollPos2 = EditorGUILayout.BeginScrollView(scrollPos2, false, true, GUILayout.Width(650), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            RoleInfo roleInfo = findResult[selectName];
            if (lookPrefab)
            {
                RolePrefabInfo rolePrefabInfo = roleInfo.rolePrefabInfos[selectObj as GameObject];
 
                EditorGUILayout.BeginVertical();
                EditorGUILayout.ObjectField(selectObj, selectObj.GetType(), true, GUILayout.Width(300));
                EditorGUILayout.LabelField("贴图数量：" + rolePrefabInfo.textures.Count);
                float width = 90;
                foreach(var item in rolePrefabInfo.textureList)
                {
                    Texture2D tex = item;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(tex, item.GetType(), true, GUILayout.Width(250));
                    
                    EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(rolePrefabInfo.textures[tex]), GUILayout.Width(width));
                    EditorGUILayout.LabelField(tex.width + "x" + tex.width, GUILayout.Width(width));
                    EditorGUILayout.LabelField(tex.format.ToString(), GUILayout.Width(width));
                    EditorGUILayout.EndHorizontal();
                }
              
            }else
            {
                TimeLineInfo timeLineInfo = roleInfo.timeLineInfos[selectObj];

                EditorGUILayout.ObjectField(selectObj, selectObj.GetType(), true, GUILayout.Width(300));
                EditorGUILayout.LabelField("贴图数量：" + timeLineInfo.textures.Count);
                float width = 90;
                foreach (var item in timeLineInfo.textureList)
                {
                    Texture2D tex = item;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(tex, item.GetType(), true, GUILayout.Width(250));

                    EditorGUILayout.LabelField(ResCheckEditorUtil.HumanReadableFilesize(timeLineInfo.textures[tex]), GUILayout.Width(width));
                    EditorGUILayout.LabelField(tex.width + "x" + tex.width, GUILayout.Width(width));
                    EditorGUILayout.LabelField(tex.format.ToString(), GUILayout.Width(width));
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }
    }
}
