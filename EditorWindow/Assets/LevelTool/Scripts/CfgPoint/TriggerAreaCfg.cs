
using UnityEngine;
using System.Collections.Generic;

namespace UMI
{
    public class TriggerAreaCfg : MonoBehaviour
    {
        public TriggerShape Shape;

        public Vector3 Size = new Vector3(2f, 2f, 2f);

        public TriggerCondition Condition;

        public List<LayerMask> CheckLayer;

        public TriggerEffect Effect;

        public int CfgId = 0;


        private void OnDrawGizmos()
        {
            var pos = this.transform.position;
            Gizmos.color = Color.yellow;

            switch (Shape)
            {
                case TriggerShape.Box2D:
                    Gizmos.DrawCube(pos, Size+ Vector3.up * -0.8f);
                    break;
                case TriggerShape.Box:
                    Gizmos.DrawCube(pos, Size);
                    break;
                case TriggerShape.Sphere:
                    Gizmos.DrawSphere(pos, Size.x);
                    break;
                case TriggerShape.Rectangle:
                    Gizmos.DrawCube(pos, Size);
                    break;
                }

        }
    }
}