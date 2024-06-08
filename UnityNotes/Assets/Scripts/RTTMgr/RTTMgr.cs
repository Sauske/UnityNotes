using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    /// <summary>
    /// RTTMgr, 负责生成UI界面中的RTT渲染
    /// </summary>
    public class RTTMgr : MonoSingleton<RTTMgr>
    {
        private const int Layer = PhysicsLayer.RTT;

        private Dictionary<string, KeyValuePair<RTTScene, int>> RTTScenes = new Dictionary<string, KeyValuePair<RTTScene, int>>();
        private Dictionary<string, KeyValuePair<RTTScene, int>>.Enumerator _enumerator;
       // private AssetLoadAgent lightResAgent;

        private const string lightPrefab = "Assets/Resources/CommonPrefab/RTTLight.prefab";

        private GameObject rttLightObject = null;

        //>-----------------------------------------------------------------------------
        protected override void Awake()
        {
            base.Awake();

            //lightResAgent = ResourceMgr.ins.LoadAssetFromAB(lightPrefab, agent =>
            //{
            //    if (agent.AssetObject != null)
            //    {
            //        rttLightObject = (GameObject)GameObject.Instantiate(agent.AssetObject);
            //    }
            //    rttLightObject.transform.parent = CachedTrans;

            //});
        }

        //>-----------------------------------------------------------------------------
        protected override void OnDestory()
        {
            base.OnDestory();

            //lightResAgent.Release();

            //foreach (var rttScene in RTTScenes)
            //{
            //    rttScene.Value.Left.Destory();
            //}
        }

        /// <summary>
        /// 更新
        /// </summary>
        public void Update()
        {
            _enumerator = RTTScenes.GetEnumerator();
            while (_enumerator.MoveNext())
            {
               // _enumerator.Current.Value.Left.Update();
            }
        }

        //>-----------------------------------------------------------------------------
        /// <summary>
        /// 创建一个RTTScene
        /// </summary>
        /// <param name="scene"></param>
        /// <returns></returns>
        public RTTScene CreatRTTScene(string scene)
        {
            KeyValuePair<RTTScene, int> pair;
            if (RTTScenes.TryGetValue(scene, out pair))
            {
               // pair.Right++;
            }
            else
            {
                RTTScene rtn = new RTTScene(scene, gameObject, Layer);
                pair = new KeyValuePair<RTTScene, int>(rtn, 1);
                RTTScenes.Add(scene, pair);
            }

            return pair.Key;
        }

        //>-----------------------------------------------------------------------------

        /// <summary>
        /// 销毁RTT Scene
        /// </summary>
        /// <param name="scene"></param>
        public void DestoryRttScene(string scene)
        {
            KeyValuePair<RTTScene, int> pair;
            if (RTTScenes.TryGetValue(scene, out pair))
            {
                //pair.Right--;
                //if (pair.Right <= 0)
                //{
                //    pair.Left.Destory();
                //    RTTScenes.Remove(scene);
                //}
            }
        }

        public void DestoryRttScene(RTTScene scene)
        {
            if (scene != null) DestoryRttScene(scene.Name);
        }
    }
}
