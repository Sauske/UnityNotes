using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine.SceneManagement;

public enum EResType
{
    None,
    Icon,
    Table,
    Effect,
    Avatar,
    Audio,
    Scene,
    UI,
    Behavior,
    XML,
    Text,
}
public class CResourceMgr : Singleton<CResourceMgr>
{
    public byte[] LoadTable(string path)
    {
        return LoadBinary(EResType.Table, path);
    }

    public byte[] LoadBinary(EResType resType, string path)
    {
        string filePath = GetCachePath(resType, path);
        if (File.Exists(filePath))
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                byte[] buffSrc = new byte[fs.Length];
                fs.Read(buffSrc, 0, (int)fs.Length);

                bool isCrcSucess = false;
                bool isPackData = false;
                byte[] buffDest = UnPackTools.UnPackData(buffSrc, out isPackData, out isCrcSucess);
                if (!isCrcSucess)
                {
                    Debug.LogError("crc is failed");
                    return null;
                }

                return buffDest;
            }
        }

        TextAsset txtAsset = Resources.Load(path) as TextAsset;
        return txtAsset != null ? txtAsset.bytes : null;
    }

    public GameObject LoadEffect(string path)
    {
        string filePath = GetCachePath(EResType.Effect, path);
        byte[] buff = UnpackBuffFromFile(filePath);
        if (buff != null)
        {
            AssetBundle ab = AssetBundle.LoadFromMemoryAsync(buff).assetBundle;

            GameObject go = null;

            go = ab.LoadAsset<GameObject>(Path.GetFileName(path));

          //  go.AddComponent<FixShader>();
            ab.Unload(false);
            return go;
        }

        return Resources.Load(path) as GameObject;        
    }

    public GameObject LoadAvatar(string path)
    {
        string filePath = GetCachePath(EResType.Avatar, path);
        byte[] buff = UnpackBuffFromFile(filePath);
        if (buff != null)
        {
            AssetBundle ab = AssetBundle.LoadFromMemoryAsync(buff).assetBundle;

            GameObject go = null;

            go = ab.LoadAsset<GameObject>(Path.GetFileName(path));

            ab.Unload(false);
            return go;
        }

        return Resources.Load(path) as GameObject;        
    }

    public GameObject LoadUI(string strUIPath)
    {
        string filePath = GetCachePath(EResType.UI, strUIPath);
        filePath += ".prefab";
        byte[] buff = UnpackBuffFromFile(filePath);
        if (buff != null)
        {
            AssetBundle ab = AssetBundle.LoadFromMemoryAsync(buff).assetBundle;

            GameObject retObj = null;

            retObj = ab.LoadAsset<GameObject>(Path.GetFileName(strUIPath));

            ab.Unload(false);
            return (GameObject)GameObject.Instantiate(retObj);
        }
        return (GameObject)GameObject.Instantiate(Resources.Load(strUIPath));
    }

    public AudioClip LoadSound(string path)
    {
        string filePath = GetCachePath(EResType.Audio, path);

        byte[] buff = UnpackBuffFromFile(filePath);
        if (buff != null)
        {
            AssetBundle ab = AssetBundle.LoadFromMemoryAsync(buff).assetBundle;

            AudioClip clip = null;

            clip = ab.LoadAsset<AudioClip>(Path.GetFileName(path));

            ab.Unload(false);
            return clip;
        }

        return (AudioClip)Resources.Load(path);
    }

    public Texture2D LoadTexture(string path)
    {
        string filePath = GetCachePath(EResType.Icon, path);
        byte[] buff = UnpackBuffFromFile(filePath);
        if(buff !=  null)
        {
            AssetBundle ab = AssetBundle.LoadFromMemoryAsync(buff).assetBundle;

            Texture2D tex = null;

            tex = ab.LoadAsset<Texture2D>(Path.GetFileName(path));

            ab.Unload(false);
            return tex;
        }

        return Resources.Load(path) as Texture2D;
    }

    public void LoadScene(string sceneName)
    {
        string filePath = GetCachePath(EResType.Scene, sceneName);
        byte[] buff = UnpackBuffFromFile(filePath);
        if (buff != null)
        {
            AssetBundle ab = AssetBundle.LoadFromMemoryAsync(buff).assetBundle;

#if UNITY_4_7
            ab.Load(sceneName);
#else
            SceneManager.LoadScene(sceneName);
#endif
        }

        Resources.Load(sceneName);
    }

    public string GetVersion()
    {
        return "";
    }

    public string GetCachePath(EResType type, string path)
    {
        return Application.persistentDataPath + "/" + path + GetVersion();
    }

    private byte[] UnpackBuffFromFile(string filePath)
    {
        if(File.Exists(filePath))
        {
            using (FileStream fs = new FileStream(filePath,FileMode.Open))
            {
                byte[] buffSrc = new byte[fs.Length];
                fs.Read(buffSrc,0,(int)fs.Length);
                                
                bool isPackData = false;
                bool isCrcSuccess = false;
                byte[] buffDest = UnPackTools.UnPackData(buffSrc,out isPackData,out isCrcSuccess);
                
                if(!isCrcSuccess)
                {
                    Debug.LogError("crc is failed");
                    return null;
                }
                return buffDest;
            }
        }
        return null;
    }
}