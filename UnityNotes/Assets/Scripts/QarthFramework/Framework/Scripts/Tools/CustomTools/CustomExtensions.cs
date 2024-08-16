using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using MainGame;
using System.Text;
#if UNITY_ANDROID
using UnityEngine.Android;
#endif

namespace Qarth
{
    public static class CustomExtensions
    {
        #region helper
        public static string ToStr<T>(this List<T> list)
        {
            StringBuilder str = new StringBuilder("List ToString:");
            foreach (var item in list)
            {
                str.Append($"{item }");
            }
            return str.ToString();
        }

        public static void ShuffleList<T>(this List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static List<T> GetRandomList<T>(List<T> inputList)
        {
            //Set outputList and random
            List<T> outputList = new List<T>();

            while (inputList.Count > 0)
            {
                //Select an index and item
                int rdIndex = RandomHelper.Range(0, inputList.Count);
                T remove = inputList[rdIndex];

                //remove it from copyList and add it to output
                inputList.Remove(remove);
                outputList.Add(remove);
            }
            return outputList;
        }

        public static T CreateHandler<T>(string className) where T : class
        {
            if (string.IsNullOrEmpty(className))
            {
                return null;
            }

            Type type = Type.GetType(className);
            if (type == null)
            {
                Debug.LogError("Not Find Handler Class:" + className);
                return null;
            }

            try
            {
                object obj = Activator.CreateInstance(type, true);

                return obj as T;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return null;
        }

        //震动
        public static void DoBuzz(long milliseconds = 100)
        {
            if (PlayerPrefs.GetInt(EngineDefine.BUZZ_STATE, 1) > 0 && CustomVibration.HasVibrator())
            {
                CustomVibration.Vibrate(milliseconds);
            }
        }
        
        public static T GetStringEnum<T>(string val)
        {
            return (T)Enum.Parse(typeof(T), val);
        }

        //延迟 delay=-1 等一帧
        public static Coroutine CallWithDelay(this MonoBehaviour obj, System.Action call, float delay)
        {
            return obj.StartCoroutine(doCallWithDelay(call, delay));
        }

        static IEnumerator doCallWithDelay(System.Action call, float delay)
        {
            if (delay <= 0)
                yield return null;
            else
            {
                float start = Time.realtimeSinceStartup;
                while (Time.realtimeSinceStartup < start + delay)
                {
                    yield return null;
                }
            }

            if (call != null)
                call.Invoke();
        }

        public static T AddMissingComponent<T>(this GameObject go) where T : Component
        {

            T comp = go.GetComponent<T>();

            if (comp == null)
            {
                comp = go.AddComponent<T>();
            }

            return comp;
        }

        public static string TrimHeadChar(string str, string item)
        {
            if (str.Substring(0, 1) == item)
            {
                return TrimHeadChar(str.Substring(1, str.Length - 1), item);
            }
            else
                return str;
        }

        public static TValue TryGetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue value = default(TValue);
            dict.TryGetValue(key, out value);
            return value;
        }
        public static TValue GetValue<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            TValue value = default(TValue);
            dict.TryGetValue(key, out value);
            return value;
        }
        public static TKey GetKey<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue value)
        {
            TKey key = default(TKey);
            foreach (KeyValuePair<TKey, TValue> kvp in dict)
            {
                if (kvp.Value.Equals(value))
                {
                    key = kvp.Key;
                }
            }
            return key;
        }

        /// <summary>
        /// 给Key赋值，如无Key则添加Key
        /// </summary>
        public static bool AddAndCover<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            bool flag = dict.ContainsKey(key);
            if (flag)
            {
                dict[key] = value;
            }
            else
            {
                dict.Add(key, value);
            }
            return flag;
        }


        #endregion


        #region transform
        // 归零
        static public void ResetTrans(this Transform trans, float scaleRate = 1, bool local = true)
        {
            trans.localPosition = Vector3.zero;
            trans.localEulerAngles = Vector3.zero;
            trans.localScale = Vector3.one * scaleRate;
        }

        static public void SetLocalPos(this GameObject obj, Vector3 pos)
        {
            obj.transform.localPosition = pos;
        }
        static public void SetPos(this GameObject obj, Vector3 pos)
        {
            obj.transform.position = pos;
        }
        static public void SetAngle(this GameObject obj, Vector3 angle)
        {
            obj.transform.localEulerAngles = angle;
        }
        /// 设置X，只改变X c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetX(this Transform trans, float x)
        {
            trans.position = new Vector3(x, trans.position.y, trans.position.z);
        }
        /// <summary>
        /// 设置Y，只改变Y c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetY(this Transform trans, float y)
        {
            trans.position = new Vector3(trans.position.x, y, trans.position.z);
        }
        /// <summary>
        /// 设置Z，只改变Z c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetZ(this Transform trans, float z)
        {
            trans.position = new Vector3(trans.position.x, trans.position.y, z);
        }
        /// <summary>
        /// 设置LocalX，只改变LocalX c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetLocalX(this Transform trans, float x)
        {
            trans.localPosition = new Vector3(x, trans.localPosition.y, trans.localPosition.z);
        }
        /// <summary>
        /// 设置LocalY，只改变LocalY c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetLocalY(this Transform trans, float y)
        {
            trans.localPosition = new Vector3(trans.localPosition.x, y, trans.localPosition.z);
        }
        /// <summary>
        /// 设置LocalZ，只改变LocalZ c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="x"></param>
        static public void SetLocalZ(this Transform trans, float z)
        {
            trans.localPosition = new Vector3(trans.localPosition.x, trans.localPosition.y, z);
        }
        /// <summary>
        /// 在游戏中，朝向的时候只朝向某个坐标，不低头 c
        /// </summary>
        /// <param name="trans"></param>
        /// <param name="target"></param>
        static public void LookAtXZ(this Transform trans, Vector3 target)
        {
            trans.LookAt(new Vector3(target.x, trans.position.y, target.z));
        }
        /// <summary>
        /// 遍历go c
        /// </summary>
        /// <param name="go"></param>
        /// <param name="handle"></param>
        static public void IterateGameObject(this GameObject go, Action<GameObject> handle)
        {
            Queue q = new Queue();
            q.Enqueue(go);
            while (q.Count != 0)
            {
                GameObject tmpGo = (GameObject)q.Dequeue();
                foreach (Transform t in tmpGo.transform)
                {
                    q.Enqueue(t.gameObject);
                }
                if (handle != null)
                {
                    handle(tmpGo);
                }
            }
        }
        /// <summary>
        /// 设置go层级关系 c
        /// </summary>
        /// <param name="go"></param>
        /// <param name="layer"></param>
        static public void SetAllLayer(this GameObject go, int layer)
        {
            IterateGameObject(go, (g) =>
            {
                g.layer = layer;
            });
        }
        /// <summary>
        /// 重置某个物体的三围等 c
        /// </summary>
        /// <param name="go"></param>
        static public void ResetTrs(this GameObject go)
        {
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
        static public void ResetLocalAngle(this GameObject go)
        {
            go.transform.localEulerAngles = Vector3.zero;
        }
        static public void ResetLocalPos(this GameObject go)
        {
            go.transform.localPosition = Vector3.zero;
        }

        static public List<Transform> GetChildTrsList(this Transform trsRoot)
        {
            List<Transform> parts = new List<Transform>();
            for (int i = 0; i < trsRoot.childCount; i++)
                parts.Add(trsRoot.GetChild(i));
            return parts;
        }

        public static float Pixel2DP(float pixel)
        {
            return pixel * 160 / Screen.dpi;
        }

        public static float DP2Pixel(float dp)
        {
            return dp * (Screen.dpi / 160);
        }

        /// <summary>
        /// 递归查找子物体
        /// </summary>
        /// <typeparam name="T">要返回的Component</typeparam>
        /// <param name="fullMatch">全字匹配</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns>没找到返回null</returns>
        public static T FindWithSubString<T>(this Transform transform, string name, bool fullMatch = true, bool ignoreCase = false) where T : Component
        {
            Transform trans = FindFunc(transform, name, fullMatch, ignoreCase);
            if (trans != null)
            {
                return trans.GetComponent<T>();
            }
            else
            {
                return null;
            }
        }
        private static Transform FindFunc(Transform transform, string name, bool fullMatch = true, bool ignoreCase = false)
        {
            Transform target = transform.Find(name);
            if (target != null)
            {
                return target;
            }
            for (int i = 0; i < transform.childCount; i++)
            {
                target = FindFunc(transform.GetChild(i), name);
                if (target != null)
                {
                    return target;
                }
            }
            return null;
        }

        #endregion

        #region physics
        static public void SetRigidBodiesKinematic(this GameObject obj, bool state)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
            {
                if (bodies[i].gameObject.tag == "StayKinematic")
                    bodies[i].isKinematic = true;
                else
                    bodies[i].isKinematic = state;
            }
        }
        static public void SetColliderEnable(this GameObject obj, bool state)
        {
            var cols = obj.GetComponentsInChildren<Collider>();
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].enabled = state;
            }
        }

        static public void AddRigidBodiesForce(this GameObject obj, Vector3 force, ForceMode mode = ForceMode.Force)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
                bodies[i].AddForce(force, mode);
        }
        static public void SetRigidBodiesDrag(this GameObject obj, float dragVal = 0)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
                bodies[i].drag = dragVal;
        }
        static public void SetRigidBodiesAngularDrag(this GameObject obj, float dragVal = 0.05f)
        {
            var bodies = obj.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < bodies.Length; i++)
                bodies[i].angularDrag = dragVal;
        }
        #endregion
        
        #region render
        public static Color HexToColor(string hex, bool alpha = false)
        {
            if (hex.StartsWith("#"))
            {
                hex = hex.Split('#')[1];
            }
            byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            float r = br / 255f;
            float g = bg / 255f;
            float b = bb / 255f;
            if (alpha)
            {
                byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                float a = cc / 255f;
                return new Color(r, g, b, a);
            }
            else
                return new Color(r, g, b);
        }

        public static Texture2D toTexture2D(this RenderTexture rTex, int size = 256)
        {
            Texture2D tex = new Texture2D(size, size, TextureFormat.ARGB32, false);
            RenderTexture.active = rTex;
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();
            return tex;
        }

        static public void SampleAnim(this Animator animator, string stateName, float normalizedTime, int layer = -1)
        {
            animator.StopPlayback();
            animator.Play(stateName, layer, normalizedTime);
            animator.StartPlayback();
        }
        static public void SampleAnim(this Animation anim, string stateName, float normalizedTime)
        {
            anim.Play(stateName);
            anim[stateName].speed = 1;
            anim[stateName].normalizedTime = normalizedTime;
            anim[stateName].speed = 0;
        }
        static public void PlayAnim(this Animation anim, string stateName)
        {
            anim.Play(stateName);
            anim[stateName].speed = 1;
        }
        static public void AddAnimationEventAndPlay(this Animation anim, string clipName, string evnetFunction)
        {
            var clip = anim.GetClip(clipName);
            clip = anim.GetClip(clipName);
            //创建动画事件
            AnimationEvent animationEvent = new AnimationEvent();
            //设置事件回掉函数名字
            animationEvent.functionName = evnetFunction;
            animationEvent.time = clip.length;
            //注册事件
            clip.AddEvent(animationEvent);
        }

        //获取网格的size
        static public Vector3 GetMeshSize(this GameObject objRoot)
        {
            var mesh = objRoot.GetComponent<Renderer>();
            return mesh == null ? Vector3.zero : mesh.bounds.size;
        }
        //获取网格的中心
        static public Vector3 GetMeshCenter(this GameObject objRoot)
        {
            Vector3 boundsCenter = Vector3.zero;
            var meshes = objRoot.GetComponentsInChildren<Renderer>();
            for (int j = 0; j < meshes.Length; j++)
            {
                boundsCenter += meshes[j].bounds.center;
            }
            boundsCenter /= meshes.Length * 1.0f;
            return boundsCenter;
        }

        private static SpritesHandler m_GlobalSprHandler = new SpritesHandler();
        public static Sprite FindSprite(ResLoader loader, string assetName, string spriteName)
        {
            Sprite result = null;
            result = loader.LoadSync(spriteName) as Sprite;
            if (result == null)
            {
                var data = loader.LoadSync(assetName) as SpritesData;
                if (data != null)
                {
                    m_GlobalSprHandler.SetData(new SpritesData[] { data });
                    return m_GlobalSprHandler.FindSprite(spriteName) as Sprite;
                }
            }
            return result;
        }
        #endregion

        #region time
        private static long Jan1st1970Ms = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Local).Ticks;

        public static long CurrentTimeMillis()
        {
            return (System.DateTime.UtcNow.Ticks - Jan1st1970Ms) / 10000;
        }
        public static long CurrentTimeMillis(long ticks)
        {
            return (ticks - Jan1st1970Ms) / 10000;
        }

        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static string GetTimeStampSec()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string GetTimeStampUni()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static string GetTimeStampUniSec()
        {
            TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


        public static DateTime GetTimeFromTimestamp(string timestamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return dtStart.AddMilliseconds(long.Parse(timestamp));
        }

        public static long GetSecFromTimestamps(string timestamp)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dtLast = dtStart.AddMilliseconds(long.Parse(timestamp));
            return (long)(DateTime.Now - dtLast).TotalSeconds;
        }

        public static bool GetIsPassTimeStamp(string timestamp)
        {
            if (string.IsNullOrEmpty(timestamp)) return false;
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds) < long.Parse(timestamp);

        }

        public static string GetNextDayStartTimeStamp()
        {
            var dt = DateTime.Now;
            TimeSpan ts = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0).AddSeconds(86400) - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalMilliseconds).ToString();
        }

        public static string GetFormatTimeString2NextDay()
        {
            var dt = DateTime.Now;
            TimeSpan ts = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0).AddSeconds(86400) - dt;
            return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}"; //"{0:D2,ts.Hours}:{0:D2,ts.Minutes}:{ts.Seconds}";
        }

        public static string GetFormatTimeString2TargetTime(string timeStamp)
        {
            var dt = DateTime.Now;
            TimeSpan ts = GetTimeFromTimestamp(timeStamp) - dt;
            return $"{ts.Hours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}"; //"{0:D2,ts.Hours}:{0:D2,ts.Minutes}:{ts.Seconds}";
        }

        public static string GetTimeStampAfterSec(string timestamp, int sec)
        {
            DateTime dtStart = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var dtLast = dtStart.AddMilliseconds(long.Parse(timestamp)).AddSeconds(sec);
            return Convert.ToInt64((dtLast - dtStart).TotalMilliseconds).ToString();
        }
        public static string GetTimeStamp(DateTime dt)
        {
            return ((dt.Ticks - 621355968000000000) / 10000000).ToString();
        }

        public static void ProcessFuncRunTime(string tag, Action run)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

            stopwatch.Start(); //  开始监视代码运行时间
            run.Invoke();
            stopwatch.Stop(); //  停止监视

            //  获取当前实例测量得出的总时间
            System.TimeSpan timespan = stopwatch.Elapsed;
            //   double hours = timespan.TotalHours; // 总小时
            //    double minutes = timespan.TotalMinutes;  // 总分钟
            //    double seconds = timespan.TotalSeconds;  //  总秒数
            double milliseconds = timespan.TotalMilliseconds;  //  总毫秒数

            //打印代码执行时间
            Debug.Log(string.Format("{0} 运行用时 : {1} ", tag, milliseconds));
        }

        public static int CheckIsNewDay(string dayKey = "lastsignstr--12354371")
        {
            string timeString = PlayerPrefs.GetString(dayKey, "");
            DateTime lastSignDate;
            if (!string.IsNullOrEmpty(timeString))
            {
                if (DateTime.TryParse(timeString, out lastSignDate))
                {
                    DateTime today = DateTime.Today;
                    TimeSpan pass = today - lastSignDate;

                    if (pass.TotalDays >= 1)
                    {
                        PlayerPrefs.SetString(dayKey, DateTime.Today.ToShortDateString());
                    }
                    return pass.Days;
                }
                else
                {
                    PlayerPrefs.SetString(dayKey, DateTime.Today.ToShortDateString());
                    return -1;
                }
            }
            else
            {
                PlayerPrefs.SetString(dayKey, DateTime.Today.ToShortDateString());
                return 999999;
            }
        }
        #endregion

        #region screen_pos
        public static void ScreenPosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScreen, Transform uiTarget)
        {
            Vector3 viewportPos = sceneCamera.ScreenToViewportPoint(posInScreen);
            Vector3 worldPos = uiCamera.ViewportToWorldPoint(viewportPos);
            uiTarget.position = worldPos;
            Vector3 localPos = uiTarget.localPosition;
            localPos.z = 0f;
            uiTarget.localPosition = localPos;
        }
        public static void ScenePosition2UIPosition(Camera sceneCamera, Camera uiCamera, Vector3 posInScene, Transform uiTarget)
        {
            Vector3 viewportPos = sceneCamera.WorldToViewportPoint(posInScene);
            Vector3 worldPos = uiCamera.ViewportToWorldPoint(viewportPos);
            uiTarget.position = worldPos;
            Vector3 localPos = uiTarget.localPosition;
            localPos.z = 0f;
            uiTarget.localPosition = localPos;
        }

        public static Vector3 UIPosToScenePos(Camera sceneCamera, Camera uiCamera, Vector3 uiPos)
        {
            Vector3 viewPos = uiCamera.WorldToViewportPoint(uiPos);
            Vector3 worldPos = sceneCamera.ViewportToWorldPoint(viewPos);
            worldPos.z = 0;
            return worldPos;
        }
        #endregion
        
        public static bool GetInternetState()
        {
            bool state = Application.internetReachability != NetworkReachability.NotReachable;
            return state;
        }
    }

}