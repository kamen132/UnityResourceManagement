
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

//1、找到Unity所有预设体和材质资源 
//2、拿到这些资源的依赖文件 
//3、找到所有 依赖文件里的 Texture Shader Material Sprite
//3.1、 拿到依赖文件里的预设体 加载出来 遍历组件 
//3.2、 拿到依赖文件里的Material 加载出来 查看shader 和贴图
//3.5 、 Texture 可能在RawImage组件里 可能在Material组件里 
//3.6 、Shader只可能在Materai里 
//3.7 、Sprite在Image组件里 
//4、AssetDatabase.GetAssetPath(Object) 拿到这个资源的路径
//5、如果这个路径包含“builtin” 说明这个资源是unity内置的
//--------------------- 
namespace GMResChecker
{
    public class FindBuildinResources : ResCheckBaseSubWindowEditor
    {
        private enum SelectState
        {
            Shader = 0,
            Texture,
            Mat,
            MissingShader
        }
        private static Vector2 scrollPos = Vector2.zero;
        private static Dictionary<Object, Dictionary<Object, Object>> missingShaderDic = new Dictionary<Object, Dictionary<Object, Object>>();
        private static Dictionary<Object, Dictionary<Object, Object>> shaderDic = new Dictionary<Object, Dictionary<Object, Object>>();
        private static Dictionary<Object, Dictionary<Object, Object>> texDic = new Dictionary<Object, Dictionary<Object, Object>>();
        private static Dictionary<Object, Dictionary<Object, Object>> matDic = new Dictionary<Object, Dictionary<Object, Object>>();

        private const string builtin = "builtin";
        private const string builtinUniversal = "universal";
        private const string tempmat = "tempmat";
        private const string MissingShader = "Library/unity default resources";
        
        private static string nowSelect = null;
        private static SelectState selectState = SelectState.Shader;
        private static bool isInit = false;
        private static Dictionary<string, bool> GetNeedCheckPrefabData()
        {
            Dictionary<string, bool> allfiles = ResCheckJenkinsEntrance.GetHaveDepsFile();
            Dictionary<string, bool> totals = new Dictionary<string, bool>();

            foreach (var item in allfiles)
            {
                var assetPath = ResCheckEditorUtil.GetAssetPath(item.Key);
                var deps = ResCheckJenkinsEntrance.GetDependencies(assetPath, true);
                totals[assetPath] = true;
                foreach (var dep in deps)
                {
                    if (ResCheckEditorUtil.IsPrefab(dep))
                    {
                        if (!totals.ContainsKey(dep))
                        {
                            totals.Add(dep, true);
                        }
                    }
                }
            }
            return totals;
        }
        public override void OnGUIDraw()
        {
            GUILayout.Space(5);
            float width = 200;
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Shader", GUILayout.Width(width), GUILayout.Height(20)))
            {
                GetBuildinResource();
                selectState = SelectState.Shader;
            }
            if (GUILayout.Button("Texture", GUILayout.Width(width), GUILayout.Height(20)))
            {
                GetBuildinResource();
                selectState = SelectState.Texture;
            }
            if (GUILayout.Button("Mat", GUILayout.Width(width), GUILayout.Height(20)))
            {
                GetBuildinResource();
                selectState = SelectState.Mat;
            }
            if (GUILayout.Button("MissingShader", GUILayout.Width(width), GUILayout.Height(20)))
            {
                GetBuildinResource();
                selectState = SelectState.MissingShader;
            }
            if (GUILayout.Button("Reset", GUILayout.Width(width), GUILayout.Height(20)))
            {
                isInit = false;
                texDic.Clear();
                shaderDic.Clear();
                matDic.Clear();
            }
          
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.LabelField("======================================================");
            if (GUILayout.Button("ExportExcle", GUILayout.Width(100), GUILayout.Height(20)))
            {
                ExportExcle();
            }
            EditorGUILayout.LabelField("======================================================");
            ShowFindResult();

        }
        private static void ExportExcle()
        {
            Dictionary<Object, Dictionary<Object, Object>> data = null;
            if (selectState == SelectState.Shader)
            {
                data = shaderDic;
            }
            else if (selectState == SelectState.Mat)
            {
                data = matDic;
            }
            else if (selectState == SelectState.Texture)
            {
                data = texDic;
            }
            else if (selectState == SelectState.MissingShader)
            {
                data = missingShaderDic;
            }
            string __selectedPath = EditorUtility.OpenFolderPanel("请选择要保存的文件夹", Application.dataPath + "/../", "");
            List<string> title = new List<string>();
            title.Add("ObjectName");
            title.Add("ObjectName");
            title.Add("ObjectName");
            List<List<string>> result = new List<List<string>>();
            foreach (var item in data)
            {
                string path = ResCheckJenkinsEntrance.GetAssetPath(item.Key);
                foreach (var subItem2 in item.Value)
                {
                    List<string> itemData = new List<string>();
                    itemData.Add(path);
                    itemData.Add(subItem2.Key.name);
                    itemData.Add(subItem2.Value.name);
                    result.Add(itemData);
                }
            }
            ResCheckJenkinsEntrance.ExportExcle(__selectedPath, "BuildInData_" + selectState, "sheet", title, result);
        }
        private static void GetBuildinResource()
        {
            if(isInit)
            {
                return;
            }
            isInit = true;
            GetPrefabData();
            GetMatData();
        }

        public static string GetBuildInResources()
        {
            GetPrefabData();
            GetMatData();
            StringBuilder builder = new StringBuilder();

            builder.Append("=========find BuildIn Result:" + shaderDic.Count + "=========\n");

            foreach (var item in shaderDic)
            {
                string key = item.Key.name;
                Dictionary<Object, Object> value = item.Value;
                foreach (var valueItem in value)
                {
                    builder.Append(key);
                    builder.Append("==");
                    string path = ResCheckJenkinsEntrance.GetAssetPath(valueItem.Value);
                    builder.Append(path);
                    builder.Append("\n");
                }
            }
            builder.Append("\n\n");
            return builder.ToString();
        }

        private static void GetPrefabData()
        {
            texDic.Clear();
            shaderDic.Clear();
            matDic.Clear();
            Dictionary<string, bool> totals = GetNeedCheckPrefabData();
            var index = 0;
            foreach (var it in totals)
            {
                string assetPath = it.Key;
                EditorUtility.DisplayProgressBar("加载中", Path.GetFileName(assetPath), (float)index++ / totals.Count);
                if (ResCheckEditorUtil.IsPrefab(assetPath) == false || ResCheckEditorUtil.IsTImeline(assetPath))
                {
                    continue;
                }
               
                GameObject go = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as GameObject;
                if (go)
                {
                    Renderer[] renders = go.GetComponentsInChildren<Renderer>(true);
                    foreach (var render in renders)
                    {
                        var mats = new List<Material>(render.sharedMaterials);
                        foreach (var mat in render.sharedMaterials)
                        {
                            if (!mat) continue;
                            //判断材质是不是用的builtin的
                            if (ResCheckJenkinsEntrance.GetAssetPath(mat).Contains(builtin))
                            {
                                if (matDic.ContainsKey(go) == false)
                                {
                                    matDic[go] = new Dictionary<Object, Object>();
                                }
                                matDic[go][render.gameObject] = mat;
                            }
                            //判断shader是不是builtin的
                            string shaderPath = ResCheckJenkinsEntrance.GetAssetPath(mat.shader);
                            //if(shaderPath.Contains("Editor/Resources") == false)
                            //{
                            //    Debug.Log("shaderPath==" + shaderPath + "  go===" + go + "===" + render.gameObject);
                            //}
                           
                            if (IsBuildInShader(shaderPath))
                            {
                                if (shaderDic.ContainsKey(go) == false)
                                {
                                    shaderDic[go] = new Dictionary<Object, Object>();
                                }
                                shaderDic[go][render.gameObject] = mat.shader;
                            }
                            if (shaderPath.Contains(MissingShader) && mat.name.Equals("tmpmat") == false)
                            {
                                if (missingShaderDic.ContainsKey(go) == false)
                                {
                                    missingShaderDic[go] = new Dictionary<Object, Object>();
                                }
                                missingShaderDic[go][render.gameObject] = mat.shader;
                            }
                            //判断shader用的贴图是不是用的builtin的
                            for (int i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
                            {
                                if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                                {
                                    string propertyname = ShaderUtil.GetPropertyName(mat.shader, i);
                                    Texture t = mat.GetTexture(propertyname);
                                    if (t && ResCheckJenkinsEntrance.GetAssetPath(t).Contains(builtin))
                                    {
                                        if (texDic.ContainsKey(go) == false)
                                        {
                                            texDic[go] = new Dictionary<Object, Object>();
                                        }
                                        texDic[go][render.gameObject] = t;
                                    }
                                }
                            }
                        }
                    }

                    Image[] images = go.GetComponentsInChildren<Image>(true);
                    foreach (var img in images)
                    {
                        if (ResCheckJenkinsEntrance.GetAssetPath(img.sprite).Contains(builtin))
                        {
                            if (texDic.ContainsKey(go) == false)
                            {
                                texDic[go] = new Dictionary<Object, Object>();
                            }
                            texDic[go][img.gameObject] = img.sprite;
                        }
                    }

                    RawImage[] rawimgs = go.GetComponentsInChildren<RawImage>(true);
                    foreach (var rawimg in rawimgs)
                    {
                        if (rawimg.texture && ResCheckJenkinsEntrance.GetAssetPath(rawimg.texture).Contains(builtin))
                        {
                            if (texDic.ContainsKey(go) == false)
                            {
                                texDic[go] = new Dictionary<Object, Object>();
                            }
                            texDic[go][rawimg.gameObject] = rawimg.texture;
                        }
                    }
                }
                else
                {
                    Debug.LogError("nil obj assetPath==" + assetPath);
                }
                
            }
            EditorUtility.ClearProgressBar();
        }
        private static void GetMatData()
        {
            Dictionary<string, bool> totals = GetNeedCheckPrefabData();
            foreach (var item in totals)
            {
                string assetPath = item.Key;
                if (ResCheckEditorUtil.IsPrefab(assetPath))
                {
                    continue;
                }
                assetPath = ResCheckEditorUtil.GetAssetPath(assetPath);
                Material mat = ResCheckJenkinsEntrance.LoadAssetAtPath(assetPath) as Material;
                if (!mat) continue;
                string shaderPath = ResCheckJenkinsEntrance.GetAssetPath(mat.shader);
                if (IsBuildInShader(shaderPath))
                {
                    if (shaderDic.ContainsKey(mat) == false)
                    {
                        shaderDic[mat] = new Dictionary<Object, Object>();
                    }
                    shaderDic[mat][mat] = mat.shader;
                }
                if (shaderPath.Contains(MissingShader) && mat.name.Equals("tempmat") == false)
                {
                    if (missingShaderDic.ContainsKey(mat) == false)
                    {
                        missingShaderDic[mat] = new Dictionary<Object, Object>();
                    }
                    missingShaderDic[mat][mat] = mat.shader;
                }
                for (int i = 0; i < ShaderUtil.GetPropertyCount(mat.shader); i++)
                {
                    if (ShaderUtil.GetPropertyType(mat.shader, i) == ShaderUtil.ShaderPropertyType.TexEnv)
                    {
                        string propertyname = ShaderUtil.GetPropertyName(mat.shader, i);
                        Texture tex = mat.GetTexture(propertyname);
                        string texPath = ResCheckJenkinsEntrance.GetAssetPath(tex);
                        if (tex && texPath.Contains(builtin))
                        {
                            if (texDic.ContainsKey(mat) == false)
                            {
                                texDic[mat] = new Dictionary<Object, Object>();
                            }
                            texDic[mat][mat.shader] = tex;
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        void ReplaceShader()
        {
            if (false)
            //if (nowSelect.Equals(name))
            {
                //if (GUILayout.Button("将此buildInShader替换成本地shader", GUILayout.Width(300), GUILayout.Height(20)))
                //{
                //    Dictionary<UnityEngine.Object, int> datas = _FindBuildInShaders[nowSelect];
                //    foreach (var needChangeItem in datas)
                //    {
                //        TransforNode(res[needChangeItem.Key], (s) =>
                //        {
                //        //Debug.Log("s.des===" + s.des);
                //        if (s.des == material)
                //            {
                //                Material mt = s.content as Material;
                //                if (mt)
                //                {
                //                    string shaderPath = ResCheckJenkinsEntrance.GetAssetPath(mt.shader);
                //                    if (shaderPath.Contains(builtin))
                //                    {
                //                        mt.shader = Shader.Find(mt.shader.name);
                //                        shaderPath = ResCheckJenkinsEntrance.GetAssetPath(mt.shader);
                //                    }

                //                }
                //            }
                //        });
                //    }
                //    AssetDatabase.SaveAssets();
                //    AssetDatabase.Refresh();
                //}
            }
        }
        private static void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(800), GUILayout.Height(600));
            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------find result------------------------------------------");
            Dictionary<Object, Dictionary<Object, Object>> data = null;
            if (selectState == SelectState.Shader)
            {
                data = shaderDic;
            }
            else if (selectState == SelectState.Mat)
            {
                data = matDic;
            }
            else if (selectState == SelectState.Texture)
            {
                data = texDic;
            }
            else if (selectState == SelectState.MissingShader)
            {
                data = missingShaderDic;
            }
            foreach (var item in data)
            {
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Key, item.Key.GetType(), true, GUILayout.Width(250));

                EditorGUILayout.BeginVertical();
                foreach (var item2 in item.Value)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.ObjectField(item2.Key, item2.Key.GetType(), true, GUILayout.Width(250));
                    EditorGUILayout.ObjectField(item2.Value, item2.Value.GetType(), true, GUILayout.Width(250));
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }

            GUILayout.Space(5);
            EditorGUILayout.LabelField("--------------------------------------find result------------------------------------------");
            EditorGUILayout.EndScrollView();
        }
        
        static bool IsBuildInShader(string shaderPath)
        {
            return shaderPath.Contains(builtin)
                || shaderPath.Contains("ASEShaders")
                || shaderPath.Contains("UnityBulidIn")
                || shaderPath.Contains(builtinUniversal);
        }

        
    }
}
