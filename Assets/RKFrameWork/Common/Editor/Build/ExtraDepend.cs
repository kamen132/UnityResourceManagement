using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ExtraDepend
{
    public Dictionary<string, List<string>> depends;
    public Dictionary<string, List<string>> groups;
    public Dictionary<string, string> spriteatlas;
    public ExtraDepend()
    {
        depends = new Dictionary<string, List<string>>();
        groups = new Dictionary<string, List<string>>();
    }

    public ExtraDepend(Dictionary<string, List<string>>  depends, List<List<string>> groups, Dictionary<string, string> spriteatlas)
    {
        this.spriteatlas = spriteatlas;
        var newdepends = new Dictionary<string, List<string>>();
        foreach (var it in depends)
        {
            newdepends.Add(it.Key, GetFull(it.Value));
        }
        this.depends = newdepends;

        //合并groups
        var newgroups = new Dictionary<string, Dictionary<int, bool>>();
        for( int i = 0;i<groups.Count;i++)
        {
            var arr = groups[i];
            foreach (var it in arr)
            {
                if (!newgroups.ContainsKey(it))
                    newgroups.Add(it, new Dictionary<int, bool>());
                if (!newgroups[it].ContainsKey(i))
                    newgroups[it].Add(i, true);
            }
        }

        var needmerge = new Dictionary<int, Dictionary<int, bool>>();
        foreach( var it in newgroups)
        {
            var indexs = it.Value;
            if (indexs.Count > 1)
            {
                var dics = new Dictionary<object, bool>();
                foreach (var index in it.Value)
                {
                    Dictionary<int, bool> dic;
                    if (needmerge.TryGetValue(index.Key, out dic))
                        if (!dics.ContainsKey(dic))
                            dics.Add(dic, true);
                }
                foreach (var dicit in dics)
                {
                    var dic = dicit.Key as Dictionary<int, bool>;
                    foreach (var index in dic)
                    {
                        if (indexs.ContainsKey(index.Key))
                            indexs.Add(index.Key, true);
                    }
                }
                foreach (var index in it.Value)
                    needmerge[index.Key] = it.Value;
            }
                
        }
        var merges = new Dictionary<object, bool>();
        foreach (var it in needmerge)
        {
            if (!merges.ContainsKey(it.Value))
                merges.Add(it.Value, true);
        }
        var allmerges = new Dictionary<int, bool>();
        foreach( var it in merges )
        {
            var dic = it.Key as Dictionary<int, bool>;
            var arr = ToKeyList(dic);
            arr.Sort();
           for( int i = 1;i<arr.Count;i++)
            {
                allmerges.Add(arr[i], true);
                groups[arr[0]].AddRange(groups[i]);
            }
        }
        var mergearr = ToKeyList(allmerges);
        mergearr.Sort();
        for( int i = mergearr.Count-1; i>=0;i--)
        {
            groups.RemoveAt(i);
        }

        this.groups = new Dictionary<string, List<string>>();
        foreach( var group in groups )
        {
            var reshs = GetFull(group);
            foreach (var res in group)
                this.groups.Add(res, reshs);
        }
    }

    public List<string> GetFull(List<string> arr)
    {
        var reshs = new Dictionary<string, bool>();
        foreach (var res in arr)
        {
            var ds = AssetDatabase.GetDependencies(res, true);
            foreach (var depend in ds)
            {
                if (!reshs.ContainsKey(depend))
                    reshs.Add(depend, true);
            }
        }
        CullAtlas(reshs);
        var ret = ToKeyList(reshs);
        reshs.Clear();
        return ret;
    }

    public void CullAtlas( Dictionary<string, bool> ress )
    {
        var pngs = new Dictionary<string, bool>();
        var atlass = new Dictionary<string, bool>();
        foreach (var item in ress)
        {
            var path = item.Key;
            if (spriteatlas.ContainsKey(path))
            {
                pngs.Add(path, true);
                var atlas = spriteatlas[path];
                if(!atlass.ContainsKey(atlas))atlass.Add(atlas, true);
            }
        }
        foreach( var it in pngs)
        {
            ress.Remove(it.Key);
        }
        foreach(var it in atlass)
        {
            if(!ress.ContainsKey(it.Key))ress.Add(it.Key, true);
        }
    }

    public static ExtraDepend CreateFromFile( string filename )
    {
        var lines = File.ReadAllLines(filename);
        string kind = null;
        //var ress = new Dictionary<string, bool>();
        var instance = new ExtraDepend();
        for( int i = 0;i<lines.Length;i++)
        {
            var line = lines[i].Trim();
            if (line == "group")//组用于人物特效放一组
            {
                kind = "group";
            }else if (line == "depend")//依赖用于，怪物特效怪物
            {
                kind = "depend";
            }
            else if(line == "cluster")
            {
                kind = "cluster";
            }else
            {
                if( kind == "group" )
                {
                    var arr = new List<string>(line.Split('|'));
                    arr.Sort();
                    foreach( var it in arr )
                    {
                        if(instance.groups.ContainsKey(it))
                        {
                            instance.groups[it].AddRange(arr);
                        }else
                            instance.groups.Add(it, new List<string>(arr));
                    }
                }else if(kind == "depend")
                {
                    var arr = line.Split('=');
                    Debug.Assert(arr.Length == 2, "Error Format");
                    instance.depends.Add(arr[0], new List<string>(arr[1].Split('|')));
                }
            }

        }
        return instance;
    }

    private void Merge( Dictionary<string, bool> dic, List<string> arr)
    {
        for( int i = 0;i<arr.Count;i++)
        {
            if (!dic.ContainsKey(arr[i]))
            {
                dic.Add(arr[i], true);
                //Merge(dic, AssetDatabase.GetDependencies(arr[i], true));
                //MergeDepend(dic, arr[i]);
            }
                
        }
    }

    /*private void Merge(Dictionary<string, bool> dic, string[] arr)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            if (!dic.ContainsKey(arr[i]))
                dic.Add(arr[i], true);
        }
    }*/

    private string[] ToKeys(Dictionary<string, bool> dic)
    {
        return ToKeyList(dic).ToArray();
    }

    private List<T> ToKeyList<T>(Dictionary<T, bool> dic)
    {
        var arr = new List<T>();
        foreach (var it in dic)
        {
            arr.Add(it.Key);
        }
        return arr;
    }

    /*private string[] FindUsedAtlas(Dictionary<string, bool> ress, string pathname)
    {
        var atlas = new Dictionary<string, bool>();
        foreach( var item in ress )
        {
            var path = item.Key;
            if(spriteatlas.ContainsKey(path) && !atlas.ContainsKey(spriteatlas[path]) && path != pathname)
            {
                atlas.Add(spriteatlas[path], true);
            }
        }
        return ToKeys(atlas);
    }*/

    public Dictionary<string, bool> MergeDepend(Dictionary<string, bool> ress, string pathname)
    {
        var ods = AssetDatabase.GetDependencies(pathname, true);
        var ds = new List<string>();

        foreach (var d in ods)
            if (!Directory.Exists(d))
                ds.Add(d);
            //else
            //    Debug.LogWarningFormat("GetDependencies contain directory[{0}]########################{1}", pathname, d);

        //TODOcluster先不实现
        Merge(ress, new List<string>(ds.ToArray()));
        if (groups.ContainsKey(pathname))
        {
            Merge(ress, groups[pathname]);
        }
        if (depends.ContainsKey(pathname))
        {
            Merge(ress, depends[pathname]);
        }
        CullAtlas(ress);
       // Merge(ress, new List<string>(FindUsedAtlas(ress, pathname)));
        return ress;
    }
    public string[] GetDepend( string pathname )
    {
        var ress = new Dictionary<string, bool>();
        MergeDepend(ress, pathname);
        return ToKeys(ress);
    }
}
