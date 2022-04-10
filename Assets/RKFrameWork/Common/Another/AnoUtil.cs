using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;

public static class AnoUtil {
    public static byte[] oneByteArray = new byte[1];
    public static int ReadByteBuffer(this Stream stream)
    {
        Contract.Ensures(Contract.Result<int>() >= -1);
        Contract.Ensures(Contract.Result<int>() < 256);
        oneByteArray[0] = 0;
        int r = stream.Read(oneByteArray, 0, 1);
        if (r == 0)
            return -1;
        return oneByteArray[0];
    }
    public static T Pop<T>(this List<T> list)
    {
        T result = default(T); 
        int index = list.Count - 1;
        if (index >=0)
        {
            result = list[index];
            list.RemoveAt(index);
            return result;
        }

        return result;
    }
    public static List<T>  Copy<T>(this List<T> list)
    {
        List<T> result = new List<T>();
        int count = list.Count;
        for(int i = 0;i < count;i++)
        {
            result.Add(list[i]);
        }
        return result;
    }
}
