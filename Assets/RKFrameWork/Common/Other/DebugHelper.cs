using System.Text;
using UnityEngine;
using UnityEngine.Profiling;

public class DebugHelper
{
    public static string GetFormatValue(long value)
    {
        float KB = 1024;
        float MB = 1024 * 1024;
        if(value > MB)
        {
            return (value / MB).ToString("0.000") + "MB";
        }else if(value > KB)
        {
            return (value / KB).ToString("0.000") + "KB";
        }
        return value.ToString("0.00") + "B";
    }
    
    public static string GetMemory()
    {
        StringBuilder builder = new StringBuilder();
        if(MacroDefinition.UNITY_EDITOR())
        {
            builder.Append("Unity能够分配到的内存：" + GetFormatValue(NativeCenter.Instance.GetTotalReservedMemoryLong()) + " ");
            builder.Append("Unity已使用的内存：" + GetFormatValue(NativeCenter.Instance.GetTotalAllocatedMemoryLong()) + "\n");
            builder.Append("Unity空闲中的内存：" + GetFormatValue(NativeCenter.Instance.GetTotalUnusedReservedMemoryLong()) + "  ");
        }else
        {
            builder.Append("系统可用内存：" + GetFormatValue(NativeCenter.Instance.GetSystemAvailableMemory()) + " ");
            builder.Append("App已使用内存：" + GetFormatValue(NativeCenter.Instance.GetAppUsingMemory()) + "\n");
        }
    
        builder.Append("GPU分配内存：" + GetFormatValue(Profiler.GetAllocatedMemoryForGraphicsDriver()) + "  ");
        builder.Append("Mono分配内存：" + GetFormatValue(Profiler.GetMonoHeapSizeLong()) + "\n");
        builder.Append("Mono使用内存：" + GetFormatValue(Profiler.GetMonoUsedSizeLong()) + "  ");
        builder.Append("临时分配器的内存：" + GetFormatValue(Profiler.GetTempAllocatorSize()) + "\n");
        
        builder.Append("系统总共空间大小：" + NativeCenter.Instance.GetTotalSpace() + "MB  ");
        builder.Append("系统已用空间大小：" + (NativeCenter.Instance.GetBusySpace()) + "MB\n");
        builder.Append("系统可用空间大小：" + (NativeCenter.Instance.GetAvailableSpace()) + "MB  ");
        return builder.ToString();
    }

    public static string GetSystemInfo()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("设备分级(1-3)：" + DeviceLevelManager.Instance.GetDeviceLevel() + "\n");
        builder.Append("SafeArea ：" + Screen.safeArea + "\n");
        builder.Append("ScreenTopMargin ：" + NativeCenter.Instance.GetScreenTopMargin() + "\n");
        
        builder.Append("DPI：" + Screen.dpi + "\n");
        builder.Append("分辨率：" + Screen.currentResolution.ToString() + "\n");

        builder.Append("操作系统：" + SystemInfo.operatingSystem + "\n");
        builder.Append("系统内存：" + SystemInfo.systemMemorySize + "MB \n");
        builder.Append("处理器：" + SystemInfo.processorType + "\n");
        builder.Append("处理器数量：" + SystemInfo.processorCount + "\n");
        builder.Append("显卡：" + SystemInfo.graphicsDeviceName + "\n");
        builder.Append("显卡类型：" + SystemInfo.graphicsDeviceType + "\n");
        builder.Append("显存：" + SystemInfo.graphicsMemorySize + "MB \n");
        builder.Append("显卡标识：" + SystemInfo.graphicsDeviceID + "\n");
        builder.Append("显卡供应商：" + SystemInfo.graphicsDeviceVendor + "\n");
        builder.Append("显卡供应商标识码：" + SystemInfo.graphicsDeviceVendorID + "\n");
        builder.Append("设备模式：" + SystemInfo.deviceModel + "\n");
        builder.Append("设备名称：" + SystemInfo.deviceName + "\n");
        builder.Append("设备类型：" + SystemInfo.deviceType + "\n");
        builder.Append("设备标识：" + SystemInfo.deviceUniqueIdentifier + "\n \n");
        builder.Append("项目名称：" + Application.productName + "\n");

        builder.Append("项目ID：" + Application.identifier + "\n");
        builder.Append("项目版本：" + Application.version + "\n");
        builder.Append("Unity版本：" + Application.unityVersion + "\n");
        builder.Append("公司名称：" + Application.companyName);

        builder.Append(SystemInfo.graphicsMultiThreaded ? " multi-threaded\n" : "\n");

        builder.Append("Max Texture Size: ").Append(SystemInfo.maxTextureSize).Append("\n");

        builder.Append("Max Cubemap Size: ").Append(SystemInfo.maxCubemapSize).Append("\n");

        builder.Append("Accelerometer: ").Append(SystemInfo.supportsAccelerometer ? "supported\n" : "not supported\n");
        builder.Append("Gyro: ").Append(SystemInfo.supportsGyroscope ? "supported\n" : "not supported\n");
        builder.Append("Location Service: ").Append(SystemInfo.supportsLocationService ? "supported\n" : "not supported\n");

        builder.Append("Image Effects: ").Append(SystemInfo.supportsImageEffects ? "supported\n" : "not supported\n");
        builder.Append("RenderToCubemap: ").Append(SystemInfo.supportsRenderToCubemap ? "supported\n" : "not supported\n");

        builder.Append("Compute Shaders: ").Append(SystemInfo.supportsComputeShaders ? "supported\n" : "not supported\n");
        builder.Append("Shadows: ").Append(SystemInfo.supportsShadows ? "supported\n" : "not supported\n");
        builder.Append("Instancing: ").Append(SystemInfo.supportsInstancing ? "supported\n" : "not supported\n");
        builder.Append("Motion Vectors: ").Append(SystemInfo.supportsMotionVectors ? "supported\n" : "not supported\n");
        builder.Append("3D Textures: ").Append(SystemInfo.supports3DTextures ? "supported\n" : "not supported\n");

        builder.Append("3D Render Textures: ").Append(SystemInfo.supports3DRenderTextures ? "supported\n" : "not supported\n");

        builder.Append("2D Array Textures: ").Append(SystemInfo.supports2DArrayTextures ? "supported\n" : "not supported\n");
        builder.Append("Cubemap Array Textures: ").Append(SystemInfo.supportsCubemapArrayTextures ? "supported" : "not supported");
        return builder.ToString();
    }
}