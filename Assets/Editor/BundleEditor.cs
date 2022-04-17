using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class BundleEditor
{
    public static string m_BundleTarget = Application.streamingAssetsPath;
    public static string ABCONFIG_PATH =  "Assets/Editor/ABConfig.asset";
    //key是ab包名，value是路径，所有文件夹ab包dic
    private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
    //过滤的list
    private static List<string> m_AllFileAB = new List<string>();
    //单个prefab的ab包
    private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();
    //储存所有有效路径
    private static List<string> m_ConfigFil = new List<string>();
    
    
    [MenuItem("Kamen/打包")]
    public static void Build()
    {
        m_AllFileAB.Clear();
        m_AllFileDir.Clear();
        m_AllPrefabDir.Clear();
        ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIG_PATH);
        foreach (var fileDir in abConfig.m_AllFileDirAB)
        {
            if (m_AllFileDir.ContainsKey(fileDir.ABName))
            {
                Debug.LogError("AB包配置名字重复，请检查！");
            }
            else
            {
                m_AllFileDir.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }

        string[] allStr = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray());
        for (int i = 0; i < allStr.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("查找所有prefab", "Prefab:" + path, i * 1.0f / allStr.Length);
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path))
            {
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                //找到所有的依赖项
                string[] allDepend = AssetDatabase.GetDependencies(path);
                List<string> allDependPath = new List<string>();
               
                for (int j = 0; j < allDepend.Length; j++)
                {
                    if (!ContainAllFileAB(allDepend[j])&&!allDepend[j].EndsWith(".cs"))
                    {
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[i]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name))
                {
                    Debug.LogError("存在相同名称的Prefab:"+obj.name);
                }
                else
                {
                    //单个prefab  及其所有依赖项
                    m_AllPrefabDir.Add(obj.name, allDependPath);
                }
            }
        }

        foreach (var name in m_AllFileDir.Keys)
        {
            SetABName(name, m_AllFileDir[name]);
        }

        foreach (var name in m_AllPrefabDir.Keys)
        {
            SetABName(name, m_AllPrefabDir[name]);
        }

        BuildAssetBundle();
        
        string[] oldABName = AssetDatabase.GetAllAssetBundleNames();
        for (int i = 0; i < oldABName.Length; i++)
        {
            AssetDatabase.RemoveAssetBundleName(oldABName[i], true);
            EditorUtility.DisplayProgressBar("清除AB包名", "名字" + oldABName[i], i * 1.0f / oldABName.Length);
        }

        

        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 删除无用ab包
    /// </summary>
    static void DeleteAB()
    {
        string[] allBundleName = AssetDatabase.GetAllAssetBundleNames();
        DirectoryInfo directoryInfo = new DirectoryInfo(m_BundleTarget);
        FileInfo[] fileInfo = directoryInfo.GetFiles("*",SearchOption.AllDirectories);
        for (int i = 0; i < fileInfo.Length; i++)
        {
            if (ContainABName(fileInfo[i].Name,allBundleName)||fileInfo[i].Name.EndsWith(".meta"))
            {
                continue;
            }
            else
            {
                Debug.Log("此AB包已经被删除或者被改名了"+fileInfo[i].Name);
                if (File.Exists(fileInfo[i].FullName))
                {
                    File.Delete(fileInfo[i].FullName);
                }
            }
        }

    } 
    
    static void SetABName(string abName, string path)
    {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter==null)
        {
            Debug.LogError("不存在此路径文件:" + path);
        }
        else
        {
            assetImporter.assetBundleName = abName;
        }
    }
    static void SetABName(string name, List<string> path)
    {
        for (int i = 0; i < path.Count; i++)
        {
            SetABName(name,path[i]);
        }
    }
    /// <summary>
    /// 是否有效路径
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ValidPath(string path)
    {
        for (int i = 0; i < m_ConfigFil.Count; i++)
        {
            if (path.Contains(m_ConfigFil[i]))
            {
                return true;
            }
        }
        return false;
    }
    static void BuildAssetBundle()
    {
        string[] allBundles = AssetDatabase.GetAllAssetBundleNames();
        //全路径 包名
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();
        for (int i = 0; i < allBundles.Length; i++)
        {
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundles[i]);
            for (int j = 0; j < allBundlePath.Length; j++)
            {
                if (allBundlePath[j].EndsWith(".cs"))
                {
                    continue;
                }
                resPathDic.Add(allBundlePath[j],allBundles[j]);
                Debug.Log("此AB包："+allBundles[j]+"  下面包含的子啊元文件路径："+allBundlePath[j]);
            }
        }
        DeleteAB();
        //生成自己的配置表
        WriteData(resPathDic);
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
    }
    
    static void WriteData(Dictionary<string,string> resPathDic)
    {
        AssetBundleConfig config=new AssetBundleConfig();
        config.ABList = new List<ABBase>();
        foreach (var path in resPathDic.Keys)
        {
            if (!ValidPath(path))
                continue;
            ABBase abBase=new ABBase();
            abBase.Path = path;
            abBase.Crc = Crc32.GetCrc32(path);
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/")+1);
            abBase.ABDependce = new List<string>();
            string[] resDependace = AssetDatabase.GetDependencies(path);
            for (int i = 0; i < resDependace.Length; i++)
            {
                string tmpPath = resDependace[i];
                if (tmpPath!=path||path.EndsWith(".cs"))
                    continue;

                string abName = "";
                if (resPathDic.TryGetValue(tmpPath,out abName))
                {
                    if (abName==resPathDic[path])
                        continue;
                    if (!abBase.ABDependce.Contains(abName))
                    {
                        abBase.ABDependce.Add(abName);
                    }
                }
            }
            config.ABList.Add(abBase);
        }
        //写入xml  为了方便认出ab包包含的东西
        string xmlPath = Application.dataPath + "/AssetBundleConfig.xml";
        if (File.Exists(xmlPath))
            File.Delete(xmlPath);
        FileStream fileStream = new FileStream(xmlPath,FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        StreamWriter sw = new StreamWriter(fileStream, Encoding.UTF8);
        XmlSerializer xs=new XmlSerializer(config.GetType());
        xs.Serialize(sw, config);
        sw.Close();
        fileStream.Close();
        
        //写入二进制
        string bytePath = m_BundleTarget + "/AssetBundleConfig.bytes";
        FileStream fs = new FileStream(bytePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(fs, config);
        fs.Close();

    }

    static bool ContainABName(string name, string[] strs)
    {
        for (int i = 0; i < strs.Length; i++)
        {
            if (name == strs[i])
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否包含所有的ab
    /// </summary>
    /// <returns></returns>
    static bool ContainAllFileAB(string path)
    {
        for (int i = 0; i < m_AllFileAB.Count; i++)
        {
            if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && (path.Replace(m_AllFileAB[i], "")[0] == '/')))
                return true;
        }
        return false;
    }
}
