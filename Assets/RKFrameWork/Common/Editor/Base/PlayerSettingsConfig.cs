
using UnityEditor;
using UnityEngine;
//RenderSettings
//Color ambientEquatorColor 两侧的环境照明
//Color  ambientGroundColor 下方的环境照明
//float ambientIntensity    /来自环境光源的光线会对场景产生多大影响。
//Color ambientLight   平坦的环境照明颜色
//static AmbientMode ambientMode 环境照明模式 Skybox:Skybox  Trilight:gradient  Flat:color  Custom 环境光照由自定义立方体贴图定义。

//SystemInfo
//batteryLevel 当前电池电量（只读）。
//batteryStatus 设备电池的当前状态 是否充电中
//copyTextureSupport  支持各种Graphics.CopyTexture案例（只读）。
//deviceModel 设备型号（只读）。
//deviceName 用户定义的设备名称（只读）。
//deviceType 返回运行应用程序的设备类型（只读）。
//deviceUniqueIdentifier  /唯一的设备标识符。保证每个设备都是唯一的（只读）。
//graphicsDeviceID 图形设备的标识符代码（只读）。
//graphicsDeviceName 图形设备的名称（只读）。
//graphicsDeviceType 图形设备使用的图形API类型（只读）。
//graphicsDeviceVendor 图形设备的供应商（只读）。
//graphicsDeviceVendorID 图形设备供应商的标识符代码（只读）。
//graphicsMultiThreaded 图形设备是否使用多线程渲染（只读
//graphicsShaderLevel  图形设备着色器功能级别（只读）。
//graphicsUVStartsAtTop  //如果此平台的纹理UV坐标约定为Y，则返回true//从图像顶部开始。
//maxCubemapSize  最大立方体贴图纹理大小（只读）。
//maxTextureSize 最大纹理大小（只读）。
//npotSupport GPU提供什么NPOT（两种尺寸的非功率）纹理支持 （只读）。
//processorCount 存在的处理器数量（只读）。
//processorFrequency 处理器频率（MHz）（只读）。
//processorType 处理器名称（只读）。
//supportedRenderTargetCount 支持多少个同时渲染目标（MRT）？ （只读）
//supports2DArrayTextures 是否支持2D数组纹理？ （只读）
//supports3DRenderTextures 是否支持3D（音量）RenderTextures？ （只读）
//supports3DTextures 是否支持3D（体积）纹理？ （只读）
//supportsAccelerometer 设备上是否有加速度计？
//supportsAudio 是否有可播放的音频设备？
//supportsGyroscope 设备上是否有陀螺仪？
//supportsInstancing 是否支持GPU绘制调用实例化？ （只读）
//supportsLocationService 设备是否能够报告其位置？
//supportsRawShadowDepthSampling 是否支持从阴影贴图中采样原始深度？ （只读）
//systemMemorySize 系统存储器的数量
//Preferences中的Compress Assets on Import是决定资源导入时是否压缩资源，对于贴图资源来说，就是决定导入时是否压缩贴图。而TextureImporter中的贴图格式决定的是贴图的压缩格式。
//Splash Screen settings
//Show Splash Screen

//Other Settings
//Rendering
//Color Space 

//Auto Graphics API: 勾选 选中此选项可使Unity从Open Graphics Library（OpenGL）中自动选择图形API。 选中后，Unity将尝试GLES3.1，如果设备不支持GLES3.1，则返回GLES3或GLES2。 取消选中后，您可以手动选择和重新排序图形API。 如果列表中只有GLES3，则另外两个复选框; 需要ES3.1和要求ES3.1 + AEP，允许您强制使用相应的图形API。
//Multithreaded Rendering  勾选， 选中此框可将Unity的主线程中的图形API调用移动到单独的工作线程。 这有助于提高主线程上CPU使用率高的应用程序的性能。
//Static Batching 勾选,选中此框以在构建上使用静态批处理（默认情况下启用）。
//Dynamic Batching 勾选,选中此框以在构建上使用动态批处理（默认情况下启用）。
//GPU Skinning 勾选 下面能设置
//Graphics Jobs(Experimental) 勾选 下面能设置
//Protect Graphics Memory 都行 选中此框可强制仅通过受硬件保护的路径显示图形缓冲区。 仅适用于支持它的设备。
//Configuration
//Scripting Backend   IL2CPP or    Mono2x
//Compatibility Level  .Net 2.0 Subset
//Mute Other Audio Sources) 下面有设置，应该是true
//Disable HW Statistics 勾选 默认情况下，Unity Android应用程序向Unity发送匿名HW统计信息。 这为您提供了汇总信息，以帮助您作为开发人员做出决策。 在http://stats.unity3d.com/上查找这些统计信息。 选中此选项可停止Unity发送这些统计信息。
//Device Filter  armv7 允许应用程序在指定的CPU上运行。
//Install Location Automatic //让操作系统决定。 用户可以来回移动应用程序。
//Internet Access  auto 设置为Require时，即使您未使用任何网络API，也会将此网络（INTERNET）权限添加到Android清单中。 默认情况下，这对于开发版本启用。
//Write Access .internal, 设置为外部（SDCard）时，启用对SD卡等外部存储的写访问权限，并为Android清单添加相应的权限。 默认情况下为开发版本启用。
//Android TV Compatibility 不勾选
//Android Game 不勾选 Android TV - 选中此框可将输出包（APK）标记为游戏而非常规应用程序。
//Android Gamepad Support Level 不勾选 Android TV - 此选项允许您定义应用程序为游戏手柄提供的支持级别。 选项是使用D-Pad，支持游戏手柄和需要游戏手柄。

//Optimization 
//Prebake Collision Meshes 勾选 是否应在构建时将碰撞数据添加到网格中？
//Strip Engine Code 不勾选 启用代码剥离。 （此设置仅适用于IL2CPP脚本后端。）
//Enable Internal profiler 不勾选 如果要在测试项目时在Android SDK的adblogcat输出中从设备获取探查器数据，请选中此框（仅在开发版本中可用）。
//Vertex Compression  选择应压缩的顶点通道。 压缩可以节省内存和带宽，但精度会降低。
//Optimize Mesh Data  不勾选 从网格中删除应用于它们的材质不需要的任何数据（切线，法线，颜色，UV）。
//ios------------------------------------------
//Debugging and crash reporting
//Enable Internal Profiler(Deprecated) 不勾选 启用内部分析器，该分析器收集应用程序的性能数据并将报告打印到控制台。 该报告包含每个Unity子系统在每个帧上执行所花费的毫秒数。 数据在30帧内平均。
//On.Net UnhandledException Crash 对.NET未处理的异常采取的操作。 选项是Crash（应用程序几乎没有崩溃，迫使iOS生成可由应用程序用户提交给iTunes并由开发人员检查的崩溃报告），Silent Exit（应用程序正常退出）。
//Log ObjC uncaught exceptions 不勾选 启用自定义Objective-C Uncaught Exception处理程序，该处理程序将异常信息打印到控制台。
//Enable Crash Report API 不勾选 允许自定义崩溃报告器捕获崩溃。 崩溃日志将通过CrashReport API提供给脚本。

//other
//Use on Demand Resource 启用后，允许您使用按需资源。
//Prepare iOS for Recording 不勾选 ,选中后，将初始化麦克风录制API。 这使得录制延迟降低，但在iPhone上它仅通过耳机重新路由音频输出。
//Requires Persistent WiFi 不勾选 指定应用程序是否需要Wi-Fi连接。 iOS在应用程序运行时保持活动的Wi-Fi连接。


public class PlayerSettingsConfig
{
    public static  string newIdentifier = "com.maxbet.wondercashcasino";
    public static string applicationIdentifier = "com.maxbet.live777.inhouse";//包名
    public static string applicationIdentifier2 = "com.maxbet.live777.inhouse2";//包名
    public static string companyName = "diandian"; //公司名
    public static string productName = "live777"; //项目名
    public static string bundleVersion = "1.0.1";
    public static int bundleVersionCode = 1;

    private const string iPhoneDeveloper = "iPhone Developer";
    private const string iPhoneDistribution = "iPhone Distribution";

    public static string iOSManualProvisioningProfileType = iPhoneDeveloper;
    public static void SetConfig(BuildTarget buildTarget, bool isDebug)
    {
        BuildTargetGroup target = BuildTargetGroup.Android;
        if(buildTarget == BuildTarget.Android)
        {
            target = BuildTargetGroup.Android;
            if(isDebug)
            {
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
                PlayerSettings.productName = productName + "Debug";
                PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.Mono2x);
                PlayerSettings.SetApplicationIdentifier(target, newIdentifier);
            }
            else
            {
                PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
                PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.IL2CPP);
                PlayerSettings.SetApplicationIdentifier(target, newIdentifier);
            }
        }
        else
        {
            target = BuildTargetGroup.iOS;
            PlayerSettings.SetScriptingBackend(target, ScriptingImplementation.IL2CPP);
        }
       
        PlayerSettings.SplashScreen.show = false;
        //PlayerSettings.useSecurityBuild = true;
        PlayerSettings.enableInternalProfiler = false;
        PlayerSettings.protectGraphicsMemory = false;//启用后，如果设备和平台支持，则图形内存将免受外部读取影响。 这将确保用户无法截图。 在Android上，这需要一个支持EGL_PROTECTED_CONTENT_EXT扩展的设备
        PlayerSettings.accelerometerFrequency = 60;//ios  //加速度计更新频率。注意：构建时选项。 如果在应用程序已经运行时更改，则不起作用。
        //- PlayerSettings.actionOnDotNetUnhandledException //设置.NET未处理异常的崩溃行为。选项是ActionOnDotNetUnhandledException.Crash（应用程序几乎崩溃并强制iOS生成可由应用程序用户提交给iTunes并由开发人员检查的崩溃报告）和ActionOnDotNetUnhandledException.Silent Exit（应用程序正常退出）。
        //-PlayerSettings.advancedLicense //是否使用了高级版本？Unity可以个人或专业版的形式提供。 在Professional版本上运行时，advancedLicense将返回true。
        PlayerSettings.allowFullscreenSwitch = false;//如果启用，则允许用户使用操作系统特定的键盘快捷方式在全屏和窗口模式之间切换
                                                     //PlayerSettings.aotOptions -//其他AOT编译选项。 由AOT平台共享。
                                                     //PlayerSettings.applicationIdentifier = applicationIdentifier;   //包名
        PlayerSettings.bakeCollisionMeshes = true; //预烘烤碰撞网格在玩家身上。
        PlayerSettings.bundleVersion = bundleVersion; //iOS和Android平台之间共享的应用程序包版本。
        //--PlayerSettings.captureSingleScreen = false; //定义全屏游戏是否应使辅助显示屏变暗。
        PlayerSettings.colorSpace = ColorSpace.Gamma; //设置当前项目的渲染颜色空间。用线性空间(Color Space)的一个显着优点是，
                                                      //随着光强度的增加，提供给场景中的着色器的颜色会线性地变亮。而“伽玛(Camma)”色彩空间，随着数值上升时，亮度将迅速增强直至变成白色，这对图像质量是不利的。线性颜色空间支持PC和一些最新的移动设备上。
        PlayerSettings.companyName = companyName;
        //PlayerSettings.productName = productName;
        //PlayerSettings.d3d11FullscreenMode = D3D11FullscreenMode.ExclusiveMode;// win FullscreenWindow
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.AutoRotation;
        PlayerSettings.useAnimatedAutorotation = true;
        PlayerSettings.allowedAutorotateToPortrait = false;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft = true;
        PlayerSettings.allowedAutorotateToLandscapeRight = true;
        PlayerSettings.fullScreenMode = FullScreenMode.FullScreenWindow;//全屏
        //启用CrashReport API。启用自定义崩溃记录器来捕获崩溃。 通过CrashReport API可以为脚本提供崩溃日志。

        PlayerSettings.enableInternalProfiler = false;//启用内部分析器。启用内部分析器，收集应用程序的性能数据并将报告输出到控制台。 该报告包含每个Unity子系统在每个帧上执行的毫秒数。 数据平均跨30帧。
        // PlayerSettings.forceSingleInstance = false; //win将独立播放器限制为单个并发运行实例。这会在启动时检测同一个播放器的另一个实例是否已在运行，如果是，则会以错误消息中止。

        PlayerSettings.gpuSkinning = true; //在有能力的平台上启用GPU皮肤。以下支持GPU上的网格蒙皮：DX11，DX12，OpenGL ES 3.0，Xbox One，PS4，Nintendo Switch和Vulkan（Windows，Mac和Linux）
        //PlayerSettings.graphicsJobMode = GraphicsJobMode.Native; //不敢用 选择图形作业模式以在支持Native和Legacy图形作业的平台上使用。
        PlayerSettings.graphicsJobs = true; //启用图形作业（多线程渲染）。这使得渲染代码可以在多核心机器上的多个核心上并行分割和运行。
        //PlayerSettings.keyaliasPass
        PlayerSettings.logObjCUncaughtExceptions = false;//ObjC未捕获的异常是否被记录？启用自定义的Objective - C未捕获异常处理程序，该处理程序将向控制台打印异常信息。
        PlayerSettings.MTRendering = true; //是否启用了多线程渲染？
        PlayerSettings.muteOtherAudioSources = false;//true停止其他应用程序的音频在Unity应用程序运行时在后台播放。
        //PlayerSettings.preserveFramebufferAlpha = true;//不敢用 启用后，在帧缓冲区中保留alpha值以支持在Android上通过本机UI进行渲染。

        PlayerSettings.runInBackground = true; //默认是false 如果启用，您的游戏将在失去焦点后继续运行。
        PlayerSettings.statusBarHidden = true;//如果状态栏应该隐藏，则返回。 仅在iOS上支持; 在Android上，状态栏始终隐藏。
        PlayerSettings.stripEngineCode = false; //从您的版本中删除未使用的引擎代码（仅限IL2CPP）。如果启用此功能，则在IL2CPP版本中将删除未使用的Unity Engine 代码库的模块和类。 这将导致更小的二进制大小。 建议使用此设置，但是，如果您怀疑这会导致项目出现问题，则可能需要禁用该设置。请注意，托管程序集的字节码剥离始终对IL2CPP脚本后端启用。PlayerSettings.strippingLevel = StrippingLevel.Disabled; //托管代码剥离级别。
        PlayerSettings.SetManagedStrippingLevel(target, ManagedStrippingLevel.Disabled);
        //keep loaded shaders alive = true
        PlayerSettings.stripUnusedMeshComponents = false;//是否应该从游戏构建中排除未使用的Mesh组件？当此设置打开时，未使用的网格组件（例如切线矢量，顶点颜色等）将被删除。 这有利于游戏数据大小和运行时性能。
        PlayerSettings.use32BitDisplayBuffer = true;//使用32位显示缓冲区。
        // PlayerSettings.useAnimatedAutorotation = false;//随着设备方向改变，让操作系统自动旋转屏幕。
        PlayerSettings.useHDRDisplay = false;//将显示切换到HDR模式（如果可用）。
        //PlayerSettings.useMacAppStoreValidation = true; //不敢设置
        PlayerSettings.usePlayerLog = false; //用调试信息写一个日志文件。如果您的游戏存在问题，这对了解发生了什么很有用。 在为Apple的Mac App Store发布游戏时，建议关闭它，因为Apple可能会拒绝您的提交。
                                             //PlayerSettings.virtualRealitySplashScreen //虚拟现实特定的启动画面。
        
        PlayerSettings.allowUnsafeCode = true;
        #region vr
        PlayerSettings.virtualRealitySupported = false; //在当前构建目标上启用虚拟现实支持。
        #endregion
        PlayerSettings.visibleInBackground = false; //Windows上，如果使用全屏窗口模式，则在后台显示应用程序。

        PlayerSettings.Android.androidTVCompatibility = false;
        PlayerSettings.Android.preferredInstallLocation = AndroidPreferredInstallLocation.Auto;

       
        
        PlayerSettings.Android.startInFullscreen = true;

        PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel22;
        PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
        PlayerSettings.Android.forceSDCardPermission = false;
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.Android, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, true);
        PlayerSettings.Android.forceInternetPermission = true;
        PlayerSettings.Android.disableDepthAndStencilBuffers = false;
        PlayerSettings.Android.androidIsGame = true;
        PlayerSettings.Android.keystorePass = "maxbet123";
        PlayerSettings.Android.keyaliasName = "maxbet";
        PlayerSettings.Android.keyaliasPass = "maxbet123";
        PlayerSettings.Android.useAPKExpansionFiles = false;
        PlayerSettings.Android.bundleVersionCode = bundleVersionCode;
        PlayerSettings.Android.ARCoreEnabled = false;
        PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, true);
        PlayerSettings.iOS.requiresFullScreen = true;
        //PlayerSettings.iOS.appleEnableAutomaticSigning =  isDebug;
        PlayerSettings.SetApiCompatibilityLevel(BuildTargetGroup.iOS, ApiCompatibilityLevel.NET_4_6);
        PlayerSettings.iOS.disableDepthAndStencilBuffers = false;
        PlayerSettings.iOS.hideHomeButton = false;
        PlayerSettings.iOS.deferSystemGesturesMode = UnityEngine.iOS.SystemGestureDeferMode.All;
        PlayerSettings.iOS.allowHTTPDownload = true;
        PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK;
        PlayerSettings.iOS.targetOSVersionString = "10.0";
        PlayerSettings.iOS.buildNumber = bundleVersionCode.ToString();
        EditorUserBuildSettings.buildAppBundle = false;
#if UNITY_IOS
        Device.deferSystemGesturesMode = SystemGestureDeferMode.All;
        Device.hideHomeButton = false;
#else
        SetCGSetting();
#endif

    }

    public static void SetCGSetting()
    {
        //CGSettings.IOSSocialEnabled = false;
        //CGSettings.IOSFacebookEnabled = false;

    }
    public static void SetADEnable(bool value)
    {
        //CGSettings.AndroidAdvertisingEnabled = value;
        //CGSettings.AndroidAdvertisingAdmobEnabled = false;
        //CGSettings.AndroidAdvertisingMopubEnabled = value;
        //CGSettings.AndroidAdvertisingHuaweiEnabled = false;
        //CGSettings.AndroidAdvertisingMopubMintegarlEnabled = value;

        //CGSettings.IOSAdvertisingEnabled = value;
        //CGSettings.IOSAdvertisingMopubEnabled = value;
        //CGSettings.IOSAdvertisingEnabled = false;
        //CGSettings.IOSAdvertisingMopubMtgEnabled = value;
    }
    public static void SetAndroidEnv(string env)
    {
        EditorUserBuildSettings.buildAppBundle = false;
        //CGSettings.Environment = false;
        SetADEnable(true);
#if UNITY_ANDROID
        //centurygame.Internal.InstallSdkInAndroid.isNeedAndroidSDK = false;
#endif
        if (env.Equals("android_master"))
        {
#if UNITY_ANDROID
            //centurygame.Internal.InstallSdkInAndroid.isNeedAndroidSDK = true;
#endif
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino");
            PlayerSettings.productName = "android_master";
        }
        if (env.Equals("android_adhoc"))
        {
#if UNITY_ANDROID
            //centurygame.Internal.InstallSdkInAndroid.isNeedAndroidSDK = true;
#endif
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino");
            PlayerSettings.productName = "android_adhoc";
            //CGSettings.Environment = true;
        }
        else if (env.Equals("android_test"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino_test");
            PlayerSettings.productName = "android_test";
            //PlayerSettings.Android.useAPKExpansionFiles = true;
        }
        else if (env.Equals("uwa_android"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino_uwa");
            PlayerSettings.productName = "WonderUWA";
        }
        else if (env.Equals("uwa_android_test"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino_uwa_test");
            PlayerSettings.productName = "WonderUWATest";
        }
        else if (env.Equals("uwa_android_got_online"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino_uwa_gotonline");
            PlayerSettings.productName = "WonderUWAGotOnLine";

        }
        else if (env.Equals("android_develop"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino_develop");
            PlayerSettings.productName = "android_develop";
        }
        else if (env.Equals("android_dev_qa"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino_dev_qa");
            PlayerSettings.productName = "android_dev_qa";
        }
        else if (env.Equals("googleplay"))
        {
#if UNITY_ANDROID
            //centurygame.Internal.InstallSdkInAndroid.isNeedAndroidSDK = true;
#endif
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino");
            PlayerSettings.productName = "Wonder Cash";
            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.Android.useAPKExpansionFiles = true;
            //CGSettings.Environment = true;
        }
        else if (env.Equals("android_online"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.maxbet.wondercashcasino_online");
            PlayerSettings.productName = "Wonder Cash ONLINE";
        }
    }
    public static void SetIOSEnv(string env, ref string def)
    {
        SetADEnable(true);
        PlayerSettings.iOS.appleEnableAutomaticSigning = false;
        PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Distribution;
        //CGSettings.IosAppleSiginEnable = false;
        //CGSettings.IosPushEnable = false;
        //CGSettings.Environment = false;
        //CGSettings.IosTeamID = "3KKL24YNMG";
        iOSManualProvisioningProfileType = iPhoneDistribution;
        if (env.Equals("intranet"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, applicationIdentifier);
            PlayerSettings.iOS.appleDeveloperTeamID = "JK9HPMVJU7";
            PlayerSettings.iOS.iOSManualProvisioningProfileID = "29d1b73c-93d8-4348-b842-e1065fc5701c";
            PlayerSettings.productName = "develop_intranet";
            //CGSettings.IosProvision = "29d1b73c-93d8-4348-b842-e1065fc5701c";
            //CGSettings.IosTeamID = "DianDian Interactive USA Inc.";
        }
        else if (env.Equals("dev"))
        {
            iOSManualProvisioningProfileType = iPhoneDeveloper;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, newIdentifier);
            PlayerSettings.iOS.appleDeveloperTeamID = "3KKL24YNMG";
            PlayerSettings.iOS.iOSManualProvisioningProfileID = "86f8ea74-4462-4241-a43a-0beb91f6e1fa";
            PlayerSettings.iOS.iOSManualProvisioningProfileType = ProvisioningProfileType.Development;
            PlayerSettings.productName = "master_dev";
            //CGSettings.IosPushEnable = true;
            //CGSettings.IosAppleSiginEnable = true;
            //CGSettings.IosProvision = "86f8ea74-4462-4241-a43a-0beb91f6e1fa";
            //CGSettings.IosNotificationServiceProvision = "cc443b98-8535-47b1-8baf-d00787f4b26c";
        }
        else if (env.Equals("adhoc"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, newIdentifier);
            PlayerSettings.iOS.appleDeveloperTeamID = "3KKL24YNMG";
            PlayerSettings.iOS.iOSManualProvisioningProfileID = "9fe1dec4-965b-4886-b050-4558cb719522";
            PlayerSettings.productName = "Wonder Cash";
            //CGSettings.IosPushEnable = true;
            //CGSettings.IosAppleSiginEnable = true;
            //CGSettings.Environment = true;
            //CGSettings.IosProvision = "9fe1dec4-965b-4886-b050-4558cb719522";
            //CGSettings.IosNotificationServiceProvision = "26757698-0ff0-470b-a831-809d2ec73f1c";
        }
        else if (env.Equals("appstore"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, newIdentifier);
            PlayerSettings.iOS.appleDeveloperTeamID = "3KKL24YNMG";
            PlayerSettings.iOS.iOSManualProvisioningProfileID = "a508985c-1776-4034-977e-897a4bc2ab00";
            PlayerSettings.productName = "Wonder Cash";
            //CGSettings.IosPushEnable = true;
            //CGSettings.IosAppleSiginEnable = true;
            //CGSettings.Environment = true;
            //CGSettings.IosProvision = "a508985c-1776-4034-977e-897a4bc2ab00";
            //CGSettings.IosNotificationServiceProvision = "d9496dde-a040-4dcc-b456-43da0aca8c93";
        }
        else if (env.Equals("intranet_uwa"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, applicationIdentifier);
            PlayerSettings.iOS.appleDeveloperTeamID = "JK9HPMVJU7";
            PlayerSettings.iOS.iOSManualProvisioningProfileID = "29d1b73c-93d8-4348-b842-e1065fc5701c";
            PlayerSettings.productName = "wonder_in_uwa";
            //CGSettings.IosProvision = "29d1b73c-93d8-4348-b842-e1065fc5701c";
            //CGSettings.IosTeamID = "DianDian Interactive USA Inc.";
        }
        else if (env.Equals("qa_online_intranet"))
        {
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.iOS, applicationIdentifier2);
            PlayerSettings.iOS.appleDeveloperTeamID = "JK9HPMVJU7";
            PlayerSettings.iOS.iOSManualProvisioningProfileID = "6cddd32a-ffd2-4cae-9087-d04f6e2df753";
            PlayerSettings.productName = "master_intranet";
            //CGSettings.IosProvision = "6cddd32a-ffd2-4cae-9087-d04f6e2df753";
            //CGSettings.IosTeamID = "DianDian Interactive USA Inc.";
        }
    }
    public static void BackConfig()
    {
        PlayerSettings.runInBackground = true;
    }
}
