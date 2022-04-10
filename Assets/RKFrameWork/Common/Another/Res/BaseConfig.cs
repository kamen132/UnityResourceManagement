using System.Collections.Generic;
using UnityEngine;

namespace Majic.CM
{

    public class BaseConfig
    {
        public static bool DOWNLOAD_AB = false;
        public static bool RES_FROM_AB = true;
        public static bool UNITY_IOS = false;
        public static bool UNITY_ANDROID = true;

        public static bool IsDevelopTest()
        {
            return true;
        }

    }
}

