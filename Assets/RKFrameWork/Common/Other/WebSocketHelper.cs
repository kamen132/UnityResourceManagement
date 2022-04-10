// #if !BESTHTTP_DISABLE_WEBSOCKET
// using BestHTTP;
// using BestHTTP.WebSocket;
// using System;
// using System.IO;
// using System.Linq;
//
// public class WebSocketHelper
// {
//     private WebSocket webSocket = null;
//    
//     private OnWebSocketMessageDelegate messageCallBackForLua = null;
//     private OnWebSocketOpenDelegate openCallBackForLua = null;
//     private OnWebSocketErrorDelegate errorCallBackForLua = null;
//  
//    
//
//     public void Send(string message)
//     {
//         if (webSocket != null)
//         {
//             webSocket.Send(message);
//         }
//     }
//
//     public void CloseWebSocket(int code, string message)
//     {
//         if (webSocket != null)
//         {
//             webSocket.Close((UInt16) code, message);
//         }
//         Dispose();
//     }
//
//     public void OpenWebSocket()
//     {
//         if (webSocket != null)
//         {
//             webSocket.Open();
//         }
//     }
//
//    
//
//     public void OnOpenForLua(OnWebSocketOpenDelegate callBack)
//     {
//         openCallBackForLua = callBack;
//     }
//     public void OnErrorForLua(OnWebSocketErrorDelegate callBack)
//     {
//         errorCallBackForLua = callBack;
//     }
//     public void OnMessageForLua(OnWebSocketMessageDelegate callBack)
//     {
//          messageCallBackForLua = callBack;
//     }
//
//
//     public void Send(short msgID,byte[] msgBody)
//     {
//         if (webSocket != null)
//         {
//             byte[] msgHeader = BitConverter.GetBytes(msgID);
//             MemoryStream stream = new MemoryStream(msgHeader.Length + msgBody.Length);
//             stream.Write(msgHeader, 0, msgHeader.Length);
//             stream.Write(msgBody, 0, msgBody.Length);
//             webSocket.Send(stream.ToArray());
//         }
//     }
//     public void Dispose()
//     {
//
//     }
// }
// #endif