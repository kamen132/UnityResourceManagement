// using Spine.Unity;
// using UnityEngine;
//
// public class UISpineList : MonoBehaviour
// {
//     public enum EndBehave
//     {
//         Hide,
//         Loop,
//         None
//     }
//     
//     public Texture2D texture2D;
//     public TextAsset atlas;
//     public TextAsset skeleton;
//     [SerializeField]
//     public Material mat;
//     public string[] animations;
//     public EndBehave endBeh = EndBehave.None;
//     public bool isNeedCanvas = true;
//     public bool disableRaycastTarget = false;
//     private SkeletonGraphic skeletonGraphic = null;
//     private int curAnimationIndex = 0;
//     public bool playOnAwake = true;
//     void Start()
//     {
//         if (playOnAwake)
//         {
//             Init();
//         }
//     }
//
//     public SkeletonGraphic Init()
//     {
//         var go = SpineUtils.CreateSpine(texture2D, atlas, skeleton, mat, mat);
//         go.name = "Spine";
//         
//         var rectTransform = go.GetComponent<RectTransform>();
//         rectTransform.SetParent(transform);
//         rectTransform.sizeDelta = Vector2.zero;
//         rectTransform.anchoredPosition3D = Vector3.zero;
//         rectTransform.localRotation = Quaternion.Euler(Vector3.zero);
//         rectTransform.localScale = Vector3.one;
//
//         if(isNeedCanvas)
//         {
//             var canvas = gameObject.GetComponent<Canvas>();
//             if (canvas == null)
//             {
//                 gameObject.AddComponent<Canvas>();
//             }
//         }
//         
//         skeletonGraphic = go.GetComponent<SkeletonGraphic>();
//
//         skeletonGraphic.AnimationState.Complete += entry =>
//         {
//             PlayNext();
//         };
//         
//         PlayNext();
//
//         return skeletonGraphic;
//     }
//
//     private void PlayNext()
//     {
//         if (curAnimationIndex + 1 > animations.Length)
//         {
//             if (endBeh == EndBehave.Hide)
//             {
//                 transform.SetActiveVirtual(false);
//             }
//             return;
//         }
//         
//         bool isLoop = curAnimationIndex == animations.Length - 1 && endBeh == EndBehave.Loop;
//         skeletonGraphic.AnimationState.SetAnimation(1, animations[curAnimationIndex], isLoop);
//
//         curAnimationIndex++;
//     }
//
//     private void OnDestroy()
//     {
//         if(skeletonGraphic)
//         {
//             skeletonGraphic.Clear();
//             //skeletonGraphic.ClearData();
//             skeletonGraphic = null;
//         }
//     }
// }
//
//
//
//
//
//
//
//
//
//
//
//
