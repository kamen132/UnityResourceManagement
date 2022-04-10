using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BundleEditor
{
    [MenuItem("Kamen/打包")]
    public static void Build()
    {
        BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath, BuildAssetBundleOptions.ChunkBasedCompression,
            EditorUserBuildSettings.activeBuildTarget);
        AssetDatabase.Refresh();
    }
}
