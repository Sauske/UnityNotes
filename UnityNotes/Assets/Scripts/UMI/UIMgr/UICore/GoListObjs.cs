using System.Collections.Generic;

using UnityEngine;

namespace UMI
{
    /// <summary>
    /// 用于自动化脚本中的 数组 list GameObject 绑定
    /// </summary>
    public class GoListObjs : MonoBehaviour
    {
        public string mNameStart = string.Empty;
        private List<GameObject> mObjLst = new List<GameObject>();

        public List<GameObject> GetLst
        {
            get
            {
                if (mObjLst.Count == 0)
                {
                    ResetList();
                }

                return mObjLst;
            }
        }

        public void ResetList()
        {
            mObjLst.Clear();
            if (string.IsNullOrEmpty(mNameStart))
            {
                return;
            }

            mNameStart = mNameStart.Trim().ToLower();
            int childCount = this.transform.childCount;
            GameObject obj = null;
            for (int i = 0; i < childCount; i++)
            {
                obj = this.transform.GetChild(i).gameObject;
                if (string.IsNullOrEmpty(mNameStart))
                {
                    mObjLst.Add(obj);
                }
                else
                {
                    if (obj.name.ToLower().StartsWith(mNameStart))
                    {
                        mObjLst.Add(obj);
                    }
                }
            }
        }

        public GameObject this[int i]
        {
            get
            {
                if (GetLst.Count > i)
                {
                    return GetLst[i];
                }

                return null;
            }
        }

        public int Count => GetLst.Count;
    }
}
