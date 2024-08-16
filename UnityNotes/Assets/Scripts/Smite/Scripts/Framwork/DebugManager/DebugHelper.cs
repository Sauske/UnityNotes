using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;

#if UNITY_EDITOR
using UnityEditor;
#endif

//adb logcat -s Unity
//adb shell dumpsys meminfo com.tencent.tmgp.sgame
//adb shell top -m 10 -s cpu
//adb forward tcp:54999 localabstract:Unity-com.tencent.tmgp.sgame
public enum SLogTypeDef
{
    LogType_None,   //不输出log
    LogType_System, //系统默认方式
    LogType_Custom, //自定义方式
};

public enum SLogCategory
{
    Normal,		//普通log
	Msg,		//输入消息
	Actor,		//Actor状态
	Rvo,		//寻路状态
    Skill,		//技能相关
    Motion,		//移动相关
    Misc,		//杂项
    Fow,
    Max
};

public class DebugHelper : MonoBehaviour
{

	#if UNITY_IPHONE
	[DllImport ("__Internal")]
	private static extern void _dtLog(string logStr);
	#endif
    static DebugHelper instance = null;
    static SLogTypeDef logMode = SLogTypeDef.LogType_Custom;

    static SLogObj[] s_logers = new SLogObj[(int)SLogCategory.Max];

    //给外面选择可以配置log方式
    public SLogTypeDef cfgMode = SLogTypeDef.LogType_System;

    //日志开关，外部可以通过这个控制日志的开启与关闭
    public static bool enableLog = true;


    public static void OpenLoger(SLogCategory logType, string logFile)
    {
        var idx = (int)logType;
        s_logers[idx].Flush();
        s_logers[idx].Close();
        s_logers[idx].TargetPath = logFile;
    }

    public static void CloseLoger(SLogCategory logType)
    {
        var idx = (int)logType;
        s_logers[idx].Flush();
        s_logers[idx].Close();
		s_logers[idx].TargetPath = null;
    }

	public static void BeginLogs()
	{
		CloseLogs();
		
		string folder = logRootPath;
		string dt = DateTime.Now.ToString("yyyyMMdd_HHmmss");
		
		OpenLoger(SLogCategory.Normal, string.Format("{0}/{1}_normal.log", folder, dt));
		OpenLoger(SLogCategory.Skill, string.Format("{0}/{1}_skill.log", folder, dt));
		OpenLoger(SLogCategory.Misc, string.Format("{0}/{1}_misc.log", folder, dt));
		OpenLoger(SLogCategory.Msg, string.Format("{0}/{1}_msg.log", folder, dt));
		OpenLoger(SLogCategory.Actor, string.Format("{0}/{1}_actor.log", folder, dt));
		OpenLoger(SLogCategory.Rvo, string.Format("{0}/{1}_rvo.log", folder, dt));
        OpenLoger(SLogCategory.Fow, string.Format("{0}/{1}_fow.log", folder, dt));
	}
	
	public static void CloseLogs()
	{
		CloseLoger(SLogCategory.Normal);
		CloseLoger(SLogCategory.Skill);
		CloseLoger(SLogCategory.Misc);
		CloseLoger(SLogCategory.Msg);
		CloseLoger(SLogCategory.Actor);
		CloseLoger(SLogCategory.Rvo);
        CloseLoger(SLogCategory.Fow);
	}

	public static void ClearLogs(int passedMinutes = 60)
	{
		DateTime now = DateTime.Now;
		DirectoryInfo dirInfo = new DirectoryInfo(logRootPath);
		if (dirInfo.Exists)
		{
			string[] files = Directory.GetFiles(dirInfo.FullName, "*.log", SearchOption.TopDirectoryOnly);
			for (int i = 0; i < files.Length; ++i)
			{
				try
				{
					string filePath = files[i];
					FileInfo fi = new FileInfo(filePath);
					if (fi.Exists && (now - fi.CreationTime).TotalMinutes > passedMinutes)
					{
						File.Delete(filePath);
					}
				}
				catch
				{
				}
			}
		}
	}

	public static SLogObj GetLoger(SLogCategory logType)
	{
		return s_logers[(int)logType];
	}

	public static string GetLogerPath(SLogCategory logType)
	{
		return s_logers[(int)logType].LastTargetPath;
	}

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    static public void EditorAssert(bool InCondition, string InFormat = null, params object[] InParameters)
    {
        Assert(InCondition, InFormat, InParameters);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_IPHONE")]
    static public void Assert(bool InCondition)
    {
        Assert(InCondition, null, null);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_IPHONE")]
    static public void Assert(bool InCondition, string InFormat )
    {
        Assert(InCondition, InFormat, null);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_IPHONE")]
    static public void Assert(bool InCondition, string InFormat, params object[] InParameters )
    {
        if (enableLog)
        {
            // 虽然有了Conditional了 但是保险起见 还是用宏包一层 双管齐下 双保险
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || FORCE_LOG || UNITY_IPHONE
            if (!InCondition)
            {
                try
                {
                    string failedMessage = null;

                    if (!string.IsNullOrEmpty(InFormat))
                    {
                        try
                        {
                            if (InParameters != null)
                            {
                                failedMessage = string.Format(InFormat, InParameters);
                            }
                            else
                            {
                                failedMessage = InFormat;
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
#if UNITY_ANDROID || UNITY_IPHONE
                else
                {
                    failedMessage = string.Format(" no assert detail, stacktrace is :{0}", Environment.StackTrace);
                }
#endif

#if UNITY_EDITOR || UNITY_STANDALONE
                    if (failedMessage != null)
                    {
                        Debug.LogError("Assert failed! " + failedMessage);
                    }
                    else
                    {
                        Debug.LogError("Assert failed!");
                    }

                    string msg = "Assert failed! ";
                    if (!string.IsNullOrEmpty(failedMessage))
                    {
                        msg += failedMessage;
                    }

                    var trace = new System.Diagnostics.StackTrace();
                    var frames = trace.GetFrames();
                    for (int i = 0; i < frames.Length; ++i)
                    {
                        msg += frames[i].ToString();
                    }

                    try
                    {
                        LogInternal(SLogCategory.Normal, msg);
                    }
                    catch (Exception)
                    {
                    }
#else
                if (failedMessage != null)
                {
                    var str = "Assert : " + failedMessage;

                    CustomLog(str);
                }
                else
                {
                    Debug.LogWarning("Assert failed!");
                }
#endif
                }
                catch (Exception)
                {
                    // ignore all exception in logger.
                }
            }
#endif
        }
    }

	[Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_IPHONE")]
	public static void CustomLog(string str, params object[] InParameters)
    {
        if (enableLog)
        {
            try
            {
#if  UNITY_ANDROID && !UNITY_EDITOR 
        string packageName = "com.tencent.tmgp.sgame.SGameUtility";
#if USE_CE
        packageName = "com.tencent.tmgp.sgamece.SGameUtility";
#endif
            if(InParameters != null)
            {
                str = string.Format(str, InParameters);
            }

            str = DateTime.Now.ToString("T") + " " + str;

            Debug.Log(str);

            AndroidJavaClass ajc = new AndroidJavaClass(packageName);
                 ajc.CallStatic("dtLog", str);
                 ajc.Dispose();
#elif UNITY_IPHONE 
            if(InParameters != null)
            {
                str = string.Format(str, InParameters);
            }

            str = DateTime.Now.ToString("T") + " " + str + "\n";

           // Debug.Log(str);
            _dtLog(str);



#endif

#if UNITY_EDITOR
                if (InParameters != null)
                {
                    str = string.Format(str, InParameters);
                }

                str = DateTime.Now.ToString("T") + " " + str;

                Debug.Log(str);
#endif
            }
            catch (Exception)
            {
            }
        }
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG"), Conditional("UNITY_ANDROID"), Conditional("UNITY_IPHONE")]
    public static void CustomLog(string str)
    {
        CustomLog(str, null);
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void LogInternal(SLogCategory logType, string logmsg)
    {
        if (enableLog)
        {
            s_logers[(int)logType].Log(logmsg);
        }
    }


    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void Log(string logmsg)
    {
        Debug.Log(logmsg);

//        if (enableLog)
//        {
//#if UNITY_EDITOR
//            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
//            {
//                Debug.Log(logmsg);
//            }
//#endif


//#if UNITY_STANDALONE_WIN || UNITY_EDITOR
//            Debug.Log(logmsg);
//#endif

//            if (DebugHelper.logMode == SLogTypeDef.LogType_Custom)
//            {
//                LogInternal(SLogCategory.Normal, logmsg);
//            }
//        }
    }

    public static string logRootPath
    {
        get
        {
            if( CachedLogRootPath == null )
            {
#if UNITY_EDITOR
                string folder = string.Format("{0}/../../Replay/", Application.dataPath);
#elif UNITY_STANDALONE
                string folder = string.Format("{0}/../Replay/", Application.dataPath);
#else
                string folder = string.Format("{0}/Replay/", Application.persistentDataPath);
#endif

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                CachedLogRootPath = folder;
            }

            return CachedLogRootPath;
        }
    }

    private static string CachedDownloadReplayPath;

    public static string GetDownloadReplayPathWithoutCreate()
    {
        return CachedDownloadReplayPath;
    }

    public static string downloadReplayPath
    {
        get
        {
            if (CachedDownloadReplayPath == null)
            {
#if UNITY_EDITOR
                string folder = string.Format("{0}/../../DownloadReplay/", Application.dataPath);
#elif UNITY_STANDALONE
                string folder = string.Format("{0}/../DownloadReplay/", Application.dataPath);
#else
                string folder = string.Format("{0}/DownloadReplay/", Application.persistentDataPath);
#endif

                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }

                CachedDownloadReplayPath = folder;
            }

            return CachedDownloadReplayPath;
        }
    }

    private static string CachedLogRootPath;

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void LogMisc(string logmsg)
    {
        if (enableLog)
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Debug.Log(logmsg);
            }
#endif

            if (DebugHelper.logMode == SLogTypeDef.LogType_System)
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
                Debug.Log(logmsg);
#endif
            }
            else if (DebugHelper.logMode == SLogTypeDef.LogType_Custom)
            {
                LogInternal(SLogCategory.Misc, logmsg);
            }
        }
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void LogError(string errmsg)
    {
        Debug.LogError(errmsg);
        //if (enableLog)
        //{
        //    Debug.LogError(errmsg);
        //}
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void LogWarning(string warmsg)
    {
        if (enableLog)
        {
            Debug.LogWarning(warmsg);
        }
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void ConsoleLog(string logmsg)
    {
        if (enableLog)
        {
            Debug.Log(logmsg);
        }
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void ConsoleLogError(string logmsg)
    {
        if (enableLog)
        {
            Debug.LogError(logmsg);
        }
    }

    [Conditional("UNITY_STANDALONE_WIN"), Conditional("UNITY_EDITOR"), Conditional("FORCE_LOG")]
    public static void ConsoleLogWarning(string logmsg)
    {
        if (enableLog)
        {
            Debug.LogWarning(logmsg);
        }
    }

    private void Awake()
    {
        DebugHelper.Assert(instance == null);
        instance = this;
        DebugHelper.logMode = cfgMode;

        int cnt = (int)SLogCategory.Max;
        for (int i = 0; i < cnt; i++)
        {
            s_logers[i] = new SLogObj();
        }
    }

    protected void OnDestroy()
    {
        int cnt = (int)SLogCategory.Max;
        for (int i = 0; i < cnt; i++)
        {
            s_logers[i].Flush();
            s_logers[i].Close();
        }

#if !WITHOUT_STREAM_WRITER_PROXY
      //  BackgroundWorker.DestroyInstance();
#endif
    }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN || FORCE_LOG
    void Update()
    {
        int cnt = (int)SLogCategory.Max;
        for (int i = 0; i < cnt; i++)
        {
            if (s_logers[i]!=null)
            {
                s_logers[i].Flush();
            }            
        }
    }
#endif

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public static void ClearConsole()
    {
        return;

//#if UNITY_EDITOR
#if false
        try
        {
            var assembly = System.Reflection.Assembly.GetAssembly(typeof(SceneView));

            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            System.Reflection.MethodInfo method = type.GetMethod("Clear");
            method.Invoke(new object(), null);
        }
        catch (Exception)
        {
        }
#endif
    }

#if UNITY_STANDALONE_WIN || UNITY_EDITOR || FORCE_LOG
	private static Socket _socket = null;
	private static IPEndPoint _remotePoint = null;
	private static string _remoteHost = null;
	private static uint _sequence = 0;
	public static void LogToRemote(string logMsg, string remoteHost = "localhost")
	{
		try
		{
			if (null == _socket || remoteHost != _remoteHost)
			{
				_remoteHost = remoteHost;
				if (null != _socket) _socket.Close();
				IPHostEntry remoteEntry = Dns.GetHostEntry(_remoteHost);
				IPAddress remoteAddr = null;
				for (int i = 0; i < remoteEntry.AddressList.Length; ++i)
				{
					if (remoteEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
					{
						remoteAddr = remoteEntry.AddressList[i];
						break;
					}
				}
				if (null == remoteAddr)
				{
					Log("IPV4 network not found!");
					return;
				}
				_remotePoint = new IPEndPoint(remoteAddr, 11557);
				_socket = new Socket(remoteAddr.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
				_sequence = 0;
			}

			byte[] msg = System.Text.Encoding.UTF8.GetBytes((++_sequence) + ":" + logMsg);
			_socket.SendTo(msg, SocketFlags.None, _remotePoint);
		}
		catch (Exception ecp)
		{
			Log(ecp.Message);
		}
	}
#endif
}



public class SLogObj
{
#if !WITHOUT_STREAM_WRITER_PROXY
    StreamWriter streamWriter = null;
  //  StreamWriterProxy streamWriter = null;
#else
    StreamWriter streamWriter = null;
#endif
    string filePath = null;

    List<string> sLogTxt = new List<string>();

    string targetPath = null;
	string lastTargetPath = null;
    public string TargetPath
    {
        get
        {
            return targetPath;
        }

        set
        {
            targetPath = value;
			if (!string.IsNullOrEmpty(targetPath))
				lastTargetPath = targetPath;
            filePath = null;
        }
    }

	public string LastTargetPath
	{
		get { return lastTargetPath; }
	}

#if UNITY_STANDALONE_WIN || UNITY_EDITOR || FORCE_LOG
    bool Begin()
    {
        if (streamWriter != null)
            return true;
        
    //    bool createNew = false;

        if (TargetPath != null)
        {
            if (filePath != TargetPath)
            {
                filePath = targetPath;
              //  createNew = true;
            }
        }
        else
        {
            if (filePath == null)
            {
              //  createNew = true;

                //string dt = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string dt = "debug";
                filePath = string.Format("{0}/../sgame_{1}.txt", Application.dataPath, dt);
            }
        }

        try
        {
#if !WITHOUT_STREAM_WRITER_PROXY
            streamWriter = null;// new StreamWriterProxy(filePath, createNew);
#else
            FileStream fs = null;

            if (createNew)
            {
                fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(filePath, FileMode.Append, FileAccess.Write);
            }

            streamWriter = new StreamWriter(fs);
#endif
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    void End()
    {
        streamWriter.Close();
        streamWriter = null;
    }
#endif

    public void Close()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || FORCE_LOG
        Flush();

        if (streamWriter != null)
        {
            streamWriter.Close();
        }
        streamWriter = null;
        filePath = null;
#endif
    }

    public void Log(string str)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || FORCE_LOG
        //if (!Begin())
        //    return;

        //streamWrtier.Write(str);
        //End();
        sLogTxt.Add(str);
#endif
    }

    public void Flush()
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR || FORCE_LOG
        if (sLogTxt.Count > 0)
        {
            if (!Begin())
                return;

            for (int i = 0; i < sLogTxt.Count; ++i)
            {
                streamWriter.WriteLine(sLogTxt[i]);
            }
            sLogTxt.Clear();

            End();
        }
#endif
    }
}