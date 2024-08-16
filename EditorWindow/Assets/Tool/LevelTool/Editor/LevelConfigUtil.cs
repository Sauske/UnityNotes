using UnityEditor;
using UnityEngine;
using System.IO;
using System;
using System.Collections;
using LitJson;
//using Protocol;

namespace UMI
{
    public static class LevelConfigUtil
    {
        public static void CreateAreaTrigger(object objArr)
        {
            if (objArr is object[])
            {
                var array = (object[])objArr;
                var v3 = (Vector3)array[0];
                var root = (GameObject)array[1];

                GameObject point = new GameObject("AreaTrigger");
                point.transform.parent = root.transform;
                point.transform.position = v3;

                var cfg = point.AddComponent<TriggerAreaCfg>();
            }
        }

        public static void CreateActionPoint(object objArr)
        {
            if (objArr is object[])
            {
                var array = (object[])objArr;
                var v3 = (Vector3)array[0];
                var root = (GameObject)array[1];

                GameObject point = new GameObject("ActionPoint");
                point.transform.parent = root.transform;
                point.transform.position = v3;

                var cfg = point.AddComponent<ActionPointCfg>();
            }
        }

        public static void EYS_CreatePoint(object objArr)
        {
            if (objArr is object[])
            {
                var array = (object[])objArr;
                var v3 = (Vector3)array[0];
                var root = (GameObject)array[1];

                GameObject point = new GameObject("配置点");
                point.transform.parent = root.transform;
                point.transform.position = v3;

                point.AddComponent<EysCfgPoint>();
            }
        }



        public static void CreateGamePlayTrigger(object objArr)
        {
            if (objArr is object[])
            {
                var array = (object[])objArr;
                var v3 = (Vector3)array[0];
                var root = (GameObject)array[1];

                GameObject point = new GameObject("区域");
                point.transform.parent = root.transform;
                point.transform.position = v3;

                var cfg = point.AddComponent<GamePlayTriggerCfg>();
                cfg.cubeSize = 5;

            }
        }

        public static void Export(LevelConfigHelper helper, string path)
        {
            //if (helper.GamePlayTriggerRoot == null)
            //{
            //    return;
            //}

            //var pointList = helper.GamePlayTriggerRoot.GetComponentsInChildren<GamePlayTriggerCfg>();

            //GamePlayTriggerArea area = new GamePlayTriggerArea();

            //foreach (var point in pointList)
            //{
            //    var item = new EysVoteItem();
            //    item.Pos = point.transform.position.ToClientV3();
            //    item.Forward = point.transform.forward.ToClientV3();
            //    //voteRoom.List.Add(item);
            //}

            //var bytes = voteRoom.ToByteArray();
            //if (System.IO.Directory.Exists(path) == false)
            //{
            //    System.IO.Directory.CreateDirectory(path);
            //}
            //if (ToolKit.SaveFile(bytes, path + helper.ConfigName + ".bytes"))
            //{
            //    Debug.Log("保存完毕:" + path);
            //}
        }

        public static void ExportAction(LevelConfigHelper helper, string path)
        {
            LevelCfg levelCfg = new LevelCfg();

            if (helper.PointRoot != null)
            {
                var pointList = helper.PointRoot.GetComponentsInChildren<ActionPointCfg>();

                var actionPointGroup = new ActionPointGroup();

                foreach (var point in pointList)
                {
                    var item = new ActionPoint();
                    item.Pos = point.transform.position.ToClientV3();
                    item.Forward = point.transform.forward.ToClientV3();
                    actionPointGroup.List.Add(item);
                    Debug.Log("记录ActionPoint");
                }

                levelCfg.PointList = actionPointGroup;
            }


            if (helper.GamePlayTriggerRoot != null)
            {
                var areaList = helper.GamePlayTriggerRoot.GetComponentsInChildren<TriggerAreaCfg>();

                var triggerAreaGroup = new TriggerAreaGroup();

                foreach (var area in areaList)
                {
                    var item = new TriggerArea();
                    item.Pos = area.transform.position.ToClientV3();
                    item.Forward = area.transform.forward.ToClientV3();
                    item.Shape = (int)area.Shape;
                    item.Size = area.Size.ToClientV3();
                    item.Condition = (int)area.Condition;
                    foreach (var val in area.CheckLayer)
                    {
                        item.CheckLayerList.Add(val);
                    }
                    item.Effect = (int)area.Effect;
                    item.CfgId = area.CfgId;
                    triggerAreaGroup.List.Add(item);
                    Debug.Log("记录TriggerArea:" + item.ToString()) ;
                }

                levelCfg.AreaList = triggerAreaGroup;
            }

            var bytes = JsonMapper.ToJson(levelCfg);// levelCfg.ToByteArray();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (ToolKit.SaveFile(bytes, path + helper.ConfigName + ".json"))
            {
                Debug.Log("保存完毕:" + path + helper.ConfigName);
            }
        }

        public static void DrawPointAndForward(Vector3 pos, Vector3 forward)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pos, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, pos + Quaternion.LookRotation(forward, Vector3.up) * Vector3.forward);
        }
    }
}