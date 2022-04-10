

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DeviceLevelManager : Singleton<DeviceLevelManager>
{
    private string graphicsDeviceName;
    private string deviceModel;
    //private int lowestMemoryLevel = 2200;
    private int deviceLevel; //设备高中低等级，1 2 3三个等级
    private int deviceMemoryLevel;
    private string graphicsDeviceVendor;
    public void Init()
    {
        graphicsDeviceName = SystemInfo.graphicsDeviceName;
        deviceModel = SystemInfo.deviceModel;
        deviceModel = deviceModel.ToLower();
        graphicsDeviceVendor = SystemInfo.graphicsDeviceVendor;
        deviceLevel = AnalyseDeviceLevel();
    }
    private int AnalyseDeviceLevel()
    {
        int deviceLevel = 1;
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            deviceLevel = HandleIOSDeviceLevel();
        } else if (Application.platform == RuntimePlatform.Android)
        {
            if (graphicsDeviceVendor == "Qualcomm")
            {
                deviceLevel = HandleQualcommDevice();
            }else if(graphicsDeviceVendor == "ARM")
            {
                deviceLevel = HandleARMDevice();
            } else if (graphicsDeviceVendor == "Imagination Technologies")
            {
                deviceLevel = HandlePowerVRDevice();
            }
        }else
        {
            deviceLevel = 3;
        }
        return deviceLevel;
    }

    private int HandleIOSDeviceLevel()
    {
        if (iphoneDiviceInfo.ContainsKey(deviceModel))
        {
            return iphoneDiviceInfo[deviceModel];
        }
        
        return 2;
    }

    private int HandlePowerVRDevice()
    {
        int level = 1;
        if(graphicsDeviceName.Contains("SGX"))
        {
            level = 1;
        }else
        {
            string gpuModel = GetPowerVRGPUModel(graphicsDeviceName, "G");
            if(gpuModel == null)
            {
                gpuModel = GetPowerVRGPUModel(graphicsDeviceName, "GT");
            }
            if (gpuModel == null)
            {
                gpuModel = GetPowerVRGPUModel(graphicsDeviceName, "GX");
            }
            if (gpuModel == null)
            {
                gpuModel = GetPowerVRGPUModel(graphicsDeviceName, "GE");
            }
            if (gpuModel == null)
            {
                gpuModel = GetPowerVRGPUModel(graphicsDeviceName, "GM");
            }
            int gpuType = -1;
            int.TryParse(gpuModel, out gpuType);
            if(gpuType == -1)
            {
                level = 2;
            }else if(gpuType <= 8000)
            {
                level = 1;
            }
            else if (gpuType <= 8300)
            {
                level = 2;
            }
            else
            {
                level = 3;
            }
        }
        return level;
    }
    private string GetPowerVRGPUModel(string s, string p)
    {
        string result = null;
        if (s.Contains(p))
        {
            result = s.Substring(s.IndexOf(p));
        }
        return result;
    }
    private int HandleARMDevice()
    {
        int level = 1;
        string modelChar = graphicsDeviceName.Substring(5, 1);
        int graphicsType = -1;
        int.TryParse(graphicsDeviceName.Substring(6), out graphicsType);
        if (graphicsType != -1)
        {
            if(modelChar == "T")
            {
                level = 1;
            }else if(modelChar == "G")
            {
                if(graphicsType <= 72)
                {
                    level = 2;
                }else
                {
                    level = 3;
                }
            }else
            {
                level = 2;
            }
        }
        return level;
    }

    private int HandleQualcommDevice()
    {
        int level = 2;
        int gpuType = GeyGpuType(graphicsDeviceName);
        if (gpuType < 508 || gpuType == 605)
        {
            level = 1;
        } else if ((gpuType >= 508 && gpuType < 540) || gpuType == 610 || gpuType == 612)
        {
            level = 2;
        } else if (gpuType >= 615 || gpuType == 540)
        {
            level = 3;
        }
        return level;
    }

    private int GeyGpuType(string gpuName)
    {
        int type = -1;
        int length = gpuName.Length;
        int index = gpuName.LastIndexOf(" ");
        if (index > 0)
        {
            string name =  gpuName.Substring(index);
            int.TryParse(name, out type);
        }
        return type;
    }

    //这里只找1 和2 因为越往后，内存和cpu越强，越是高端机
    private Dictionary<string, int> iphoneDiviceInfo = new Dictionary<string, int>()
    {
        { "iphone3,1" , 1},//iphone 4
        { "iphone3,2" , 1},//iphone 4
        { "iphone3,3" , 1},//iphone 4
        { "iphone4,1" , 1},//iphone 4S

        { "iphone5,1" , 1},//iphone 5
        { "iphone5,2" , 1},//iphone 5
        { "iphone5,3" , 1},//iphone 5c
        { "iphone5,4" , 1},//iphone 5c

        { "iphone6,1" , 1},//iphone 5s
        { "iphone6,2" , 1},//iphone 5s
        { "iphone7,1" , 1},//iphone 6 Plus
        { "iphone7,2" , 1},//iphone 6

       


        { "ipad1,1" , 1},// ipad
        { "ipad1,2" , 1},// ipad 3G

        { "ipad2,1" , 1},// ipad 2
        { "ipad2,2" , 1},// ipad 2
        { "ipad2,3" , 1},// ipad 2
        { "ipad2,4" , 1},// ipad 2
        { "ipad2,5" , 1},// ipad Mini
        { "ipad2,6" , 1},// ipad Mini
        { "ipad2,7" , 1},//  ipad Mini

        { "ipad3,1" , 1},//  ipad 3
        { "ipad3,2" , 1},//  ipad 3
        { "ipad3,3" , 1},//  ipad 3
        { "ipad3,4" , 1},//  ipad 4
        { "ipad3,5" , 1},//   ipad 4
        { "ipad3,6" , 1},//   ipad 4

        { "ipad4,1" , 1},//    ipad Air
        { "ipad4,2" , 1},//    ipad Air
        { "ipad4,3" , 1},//    ipad mini2
        { "ipad4,4" , 1},//    ipad mini2
        { "ipad4,5" , 1},//    ipad mini2
        { "ipad4,6" , 1},//    ipad mini2
        { "ipad4,7" , 1},//    ipad mini3
        { "ipad4,8" , 1},//    ipad mini3
        { "ipad4,9" , 1},//   ipad mini3

        { "ipad5,1" , 2},
        { "ipad5,2" , 2},
        { "ipad5,3" , 2},
        { "ipad5,4" , 2},
        { "ipad6,7" , 2},
        { "ipad6,8" , 2},
        { "iphone8,1" , 2},
        { "iphone8,2" , 2},
        { "iphone8,4" , 2},
    };

    public int GetDeviceLevel()
    {
        return deviceLevel;
    }
}





















