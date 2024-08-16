//using Protocol;
using UnityEngine;
using System.Collections.Generic;

namespace UMI
{
    /// <summary>
    /// 通过 collider 或者 overlap 自定义检测都可以
    /// 先用 collider 做检测，后期用 overlap 性能更好
    /// </summary>
    public abstract class TriggerAreaRuntime : MonoBehaviour
    {
        public TriggerArea AreaCfg;

        private float CheckTime = 0.03f;

        private bool Inited = false;

        /// <summary>
        /// 在区域中的对象
        /// </summary>
        private List<Transform> activeList;
        List<Transform> tempResList;
        List<Transform> tempFristIn;
        List<Transform> tempFristOut;

        public void InitArea(TriggerArea cfg, int checkFPS = 30)
        {
            AreaCfg = cfg;

            CheckTime = 1f / checkFPS;

            activeList = new List<Transform>();

            Inited = true;

            transform.position = AreaCfg.Pos.ToVector3();
        }

        float tickTime;

        private void Update()
        {
            if (Inited == false) return;

            tickTime += Time.deltaTime;
            if (tickTime > CheckTime)
            {
                tickTime -= CheckTime;
                CheckArea();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            switch ((TriggerShape)AreaCfg.Shape)
            {
                case TriggerShape.Box:
                    Gizmos.DrawWireCube(AreaCfg.Pos.ToVector3(), AreaCfg.Size.ToVector3());
                    break;
                case TriggerShape.Box2D:
                    Gizmos.DrawWireCube(AreaCfg.Pos.ToVector3(), (AreaCfg.Size.ToVector3()) + Vector3.up * -0.8f);
                    break;
                case TriggerShape.Sphere:
                    Gizmos.DrawWireSphere(AreaCfg.Pos.ToVector3(), AreaCfg.Size.ToVector3().x);
                    break;
                case TriggerShape.Rectangle:
                    Gizmos.DrawWireCube(AreaCfg.Pos.ToVector3(), AreaCfg.Size.ToVector3());
                    break;
            }
        }


        private void CheckArea()
        {
            ResetList(ref tempResList);

            int layer = 0;
            foreach (var item in AreaCfg.CheckLayerList)
            {
                layer = layer | item;
            }
            switch ((TriggerShape)AreaCfg.Shape)
            {
                case TriggerShape.Box:
                    CheckCube(AreaCfg.Pos.ToVector3(), AreaCfg.Size.ToVector3(), AreaCfg.Forward.ToVector3(), layer, ref tempResList);
                    break;
                case TriggerShape.Box2D:
                    CheckCube(AreaCfg.Pos.ToVector3(), (AreaCfg.Size.ToVector3()) + Vector3.up * -0.8f, AreaCfg.Forward.ToVector3(), layer, ref tempResList);
                    break;
                case TriggerShape.Sphere:
                    CheckSphere(AreaCfg.Pos.ToVector3(), AreaCfg.Size.ToVector3(), layer, ref tempResList);
                    break;
                case TriggerShape.Rectangle:
                    CheckRectangle(AreaCfg.Pos.ToVector3(), AreaCfg.Size.ToVector3(), AreaCfg.Forward.ToVector3(), layer, ref tempResList);
                    break;
            }
            CompareList(tempResList, ref tempFristIn, ref tempFristOut);

            OnTriggerIn(tempFristIn);

            OnTriggerOut(tempFristOut);
        }

        private void CheckCube(Vector3 pos, Vector3 sizeV3, Vector3 forward, int layer, ref List<Transform> resList)
        {
            var res = Physics.OverlapBox(pos, sizeV3 * 0.5f, Quaternion.LookRotation(forward), layer);

            foreach (var item in res)
            {
                resList.Add(item.transform);
            }
        }

        private void CheckSphere(Vector3 pos, Vector3 radius, int layer, ref List<Transform> resList)
        {
            var res = Physics.OverlapSphere(pos, radius.x, layer);

            foreach (var item in res)
            {
                resList.Add(item.transform);
            }
        }

        private void CheckRectangle(Vector3 pos, Vector3 sizeV3, Vector3 forward, int layer, ref List<Transform> resList)
        {
            var res = Physics.OverlapBox(pos, sizeV3, Quaternion.LookRotation(forward),layer);

            foreach (var item in res)
            {
                resList.Add(item.transform);
            }
        }

        private void CompareList(List<Transform> tempList, ref List<Transform> fristInList, ref List<Transform> fristOutList)
        {
            ResetList(ref fristInList);
            ResetList(ref fristOutList);

            foreach (var tmp in tempList)
            {
                if (activeList.Contains(tmp) == false)
                {
                    fristInList.Add(tmp);
                }
            }

            foreach (var item in activeList)
            {
                if (tempList.Contains(item) == false)
                {
                    fristOutList.Add(item);
                }
            }

            activeList.Clear();
            foreach (var temp in tempList)
            {
                activeList.Add(temp);
            }
        }

        private void ResetList(ref List<Transform> list)
        {
            if (list == null)
            {
                list = new List<Transform>();
            }
            else
            {
                list.Clear();
            }
        }



        protected abstract void OnTriggerIn(List<Transform> res);

        protected abstract void OnTriggerOut(List<Transform> res);
    }
}