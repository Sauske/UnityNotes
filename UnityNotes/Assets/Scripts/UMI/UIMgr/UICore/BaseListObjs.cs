using System.Collections.Generic;

using UnityEngine;

namespace UMI
{
    /// <summary>
    /// 用于自动化脚本中的 数组 list Behaviour 绑定
    /// </summary>
    public class BaseListObjs<T> : MonoBehaviour where T : Behaviour
    {
        public string mNameStart = string.Empty;
        protected List<T> mObjLst = new List<T>();

        public List<T> GetLst
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

        public virtual void ResetList()
        {
            mObjLst.Clear();

            mNameStart = mNameStart.ToLower();

            foreach (var child in this.GetComponentsInChildren<T>())
            {
                if (child.gameObject != this.gameObject)
                {
                    if (string.IsNullOrEmpty(mNameStart))
                    {
                        mObjLst.Add(child);
                    }
                    else if (child.name.ToLower().StartsWith(mNameStart))
                    {
                        mObjLst.Add(child);
                    }
                }
            }
        }

        public virtual T this[int i]
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
    }


}
