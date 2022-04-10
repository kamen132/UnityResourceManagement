using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
namespace GMResChecker
{
    public class CompressAnimationRes : ResCheckBaseSubWindowEditor
    {
        private List<string> paths = new List<string>()
        {
            "Assets/WorkAssets/Effect",
        };
        private List<Object> pathObjs = null;
        public override void OnGUIDraw()
        {
            EditorGUILayout.LabelField("--------------------------------------------------------------------------------");

            if(pathObjs == null)
            {
                pathObjs = new List<Object>();
                foreach (var item in paths)
                {
                    var obj = ResCheckJenkinsEntrance.LoadAssetAtPath(item);
                    if(obj != null)
                    {
                        pathObjs.Add(obj);
                    }
                }
            }
            EditorGUILayout.LabelField("处理的路径：");
            foreach (var item in pathObjs)
            {
                EditorGUILayout.ObjectField(item, item.GetType(), true, GUILayout.Width(400));
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("压缩Animation", GUILayout.Width(400), GUILayout.Height(20)))
            {
                foreach (var item in paths)
                {
                    string path = Path.GetFullPath(item);
                    string[]  subPaths = FileManager.GetAllFilesInFolder(path);
                    foreach(var subItem in subPaths)
                    {
                        string itemAssetPath = PathUtil.GetAssetPath(subItem);
                        if (itemAssetPath.EndsWith(".anim"))
                        {
                            
                            AnimationClip clip = ResCheckJenkinsEntrance.LoadAssetAtPath(itemAssetPath) as AnimationClip;
                            CompressAnimationClip(clip);
                        }
                        
                    }
                }
                AssetDatabase.SaveAssets();
            }
           
            EditorGUILayout.EndHorizontal();
        }

        //移除scale
        public static void RemoveAnimationCurve(AnimationClip animClip)
        {
            foreach (EditorCurveBinding curveBinding in AnimationUtility.GetObjectReferenceCurveBindings(animClip))
            {
                string tName = curveBinding.propertyName.ToLower();
                if (tName.Contains("scale"))
                {
                    AnimationUtility.SetEditorCurve(animClip, curveBinding, null);
                }
            }
            EditorUtility.SetDirty(animClip);
            AssetDatabase.SaveAssets();
        }

        public static void CompressAnimationClip(AnimationClip _clip)
        {
            var floatFormat = "f3";
            EditorCurveBinding[] curveBinding = AnimationUtility.GetCurveBindings(_clip);
            foreach (var bind in curveBinding)
            {
                var curve = AnimationUtility.GetEditorCurve(_clip, bind);
                var keys = curve.keys;
                for (int index = 0; index < keys.Length; index++)
                {
                    var keyframe = keys[index];
                    keyframe.value = float.Parse(keyframe.value.ToString(floatFormat));
                    keyframe.inTangent = float.Parse(keyframe.inTangent.ToString(floatFormat));
                    keyframe.outTangent = float.Parse(keyframe.outTangent.ToString(floatFormat));
                    keyframe.inWeight = float.Parse(keyframe.inWeight.ToString(floatFormat));
                    keyframe.outWeight = float.Parse(keyframe.outWeight.ToString(floatFormat));
                    keyframe.time = float.Parse(keyframe.time.ToString(floatFormat));

                    keys[index] = keyframe;
                }
                curve.keys = keys;
                AnimationUtility.SetEditorCurve(_clip, bind, curve);
            }
            EditorUtility.SetDirty(_clip);
        }

    }
}

