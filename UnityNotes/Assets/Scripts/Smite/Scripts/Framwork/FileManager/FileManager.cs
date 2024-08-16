//==================================================================================
///文件管理器
///仅用于FileStream能够使用的场合
///如Android的StreamingAssets目录由于Jar的原因，无法应用该文件管理器
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Security.Cryptography;
using System;
using System.Text;

namespace Framework
{

    public enum enFileOperation
    {
        ReadFile,
        WriteFile,
        DeleteFile,
        CreateDirectory,
        DeleteDirectory,
    }
    
    public class CFileManager : Singleton<CFileManager>
    {
        //数据缓存路径
        private static string cachePath = null;

        //IFS解压路径
        public static string ifsExtractFolder = "Resources";

        private static string ifsExtractPath = null;

        //MD5计算器
        public static MD5CryptoServiceProvider md5Provider = new MD5CryptoServiceProvider();

        //文件操作失败事件
        public delegate void DelegateOnOperateFileFail(string fullPath,enFileOperation fileOperation,Exception ex);
        public static DelegateOnOperateFileFail delegateOnOperationFileFail = delegate { };

        public override void Init()
        {
            base.Init();
        
        }


        public override void UnInit()
        {
            base.UnInit();
        
        }


        public static bool IsFileExist(string filePath)
        {
            return File.Exists(filePath);
        }

        public static bool IsDirectoryExist(string directory)
        {
            return Directory.Exists(directory);
        }

        public static bool CreateDirectory(string directory)
        {
            if (IsDirectoryExist(directory))
            {
                return true;
            }
            int tryCount = 0;

            while (true)
            {
                try
                {
                    Directory.CreateDirectory(directory);
                    return true;
                }
                catch (Exception ex)
                {
                    tryCount++;
                    if (tryCount >= 3)
                    {
                        UnityEngine.Debug.Log("Create Directory " + directory + " Error Exception " + ex.Message);

                        delegateOnOperationFileFail(directory, enFileOperation.CreateDirectory, ex);
                        return false;
                    }                    
                }
            }
        }



        public static bool DeleteDirectory(string directory)
        {
            if (!IsDirectoryExist(directory))
            {
                return true;
            }

            int tryCount = 0;

            while (true)
            {
                try
                {
                    Directory.Delete(directory);
                    return true;
                }
                catch (Exception ex)
                {
                    tryCount++;

                    if (tryCount >= 3)
                    {
                        UnityEngine.Debug.Log("Delete Directory " + directory + " Error Exception = " + ex.Message);
                        delegateOnOperationFileFail(directory, enFileOperation.DeleteDirectory, ex);
                        return false;
                    }
                }
            }
        }


        public static int GetFileLength(string filePath)
        {
            if (!IsFileExist(filePath))
            {
                return 0;
            }

            int tryCount = 0;

            while (true)
            {
                try
                {

                }
                catch (Exception ex)
                {
                    DebugHelper.LogError(string.Format("GetFileLength is error:{0}", ex.Message));
                    tryCount++;
                }
            }
        }



        //----------------------------------------------
        /// 读取文件
        /// @filePath
        //----------------------------------------------
        public static byte[] ReadFile(string filePath)
        {
            if (!IsFileExist(filePath))
            {
                return null;
            }

            byte[] data = null;
            int tryCount = 0;

            while (true)
            {
                System.Exception CurEexception = null;
                try
                {
                    data = System.IO.File.ReadAllBytes(filePath);
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Read File " + filePath + " Error! Exception = " + ex.ToString() + ", TryCount = " + tryCount);
                    data = null;
                    CurEexception = ex;
                }

                if (data == null || data.Length <= 0)
                {
                    tryCount++;

                    if (tryCount >= 3)
                    {
                        Debug.Log("Read File " + filePath + " Fail!, TryCount = " + tryCount);

                        //派发事件
                        delegateOnOperationFileFail(filePath, enFileOperation.ReadFile, CurEexception);

                        return null;
                    }
                }
                else
                {
                    return data;
                }
            }
        }

        //----------------------------------------------
        /// 写入文件
        /// @filePath
        /// @data
        //----------------------------------------------
        public static bool WriteFile(string filePath, byte[] data)
        {
            int tryCount = 0;

            while (true)
            {
                try
                {
                    System.IO.File.WriteAllBytes(filePath, data);
                    return true;
                }
                catch (System.Exception ex)
                {
                    tryCount++;

                    if (tryCount >= 3)
                    {
                        Debug.Log("Write File " + filePath + " Error! Exception = " + ex.ToString());

                        //这里应该删除文件以防止数据错误
                        DeleteFile(filePath);

                        //派发事件
                        delegateOnOperationFileFail(filePath, enFileOperation.WriteFile, ex);

                        return false;
                    }
                }
            }
        }

        //----------------------------------------------
        /// 写入文件
        /// @filePath
        /// @data
        /// @offset
        /// @length
        //----------------------------------------------
        public static bool WriteFile(string filePath, byte[] data, int offset, int length)
        {
            FileStream fileStream = null;

            int tryCount = 0;

            while (true)
            {
                try
                {
                    fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                    fileStream.Write(data, offset, length);
                    fileStream.Close();

                    return true;
                }
                catch (System.Exception ex)
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }

                    tryCount++;

                    if (tryCount >= 3)
                    {
                        Debug.Log("Write File " + filePath + " Error! Exception = " + ex.ToString());

                        //这里应该删除文件以防止数据错误
                        DeleteFile(filePath);

                        //派发事件
                        delegateOnOperationFileFail(filePath, enFileOperation.WriteFile, ex);

                        return false;
                    }
                }
            }
        }

        //----------------------------------------------
        /// 删除文件
        /// @filePath
        //----------------------------------------------
        public static bool DeleteFile(string filePath)
        {
            if (!IsFileExist(filePath))
            {
                return true;
            }

            int tryCount = 0;

            while (true)
            {
                try
                {
                    System.IO.File.Delete(filePath);
                    return true;
                }
                catch (System.Exception ex)
                {
                    tryCount++;

                    if (tryCount >= 3)
                    {
                        Debug.Log("Delete File " + filePath + " Error! Exception = " + ex.ToString());

                        //派发事件
                        delegateOnOperationFileFail(filePath, enFileOperation.DeleteFile, ex);

                        return false;
                    }
                }
            }
        }

        //----------------------------------------------
        /// 拷贝文件
        /// @srcFile
        /// @dstFile
        //----------------------------------------------
        public static void CopyFile(string srcFile, string dstFile)
        {
            System.IO.File.Copy(srcFile, dstFile, true);
        }

        //----------------------------------------------
        /// 返回文件md5
        /// @filePath
        //----------------------------------------------
        public static string GetFileMd5(string filePath)
        {
            if (!IsFileExist(filePath))
            {
                return string.Empty;
            }

            return System.BitConverter.ToString(md5Provider.ComputeHash(ReadFile(filePath))).Replace("-", "");
        }

        //----------------------------------------------
        /// 返回字节流md5
        /// @data
        //----------------------------------------------
        public static string GetMd5(byte[] data)
        {
            return System.BitConverter.ToString(md5Provider.ComputeHash(data)).Replace("-", "");
        }

        //----------------------------------------------
        /// 返回字符串md5
        /// @str
        //----------------------------------------------
        public static string GetMd5(string str)
        {
            return System.BitConverter.ToString(md5Provider.ComputeHash(Encoding.UTF8.GetBytes(str))).Replace("-", "");
        }

        //----------------------------------------------
        /// 合并路径
        /// @path1
        /// @path2
        //----------------------------------------------
        public static string CombinePath(string path1, string path2)
        {
            if (path1.LastIndexOf('/') != path1.Length - 1)
            {
                path1 += "/";
            }

            if (path2.IndexOf('/') == 0)
            {
                path2 = path2.Substring(1);
            }

            return path1 + path2;
            //return System.IO.Path.Combine(path1, path2);
        }

        //----------------------------------------------
        /// 合并路径
        /// @values
        //----------------------------------------------
        public static string CombinePaths(params string[] values)
        {
            if (values.Length <= 0)
            {
                return string.Empty;
            }
            else if (values.Length == 1)
            {
                return CombinePath(values[0], string.Empty);
            }
            else if (values.Length > 1)
            {
                string path = CombinePath(values[0], values[1]);

                for (int i = 2; i < values.Length; i++)
                {
                    path = CombinePath(path, values[i]);
                }

                return path;
            }

            return string.Empty;
        }

        //----------------------------------------------
        /// 返回StreamingAssets路径
        /// @返回值为带上file:///的可用于www方式加载的路径
        //----------------------------------------------
        public static string GetStreamingAssetsPathWithHeader(string fileName)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_IPHONE
            return GetLocalPathHeader() + System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
#elif UNITY_ANDROID
            return System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
#endif
        }

        //----------------------------------------------
        /// 返回Cache文件存储路径
        /// @返回值为标准路径
        //----------------------------------------------
        public static string GetCachePath()
        {
            //Android上temporaryCachePath中的数据在磁盘空间不足的情况下有可能会被删除，而persistentDataPath不会
            //IOS上persistentDataPath中的数据会被同步至iCloud，并且使用该目录可能导致审核被拒
            if (cachePath == null)
            {
#if UNITY_ANDROID || (UNITY_STANDALONE_WIN && !UNITY_EDITOR)
            cachePath = Application.persistentDataPath;
#else
                cachePath = Application.temporaryCachePath;
#endif
            }

            return cachePath;
        }

        //----------------------------------------------
        /// 返回Cache文件存储路径
        /// @返回值为标准路径
        //----------------------------------------------
        public static string GetCachePath(string fileName)
        {
            return CombinePath(GetCachePath(), fileName);
        }

        //----------------------------------------------
        /// 返回Cache文件存储路径
        /// @返回值为带上file:///的可用于www方式加载的路径
        //----------------------------------------------
        public static string GetCachePathWithHeader(string fileName)
        {
            return (GetLocalPathHeader() + GetCachePath(fileName));
        }

        //----------------------------------------------
        /// 返回IFS解压路径
        //----------------------------------------------
        public static string GetIFSExtractPath()
        {
            if (ifsExtractPath == null)
            {
                ifsExtractPath = CombinePath(GetCachePath(), ifsExtractFolder);
            }

            return ifsExtractPath;
        }

        //----------------------------------------------
        /// 返回带扩展名的全名
        /// @fullPath : 带扩展名的完整路径
        //----------------------------------------------
        public static string GetFullName(string fullPath)
        {
            if (fullPath == null)
            {
                return null;
            }

            int index = fullPath.LastIndexOf("/");

            if (index > 0)
            {
                return fullPath.Substring(index + 1, fullPath.Length - index - 1);
            }
            else
            {
                return fullPath;
            }
        }

        //----------------------------------------------
        /// 移除扩展名
        //----------------------------------------------
        public static string EraseExtension(string fullName)
        {
            if (fullName == null)
            {
                return null;
            }

            int index = fullName.LastIndexOf('.');

            if (index > 0)
            {
                return fullName.Substring(0, index);
            }
            else
            {
                return fullName;
            }
        }

        //----------------------------------------------
        /// 返回扩展名
        /// @返回值包括"."
        //----------------------------------------------
        public static string GetExtension(string fullName)
        {
            int index = fullName.LastIndexOf('.');

            if (index > 0 && index + 1 < fullName.Length)
            {
                return fullName.Substring(index);
            }
            else
            {
                return string.Empty;
            }
        }

        //----------------------------------------------
        /// 返回完整目录
        /// @注意:"a/b/c"会返回"a/c"
        /// @"a/b/c/"才是我们想要的效果
        /// @fullPath
        //----------------------------------------------
        public static string GetFullDirectory(string fullPath)
        {
            return System.IO.Path.GetDirectoryName(fullPath);
        }

        //----------------------------------------------
        /// 清除目录下所有文件及文件夹，并保留目录
        /// @fullPath
        //----------------------------------------------
        public static bool ClearDirectory(string fullPath)
        {
            try
            {
                //删除文件
                string[] files = System.IO.Directory.GetFiles(fullPath);
                for (int i = 0; i < files.Length; i++)
                {
                    System.IO.File.Delete(files[i]);
                }

                //删除文件夹
                string[] dirs = System.IO.Directory.GetDirectories(fullPath);
                for (int i = 0; i < dirs.Length; i++)
                {
                    System.IO.Directory.Delete(dirs[i], true);
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        //----------------------------------------------
        /// 清除目录下指定文件及文件夹，并保留目录
        /// @fullPath
        //----------------------------------------------
        public static bool ClearDirectory(string fullPath, string[] fileExtensionFilter, string[] folderFilter)
        {
            try
            {
                //删除文件
                if (fileExtensionFilter != null)
                {
                    string[] files = System.IO.Directory.GetFiles(fullPath);
                    for (int i = 0; i < files.Length; i++)
                    {
                        if (fileExtensionFilter != null && fileExtensionFilter.Length > 0)
                        {
                            for (int j = 0; j < fileExtensionFilter.Length; j++)
                            {
                                if (files[i].Contains(fileExtensionFilter[j]))
                                {
                                    DeleteFile(files[i]);
                                    break;
                                }
                            }
                        }
                    }
                }

                //删除文件夹
                if (folderFilter != null)
                {
                    string[] dirs = System.IO.Directory.GetDirectories(fullPath);
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        if (folderFilter != null && folderFilter.Length > 0)
                        {
                            for (int j = 0; j < folderFilter.Length; j++)
                            {
                                if (dirs[i].Contains(folderFilter[j]))
                                {
                                    DeleteDirectory(dirs[i]);
                                    break;
                                }
                            }
                        }
                    }
                }

                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        //----------------------------------------------
        /// 清空缓存
        //----------------------------------------------
        //public static bool ClearCachePath()
        //{
        //    return ClearDirectory(GetCachePath());
        //}

        //--------------------------------------------------
        /// 获取本地路径前缀(file:///)
        //--------------------------------------------------
        private static string GetLocalPathHeader()
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            return "file:///";
#elif UNITY_ANDROID
        return "file://";
#elif UNITY_IPHONE
        return "file:///";
#else
        return "file:///";
#endif
        }

    }
}