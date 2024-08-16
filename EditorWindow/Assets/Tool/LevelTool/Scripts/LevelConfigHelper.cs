using UnityEngine;

namespace UMI
{
    [ExecuteInEditMode]
    public class LevelConfigHelper : MonoBehaviour
    {
        public string ConfigName;

        public GameObject PointRoot;

        public GameObject GamePlayTriggerRoot;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            if (PointRoot == null)
            {
                PointRoot = new GameObject("点位Root");
                PointRoot.transform.SetParent(this.transform);
            }

            if (GamePlayTriggerRoot == null)
            {
                GamePlayTriggerRoot = new GameObject("触发区域");
                GamePlayTriggerRoot.transform.SetParent(this.transform);
            }
        }
    }
}