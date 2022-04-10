using GMResChecker;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace Majic.CM
{
    public class MemoryData
    {
        public Dictionary<string, RoleInfo> heroInfos = null;
        public double heroAllRam = 0;
        public Dictionary<string, RoleInfo> monsterInfos = null;
        public double monsterAllRam = 0;

        public Dictionary<string, RoleInfo> sceneInfos = null;
        public double sceneAllRam = 0;
        
        public List<TextureInfo> fxInfos = null;
        public double fxAllRam = 0;
        public List<TextureInfo> otherInfos = null;
        public double otherAllRam = 0;

        public double totleRam = 0;
    }
    public class TextureInfo
    {
        public string name;
        public double totleRam;
        public int num;
        public double ram;
        public long frame;
        public int width;
        public int height;
        public string format;
        public int mipmap;
    }

    public class RoleInfo
    {
        public List<TextureInfo> resInfos = new List<TextureInfo>();
        public double allRam = 0;
    }

    public class UwaAnalyze 
    {
        private const string _FX = "fx";
        private const string _SUIT = "suit";
        private static Dictionary<string, int> allHeroNames = new Dictionary<string, int>();
        private static Dictionary<string, int> allMonsterNames = new Dictionary<string, int>();
        private static Dictionary<string, int> allSceneNames = new Dictionary<string, int>();
        private static MemoryData memoryData;
        private static string selectedDirPath;
        [MenuItem("CheckWindow/UwaAnalyze", false)]
        public static void UwaAnalyzeSelect()
        {
            string _path = Application.dataPath;
            selectedDirPath = EditorUtility.OpenFilePanel("请选择要打全部Machine图集的文件夹", _path, "");
            if (string.IsNullOrEmpty(selectedDirPath))
            {
                return;
            }
            PreData();
            string[] config = FileManager.ReadAllLines(selectedDirPath);
            int length = config.Length;
            memoryData = new MemoryData();
            Dictionary<string, RoleInfo> heroInfos = new Dictionary<string, RoleInfo>();
            Dictionary<string, RoleInfo> monsterInfos = new Dictionary<string, RoleInfo>();
            Dictionary<string, RoleInfo> sceneInfos = new Dictionary<string, RoleInfo>();

            List<TextureInfo> fxInfos = new List<TextureInfo>();
            List<TextureInfo> otherInfos = new List<TextureInfo>();
            memoryData.heroInfos = heroInfos;
            memoryData.monsterInfos = monsterInfos;
            memoryData.fxInfos = fxInfos;
            memoryData.otherInfos = otherInfos;
            memoryData.sceneInfos = sceneInfos;
            for (int i = 0; i < length; i++)
            {
                if(i == 0)
                {
                    continue;
                }
                string line = config[i];
                string[] items = line.Split(',');
                string resName = items[0].ToLower().Replace("\"", "");

                TextureInfo textureInfo = new TextureInfo();
                textureInfo.name = resName;
                textureInfo.totleRam = long.Parse(items[1]);
                textureInfo.num = int.Parse(items[2]);
                textureInfo.ram = long.Parse(items[3]);
                textureInfo.frame = long.Parse(items[4]);
                textureInfo.width = int.Parse(items[5]);
                textureInfo.height = int.Parse(items[6]);
                textureInfo.format = items[7];
                textureInfo.mipmap = int.Parse(items[8]);
                if (resName.StartsWith(_FX))
                {
                    fxInfos.Add(textureInfo);
                    continue;
                }
                //---
                string sceneName = GetSceneNameInResName(resName);
                if (string.IsNullOrEmpty(sceneName) == false)//找到了这个 sceneName
                {
                    if (sceneInfos.ContainsKey(sceneName) == false)
                    {
                        sceneInfos[sceneName] = new RoleInfo();
                    }
                    
                    sceneInfos[sceneName].resInfos.Add(textureInfo);
                    continue;
                }
                //---
                string heroName = GetHeroNameInResName(resName);
                if (string.IsNullOrEmpty(heroName) == false)//找到了这个hero name
                {
                    if (heroInfos.ContainsKey(heroName) == false)
                    {
                        heroInfos[heroName] = new RoleInfo();
                    }
                    heroInfos[heroName].resInfos.Add(textureInfo);
                    continue;
                }

                string monsterName = GetMonsterNameInResName(resName);
                if (string.IsNullOrEmpty(monsterName) == false)//找到了这个monsterName
                {
                    if (monsterInfos.ContainsKey(monsterName) == false)
                    {
                        monsterInfos[monsterName] = new RoleInfo();
                    }
                    monsterInfos[monsterName].resInfos.Add(textureInfo);
                    continue;
                }
                otherInfos.Add(textureInfo);
            }
            CalTotleRam();
            ExportExcle();
            Debug.Log("222222222");
        }
        static void PreData()
        {
            allHeroNames.Clear();
            string _path = Application.dataPath + "/Product/Editor/Resources/Role";
            string heroPath = _path + "/Hero";
            string monsterPath = _path + "/Monster";
            string scenePath = Application.dataPath + "/Product/Editor/Resources/Scene";

            string[]  allHeroPaths = Directory.GetDirectories(heroPath, "*", SearchOption.TopDirectoryOnly);
            foreach(string path in allHeroPaths)
            {
                string name = Path.GetFileNameWithoutExtension(path).ToLower();
                if(name.EndsWith(_SUIT))
                {
                    continue;
                }
                allHeroNames[name] = 1;
                //Debug.Log("name==" + name);
            }

            string[] allMonsterPaths = Directory.GetDirectories(monsterPath, "*", SearchOption.TopDirectoryOnly);
            foreach (string path in allMonsterPaths)
            {
                string name = Path.GetFileNameWithoutExtension(path).ToLower();
                if (name.EndsWith(_SUIT))
                {
                    continue;
                }
                allMonsterNames[name] = 1;
                //Debug.Log("name2==" + name);
            }

            string[] allScenePaths = Directory.GetDirectories(scenePath, "*", SearchOption.TopDirectoryOnly);
            foreach (string path in allScenePaths)
            {
                string name = Path.GetFileNameWithoutExtension(path).ToLower();
                if (name.EndsWith(_SUIT) 
                    || name.Equals("common")
                    || name.Equals("_editor")
                    )
                {
                    continue;
                }
                if(name.Contains("scene_"))
                {
                    name = name.Substring(6);
                }
                if (name.Contains("_normal"))
                {
                    name = name.Substring(0, name.Length - 7);
                }
                //Debug.Log("name2==" + name);
                allSceneNames[name] = 1;
            }
        }
        
        static string GetHeroNameInResName(string resName)
        {
            foreach(var item in allHeroNames)
            {
                if(resName.Contains(item.Key))
                {
                    return item.Key;
                }
            }
            return string.Empty;
        }
        static string GetMonsterNameInResName(string resName)
        {
            foreach (var item in allMonsterNames)
            {
                if (resName.Contains(item.Key))
                {
                    return item.Key;
                }
            }
            return string.Empty;
        }
        static string GetSceneNameInResName(string resName)
        {
            foreach (var item in allSceneNames)
            {
                if (resName.Contains(item.Key))
                {
                    return item.Key;
                }
            }
            return string.Empty;
        }

        static void CalTotleRam()
        {
            foreach(var item in memoryData.heroInfos)
            {
                foreach(var subItem in item.Value.resInfos)
                {
                    item.Value.allRam += subItem.totleRam;
                    memoryData.heroAllRam += subItem.totleRam;
                }
            }

            foreach (var item in memoryData.monsterInfos)
            {
                foreach (var subItem in item.Value.resInfos)
                {
                    item.Value.allRam += subItem.totleRam;
                    memoryData.monsterAllRam += subItem.totleRam;
                }
            }

            foreach (var item in memoryData.sceneInfos)
            {
                foreach (var subItem in item.Value.resInfos)
                {
                    item.Value.allRam += subItem.totleRam;
                    memoryData.sceneAllRam += subItem.totleRam;
                }
            }

            foreach(var item in memoryData.fxInfos)
            {
                memoryData.fxAllRam += item.totleRam;
            }
            foreach (var item in memoryData.otherInfos)
            {
                memoryData.otherAllRam += item.totleRam;
            }

            memoryData.totleRam += memoryData.heroAllRam;
            memoryData.totleRam += memoryData.monsterAllRam;
            memoryData.totleRam += memoryData.sceneAllRam;
            memoryData.totleRam += memoryData.fxAllRam;
            memoryData.totleRam += memoryData.otherAllRam;
        }
        static void ExportExcle()
        {
            string fonder = PathUtil.GetFilesParentFolder(selectedDirPath);
            //public static void ExportExcle(string dir, string fileName, List<string> sheetNames, List<List<string>> titleLists, List<List<List<string>>> datas)
            List<string> sheetNames = new List<string>
            {
                "Result", "Hero", "Monster", "Scene", "FX", "Other"
            };
            
            List<List<string>> titleLists = new List<List<string>>()
            {
                new List<string>{ },
               
                new List<string>{ "Name", "ResName", "Ram" ,"Ram" },
                new List<string>{ "Name", "ResName", "Ram","Ram" },
                new List<string>{ "Name", "ResName", "Ram","Ram" },
                new List<string>{ "ResName", "Ram" ,"Ram"},
                new List<string>{ "ResName", "Ram" ,"Ram"},
            };

            List<List<List<string>>> datas = new List<List<List<string>>>();

            ExportBaseSheet(datas);
            ExportHeroSheet(datas);
            ExportMonsterSheet(datas);
            ExportSceneSheet(datas);
            ExportFXSheet(datas);
            ExportOtherSheet(datas);

            ResCheckJenkinsEntrance.ExportExcle(fonder, "Result_" + ResCheckEditorUtil.TimeStr(), sheetNames, titleLists, datas);
        }
        static void ExportHeroSheet(List<List<List<string>>> datas)
        {
            List<List<string>> baseData = new List<List<string>>();
            datas.Add(baseData);
            foreach (var item in memoryData.heroInfos)
            {
                
                foreach(var item2 in item.Value.resInfos)
                {
                    List<string> list = new List<string>();
                    list.Add(item.Key);
                    list.Add(item2.name);
                    list.Add(ResCheckEditorUtil.HumanReadableNum(item2.totleRam));
                    list.Add(item2.totleRam.ToString());
                    baseData.Add(list);
                }
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.heroAllRam) });
        }
        static void ExportMonsterSheet(List<List<List<string>>> datas)
        {
            List<List<string>> baseData = new List<List<string>>();
            datas.Add(baseData);
            foreach (var item in memoryData.monsterInfos)
            {

                foreach (var item2 in item.Value.resInfos)
                {
                    List<string> list = new List<string>();
                    list.Add(item.Key);
                    list.Add(item2.name);
                    list.Add(ResCheckEditorUtil.HumanReadableNum(item2.totleRam));
                    list.Add(item2.totleRam.ToString());
                    baseData.Add(list);
                }
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.monsterAllRam) });
        }
        static void ExportSceneSheet(List<List<List<string>>> datas)
        {
            List<List<string>> baseData = new List<List<string>>();
            datas.Add(baseData);
            foreach (var item in memoryData.sceneInfos)
            {

                foreach (var item2 in item.Value.resInfos)
                {
                    List<string> list = new List<string>();
                    list.Add(item.Key);
                    list.Add(item2.name);
                    list.Add(ResCheckEditorUtil.HumanReadableNum(item2.totleRam));
                    list.Add(item2.totleRam.ToString());
                    baseData.Add(list);
                }
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.sceneAllRam) });
        }
        static void ExportFXSheet(List<List<List<string>>> datas)
        {
            List<List<string>> baseData = new List<List<string>>();
            datas.Add(baseData);
            foreach (var item in memoryData.fxInfos)
            {
                List<string> list = new List<string>();
                list.Add(item.name);
                list.Add(ResCheckEditorUtil.HumanReadableNum(item.totleRam));
                list.Add(item.totleRam.ToString());
                baseData.Add(list);
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.fxAllRam) });
        }
        static void ExportOtherSheet(List<List<List<string>>> datas)
        {
            List<List<string>> baseData = new List<List<string>>();
            datas.Add(baseData);
            foreach (var item in memoryData.otherInfos)
            {
                List<string> list = new List<string>();
                list.Add(item.name);
                list.Add(ResCheckEditorUtil.HumanReadableNum(item.totleRam));
                list.Add(item.totleRam.ToString());
                baseData.Add(list);
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.otherAllRam) });
        }
        static void ExportBaseSheet(List<List<List<string>>> datas)
        {
            List<List<string>> baseData = new List<List<string>>();
            datas.Add(baseData);
            foreach (var item in memoryData.heroInfos)
            {
                List<string> list = new List<string>();
                list.Add("hero");
                list.Add(item.Key);
                list.Add(ResCheckEditorUtil.HumanReadableNum(item.Value.allRam));
                baseData.Add(list);
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.heroAllRam) });

            foreach (var item in memoryData.monsterInfos)
            {
                List<string> list = new List<string>();
                list.Add("monster");
                list.Add(item.Key);
                list.Add(ResCheckEditorUtil.HumanReadableNum(item.Value.allRam));
                baseData.Add(list);
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.monsterAllRam) });

            foreach (var item in memoryData.sceneInfos)
            {
                List<string> list = new List<string>();
                list.Add("scene");
                list.Add(item.Key);
                list.Add(ResCheckEditorUtil.HumanReadableNum(item.Value.allRam));
                baseData.Add(list);
            }
            baseData.Add(new List<string> { ResCheckEditorUtil.HumanReadableNum(memoryData.sceneAllRam) });
            baseData.Add(new List<string> { "FX", ResCheckEditorUtil.HumanReadableNum(memoryData.fxAllRam) });
            baseData.Add(new List<string> { "Other", ResCheckEditorUtil.HumanReadableNum(memoryData.otherAllRam) });
            baseData.Add(new List<string> { "Totle", ResCheckEditorUtil.HumanReadableNum(memoryData.totleRam) });
        }
    }
}
