using UnityEngine;

namespace UMI
{
    /// <summary>
    /// GameObject 的一些扩展，以后尽量调用这些扩展
    /// </summary>
    public static class GoExts
    {

        #region  SetGoActive

        public static void SetGoActive(this GameObject go, bool bActive)
        {
            if (go != null && go.activeSelf != bActive)
            {
                go.SetActive(bActive);
            }
        }

        public static void SetGoActive(this Transform tra, bool bActive)
        {
            if (tra != null)
            {
                tra.gameObject.SetGoActive(bActive);
            }
        }

        public static void SetGoActive<TBehaviour>(this TBehaviour mono, bool bActive) where TBehaviour : Behaviour
        {
            if (mono != null && mono.gameObject != null)
            {
                mono.gameObject.SetGoActive(bActive);
            }
        }

        #endregion


        #region SetGoLocalScale

        public static void SetGoLocalScale(this Transform tra, Vector3 localScale)
        {
            if (tra != null)
            {
                tra.localScale = localScale;
            }
        }


        public static void SetGoLocalScale(this GameObject go, Vector3 localScale)
        {
            if (go != null && go.transform != null)
            {
                go.transform.SetGoLocalScale(localScale);
            }
        }

        public static void SetGoLocalScale<TBehaviour>(this TBehaviour mono, Vector3 localScale) where TBehaviour : Behaviour
        {
            if (mono != null && mono.transform != null)
            {
                mono.transform.SetGoLocalScale(localScale);
            }
        }

        #endregion


        #region SetGoLocalPos

        public static void SetGoLocalPos(this Transform tra, Vector3 localPos)
        {
            if (tra != null)
            {
                tra.localPosition = localPos;
            }
        }


        public static void SetGoLocalPos(this GameObject go, Vector3 localPos)
        {
            if (go != null && go.transform != null)
            {
                go.transform.SetGoLocalPos(localPos);
            }
        }

        public static void SetGoLocalPos<TBehaviour>(this TBehaviour mono, Vector3 localPos) where TBehaviour : Behaviour
        {
            if (mono != null && mono.transform != null)
            {
                mono.transform.SetGoLocalPos(localPos);
            }
        }

        public static void SetGoLocalPosX(this GameObject go, float x)
        {
            if (go != null && go.transform != null)
            {
                Vector3 posOri = go.transform.localPosition;
                go.transform.SetGoLocalPos(new Vector3(x, posOri.y, posOri.z));
            }
        }

        public static void SetGoLocalPosX(this Transform tra, float x)
        {
            if (tra != null )
            {
                Vector3 posOri = tra.localPosition;
                tra.SetGoLocalPos(new Vector3(x, posOri.y, posOri.z));
            }
        }

        public static void SetGoLocalPosX<TBehaviour>(this TBehaviour mono, float x) where TBehaviour : Behaviour
        {
            if (mono != null && mono.transform != null)
            {
                Vector3 posOri = mono.transform.localPosition;
                mono.transform.SetGoLocalPos(new Vector3(x, posOri.y, posOri.z));
            }
        }

        public static void SetGoLocalPosY(this GameObject go, float y)
        {
            if (go != null && go.transform != null)
            {
                Vector3 posOri = go.transform.localPosition;
                go.transform.SetGoLocalPos(new Vector3(posOri.x, y, posOri.z));
            }
        }

        public static void SetGoLocalPosY(this Transform tra, float y)
        {
            if (tra != null)
            {
                Vector3 posOri = tra.localPosition;
                tra.SetGoLocalPos(new Vector3(posOri.x, y, posOri.z));
            }
        }

        public static void SetGoLocalPosY<TBehaviour>(this TBehaviour mono, float y) where TBehaviour : Behaviour
        {
            if (mono != null && mono.transform != null)
            {
                Vector3 posOri = mono.transform.localPosition;
                mono.transform.SetGoLocalPos(new Vector3(posOri.x, y, posOri.z));
            }
        }

        #endregion



        #region SetGoParent

        public static void SetGoParent(this Transform tra, Transform parentTra, bool worldPositionStays = true)
        {
            if (tra != null && parentTra != null)
            {
                tra.SetParent(parentTra, worldPositionStays);
            }
        }

        public static void SetGoParent(this Transform tra, GameObject parentGo, bool worldPositionStays = true)
        {
            if (tra != null && parentGo != null)
            {
                tra.SetGoParent(parentGo.transform, worldPositionStays);
            }
        }

        public static void SetGoParent(this GameObject go, GameObject parentGo, bool worldPositionStays = true)
        {
            if (go != null)
            {
                go.transform.SetGoParent(parentGo.transform, worldPositionStays);
            }
        }

        public static void SetGoParent(this GameObject go, Transform parentTra, bool worldPositionStays = true)
        {
            if (go != null)
            {
                go.transform.SetGoParent(parentTra, worldPositionStays);
            }
        }

        public static void SetGoParent<T1, T2>(this T1 go, T2 parentTra, bool worldPositionStays = true) where T1 : Behaviour where T2 : Behaviour
        {
            if (go != null)
            {
                go.transform.SetGoParent(parentTra.transform, worldPositionStays);
            }
        }

        #endregion


        #region SetGoAndChildrenlayer

        /// <summary>
        /// GameObject 的 LayMask 修改
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layerValue"></param>
        /// <param name="bChildren"></param>
        public static void SetGoAndChildrenlayer(this GameObject go, int layerValue, bool bChildren = true)
        {
            if (go != null)
            {
                go.gameObject.layer = layerValue;
                if (bChildren)
                {
                    foreach (Transform goTra in go.transform.GetComponentsInChildren<Transform>())
                    {
                        if (goTra != go.transform)
                        {
                            goTra.gameObject.SetGoAndChildrenlayer(layerValue, bChildren);
                        }
                    }
                }
            }
        }

        public static void SetGoAndChildrenlayer(this GameObject go, LayerMask layerMask, bool bChildren = true)
        {
            if (go != null)
            {
                go.SetGoAndChildrenlayer(layerMask.value, bChildren);
            }
        }

        public static void SetGoAndChildrenlayer(this Transform tra, LayerMask layerMask, bool bChildren = true)
        {
            if (tra != null)
            {
                tra.gameObject.SetGoAndChildrenlayer(layerMask.value, bChildren);
            }
        }


        public static void SetGoAndChildrenlayer(this Transform tra, int layerValue, bool bChildren = true)
        {
            if (tra != null)
            {
                tra.gameObject.SetGoAndChildrenlayer(layerValue, bChildren);
            }
        }

        public static void SetGoAndChildrenlayer<T>(this T tra, int layerValue, bool bChildren = true)
            where T : Behaviour
        {
            if (tra != null)
            {
                tra.gameObject.SetGoAndChildrenlayer(layerValue, bChildren);
            }
        }

        #endregion

    }
}
