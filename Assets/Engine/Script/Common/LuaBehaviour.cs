using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using NLua;
using System.ComponentModel;
using System.Text;

/// <summary>
/// LuaBehaviour
/// @author 小陆  QQ:2604904
/// </summary>
public class LuaBehaviour : MonoBehaviour
{
    public bool usingUpdate = false;
    protected bool isLuaReady = false;
    private string script = "";

    private Lua env = API.env;
    protected LuaTable table;

    protected List<MissionPack> MissionList = new List<MissionPack>();   

    protected void Update()
    {
        if (MissionList.Count > 0)
        {
            MissionPack pack= MissionList[0];
            MissionList.RemoveAt(0);
            pack.Call();
        }

        if (usingUpdate)
        {
            CallMethod("Update");
        }
    }

    public void AddMission(LuaFunction func,params object[] args)
    {
        MissionList.Add(new MissionPack(func, args));
    }


    public string AssetPath
    {
        get
        {
            string target = string.Empty;
            if (Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXEditor)
            {
                target = "iphone";
            }
            else
            {
                target = "android";
            }
            return Application.persistentDataPath + "/asset/" + target + "/";
        }
    }

    public string AssetRoot
    {
        get
        {
            return Application.persistentDataPath + "/";
        }
    }

    public void LoadBundle(string fname, Callback<string, AssetBundle> handler)
    {
        if (API.BundleTable.ContainsKey(fname))
        {
            AssetBundle bundle = API.BundleTable[fname] as AssetBundle;
            if (handler != null) handler(name, bundle);
        }
        else
        {
            StartCoroutine(onLoadBundle(fname, handler));
        }
    }

    public void UnLoadAllBundle()
    {
        foreach (AssetBundle bundle in API.BundleTable.Values)
        {
            bundle.Unload(true);
        }
        API.BundleTable.Clear();
    }


    IEnumerator onLoadBundle(string name, Callback<string, AssetBundle> handler)
    {
        string uri = "file:///" + AssetPath + name.ToLower() + ".assetbundle";

        WWW www = new WWW(uri);
        yield return www;
        if (www.error != null)
        {
            Debug.Log("Warning erro: " + uri);
            Debug.Log("Warning erro: " + "loadStreamingAssets");
            Debug.Log("Warning erro: " + www.error);
            StopCoroutine("onLoadBundle"); 
            yield break;
        }
        while (!www.isDone)
        {
            yield return new WaitForEndOfFrame();
        }
        byte[] data = www.bytes;

        AssetBundle bundle = AssetBundle.CreateFromMemoryImmediate(data); 

        yield return new WaitForEndOfFrame();

        try
        {
            API.BundleTable[name]=bundle;
            if (handler != null) handler(name, bundle);
        }
        catch (NLua.Exceptions.LuaException e)
        {            
            Debug.LogError(FormatException(e), gameObject);
        }
    }
  
     

    public void DestroyMe()
    {
        Destroy(gameObject);
    }
    protected void OnDestroy()
    {    

        CallMethod("OnDestroy");

        if (table != null)
        {
            table.Dispose();
        }
    }

    public IEnumerator RunCoroutine()
    {        
        System.Object[] result = new System.Object[0];
        if (table == null) return (IEnumerator) result[0];
        result = CallMethod("RunCoroutine");
        return (IEnumerator)result[0];   
    }

    //加载脚本文件
    public void DoFile(string filename)
    {
        if (filename.EndsWith(".lua"))
        {
            script = Application.persistentDataPath + "/lua/" + filename;
        }
        else
        {
            script = Application.persistentDataPath + "/lua/" + filename + ".lua"; 
        }
        try
        {
           // Debug.Log("DoFile:" + script);
            object[] chunk = env.DoFile(script);

            if (chunk != null && chunk[0] != null)
            {
                table = chunk[0] as LuaTable;
                table["this"] = this;
                table["transform"] = transform;
                table["gameObject"] = gameObject;

                CallMethod("Start"); 

                isLuaReady = true;               
            }           
        }
        catch (NLua.Exceptions.LuaException e)
        {
            isLuaReady = false;
            Debug.LogError(FormatException(e), gameObject);
        }
    }
    //获取绑定的lua脚本
    public LuaTable GetChunk()
    {
        return table;
    }

    //设置lua脚本可直接使用变量
    public void SetEnv(string key,object val,bool isGlobal)
    {
        if (isGlobal)
        {
            env[key] = val; 
        }
        else
        {
            if (table != null)
            {
                table[key] = val;
            }
        }
    }
      //协程
    public void RunCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
    {
        StartCoroutine(doCoroutine(ins, func, args));
    }

    private IEnumerator doCoroutine(YieldInstruction ins, LuaFunction func, params System.Object[] args)
    {
        yield return ins;
        func.Call(args);
    }


    public System.Object[] CallMethod(string function, params System.Object[] args)
    {
        if (table == null) return null;
        LuaFunction func = table.RawGet(function) as LuaFunction;
        if (func == null) return null;
        try
        {
            if (args != null)
            {
                return func.Call(args);
            }
            else
            {
                return func.Call();
            }

        }
        catch (NLua.Exceptions.LuaException e)
        {
            Debug.LogWarning(FormatException(e), gameObject);            
        }
        return null;
    }

    public System.Object[] CallMethod(string function)
    {
        return CallMethod(function, null); 
    }

    private System.Object[] Call(string function, params System.Object[] args)
    {
        System.Object[] result = new System.Object[0];
        if (env == null || !isLuaReady) return result;
        LuaFunction lf = env.GetFunction(function);

        if (lf == null) return result;
        try
        {
            if (args != null)
            {
                result = lf.Call(args);
            }
            else
            {
                result = lf.Call();
            }
        }
        catch (NLua.Exceptions.LuaException e)
        {
            Debug.LogError(FormatException(e), gameObject);
        }
        return result;
    }

    private System.Object[] Call(string function)
    {
        return Call(function, null);
    }

    public static string FormatException(NLua.Exceptions.LuaException e)
    {
        string source = (string.IsNullOrEmpty(e.Source)) ? "<no source>" : e.Source.Substring(0, e.Source.Length - 2);
        return string.Format("{0}\nLua (at {2})", e.Message, string.Empty, source);
    }

    #region 消息中心
    //添加消息侦听
    public void AddListener(string eventType, Callback handler)
    {
        Messenger.AddListener(eventType, handler);
    }

    public void AddListener2(string eventType, Callback<object> handler)
    {
        Messenger.AddListener<object>(eventType, handler);
    }

    //移除一事件侦听
    public void RemoveListener(string eventType, Callback handler)
    {
        Messenger.RemoveListener(eventType, handler);
    }
    public void RemoveListener2(string eventType, Callback<object> handler)
    {
        Messenger.RemoveListener<object>(eventType, handler);
    }

    //触发消息广播
    public void Broadcast(string eventType)
    {
        Messenger.Broadcast(eventType);
    }

    public void Broadcast(string eventType, object args)
    {
        Messenger.Broadcast<object>(eventType, args);
    }
    #endregion   

}


public struct  MissionPack
{
    public LuaFunction func;
    public object[] args;

    public MissionPack(LuaFunction _func,params object[] _args)
    {
        func = _func;
        args = _args;
    }

    public object[] Call()
    {
        if (args != null)
        {
            return func.Call(args);
        }

        return func.Call();
    }
}
