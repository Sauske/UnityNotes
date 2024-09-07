using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UMI
{
    /// <summary>
    /// 文件读写工具
    /// </summary>
    public class FileUtils
    {

        #region File Write And Read 

        /// <summary>
        /// 文件是否存在
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void CopyFile(string sourceFileName, string destFileName, bool overwrite = true)
        {
            string folder = Path.GetDirectoryName(destFileName);
            CreateFolder(folder);
            File.Copy(sourceFileName, destFileName, overwrite);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="bytes"></param>
        /// <param name="mode"></param>
        public static void WriteFile(string filePath, byte[] bytes, FileMode mode = FileMode.CreateNew)
        {
            if (null == bytes)
            {
                bytes = new byte[] { };
            }

            CreateFolder(Path.GetDirectoryName(filePath));

            if (mode == FileMode.CreateNew)
            {
                DelFile(filePath);
            }

            //将创建文件流对象的过程写在using当中，会自动的帮助我们释放流所占用的资源
            using (FileStream fsWrite = new FileStream(filePath, mode, FileAccess.Write))
            {
                fsWrite.Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public static void WriteFile(string filePath, string data, FileMode mode = FileMode.CreateNew)
        {
            byte[] bytes = data.ToBytes();
            WriteFile(filePath, bytes, mode);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="baseFolder">基础路径</param>
        /// <param name="filePath">相对路径</param>
        /// <param name="bytes"></param>
        /// <param name="mode"></param>
        public static void WriteFile(string baseFolder, string filePath, byte[] bytes, FileMode mode = FileMode.CreateNew)
        {
            if (null == bytes)
            {
                bytes = new byte[] { };
            }

            filePath = Path.Combine(baseFolder, filePath);
            WriteFile(filePath, bytes, mode);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="baseFolder">基础路径</param>
        /// <param name="filePath">相对路径</param>
        /// <param name="data"></param>
        /// <param name="mode"></param>
        public static void WriteFile(string baseFolder, string filePath, string data, FileMode mode = FileMode.CreateNew)
        {
            CreateFolder(baseFolder, Path.GetDirectoryName(filePath));

            byte[] bytes = data.ToBytes();
            filePath = Path.Combine(baseFolder, filePath);
            WriteFile(filePath, bytes, mode);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] ReadFile(string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return null;
            }

            byte[] bytes = null;
            try
            {
                bytes = File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("ReadFile Error = {0} , strack = {1}", ex.ToString(), ex.StackTrace);
            }
            return bytes;
        }


        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadFileStr(string filePath, Encoding encoding)
        {
            byte[] bytes = ReadFile(filePath);
            return bytes.BytesToString();
        }


        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string ReadFileStr(string filePath)
        {
            return ReadFileStr(filePath, StringExts.UTF8Encoding);
        }

        public static List<string> readAllLine(string filePath)
        {
            List<string> strLst = new List<string>();
            FileInfo fileInfo = new FileInfo(filePath);
            if (!fileInfo.Exists)
            {
                return strLst;
            }

            try
            {
                strLst.AddRange(File.ReadAllLines(filePath));
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("ReadFile Error = {0} , strack = {1}", ex.ToString(), ex.StackTrace);
            }

            return strLst;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void DelFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("DelFile Error = {0} , strack = {1}", ex.ToString(), ex.StackTrace);
            }
        }

        #endregion


        #region 文件夹 Folder

        public static bool FolderExits(string folderPath)
        {
            return Directory.Exists(folderPath);
        }

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void CreateFolder(string folderPath)
        {
            string[] folderNames = null;
            string folderCombine = "";
            try
            {
                if (File.Exists(folderPath))
                {
                    DelFile(folderPath);
                }

                folderPath = folderPath.Replace("\\", "/");
                folderNames = folderPath.Split("/");
                if (folderNames.Length <= 0)
                {
                    return;
                }

                
                foreach (var name in folderNames)
                {
                    if(name == "") continue;
                    folderCombine = Path.Join(folderCombine, name);

                    if (File.Exists(folderCombine))
                    {
                        DelFile(folderCombine);
                    }
                    else if (!Directory.Exists(folderCombine))
                    {
                        Directory.CreateDirectory(folderCombine);
                    }
                }

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("CreateFolder Error = {0} , strack = {1}", ex.ToString(), ex.StackTrace);
            }
        }


        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="baseFolder">确定已经存在的目录路径</param>
        /// <param name="folderPath">相对路径</param>
        public static void CreateFolder(string baseFolder, string folderPath)
        {
            try
            {
                if (!Directory.Exists(baseFolder))
                {
                    Directory.CreateDirectory(baseFolder);
                }

                if (Directory.Exists(Path.Combine(baseFolder, folderPath)))
                {
                    return;
                }

                folderPath = folderPath.Replace("\\", "/");
                string[] folderNames = folderPath.Split("/");
                if (folderNames.Length <= 0)
                {
                    return;
                }

                string folderCombine = baseFolder;
                foreach (var name in folderNames)
                {
                    folderCombine = Path.Combine(folderCombine, name);

                    if (File.Exists(folderCombine))
                    {
                        DelFile(folderCombine);
                    }
                    else if (!Directory.Exists(folderCombine))
                    {
                        Directory.CreateDirectory(folderCombine);
                    }
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("CreateFolder Error = {0} , strack = {1}", ex.ToString(), ex.StackTrace);
            }
        }


        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void DelFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            try
            {
                DirectoryInfo folderInfo = new DirectoryInfo(folderPath);
                FileSystemInfo[] fileInfos = folderInfo.GetFileSystemInfos();
                foreach (var info in fileInfos)
                {
                    if (info is DirectoryInfo)
                    {
                        DelFolder(info.FullName);
                    }
                    else
                    {
                        File.Delete(info.FullName);
                    }
                }

                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath, true);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogErrorFormat("DelFolder Error = {0} , strack = {1}", ex.ToString(), ex.StackTrace);
            }
        }

        /// <summary>
        ///  删除文件夹
        /// </summary>
        /// <param name="baseFolder">基础路径</param>
        /// <param name="folderName">相对路径</param>
        public static void DelFolder(string baseFolder, string folderName)
        {
            if (!Directory.Exists(baseFolder))
            {
                return;
            }

            DelFolder(Path.Combine(baseFolder, folderName));
        }

        #endregion


        #region UnityPath

        /// <summary>
        /// /// 从完整路径里面获取unity asset对应的目录
        /// </summary>
        /// <param name="fullPath">文件完整路径</param>
        /// <returns>asset路径</returns>
        public static string GetUnityAssetPathFromFullPath(string fullPath)
        {
            string assetFilePath = string.Empty;
            int tmpIndex = fullPath.IndexOf("Assets");
            if (tmpIndex < 0)
            {
                /// <summary>
                /// Packages下的单独处理
                /// </summary>
                /// <returns></returns>
                if (fullPath.Contains("PackageCache"))
                {
                    string path_1 = "Packages";
                    string path_2 = fullPath.Substring(fullPath.IndexOf("com.unity")).Replace("\\","/");
                    string path_3 = path_2.Substring(0, path_2.IndexOf("@"));
                    string path_4 = path_2.Substring(path_2.IndexOf("/"));

                    assetFilePath = Path.Join(path_1,path_3 + path_4);
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                assetFilePath = fullPath.Substring(tmpIndex);
            }


            return FilePathAdjust(assetFilePath);
        }

        /// <summary>
        /// 文件路径调整
        /// 把文件路径的 \\ 转换 成 /
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>调整后的文件路径</returns>
        public static string FilePathAdjust(string path)
        {
            return path.Replace('\\', '/');
        }

        #endregion

    }
}
