using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMI
{
    [System.Serializable]
    public class TriggerArea
    {        
        public int Shape;

        public int Condition;

        public int Effect;

        public int CfgId;

        public ClientV3 Pos;

        public ClientV3 Size;

        public ClientV3 Forward;

        public List<int> CheckLayerList;

        public TriggerArea()
        {
            CheckLayerList = new List<int>();
        }
    }

    public struct ClientV3
    {
        public float X;
        public float Y;
        public float Z;

    }

    public class LevelCfg
    {
        public TriggerAreaGroup AreaList { get; set; }
        public ActionPointGroup PointList { get; set; }

        public LevelCfg()
        {
            AreaList = new TriggerAreaGroup();
            PointList = new ActionPointGroup();
        }
    }

    public class ActionPointGroup
    {
        public List<ActionPoint> List { get; set; }

        public ActionPointGroup()
        {
            List = new List<ActionPoint>();
        }
    }

    public class ActionPoint
    {
        public ClientV3 Pos { get; set; }
        public ClientV3 Forward { get; set; }
    }

    public class TriggerAreaGroup
    {
        public List<TriggerArea> List { get; set; }

        public TriggerAreaGroup()
        {
            List = new List<TriggerArea>();
        }
    }
}
