//
// using Spine.Unity;
// using System.Text;
// using UnityEngine;
//
// public class SpineUtils
// {
//     private static readonly int STRAIGHT_ALPHA_PARAM_ID = Shader.PropertyToID("_StraightAlphaInput");
//     private static readonly string ALPHAPREMULTIPLY_ON_KEYWORD = "_ALPHAPREMULTIPLY_ON";
//
//     public static GameObject CreateSpine( Texture2D texture, TextAsset atlasTextAsset, TextAsset skeletonDataFile, Material graphicMat, Material atlasMat)
//     {
//         GameObject go = new GameObject();
//         go.name = texture.name;
//         Material mat = CreateMat(atlasMat.shader);
//         mat.mainTexture = texture;
//         Material[] materials = new Material[] { mat };
//         SpineAtlasAsset spineAtlasAsset = SpineAtlasAsset.CreateRuntimeInstance(atlasTextAsset, materials, true);
//         SkeletonDataAsset atlasAssets = SkeletonDataAsset.CreateRuntimeInstance(skeletonDataFile, spineAtlasAsset, true);
//         var skeletonGraphic = SkeletonGraphic.AddSkeletonGraphicComponent(go, atlasAssets, graphicMat);
//         return go;
//     }
//
//     
//     public static Material CreateMat(Shader shader)
//     {
//         Material mat = new Material(shader);
//         if (mat.HasProperty(STRAIGHT_ALPHA_PARAM_ID))
//         {
//             mat.SetInt(STRAIGHT_ALPHA_PARAM_ID, 1);
//         }
//         else
//         {
//              mat.DisableKeyword(ALPHAPREMULTIPLY_ON_KEYWORD);
//         }
//         return mat;
//     }
// }