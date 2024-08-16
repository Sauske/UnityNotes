
using UnityEngine;

namespace UMI
{
    public class EysCfgPoint : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            var pos = this.transform.position;
            var forward = this.transform.forward;
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(pos, 0.2f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, pos + Quaternion.LookRotation(forward, Vector3.up) * Vector3.forward);
        }
    }
}