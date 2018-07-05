using System.Collections;
using System.Collections.Generic;
using System;

namespace ResUpdater
{
    [Serializable]
    public class BundleMeta
    {
        public string name;     //包名
        public string md5;      //md5
        public long size;       //文件大小
        public bool bCompressed;//s是否被压缩
        public bool bCrypted;   //是否被压缩
        public bool bIsAdditionBundle;                             //是否是附加生成的bundle
        public List<string> listDependency = new List<string>();   //依赖资源包列表
        public List<string> assets = new List<string>();           //包内资源列表

        public bool Equals(BundleMeta bundle)
        {
            return bundle.name == name && bundle.md5 == md5;
        }
    }

    [Serializable]
    public class BundleInfo
	{
        public string version = "0.0.0";  //版本号
        public List<BundleMeta> metaList = new List<BundleMeta>();   //加密数据
    }
}
