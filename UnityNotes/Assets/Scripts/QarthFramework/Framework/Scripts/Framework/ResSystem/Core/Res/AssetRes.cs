//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using MainGame;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Qarth
{
    public class AssetRes : BaseRes
    {
        protected string[]            m_AssetBundleArray;
        protected AssetBundleRequest  m_AssetBundleRequest;

        public static AssetRes Allocate(string name)
        {
            AssetRes res = ObjectPool<AssetRes>.S.Allocate();
            if (res != null)
            {
                res.name = name;
                res.InitAssetBundleName();
            }
            return res;
        }

        protected string assetBundleName
        {
            get
            {
                if (m_AssetBundleArray == null)
                {
                    return null;
                }
                return m_AssetBundleArray[0];
            }
        }
        public AssetRes(string name) : base(name)
        {
            
        }

        public AssetRes()
        {

        }

        public override void AcceptLoaderStrategySync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnSyncLoadFinish(loader, this);
        }

        public override void AcceptLoaderStrategyAsync(IResLoader loader, IResLoaderStrategy strategy)
        {
            strategy.OnAsyncLoadFinish(loader, this);
        }

        public override bool LoadSync()
        {
            if (!CheckLoadAble())
            {
                return false;
            }

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return false;
            }

#if UNITY_EDITOR
            if (AppConfig.S.loadInEditor)
            {
                return LoadFileInEditor();
            }
            else
            {
                return LoadFile();
            }
#else
            return LoadFile();
#endif
            return LoadFile();
        }
#if UNITY_EDITOR
        private bool LoadFileInEditor()
        {
            AssetBundleRes abR = ResMgr.S.GetRes<AssetBundleRes>(assetBundleName);
            if (abR == null)
            {
                Log.e("Failed to Load Asset, Not Find AssetBundleImage:" + assetBundleName);
                return false;
            }
                
            var assetPaths =  AssetTableUtils.GetAssetPathsFromAssetBundleAndAssetName(abR.name, name);
            if (assetPaths.Length == 0)
            {
                Debug.LogError("Failed Load Asset:" + name);
                OnResLoadFaild();
                return false;
            }
                
            HoldDependRes();
            resState = eResState.kLoading;

            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPaths[0], typeof(UnityEngine.Object));
                
            if (obj == null)
            {
                Log.e("Failed Load Asset:" + m_Name);
                OnResLoadFaild();
                return false;
            }

            UnHoldDependRes();
            m_Asset = obj;

            resState = eResState.kReady;

            return true;
        }
#endif
        private bool LoadFile()
        {
            AssetBundleRes abR = ResMgr.S.GetRes<AssetBundleRes>(assetBundleName);

            if (abR == null || abR.assetBundle == null)
            {
                Log.e("Failed to Load Asset, Not Find AssetBundleImage:" + assetBundleName);
                return false;
            }

            resState = eResState.kLoading;

            //TimeDebugger timer = ResMgr.S.timeDebugger;

            //timer.Begin("LoadSync Asset:" + m_Name);

            HoldDependRes();

            UnityEngine.Object obj = abR.assetBundle.LoadAsset(m_Name);
            //timer.End();

            if (obj == null)
            {
                Log.e("Failed Load Asset:" + m_Name);
                OnResLoadFaild();
                return false;
            }

            UnHoldDependRes();
            m_Asset = obj;

            resState = eResState.kReady;
            //Log.i(string.Format("Load AssetBundle Success.ID:{0}, Name:{1}", m_Asset.GetInstanceID(), m_Name));

            //timer.Dump(-1);
            return true;
        }

        public override void LoadAsync()
        {
            if (!CheckLoadAble())
            {
                return;
            }

            if (string.IsNullOrEmpty(assetBundleName))
            {
                return;
            }

            resState = eResState.kLoading;

            ResMgr.S.PostIEnumeratorTask(this);
        }

        public override IEnumerator StartIEnumeratorTask(Action finishCallback)
        {
            if (refCount <= 0)
            {
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            AssetBundleRes abR = ResMgr.S.GetRes<AssetBundleRes>(assetBundleName);
            bool hasAsset;
#if UNITY_EDITOR
            if (AppConfig.S.loadInEditor)
            {
                hasAsset = abR != null;
            }
            else
            {
                hasAsset = abR != null && abR.assetBundle != null;
            }
#else
            hasAsset = abR != null && abR.assetBundle != null;
#endif
            if (!hasAsset)
            {
                Log.e("Failed to Load Asset, Not Find AssetBundleImage:" + assetBundleName);
                OnResLoadFaild();
                finishCallback();
                yield break;
            }

            //确保加载过程中依赖资源不被释放:目前只有AssetRes需要处理该情况
            HoldDependRes();

#if UNITY_EDITOR
            if (AppConfig.S.loadInEditor)
            {
                yield return LoadAssetInEditorAsync(abR.name,name);
            }
            else
            {
                yield return LoadAssetAsync(abR, name);
            }
#else
            yield return LoadAssetAsync(abR, name);
#endif
            
            
            if (!m_LoadSuccess)
            {
                finishCallback();
                yield break;
            }

            resState = eResState.kReady;

            finishCallback();
        }

        private bool m_LoadSuccess;

        private IEnumerator LoadAssetAsync(AssetBundleRes abR,string assetName)
        {
            AssetBundleRequest abQ = abR.assetBundle.LoadAssetAsync(assetName);
            m_AssetBundleRequest = abQ;

            yield return abQ;

            m_AssetBundleRequest = null;

            UnHoldDependRes();

            if (refCount <= 0)
            {
                OnResLoadFaild();
                m_LoadSuccess = false;
                yield break;
            }

            if (!abQ.isDone)
            {
                Log.e("Failed Load Asset:" + m_Name);
                OnResLoadFaild();
                m_LoadSuccess = false;
                yield break;
            }

            m_Asset = abQ.asset;

            if (m_Asset == null)
            {
                Log.e("Failed Load Asset:" + m_Name);
                OnResLoadFaild();
                m_LoadSuccess = false;
                yield break;
            }

            m_LoadSuccess = true;
        }

#if UNITY_EDITOR
        private IEnumerator LoadAssetInEditorAsync(string abName,string assetName)
        {
            var assetPaths =  AssetTableUtils.GetAssetPathsFromAssetBundleAndAssetName(abName, assetName);
            if (assetPaths.Length == 0)
            {
                Debug.LogError("Failed Load Asset:" + name);
                m_LoadSuccess = false;
                yield break;
            }
            
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(assetPaths[0], typeof(UnityEngine.Object));
            
            if (obj == null)
            {
                Log.e("Failed Load Asset:" + m_Name);
                m_LoadSuccess = false;
                yield break;
            }
            else
            {
                m_Asset = obj;
                m_LoadSuccess = true;
                yield return null;
            }
        }
#endif
        
        public override string[] GetDependResList()
        {
            return m_AssetBundleArray;
        }

        public override void OnCacheReset()
        {
            m_AssetBundleArray = null;
        }
        
        public override void Recycle2Cache()
        {
            ObjectPool<AssetRes>.S.Recycle(this);
        }

        protected override float CalculateProgress()
        {
            if (m_AssetBundleRequest == null)
            {
                return 0;
            }

            return m_AssetBundleRequest.progress;
        }

        protected void InitAssetBundleName()
        {
            m_AssetBundleArray = null;

            AssetData config = AssetDataTable.S.GetAssetData(m_Name);

            if (config == null)
            {
                Log.e("Not Find AssetData For Asset:" + m_Name);
                return;
            }

            string assetBundleName = AssetDataTable.S.GetAssetBundleName(config.assetName, config.assetBundleIndex);

            if (string.IsNullOrEmpty(assetBundleName))
            {
                Log.e("Not Find AssetBundle In Config:" + config.assetBundleIndex);
                return;
            }
            m_AssetBundleArray = new string[1];
            m_AssetBundleArray[0] = assetBundleName;
        }
    }
}
