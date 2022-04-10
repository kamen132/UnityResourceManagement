//
// using System.Collections.Generic;
// using BestHTTP.JSON;
// using UnityEngine;
//
// public class PlayerPrefsHelper : MonoBehaviour
// {
//
//     public string Key
//     {
//         get
//         {
//             return "car";
//         }
//     }
//     private Dictionary<string, string> baseData = new Dictionary<string, string>();
//     void OnInit()
//     {
//         string data = PlayerPrefs.GetString(Key, "");
//         if (string.IsNullOrEmpty(data) == false)
//         {
//             baseData = (Dictionary<string, string>)Json.Decode(data);
//         }
//     }
//     // Use this for initialization
//     void Start()
//     {
//         OnInit();
//     }
//
//     public string GetData(string key, string defaultValue)
//     {
//         if (key == null)
//         {
//             return null;
//         }
//         if (baseData.ContainsKey(key))
//         {
//             return baseData[key];
//         }
//         return defaultValue;
//     }
//     public void SetData(string key, string value, bool isSave = false)
//     {
//         if (key == null)
//         {
//             return;
//         }
//         baseData[key] = value;
//         if (isSave)
//         {
//             Save();
//         }
//     }
//     public void Save()
//     {
//         if(baseData.Count == 0)
//         {
//             return;
//         }
//         string value = Json.Encode(baseData);
//         if (string.IsNullOrEmpty(value) == false)
//         {
//             PlayerPrefs.SetString(Key, value);
//             PlayerPrefs.Save();
//         }
//
//     }
// 	private void OnApplicationQuit()
// 	{
//         Save();
// 	}
// }
