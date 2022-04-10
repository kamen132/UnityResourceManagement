#if !UNITY_EDITOR
using UnityEngine;
#endif
public partial class NativeCenter
{

#if !UNITY_EDITOR && UNITY_ANDROID
     private int GetAndroidScreenTopMargin()
    {
        CheckAndroidClass();
        if (androidObj == null)
        {
            androidObj = androidClass.CallStatic<AndroidJavaObject>("GetInstance", "Native"); 
        }
        return androidObj.Call<int>("getScreenTopMargin");
    }

    private void AndroidCopyContentToClipBoard(string str)
    {
        CheckAndroidClass();
        if (androidObj == null)
        {
            androidObj = androidClass.CallStatic<AndroidJavaObject>("GetInstance", "Native"); 
        }
        androidObj.Call("CopyContentToClipBoard", str);
    }

    private string AndroidPasteContentFormClipBoard()
    {
        CheckAndroidClass();
        if (androidObj == null)
        {
            androidObj = androidClass.CallStatic<AndroidJavaObject>("GetInstance", "Native");
        }
        return androidObj.Call<string>("PasteContentFormClipBoard");
    }

    
    private long GetAndroidAvailableSpace(bool isExternalStorage = false)
    {
        CheckAndroidClass();
        return androidClass.CallStatic<long>("availableSpace", isExternalStorage);
    }

    private long GetAndroidTotalSpace(bool isExternalStorage = false)
    {
       CheckAndroidClass();
        return androidClass.CallStatic<long>("totalSpace", isExternalStorage);
    }

    private long GetAndroidBusySpace(bool isExternalStorage = false)
    {
        CheckAndroidClass();
        return androidClass.CallStatic<long>("busySpace", isExternalStorage);
    }

      private long GetAndroidAvailableMemory()
    {
        CheckAndroidClass();
        if (androidObj == null)
        {
            androidObj = androidClass.CallStatic<AndroidJavaObject>("GetInstance", "Native"); 
        }
        return androidObj.Call<long>("GetAvailableMemory");
    }
    private long GetAndroidAppUsingMemory()
    {
        CheckAndroidClass();
        if (androidObj == null)
        {
            androidObj = androidClass.CallStatic<AndroidJavaObject>("GetInstance", "Native"); 
        }
        return androidObj.Call<long>("GetAppUsingMemory");
    }
    private AndroidJavaClass androidClass = null;
    private AndroidJavaObject androidObj = null;
    private void CheckAndroidClass()
    {
        if (androidClass == null)
        {
            androidClass =  new AndroidJavaClass("com.android.android.NativeCenter");   
        }
    }
#endif

#if !UNITY_EDITOR && UNITY_IOS
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern ulong getAvailableDiskSpace();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern ulong getTotalDiskSpace();
    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern ulong getBusyDiskSpace();
    [System.Runtime.InteropServices.DllImport( "__Internal" )]
	public static extern void IOSCopyContentToClipBoard( string text );

     [System.Runtime.InteropServices.DllImport( "__Internal" )]
	 public static extern long GetAvailableMemory();

    [System.Runtime.InteropServices.DllImport( "__Internal" )]
	 public static extern long GetUsedMemory();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    public static extern long OnGetLaunchSystemTime();

    private void OnIOSCopyContentToClipBoard(string text )
    {
        NativeCenter.IOSCopyContentToClipBoard(text);
    }
    private long GetIOSAvailableSpace()
    {
        ulong ret = NativeCenter.getAvailableDiskSpace();
        return long.Parse(ret.ToString());
    }

    private long GetIOSTotalSpace()
    {
        ulong ret = NativeCenter.getTotalDiskSpace();
        return long.Parse(ret.ToString());
    }

    private long GetIOSBusySpace()
    {
        ulong ret = NativeCenter.getBusyDiskSpace();
        return long.Parse(ret.ToString());
    }
#endif

}
