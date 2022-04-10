
public static class MacroDefinition
{
    public static bool UNITY_ANDROID()
    {
#if UNITY_ANDROID
        return true;
#else
        return false;
#endif
    }

    //ios环境
    public static bool UNITY_IOS()
    {
#if UNITY_IOS
        return true;
#else
        return false;
#endif
    }

    //编辑器环境
    public static bool UNITY_EDITOR()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }

    public static bool DOWNLOAD_AB()
    {
#if DOWNLOAD_AB
        return true;
#else
        return false;
#endif
    }
    public static bool RES_FROM_AB()
    {
#if RES_FROM_AB
        return true;
#else
        return false;
#endif
    }
    public static bool LUA_FROM_AB()
    {
#if LUA_FROM_AB
        return true;
#else
        return false;
#endif
    }
    public static bool USE_HOTFIX_DLL()
    {
#if USE_HOTFIX_DLL
        return true;
#else
        return false;
#endif
    }
}
