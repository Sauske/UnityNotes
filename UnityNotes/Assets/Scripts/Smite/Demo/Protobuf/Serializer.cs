//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;
using System;

namespace Framework
{

    //public class Serializer : Singleton<Serializer>
    //{


    //    private readonly static object Locker = new object();

    //    public override void Init()
    //    {
    //        base.Init();
                       
        
    //    }

    //    public override void UnInit()
    //    {
    //        base.UnInit();

    //    }

    //    public static bool Serialize<T>(T model,string filename) where T: class
    //    {
    //        try
    //        {
    //            string binPath = Application.dataPath + @"\";
    //            if (!System.IO.Directory.Exists(binPath))
    //            {
    //                System.IO.Directory.CreateDirectory(binPath);
    //            }
    //            lock (Locker)
    //            {
    //               using (var file = System.IO.File.Create(binPath + filename))
    //               {
    //                   ProtoBuf.Serializer.Serialize<T>(file, model);
    //                   return true;
    //               }
    //           }
    //        }
    //        catch (Exception ex)
    //        {
    //            DebugHelper.LogError(string.Format("Serizlizer is Error:{0}", ex.Message));
    //            return false;
    //        }
    //    }

    //    public static T DeSerialize<T>(string filaPath) where T: class
    //    {
    //        var dbPath = Application.dataPath +@"\" + filaPath;

    //        if (System.IO.File.Exists(dbPath))
    //        {
    //            lock (Locker)
    //            {
    //                using(var file = System.IO.File.OpenRead(dbPath))
    //                {
    //                    var result = ProtoBuf.Serializer.Deserialize<T>(file);
    //                    return result;
    //                }
    //            }
    //        }
    //        return default(T);
    //    }
       
    //}
}