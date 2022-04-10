using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Profiling;

public partial class NativeCenter : MonoBehaviour
{
    private static volatile NativeCenter ms_Instance;
    public static NativeCenter Instance
    {
        get
        {
            return ms_Instance;
        }
    }
    private void Awake()
    {
        ms_Instance = this;
    }

#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport("__Internal")]
#elif UNITY_EDITOR && UNITY_IPHONE
    [DllImport("bspatchForMac", CallingConvention = CallingConvention.Cdecl)]
#elif  UNITY_EDITOR 
    [DllImport("BspatchForWin", CallingConvention = CallingConvention.Cdecl)]
#else
    [DllImport("libbspatch", CallingConvention = CallingConvention.Cdecl)]
#endif
    public static extern int StartPatch(string[] argv);

    public static int PatchFile(string oldFilePath, string patchFilePath, string outPatchFile)
    {
        string fonder = PathUtil.GetFilesParentFolder(outPatchFile);
        FileManager.CreateDirectory(fonder);
        string[] list = new string[4];
        FileManager.DeleteFile(outPatchFile);
        list[0] = "bspatch";
        list[1] = oldFilePath;
        list[2] = outPatchFile;
        list[3] = patchFilePath;
        int value = StartPatch(list);
        return value;
    }

    public int GetScreenTopMargin()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
			return GetAndroidScreenTopMargin();
#elif UNITY_IOS
			return (int)Screen.safeArea.y;
#endif
        return 0;
    }

    public int GetScreenBottomMargin()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
			return 0;
#elif UNITY_IOS
			return (int)Screen.safeArea.x;
#endif
        return 0;
    }

    public void CopyContentToClipBoard(string str)
    {
#if UNITY_EDITOR
        GUIUtility.systemCopyBuffer = str;
#elif UNITY_ANDROID
			AndroidCopyContentToClipBoard(str );
#elif UNITY_IOS
			OnIOSCopyContentToClipBoard( str );
#endif
    }
    public long GetTotalReservedMemoryLong()
    {
#if UNITY_EDITOR
        return Profiler.GetTotalReservedMemoryLong();
#else
        return 0;
#endif
    }

    public long GetTotalAllocatedMemoryLong()
    {
#if UNITY_EDITOR
        return Profiler.GetTotalAllocatedMemoryLong();
#else
        return 0;
#endif
    }

    public long GetTotalUnusedReservedMemoryLong()
    {
#if UNITY_EDITOR
        return Profiler.GetTotalUnusedReservedMemoryLong();
#else
        return 0;
#endif
    }


    /// <returns>The available space in MB.</returns>
    public long GetAvailableSpace(bool isExternalStorage = true)
    {
#if UNITY_EDITOR
        return (long)9999999999;
#elif UNITY_ANDROID
			return GetAndroidAvailableSpace(isExternalStorage);
#elif UNITY_IOS
			return GetIOSAvailableSpace();
#endif
    }

    /// <returns>The total space in MB.</returns>
    public long GetTotalSpace(bool isExternalStorage = true)
    {
#if UNITY_EDITOR
        return (long)9999999999;
#elif UNITY_ANDROID
			return GetAndroidTotalSpace(isExternalStorage);
#elif UNITY_IOS
			return GetIOSTotalSpace();
#endif
    }
     //<returns>The free space in MB.</returns>
    public long GetBusySpace(bool isExternalStorage = true)
    {
#if UNITY_EDITOR
        return (long)9999999999;
#elif UNITY_ANDROID
			return GetAndroidBusySpace(isExternalStorage);
#elif UNITY_IOS
			return GetIOSBusySpace();
#endif
    }

    public long GetSystemAvailableMemory()
    {
#if UNITY_EDITOR
        return (long)9999999999;
#elif UNITY_ANDROID
			return GetAndroidAvailableMemory();
#elif UNITY_IOS
			return GetAvailableMemory();
#endif
    }
    public long GetAppUsingMemory()
    {
#if UNITY_EDITOR
        return (long)9999999999;
#elif UNITY_ANDROID
			return GetAndroidAppUsingMemory();
#elif UNITY_IOS
			return 0;
#endif
    }

    public long GetLaunchSystemTime()
    {
#if UNITY_EDITOR
        return System.Environment.TickCount;
#elif UNITY_ANDROID
        return System.Environment.TickCount;
#elif UNITY_IOS
        return OnGetLaunchSystemTime();
#endif
    }

}
