using UnityEngine;

namespace UMI
{
    public static class UIObjExts
    {

        #region 根据路径查找GameObject

        public static Transform FindTra(this Transform goTra, string objPath)
        {
            if (goTra == null || objPath == null)
            {
                return null;
            }

            return goTra.Find(objPath);
        }

        public static Transform FindTra(this GameObject obj, string objPath)
        {
            if (obj == null || objPath == null)
            {
                return null;
            }

            return obj.transform.FindTra(objPath);
        }

        public static GameObject FindObj(this Transform goTra, string objPath)
        {
            if (goTra == null || objPath == null)
            {
                return null;
            }

            Transform tra = goTra.Find(objPath);
            if (tra != null)
            {
                return tra.gameObject;
            }

            return null;
        }

        public static GameObject FindObj(this GameObject obj, string objPath)
        {
            if (obj == null || objPath == null)
            {
                return null;
            }

            return obj.transform.FindObj(objPath);
        }


        public static T FindObj<T>(this GameObject obj, string objPath) where T : Component
        {
            Transform tra = obj.FindTra(objPath);
            if (tra == null)
            {
                return null;
            }
            else
            {
                T result = tra.GetComponent<T>();
                return result;
            }
        }

        public static T FindObj<T>(this Transform objTra, string objPath) where T : Component
        {
            if (objTra != null)
            {
                return objTra.gameObject.FindObj<T>(objPath);
            }

            return null;
        }


        public static T FindObjAndGetOrAddComponent<T>(this GameObject obj, string objPath) where T : Component
        {
            if (string.IsNullOrEmpty(objPath))
            {
                T result = obj.transform.GetComponent<T>();
                if (result == null)
                {
                    result = obj.transform.gameObject.AddComponent<T>();
                }
                return result;
            }
            else
            {
                Transform tra = obj.FindTra(objPath);
                if (tra == null)
                {
                    return null;
                }
                else
                {
                    T result = tra.GetComponent<T>();
                    if (result == null)
                    {
                        result = tra.gameObject.AddComponent<T>();
                    }
                    return result;
                }
            }
        }

        public static T FindObjAndGetOrAddComponent<T>(this Transform objTra, string objPath) where T : Component
        {
            if (objTra != null)
            {
                return objTra.gameObject.FindObjAndGetOrAddComponent<T>(objPath);
            }

            return null;
        }

        #endregion

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T com = obj.GetComponent<T>();
            if(com == null)
            {
                com = obj.AddComponent<T>();
            }

            return com;
        }

        public static T GetOrAddComponent<T>(this Transform tra) where T : Component
        {
            if(tra != null)
            {
                return tra.gameObject.GetOrAddComponent<T>();
            }

            return null;
        }

        public static T GetOrAddComponent<T>(this Behaviour mono) where T : Component
        {
            if(mono != null)
            {
                return mono.gameObject.GetOrAddComponent<T>();
            }

            return null;
        }

    }
}
