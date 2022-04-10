using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GMResChecker
{
    public class FindUnUseRes : ResCheckBaseSubWindowEditor
    {
        private string patternDB = "(?<=\").*?(?=\")";
        private string patternSingle = "(?<=\').*?(?=\')";
        private bool isCheckLua = true;
        private static Dictionary<string, int> findLuaStrDic = new Dictionary<string, int>();
        private static Dictionary<string, string> useResDic = new Dictionary<string, string>();//用到的资源

        private static Dictionary<string, string> unUseResFirst = new Dictionary<string, string>();//第一梯度没用到的资源
        private static Dictionary<string, UnityEngine.Object> unUseResSecond = new Dictionary<string, UnityEngine.Object>();//第一梯度没用到的资源
        private static Dictionary<string, int> allDeps = new Dictionary<string, int>();
        private static Dictionary<string, string> atlasOfPngDeps = new Dictionary<string, string>();
        private static Dictionary<string, string> atlasDics = new Dictionary<string, string>();
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            EditorGUILayout.BeginHorizontal();
            isCheckLua = EditorGUILayout.ToggleLeft("是否检查lua引用", isCheckLua);
            if (GUILayout.Button("查找 无用的资源", GUILayout.Width(200), GUILayout.Height(20)))
            {
                unUseResFirst.Clear();
                findLuaStrDic.Clear();
                allDeps.Clear();
                unUseResSecond.Clear();
                atlasOfPngDeps.Clear();
                atlasDics.Clear();
                FindLuaFiles();
                FindAtlasDeps();
                FindAllRes();
                OnFindUnUseRes();
                FindUnUsePngAndAtlas();
                EditorUtility.ClearProgressBar();
            }
            if (GUILayout.Button("一键将prefab移到 backup里", GUILayout.Width(200), GUILayout.Height(20)))
            {
                List<string> removeList = new List<string>();
                foreach (var item in unUseResSecond)
                {
                    string path = ResCheckJenkinsEntrance.GetAssetPath(item.Value);
                    if (path.EndsWith(".prefab"))
                    {
                        removeList.Add(item.Key);
                    }
                }
                foreach (var item in removeList)
                {
                    MoveToBackUp(item);
                }
                AssetDatabase.Refresh();
            }
            if (GUILayout.Button("生成战斗缓存依赖", GUILayout.Width(200), GUILayout.Height(20)))
            {
                unUseResFirst.Clear();
                findLuaStrDic.Clear();
                atlasOfPngDeps.Clear();
                atlasDics.Clear();
                allDeps.Clear();
                FindLuaFiles();//查找lua文件中匹配被直接依赖的资源
                FindAllRes();//根据AB设置规则，获取到所有的资源的名字以及路径
                GetAllResDepDic();
                EditorUtility.ClearProgressBar();
            }
            if (GUILayout.Button("筛选选中文件的无用资源", GUILayout.Width(200), GUILayout.Height(20)))
            {
                unUseResSecond.Clear();
                onFindSelectedUnuseRes();
                EditorUtility.ClearProgressBar();
            }
            if (GUILayout.Button("筛选选中单个文件是否无用", GUILayout.Width(200), GUILayout.Height(20)))
            {
                unUseResSecond.Clear();
                onFindSelectedFileUnuseRes();
                EditorUtility.ClearProgressBar();
            }

            if (GUILayout.Button("输出文件的依赖", GUILayout.Width(200), GUILayout.Height(20)))
            {
                onFindSelectedFileDependices();
                EditorUtility.ClearProgressBar();
            }
            EditorGUILayout.EndHorizontal();
            ShowFindResult();
        }

        void DoSingleFile(string filePath)
        {
            string text = File.ReadAllText(filePath);
            var mc = Regex.Matches(text, patternDB);
            foreach (Match ma in mc)
            {
                findLuaStrDic[ma.Value] = 1;
            }
            mc = Regex.Matches(text, patternSingle);
            foreach (Match ma in mc)
            {
                findLuaStrDic[ma.Value] = 1;
            }
        }
        void FindLuaFiles()
        {
            string luaPath = Application.dataPath + "/StreamingAssets/lua";
            string[] pathNames = Directory.GetFiles(luaPath, "*.lua", SearchOption.AllDirectories);
            for (int i = 0; i < pathNames.Length; i++)
            {
                float per = (float)i / pathNames.Length;
                EditorUtility.DisplayProgressBar("加载lua中", ((int)(per * 100)).ToString() + "%", per);
                DoSingleFile(pathNames[i]);
            }
        }

        void FindAtlasDeps()
        {
            //atlasDeps
            string atlasPath = Application.dataPath + "/WorkAssets/ui";
            string[] pathNames = Directory.GetFiles(atlasPath, "*.spriteatlas", SearchOption.AllDirectories);
            for (int i = 0; i < pathNames.Length; i++)
            {
                float per = (float)i / pathNames.Length;
                EditorUtility.DisplayProgressBar("加载atlas中", ((int)(per * 100)).ToString() + "%", per);
                string path = pathNames[i];
                string p = ResCheckEditorUtil.GetAssetPath(path);
                string atlasName = Path.GetFileNameWithoutExtension(path);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(p, true);
                foreach (var dep in deps)
                {
                    if (Path.GetFileNameWithoutExtension(dep).Equals(atlasName))
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(ResCheckEditorUtil.GetFileExtension(dep)))//图集会引用文件夹
                    {
                        continue;
                    }
                    string depPath = ResCheckEditorUtil.GetAssetPath(dep);
                    atlasOfPngDeps[depPath] = atlasName;
                }
                atlasDics[atlasName] = p;
            }
        }
        void FindAllRes()
        {
            Dictionary<string, bool> result = new Dictionary<string, bool>();
            foreach (var item in result)
            {
                string path = item.Key;
                string name = Path.GetFileNameWithoutExtension(path);
                if (path.EndsWith(".spriteatlas"))//过滤掉图集
                {
                    continue;
                }

                if (isCheckLua && findLuaStrDic.ContainsKey(name) == false && IsRealyBeUse(path) == false)
                {
                    unUseResFirst[name] = path;
                }
                else
                {
                    useResDic[name] = path;
                }
            }
            Debug.Log("unUseResFirst===" + unUseResFirst.Count);
        }

        void GetAllResDepDic()
        {
            int index = 0;
            int totleIndex = useResDic.Count;
            foreach (var item in useResDic)
            {
                float per = (float)index / totleIndex;
                index++;
                EditorUtility.DisplayProgressBar("加载依赖中", ((int)(per * 100)).ToString() + "%", per);

                string p = ResCheckEditorUtil.GetAssetPath(item.Value);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(p, true);
                foreach (var dep in deps)
                {
                    allDeps[Path.GetFileNameWithoutExtension(dep)] = 1;//获得当前有用的所有依赖，其中一定包括uiprefab
                }
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "生成依赖缓存完成", "OK");
        }

        bool IsRealyBeUse(string path)
        {
            path = ResCheckEditorUtil.FormatPath(path);
            if (path.Contains("shader_always_in"))
            {
                return true;
            }
            string endSuffix = "*.mat";
            List<string> directoryList = ResCheckJenkinsEntrance.NeedSinglePackDirectorys[endSuffix];
            foreach (var item in directoryList)
            {
                string newItem = ResCheckEditorUtil.FormatPath(item);
                if (path.Contains(newItem))
                {
                    return true;
                }
            }
            return false;
        }
        void OnFindUnUseRes()
        {
            int index = 0;
            int totleIndex = useResDic.Count;
            foreach (var item in useResDic)
            {
                float per = (float)index / totleIndex;
                index++;
                EditorUtility.DisplayProgressBar("加载依赖中", ((int)(per * 100)).ToString() + "%", per);

                string p = ResCheckEditorUtil.GetAssetPath(item.Value);
                string[] deps = ResCheckJenkinsEntrance.GetDependencies(p, true);
                foreach (var dep in deps)
                {
                    allDeps[Path.GetFileNameWithoutExtension(dep)] = 1;//获得当前有用的所有依赖，其中一定包括uiprefab
                }
            }
            index = 0;
            totleIndex = unUseResFirst.Count;
            foreach (var item in unUseResFirst)//查找所有的依赖文件中，是否有用到，那些没有用的资源
            {
                float per = (float)index / totleIndex;
                index++;
                EditorUtility.DisplayProgressBar("检查无用资源中", ((int)(per * 100)).ToString() + "%", per);
                if (allDeps.ContainsKey(item.Key) == false)
                {
                    unUseResSecond[item.Key] = ResCheckJenkinsEntrance.LoadAssetAtPath(ResCheckEditorUtil.GetAssetPath(item.Value));
                }
            }
            Debug.Log("unUseResSecond===" + unUseResFirst.Count);
        }

        void onFindSelectedUnuseRes()
        {
            EditorUtility.DisplayProgressBar("提示", "开始执行无用资源的查询", 0);
            UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.DeepAssets);
            if (arr.Length == 0)
            {
                EditorUtility.DisplayDialog("提示", "请选中需要查询的文件夹", "OK");
                return;
            }

            for (int index = 0; index < arr.Length; index++)
            {
                var obj = arr[index];
                if (obj.GetType() == typeof(UnityEngine.GameObject))
                {
                    EditorUtility.DisplayProgressBar("提示", "检测是否被依赖 " + obj.name, (float)index / (float)arr.Length);
                    if (isCheckLua)
                    {
                        if (findLuaStrDic.ContainsKey(obj.name) == false)
                        {
                            unUseResSecond[obj.name] = obj;
                        }
                    }
                    else if (allDeps.ContainsKey(obj.name) == false)
                    {
                        unUseResSecond[obj.name] = obj;
                    }
                }
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("提示", "筛选无用资源结束", "OK");
        }

        void onFindSelectedFileUnuseRes()
        {
            var obj = Selection.activeGameObject;
            Debug.LogError(allDeps.ContainsKey(obj.name));
            Debug.LogError(unUseResFirst.ContainsKey(obj.name));
        }

        void onFindSelectedFileDependices()
        {
            allDeps.Clear();
            var obj = Selection.activeGameObject;
            string path = ResCheckJenkinsEntrance.GetAssetPath(obj);
            string[] deps = ResCheckJenkinsEntrance.GetDependencies(path, true);
            foreach (var dep in deps)
            {
                allDeps[Path.GetFileNameWithoutExtension(dep)] = 1;
                Debug.LogError(dep);
            }
        }

        void FindUnUsePngAndAtlas()
        {
            Debug.Log("总共的图集个数：" + atlasDics.Count);
            //查找没有用到的图集中的png
            foreach (var item in atlasOfPngDeps)
            {
                string name = Path.GetFileNameWithoutExtension(item.Key);
                if (allDeps.ContainsKey(name) == false && findLuaStrDic.ContainsKey(name) == false)//对于png来说，lua没用到，依赖没用到，那一定是没用到
                {
                    unUseResSecond[name] = ResCheckJenkinsEntrance.LoadAssetAtPath(item.Key);
                }
                else//用到的话，那么他所对应的图集就是用到的，安全的
                {
                    string atlasName = item.Value;
                    if (atlasDics.ContainsKey(atlasName))
                    {
                        atlasDics.Remove(atlasName);
                    }
                }
            }
            //没用到的图集
            foreach (var item in atlasDics)
            {
                unUseResSecond[item.Key] = ResCheckJenkinsEntrance.LoadAssetAtPath(item.Value);
            }
        }
        private static Vector3 scrollPos = Vector3.zero;
        private static void ShowFindResult()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Width(850), GUILayout.Height(600));
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");
            GUILayout.Space(5);
            EditorGUILayout.BeginVertical();
            string removeKey = null;
            foreach (var item in unUseResSecond)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(item.Value, typeof(UnityEngine.Object), true, GUILayout.Width(400));
                if (GUILayout.Button("移到backup里", GUILayout.Width(200), GUILayout.Height(20)))
                {
                    removeKey = item.Key;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (string.IsNullOrEmpty(removeKey) == false)
            {
                MoveToBackUp(removeKey);
                AssetDatabase.Refresh();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
        }

        static void MoveToBackUp(string removeKey)
        {
            string BackUpPath = Application.dataPath + "/../BackUp/";
            string path = ResCheckJenkinsEntrance.GetAssetPath(unUseResSecond[removeKey]);
            string fromPath = Path.GetFullPath(path);
            string toPath = BackUpPath + path;
            ResCheckEditorUtil.MoveFile(fromPath, toPath);

            string fromPathMeta = fromPath + ".meta";
            string toPathMeta = toPath + ".meta";
            ResCheckEditorUtil.MoveFile(fromPathMeta, toPathMeta);
            unUseResSecond.Remove(removeKey);
        }
    }
}
