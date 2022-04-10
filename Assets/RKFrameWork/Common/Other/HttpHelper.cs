// using BestHTTP;
// using BestHTTP.Logger;
//
// //比较公共的类，涉及到第三方
// public class HttpHelper
// {
//     public static HTTPRequest CreateHttpRequest(string url, string proxyUrl, string cookies, OnRequestFinishedDelegate callBack)
//     {
//         //var stats = HTTPManager.GetGeneralStatistics(BestHTTP.Statistics.StatisticsQueryFlags.All);
//         HTTPRequest httpRequest = new HTTPRequest(CommonUtils.GetSystemUri(url), callBack);
//         if(string.IsNullOrEmpty(proxyUrl) == false && proxyUrl.Length != 0)
//         {
//             httpRequest.Proxy = new HTTPProxy(CommonUtils.GetSystemUri(proxyUrl));
//         }
//         if (string.IsNullOrEmpty(cookies) == false && cookies.Length != 0)
//         {
//             httpRequest.Cookies.Add(new BestHTTP.Cookies.Cookie("Cookie", cookies));
//         }
//         return httpRequest;
//     }
//
//     public static void SetHttpLoggerLevel(int value)
//     {
//         HTTPManager.Logger.Level = (Loglevels)value;
//     }
//     public static void SetHttpThreaded(bool value)
//     {
//         HTTPUpdateDelegator.IsThreaded = value;
//     }
//     public static void SetHttpThreadFrequencyInMS(int value)
//     {
//         HTTPUpdateDelegator.ThreadFrequencyInMS = value;
//     }
//     public static void SetMaxConnectionPerServer(int value)
//     {
//         HTTPManager.MaxConnectionPerServer = (byte)value;
//     }
// }