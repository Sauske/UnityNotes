
using UnityEngine;

namespace UMI
{
    public class ActionPointCfg : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            //var pos = this.transform.position;
            //var forward = this.transform.forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(this.transform.position, 0.1f);
            Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 2f);
        }
    }
}