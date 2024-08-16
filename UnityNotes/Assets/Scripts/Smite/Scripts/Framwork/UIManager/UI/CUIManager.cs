//====================================
/// UI管理器
/// 所有UI页面(Form)的开/关等操作必须通过UI管理器
/// @neoyang
/// @2015.03.02
//====================================

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    class SearchBoxEnableParams
    {
        GameObject m_listElement;
        CSPkg m_netData;
        int m_enableIndex;
    }
    class SearchBoxListParams
    {
        GameObject m_list;
        CSPkg m_netData;
    }
    public class CUIManager : Singleton<CUIManager>
    {
        //Form列表
        private ListView<CUIFormScript> m_forms;

        //Pooled Form列表(为了提高效率，form不使用通用的GameObjectPool)
        private ListView<CUIFormScript> m_pooledForms;

        //Form打开顺序号，可以作为显示层次使用
        //private int m_formOpenOrder;

        //Form序列号，可以作为每个Form的唯一标识
        private int m_formSequence;

        //当前存在的Form的序列号列表（用于获取页面的打开顺序）
        private List<int> m_existFormSequences;

        //UI 元素 Root
        private GameObject m_uiRoot;

        // scotzeng++
        public delegate void OnFormSorted(ListView<CUIFormScript> inForms);
        public OnFormSorted onFormSorted = null;
        // scotzeng--

        //UI系统RenderFrameCounter
        public static int s_uiSystemRenderFrameCounter;

        //UI输入事件系统
        private EventSystem m_uiInputEventSystem;

        //Camera
        private Camera m_formCamera;

        public Camera FormCamera
        {
            get
            {
                return m_formCamera;
            }
        }
        private static string s_formCameraName = "Camera_Form";
        private const int c_formCameraMaskLayer = 5;    //LayerMask.NameToLayer("UI");
        private const int c_formCameraDepth = 10;

        //UIScene
        private static string s_uiSceneName = "UI_Scene";

        //Forms相关操作
        private bool m_needSortForms = false;
        private bool m_needUpdateRaycasterAndHide = false;

        //info form widgets
        private enum enInfoFormWidgets
        {
            Title_Text,
            Info_Text,
        }

        //edit form widgets
        public enum enEditFormWidgets
        {
            Title_Text,
            Input_Text,
            Confirm_Button,
        }

        //--------------------------------------------------
        /// 初始化
        //--------------------------------------------------
        public override void Init()
        {
            m_forms = new ListView<CUIFormScript>();
            m_pooledForms = new ListView<CUIFormScript>();
            //m_formOpenOrder = 1;
            m_formSequence = 0;
            m_existFormSequences = new List<int>();

            s_uiSystemRenderFrameCounter = 0;

            //创建UIRoot
            CreateUIRoot();

            //创建EventSystem
            CreateEventSystem();

            //创建Camera
            CreateCamera();

            CreateUISecene();           
        }

        //--------------------------------------------------
        /// 重新加载UI声音
        //--------------------------------------------------
        public void LoadSoundBank()
        {
            //加载UI声音Bank
         //   CSoundManager.GetInstance().LoadBank("UI", CSoundManager.BankType.Global);
        }

        //--------------------------------------------------
        /// Update
        //--------------------------------------------------
        public void Update()
        {
            for (int i = 0; i < m_forms.Count; )
            {
                m_forms[i].CustomUpdate();

                if (m_forms[i].IsNeedClose())
                {
                    //转为关闭状态并回收
                    if (m_forms[i].TurnToClosed(false))
                    {
                        RecycleForm(i);

                        m_needSortForms = true;
                        continue;
                    }
                }
                else if (m_forms[i].IsClosed())
                {
                    //FadeOut结束后回收
                    if (!m_forms[i].IsInFadeOut())
                    {
                        RecycleForm(i);

                        m_needSortForms = true;
                        continue;
                    }
                }

                i++;
            }

            //有form被关闭等sortingOrder发生改变的情况发生，需要对form进行排序，如果关闭form的情况，还需要重置sequence并刷新raycaster
            if (m_needSortForms)
            {
                ProcessFormList(true, true);
            }
            else if (m_needUpdateRaycasterAndHide)
            {
                ProcessFormList(false, true);
            }

            m_needSortForms = false;
            m_needUpdateRaycasterAndHide = false;
        }

        //--------------------------------------------------
        /// LateUpdate
        //--------------------------------------------------
        public void LateUpdate()
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                m_forms[i].CustomLateUpdate();
            }

            //计数器++
            s_uiSystemRenderFrameCounter++;
        }

        //--------------------------------------------------
        /// 打开页面
        /// @formPath
        /// @useFormPool
        /// @useCameraRenderMode
        //--------------------------------------------------
        public CUIFormScript OpenForm(string formPath, bool useFormPool, bool useCameraRenderMode = true)
        {
            //DebugHelper.Log("OpenForm:" + formPath);

            CUIFormScript formScript;

            //检查同名Form是否存在
            formScript = GetUnClosedForm(formPath);

            if (formScript != null && formScript.m_isSingleton)
            {
                //重置formOpenOrder
                RemoveFromExistFormSequenceList(formScript.GetSequence());
                AddToExistFormSequenceList(m_formSequence);

                //更新sequence
                int formOpenOrder = GetFormOpenOrder(m_formSequence);
                formScript.Open(m_formSequence, true, formOpenOrder);

                m_formSequence++;
                //m_formOpenOrder++;

                m_needSortForms = true;

                //更新事件输入检测
                //formScript.SetEnableInput(true);

                return formScript;
            }

            GameObject form = CreateForm(formPath, useFormPool);

            if (form == null)
            {
                DebugHelper.ConsoleLogError("Form " + formPath + " Open Fail!!!");
                return null;
            }

            //确保form为active
            if (!form.activeSelf)
            {
                form.CustomSetActive(true);
            }

            //修改form root的名字并挂到manager下面            
            string name = GetFormName(formPath);
            form.name = name;

            //将form
            if (form.transform.parent != m_uiRoot.transform)
            {
                form.transform.SetParent(m_uiRoot.transform);
            }
            
            //设置FormScript参数
            formScript = form.GetComponent<CUIFormScript>();
            if (formScript != null)
            {
                AddToExistFormSequenceList(m_formSequence);
                int formOpenOrder = GetFormOpenOrder(m_formSequence);
                formScript.Open(formPath, useCameraRenderMode ? m_formCamera : null, m_formSequence, false, formOpenOrder);

                //close 同组Form
                if (formScript.m_group > 0)
                {
                    CloseGroupForm(formScript.m_group);
                }

                m_forms.Add(formScript);
            }

            m_formSequence++;
            //m_formOpenOrder++;

            m_needSortForms = true;

            //更新事件输入检测
            //formScript.SetEnableInput(true);

            //关闭之前任何地方的点击特效
            //CUIParticleSystem.GetInstance().ClearAll();

            return formScript;
        }

        //--------------------------------------------------
        /// 关闭页面
        /// @formPath
        //--------------------------------------------------
        public void CloseForm(string formPath)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                if (string.Equals(m_forms[i].m_formPath, formPath))
                {
                    m_forms[i].Close();
                }
            }
        }

        //--------------------------------------------------
        /// 关闭页面
        /// @formScript
        //--------------------------------------------------
        public void CloseForm(CUIFormScript formScript)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                if (m_forms[i] == formScript)
                {
                    m_forms[i].Close();
                }
            }
        }

        //--------------------------------------------------
        /// 关闭页面
        /// @formSequence
        //--------------------------------------------------
        public void CloseForm(int formSequence)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                if (m_forms[i].GetSequence() == formSequence)
                {
                    m_forms[i].Close();
                }
            }
        }


        public void CloseAllFormExceptLobby(bool closeImmediately = true)
        {
            //清理大厅以外的其它UI
            string[] formPathArr = { };
            CUIManager.GetInstance().CloseAllForm(formPathArr, closeImmediately);
        }

        //--------------------------------------------------
        /// 关闭所有页面
        /// @exceptFormNames    : 排除列表
        /// @closeImmediately   : 是否立即执行close
        /// @clearFormPool      : 是否清理掉pool
        //--------------------------------------------------
        public void CloseAllForm(string[] exceptFormNames = null, bool closeImmediately = true, bool clearFormPool = true)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                bool close = true;

                if (exceptFormNames != null)
                {
                    for (int j = 0; j < exceptFormNames.Length; j++)
                    {
                        if (string.Equals(m_forms[i].m_formPath, exceptFormNames[j]))
                        {
                            close = false;
                            break;
                        }
                    }
                }

                if (close)
                {
                    m_forms[i].Close();
                }                
            }

            if (closeImmediately)
            {
                for (int i = 0; i < m_forms.Count; )
                {
                    if (m_forms[i].IsNeedClose() || m_forms[i].IsClosed())
                    {
                        //转为关闭状态并回收(忽略fadeout)
                        if (m_forms[i].IsNeedClose())
                        {
                            m_forms[i].TurnToClosed(true);
                        }

                        RecycleForm(i);

                        continue;
                    }

                    i++;
                }

                if (exceptFormNames != null)
                {
                    ProcessFormList(true, true);
                }
            }

            if (clearFormPool)
            {
                ClearFormPool();
            }
        }

        private void RecycleForm(int formIndex)
        {
            RemoveFromExistFormSequenceList(m_forms[formIndex].GetSequence());
            RecycleForm(m_forms[formIndex]);
            m_forms.RemoveAt(formIndex);
        }

        /// <summary>
        /// 把一个页面序列号加入到打开页面序列号列表
        /// </summary>
        /// <param name="formSequence"></param>
        public void AddToExistFormSequenceList(int formSequence)
        {
            if (m_existFormSequences != null)
            {
                m_existFormSequences.Add(formSequence);
                //DebugHelper.LogError(string.Format("add {0}", formSequence));
            }
        }

        /// <summary>
        /// 从打开页面序列化列表中移除某个页面序列号
        /// </summary>
        /// <param name="formSequence"></param>
        public void RemoveFromExistFormSequenceList(int formSequence)
        {
            if (m_existFormSequences != null)
            {
                m_existFormSequences.Remove(formSequence);
                //DebugHelper.LogError(string.Format("remove {0}", formSequence));
            }
        }

        /// <summary>
        /// 获取页面的打开顺序
        /// </summary>
        /// <param name="formSequence"></param>
        /// <returns></returns>
        public int GetFormOpenOrder(int formSequence)
        {
            //以formSequence的索引加1作为页面的打开顺序
            int index = m_existFormSequences.IndexOf(formSequence);
            return index >= 0 ? index + 1 : 0;
        }

        //--------------------------------------------------
        /// 是否存在UI页面
        //--------------------------------------------------
        public bool HasForm()
        {
            return (m_forms.Count > 0);
        }

        //--------------------------------------------------
        /// 获取UI页面
        /// @formPath
        //--------------------------------------------------
        public CUIFormScript GetForm(string formPath)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                if (m_forms[i].m_formPath.Equals(formPath) && !m_forms[i].IsNeedClose() && !m_forms[i].IsClosed())
                {
                    return m_forms[i];
                }
            }

            return null;
        }

        //--------------------------------------------------
        /// 获取UI页面
        /// @formSequence
        //--------------------------------------------------
        public CUIFormScript GetForm(int formSequence)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                if (m_forms[i].GetSequence() == formSequence && !m_forms[i].IsNeedClose() && !m_forms[i].IsClosed())
                {
                    return m_forms[i];
                }
            }

            return null;
        }

        //--------------------------------------------------
        /// 关闭同组Form
        /// @group
        //--------------------------------------------------
        public void CloseGroupForm(int group)
        {
            if (group == 0)
            {
                return;
            }

            for (int i = 0; i < m_forms.Count; i++)
            {
                if (m_forms[i].m_group == group)
                {
                    m_forms[i].Close();
                }
            }
        }

        //--------------------------------------------------
        /// 屏蔽输入
        //--------------------------------------------------
        public void DisableInput()
        {
            if (m_uiInputEventSystem != null)
            {
                m_uiInputEventSystem.gameObject.CustomSetActive(false);
            }
        }

        //--------------------------------------------------
        /// 允许输入
        //--------------------------------------------------
        public void EnableInput()
        {
            if (m_uiInputEventSystem != null)
            {
                m_uiInputEventSystem.gameObject.CustomSetActive(true);
            }
        }

        //清理UGUI的缓存造成的内存泄漏 
        public void ClearEventGraphicsData()
        {
            System.Reflection.MemberInfo[] memberInfos =
            typeof(UnityEngine.UI.GraphicRaycaster).GetMember("s_SortedGraphics",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.IgnoreCase);

            if (memberInfos != null && memberInfos.Length == 1)
            {
                System.Reflection.MemberInfo mi = memberInfos[0];
                if (mi != null && mi.MemberType == System.Reflection.MemberTypes.Field)
                {
                    System.Reflection.FieldInfo fi = mi as System.Reflection.FieldInfo;
                    if (fi != null)
                    {
                        List<UnityEngine.UI.Graphic> list =
                            fi.GetValue(null) as List<UnityEngine.UI.Graphic>;

                        if (list != null)
                        {
                            list.Clear();
                        }
                    }
                }
            }
        }

        //--------------------------------------------------
        /// 清理Form Pool
        //--------------------------------------------------
        public void ClearFormPool()
        {
            for (int i = 0; i < m_pooledForms.Count; i++)
            {
               // CUICommonSystem.DestoryObj(m_pooledForms[i].gameObject);
            }

            m_pooledForms.Clear();
        }

        //--------------------------------------------------
        /// 返回位于最顶层的Form
        //--------------------------------------------------
        public CUIFormScript GetTopForm()
        {
            CUIFormScript topFormScript = null;

            for (int i = 0; i < m_forms.Count; i++)
            {
                if (m_forms[i] == null)
                {
                    continue;
                }

                if (topFormScript == null)
                {
                    topFormScript = m_forms[i];
                }
                else if (m_forms[i].GetSortingOrder() > topFormScript.GetSortingOrder())
                {
                    topFormScript = m_forms[i];
                }
            }

            return topFormScript;
        }

        public ListView<CUIFormScript> GetForms()
        {
            return m_forms;
        }

        //--------------------------------------------------
        /// 返回EventSystem
        //--------------------------------------------------
        public EventSystem GetEventSystem()
        {
            return m_uiInputEventSystem;
        }

        //重置所有form的Hide状态
        public void ResetAllFormHideOrShowState()
        {
            m_needUpdateRaycasterAndHide = true;
            //ProcessFormList(false,true);
        }

        //--------------------------------------------------
        /// 处理Forms次序相关
        /// @sort : 是否排序(按显示顺序升序排列)
        /// @handleInputAndHide : 是否处理输入屏蔽及隐藏相关
        //--------------------------------------------------
        private void ProcessFormList(bool sort, bool handleInputAndHide)
        {
            if (sort)
            {
                m_forms.Sort();

//                 if (true)   //(m_formOpenOrder > 10)
//                 {
//                     for (int i = 0; i < m_forms.Count; i++)
//                     {
//                         m_forms[i].SetDisplayOrder(i + 1);
//                     }
// 
//                     m_formOpenOrder = m_forms.Count + 1;
//                 }
                for (int i = 0; i < m_forms.Count; i++)
                {
                    int formOpenOrder = GetFormOpenOrder(m_forms[i].GetSequence());
                    m_forms[i].SetDisplayOrder(formOpenOrder);
                }
            }

            if (handleInputAndHide)
            {
                UpdateFormHided();
                UpdateFormRaycaster(); 
            }

            if (onFormSorted != null)
            {
                onFormSorted(m_forms);
            }
        }

        //--------------------------------------------------
        /// 从FormPath返回FormName(不带扩展名)
        //--------------------------------------------------
        private string GetFormName(string formPath)
        {
            return CFileManager.EraseExtension(CFileManager.GetFullName(formPath));
        }

        //--------------------------------------------------
        /// 从Prefab创建出页面实例
        /// @formPrefabPath
        /// @useFormPool
        //--------------------------------------------------
        private GameObject CreateForm(string formPrefabPath, bool useFormPool)
        {
            GameObject form = null;

            //尝试从Pool中获取
            if (useFormPool)
            {                
                for (int i = 0; i < m_pooledForms.Count; i++)
                {
                    if (string.Equals(formPrefabPath, m_pooledForms[i].m_formPath, StringComparison.OrdinalIgnoreCase))
                    {
                        m_pooledForms[i].Appear();

                        form = m_pooledForms[i].gameObject;

                        m_pooledForms.RemoveAt(i);

                        break;
                    }
                }
            }

            //尝试创建
            if (form == null)
            {
                GameObject formPrefab = null;// (GameObject)CResourceManager.GetInstance().GetResource(formPrefabPath, typeof(GameObject), enResourceType.UIForm).m_content;
                if (formPrefab == null)
                {
                    DebugHelper.ConsoleLogError("Error form path:" + formPrefabPath);
                    return null;
                }

                form = (GameObject)GameObject.Instantiate(formPrefab);
            }

            if (form != null)
            {
                CUIFormScript formScript = form.GetComponent<CUIFormScript>();
                if (formScript != null)
                {
                    formScript.m_useFormPool = useFormPool;
                }
            }

            return form;
        }

        //--------------------------------------------------
        /// 回收页面实例
        //--------------------------------------------------
        private void RecycleForm(CUIFormScript formScript)
        {
            if (formScript == null)
            {
                return;
            }

            if (formScript.m_useFormPool)
            {
                //隐藏并添加到pool列表
                formScript.Hide();
                m_pooledForms.Add(formScript);
            }
            else
            {
                try
                {
                    if (formScript.m_canvasScaler != null)
                    {
                        formScript.m_canvasScaler.enabled = false;
                    }

                //   GemeObject.Destroy(formScript.gameObject,0);
                }
                catch (Exception e)
                {
                    DebugHelper.Assert(false, "Error destroy {0} formScript gameObject: message: {1}, callstack: {2}", formScript.name, e.Message, e.StackTrace);
                }
                
            }
        }

        //--------------------------------------------------
        /// 获取目前还未处于关闭状态的指定页面
        /// @formPath
        //--------------------------------------------------
        private CUIFormScript GetUnClosedForm(string formPath)
        {
            for (int i = 0; i < m_forms.Count; i++)
            {
                if (m_forms[i].m_formPath.Equals(formPath) && !m_forms[i].IsClosed())
                {
                    return m_forms[i];
                }
            }

            return null;
        }

        //--------------------------------------------------
        /// 创建UI Root
        //--------------------------------------------------
        private void CreateUIRoot()
        {
            m_uiRoot = new GameObject("CUIManager");

            GameObject bootObj = GameObject.Find("BootObj");
            if (bootObj != null)
            {
                m_uiRoot.transform.parent = bootObj.transform;
            }
        }

        //--------------------------------------------------
        /// 创建事件系统
        //--------------------------------------------------
        private void CreateEventSystem()
        {
            m_uiInputEventSystem = UnityEngine.Object.FindObjectOfType<EventSystem>();

            if (m_uiInputEventSystem == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                m_uiInputEventSystem = eventSystem.AddComponent<EventSystem>();
#if UNITY_EDITOR || UNITY_STANDALONE
                eventSystem.AddComponent<StandaloneInputModule>();
#endif
                eventSystem.AddComponent<TouchInputModule>();
            }

            m_uiInputEventSystem.gameObject.transform.parent = m_uiRoot.transform;
        }

        //--------------------------------------------------
        /// 创建Camera
        //--------------------------------------------------
        private void CreateCamera()
        {
            //if (CUIUtility.c_formRenderMode != enFormRenderMode.Camera)
            //{
            //    return;
            //}

            GameObject cameraObject = new GameObject(s_formCameraName);
            cameraObject.transform.SetParent(m_uiRoot.transform, true);
            cameraObject.transform.localPosition = Vector3.zero;
            cameraObject.transform.localRotation = Quaternion.identity;
            cameraObject.transform.localScale = Vector3.one;

            Camera camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;
            camera.orthographicSize = 50;
            camera.clearFlags = CameraClearFlags.Depth;
            camera.cullingMask = 1 << c_formCameraMaskLayer;
            camera.depth = c_formCameraDepth;

            m_formCamera = camera;
        }

        //--------------------------------------------------
        /// 创建UIScene
        //--------------------------------------------------
        private void CreateUISecene()
        {
            GameObject uiSceneObject = new GameObject(s_uiSceneName);

            uiSceneObject.transform.parent = m_uiRoot.transform;
        }

        //--------------------------------------------------
        /// Update Form 输入响应
        //--------------------------------------------------
        private void UpdateFormRaycaster()
        {
            bool respondInput = true;

            for (int i = m_forms.Count - 1; i >= 0; i--)
            {
                if (m_forms[i].m_disableInput || m_forms[i].IsHided())
                {
                    continue;
                }

                GraphicRaycaster graphicRaycaster = m_forms[i].GetGraphicRaycaster();
                if (graphicRaycaster != null)
                {
                    graphicRaycaster.enabled = respondInput;
                }

                if (m_forms[i].m_isModal && respondInput)
                {
                    respondInput = false;
                }
            }
        }

        //--------------------------------------------------
        /// Update Form 隐藏情况
        //--------------------------------------------------
        private void UpdateFormHided()
        {
            bool needHide = false;

            for (int i = m_forms.Count - 1; i >= 0; i--)
            {
                if (needHide)
                {
                    m_forms[i].Hide(enFormHideFlag.HideByOtherForm, false);
                }
                else
                {
                    m_forms[i].Appear(enFormHideFlag.HideByOtherForm, false);
                }

                if (!needHide && !m_forms[i].IsHided() && m_forms[i].m_hideUnderForms)
                {
                    needHide = true;
                }
            }
        }

        //--------------------------------------------------
        /// UIForm优先级改变事件回调
        //--------------------------------------------------
        private void OnFormPriorityChanged(CUIEvent uiEvent)
        {
            m_needSortForms = true;
        }

        //--------------------------------------------------
        /// UIForm能否显示状态改变事件回调
        //--------------------------------------------------
        private void OnFormVisibleChanged(CUIEvent uiEvent)
        {
            m_needUpdateRaycasterAndHide = true;
        }

        ///// <summary>
        ///// 显示通用tips
        ///// bReadDatabin 是否读取配置档，strContent为key
        ///// </summary>
        ///// <param name="strContent"></param>
        //public void OpenTips(string strContent, bool bReadDatabin = false, float timeDuration = 1.5f, GameObject referenceGameObject = null, params object[] replaceArr)
        //{
        //    string content = strContent;
            
        //    if (bReadDatabin)
        //    {
        //        content = CTextManager.GetInstance().GetText(strContent);
        //    }

        //    if (string.IsNullOrEmpty(content))
        //    {
        //        return;
        //    }

        //    if (replaceArr != null)
        //    {
        //        try
        //        {
        //            content = string.Format(content, replaceArr);
        //        }
        //        catch(FormatException e)
        //        {
        //            DebugHelper.Assert(false, "Format Exception for string \"{0}\", Exception:{1}", content, e.Message);
        //        }                
        //    }

        //    CUIFormScript form = CUIManager.GetInstance().OpenForm(CUIUtility.s_Form_Common_Dir + "Form_Tips.prefab", false, false);
        //    if ( form != null )
        //    {
        //        Text lblContent = form.gameObject.transform.Find( "Panel/Text" ).GetComponent<Text>();

        //        lblContent.text = content;
        //    }

        //    //居中对齐
        //    if (form != null && referenceGameObject != null)
        //    {
        //        RectTransform rectTransform = referenceGameObject.GetComponent<RectTransform>();
        //        RectTransform txtPnl = form.gameObject.transform.Find("Panel") as RectTransform;

        //        if (rectTransform != null && txtPnl != null)
        //        {
        //            //参考对象的四个角
        //            Vector3[] corners = new Vector3[4];
        //            rectTransform.GetWorldCorners(corners);

        //            float width = Math.Abs(CUIUtility.WorldToScreenPoint(CUIManager.instance.FormCamera, corners[2]).x - CUIUtility.WorldToScreenPoint(CUIManager.instance.FormCamera, corners[0]).x);
        //            float height = Math.Abs(CUIUtility.WorldToScreenPoint(CUIManager.instance.FormCamera, corners[2]).y - CUIUtility.WorldToScreenPoint(CUIManager.instance.FormCamera, corners[0]).y);


        //            Vector2 center = new Vector2(CUIUtility.WorldToScreenPoint(CUIManager.instance.FormCamera, corners[0]).x + width / 2, CUIUtility.WorldToScreenPoint(CUIManager.instance.FormCamera, corners[0]).y + height / 2);

        //            txtPnl.position = CUIUtility.ScreenToWorldPoint(null, center, txtPnl.position.z);
        //        }
        //    }

        //    if (form != null)
        //    {
        //        CUITimerScript timerScript = form.gameObject.transform.Find("Timer").GetComponent<CUITimerScript>();
        //        timerScript.EndTimer();
        //        timerScript.m_totalTime = timeDuration;
        //        timerScript.StartTimer();
        //    }

        //  //  CSoundManager.instance.PostEvent("UI_Click");
        //}

        //----------------------------------------------
        /// Tips Form是否已经存在
        //----------------------------------------------
        public bool IsTipsFormExist()
        {
            return (GetForm(CUIUtility.s_Form_Common_Dir + "Form_Tips.prefab") != null);
        }

        //前置关闭tips
        public void CloseTips()
        {
            CloseForm(CUIUtility.s_Form_Common_Dir + "Form_Tips.prefab");
        }

        //显示菊花
        public void OpenSendMsgAlert(int autoCloseTime = 5, enUIEventID timeUpEventId = enUIEventID.None)
        {
            //Debug.LogError(string.Format("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX where call alter {0}", Environment.StackTrace));
            CUIEvent evt = new CUIEvent();
            evt.m_eventID = enUIEventID.Common_SendMsgAlertOpen;
            stUIEventParams pars = new stUIEventParams();
            pars.param1 = autoCloseTime;
            pars.param2 = (int)timeUpEventId;
            evt.m_eventParams = pars;
            CUIEventManager.GetInstance().DispatchUIEvent(evt);
        }

        //显示菊花
        public void OpenSendMsgAlert(string txtContent, int autoCloseTime = 10, enUIEventID timeUpEventId = enUIEventID.None)
        {
            //Debug.LogError(string.Format("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX where call alter {0} stack {1}", txtContent, Environment.StackTrace));
            CUIEvent evt = new CUIEvent();
            evt.m_eventID = enUIEventID.Common_SendMsgAlertOpen;
            stUIEventParams pars = new stUIEventParams();
            pars.param1 = txtContent;
            pars.param2 = autoCloseTime;
          //  pars.tag2 = (int)timeUpEventId;
            evt.m_eventParams = pars;
            CUIEventManager.GetInstance().DispatchUIEvent(evt);
        }

        //关闭菊花
        public void CloseSendMsgAlert()
        {
            CUIEvent evt = new CUIEvent();
            evt.m_eventID = enUIEventID.Common_SendMsgAlertClose;
            CUIEventManager.GetInstance().DispatchUIEvent(evt);
        }

        /// <summary>
        /// 弹出MessageBox对话框
        /// </summary>
        /// <param name="strContent">提示内容</param>
        public void OpenMessageBox(string strContent, bool isContentLeftAlign = false)
        {
            stUIEventParams ue = new stUIEventParams();
            OpenMessageBoxBase(strContent, false, enUIEventID.None, enUIEventID.None, ue, isContentLeftAlign);
        }

        /// <summary>
        /// 弹出MessageBox对话框
        /// </summary>
        /// <param name="strContent">提示内容</param>
        /// <param name="confirmID">确定按钮派发事件ID</param>
        public void OpenMessageBox(string strContent, enUIEventID confirmID, bool isContentLeftAlign = false)
        {
            stUIEventParams ue = new stUIEventParams();
            OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, ue, isContentLeftAlign);
        }

        /// <summary>
        /// 弹出MessageBox对话框
        /// </summary>
        /// <param name="strContent">提示内容</param>
        /// <param name="confirmID">确定按钮派发事件ID</param>
        /// <param name="par">事件参数</param>
        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, bool isContentLeftAlign = false)
        {
            OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="par"></param>
        /// <param name="confirmStr"></param>
        /// <param name="isContentLeftAlign"></param>
        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, bool isContentLeftAlign = false)
        {
            OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="par"></param>
        /// <param name="confirmStr"></param>
        /// <param name="titleStr"></param>
        /// <param name="isContentLeftAlign"></param>
        public void OpenMessageBox(string strContent, enUIEventID confirmID, stUIEventParams par, string confirmStr, string titleStr, bool isContentLeftAlign = false)
        {
            OpenMessageBoxBase(strContent, false, confirmID, enUIEventID.None, par, isContentLeftAlign, confirmStr, "", titleStr);
        }

        /// <summary>
        /// 带取消按钮的messagebox
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>
        /// <param name="par"></param>
        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, bool isContentLeftAlign = false)
        {
            OpenMessageBoxBase(strContent, true, confirmID, cancelID, new stUIEventParams(), isContentLeftAlign);
        }

        /// <summary>
        /// 带取消按钮和参数的messagebox
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>
        /// <param name="par"></param>
        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false)
        {
            OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign);
        }

        /// <summary>
        /// 带取消按钮和参数的messagebox
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>
        /// <param name="par"></param>
        public void OpenMessageBoxWithCancelAndAutoClose(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, bool isContentLeftAlign = false, int autoCloseTime = 0 , enUIEventID timeUpID = enUIEventID.None, string confirmStr = "", string cancelStr = "")
        {
            OpenMessageBoxBase(strContent, true, confirmID, cancelID, par, isContentLeftAlign, confirmStr, cancelStr, "", autoCloseTime, timeUpID);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>        
        /// <param name="confirmStr"></param>
        /// <param name="cancelStr"></param>
        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID,
            string confirmStr, string cancelStr, bool isContentLeftAlign = false)
        {
            stUIEventParams ue = new stUIEventParams();
            OpenMessageBoxBase(strContent, true, confirmID, cancelID, ue, isContentLeftAlign, confirmStr, cancelStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>
        /// <param name="param"></param>
        /// <param name="confirmStr"></param>
        /// <param name="cancelStr"></param>
        public void OpenMessageBoxWithCancel(string strContent, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams param,
            string confirmStr, string cancelStr, bool isContentLeftAlign = false, string titleStr = "")
        {
            OpenMessageBoxBase(strContent, true, confirmID, cancelID, param, isContentLeftAlign, confirmStr, cancelStr, titleStr);
        }

        /// <summary>
        /// 弹出MessageBox对话框
        /// </summary>
        /// <param name="strContent">提示内容</param>
        /// <param name="isHaveCancelBtn">是否显示取消按钮</param>
        /// <param name="confirmID">确定派发事件ID</param>
        /// <param name="cancelID">取消派发事件ID</param>
        /// <param name="par">事件参数</param>
        /// <param name="isContentLeftAlign">文本是否左对齐(默认文本为居中对齐)</param>
        /// <param name="confirmStr">确定文本</param>
        /// <param name="cancelStr">取消文本</param>       
        private void OpenMessageBoxBase(string strContent,bool isHaveCancelBtn,enUIEventID confirmID,enUIEventID cancelID,stUIEventParams par, 
            bool isContentLeftAlign = false, string confirmStr = "", string cancelStr = "", string titleStr = "",int autoCloseTime = 0 ,
            enUIEventID timeUpID = enUIEventID.None)
        {
            CUIFormScript form = CUIManager.GetInstance().OpenForm(CUIUtility.s_Form_Common_Dir + "Form_MessageBox.prefab", false, false);
            if (form == null)
            {
                return;
            }

            GameObject root = form.gameObject;
            if (root == null)
            {
                return;
            }

            if (confirmStr == "")
            {
                confirmStr = CTextManager.GetInstance().GetText("Common_Confirm");
            }
            if (cancelStr == "")
            {
                cancelStr = CTextManager.GetInstance().GetText("Common_Cancel");
            }
            if (titleStr == "")
            {
                titleStr = CTextManager.GetInstance().GetText("Common_MsgBox_Title");
            }

            GameObject btnConfirm = root.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
            btnConfirm.GetComponentInChildren<Text>().text = confirmStr;
            GameObject btnCancel = root.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
            btnCancel.GetComponentInChildren<Text>().text = cancelStr;
            GameObject titleObj = root.transform.Find("Panel/Panel/title/Text").gameObject;
            titleObj.GetComponentInChildren<Text>().text = titleStr;

            Text txtContent = root.transform.Find("Panel/Panel/Text").GetComponent<Text>();

            txtContent.text = strContent;
            if (!isHaveCancelBtn)
            {
                btnCancel.CustomSetActive(false);
            }
            else
            {
                btnCancel.CustomSetActive(true);
            }

            CUIEventScript btnConfirmEventScript = btnConfirm.GetComponent<CUIEventScript>();
            CUIEventScript btnCancelEventScript = btnCancel.GetComponent<CUIEventScript>();

            btnConfirmEventScript.SetUIEvent(enUIEventType.Click, confirmID, par);
            btnCancelEventScript.SetUIEvent(enUIEventType.Click, cancelID, par);

            if (isContentLeftAlign)
            {
                txtContent.alignment = TextAnchor.MiddleLeft;
            }

            if (autoCloseTime != 0)
            {
                Transform timerTrans = form.transform.Find("closeTimer");
                if (timerTrans != null)
                {
                    CUITimerScript timerScript = timerTrans.GetComponent<CUITimerScript>();
                    if (timerScript != null)
                    {
                        timerScript.SetTotalTime(autoCloseTime);
                        timerScript.StartTimer();
                        timerScript.m_eventIDs[(Int32)enTimerEventType.TimeUp] = timeUpID;
                        timerScript.m_eventParams[(Int32)enTimerEventType.TimeUp] = par;
                    }
                }
            }

            //关闭菊花
            CloseSendMsgAlert();
        }

        //关闭所有MessageBox
        public void CloseMessageBox()
        {
            CUIManager.GetInstance().CloseForm(CUIUtility.s_Form_Common_Dir + "Form_MessageBox.prefab");
        }

        /// <summary>
        /// 打开小对话窗口
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="isHaveCancelBtn"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>
        /// <param name="par"></param>
        /// <param name="autoCloseTime"></param>
        /// <param name="closeTimeID">倒计时结束后发出的事件</param>
        /// <param name="isContentLeftAlign"></param>
        /// <param name="confirmStr"></param>
        /// <param name="cancelStr"></param>
        public void OpenSmallMessageBox(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par, int autoCloseTime = 0, enUIEventID closeTimeID = 0, string confirmStr = "", string cancelStr = "", bool isContentLeftAlign = false)
        {
            CUIFormScript form = CUIManager.GetInstance().OpenForm(CUIUtility.s_Form_Common_Dir + "Form_SmallMessageBox.prefab", false, false);
            if (form == null)
            {
                return;
            }

            GameObject root = form.gameObject;
            if (root == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(confirmStr))
            {
                confirmStr = CTextManager.GetInstance().GetText("Common_Confirm");
            }
            if (string.IsNullOrEmpty(cancelStr))
            {
                cancelStr = CTextManager.GetInstance().GetText("Common_Cancel");
            }


            GameObject btnConfirm = root.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
            btnConfirm.GetComponentInChildren<Text>().text = confirmStr;
            GameObject btnCancel = root.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
            btnCancel.GetComponentInChildren<Text>().text = cancelStr;

            Text txtContent = root.transform.Find("Panel/Panel/Text").GetComponent<Text>();

            txtContent.text = strContent;
            if (!isHaveCancelBtn)
            {
                btnCancel.CustomSetActive(false);
            }
            else
            {
                btnCancel.CustomSetActive(true);
            }

            CUIEventScript btnConfirmEventScript = btnConfirm.GetComponent<CUIEventScript>();
            CUIEventScript btnCancelEventScript = btnCancel.GetComponent<CUIEventScript>();

            btnConfirmEventScript.SetUIEvent(enUIEventType.Click, confirmID, par);
            btnCancelEventScript.SetUIEvent(enUIEventType.Click, cancelID, par);

            if (isContentLeftAlign)
            {
                txtContent.alignment = TextAnchor.MiddleLeft;
            }

            if (autoCloseTime != 0)
            {
                Transform timerTrans = form.transform.Find("closeTimer");
                if (timerTrans != null)
                {
                    CUITimerScript timerScript = timerTrans.GetComponent<CUITimerScript>();
                    if (timerScript != null)
                    {
                        if (closeTimeID > 0)
                        {
                            timerScript.m_eventIDs[(int)enTimerEventType.TimeUp] = closeTimeID;
                        }
                        timerScript.SetTotalTime(autoCloseTime);
                        timerScript.StartTimer();
                    }
                }
            }

            //关闭菊花
            CloseSendMsgAlert();
        }

        /// <summary>
        /// 关闭小对话窗口
        /// </summary>
        public void CloseSmallMessageBox()
        {
            CUIManager.GetInstance().CloseForm(CUIUtility.s_Form_Common_Dir + "Form_SmallMessageBox.prefab");
        }


        public void OpenMessageBoxBig(string strContent, bool isHaveCancelBtn, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par,bool isContentLeftAlign = false, string confirmStr = "", string cancelStr = "", string titleStr = "", int autoCloseTime = 0,enUIEventID timeUpID = enUIEventID.None)
        {
            CUIFormScript form = CUIManager.GetInstance().OpenForm(CUIUtility.s_Form_Common_Dir + "Form_MessageBoxBig.prefab", false, false);
            if (form == null)
            {
                return;
            }

            GameObject root = form.gameObject;
            if (root == null)
            {
                return;
            }

            if (confirmStr == "")
            {
                confirmStr = CTextManager.GetInstance().GetText("Common_Confirm");
            }
            if (cancelStr == "")
            {
                cancelStr = CTextManager.GetInstance().GetText("Common_Cancel");
            }
            if (titleStr == "")
            {
                titleStr = CTextManager.GetInstance().GetText("Common_MsgBox_Title");
            }

            GameObject btnConfirm = root.transform.Find("Panel/Panel/btnGroup/Button_Confirm").gameObject;
            btnConfirm.GetComponentInChildren<Text>().text = confirmStr;
            GameObject btnCancel = root.transform.Find("Panel/Panel/btnGroup/Button_Cancel").gameObject;
            btnCancel.GetComponentInChildren<Text>().text = cancelStr;
            GameObject titleObj = root.transform.Find("Panel/Panel/title/Text").gameObject;
            titleObj.GetComponentInChildren<Text>().text = titleStr;

            Text txtContent = root.transform.Find("Panel/Panel/ScrollRect/Text").GetComponent<Text>();

            txtContent.text = strContent;
            if (!isHaveCancelBtn)
            {
                btnCancel.CustomSetActive(false);
            }
            else
            {
                btnCancel.CustomSetActive(true);
            }

            CUIEventScript btnConfirmEventScript = btnConfirm.GetComponent<CUIEventScript>();
            CUIEventScript btnCancelEventScript = btnCancel.GetComponent<CUIEventScript>();

            btnConfirmEventScript.SetUIEvent(enUIEventType.Click, confirmID, par);
            btnCancelEventScript.SetUIEvent(enUIEventType.Click, cancelID, par);

            if (isContentLeftAlign)
            {
                txtContent.alignment = TextAnchor.MiddleLeft;
            }

            if (autoCloseTime != 0)
            {
                Transform timerTrans = form.transform.Find("closeTimer");
                if (timerTrans != null)
                {
                    CUITimerScript timerScript = timerTrans.GetComponent<CUITimerScript>();
                    if (timerScript != null)
                    {
                        timerScript.SetTotalTime(autoCloseTime);
                        timerScript.StartTimer();
                        timerScript.m_eventIDs[(Int32)enTimerEventType.TimeUp] = timeUpID;
                        timerScript.m_eventParams[(Int32)enTimerEventType.TimeUp] = par;
                    }
                }
            }

            //关闭菊花
            CloseSendMsgAlert();
        }

        /// <summary>
        /// 弹出MessageBox对话框
        /// </summary>
        /// <param name="strContent">提示内容</param>
        /// <param name="confirmID">确定按钮派发事件ID</param>
        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, stUIEventParams par)
        {
            OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, par);
        }

        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID)
        {
            stUIEventParams par = new stUIEventParams();

            OpenInputBoxBase(title, inputTip, confirmID, enUIEventID.None, par);
        }

        /// <summary>
        /// 带取消按钮和参数的messagebox
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>
        /// <param name="par"></param>
        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par)
        {
            OpenInputBoxBase(title, inputTip, confirmID, cancelID, par);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strContent"></param>
        /// <param name="confirmID"></param>
        /// <param name="cancelID"></param>        
        /// <param name="confirmStr"></param>
        /// <param name="cancelStr"></param>
        public void OpenInputBox(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par,
            string confirmStr, string cancelStr)
        {
            OpenInputBoxBase(title, inputTip, confirmID, cancelID, par, confirmStr, cancelStr);
        }

        /// <summary>
        /// 弹出MessageBox对话框
        /// </summary>
        /// <param name="strContent">标题</param>
        /// <param name="inputTip">输入框提示信息</param>
        /// <param name="confirmID">确定派发事件ID</param>
        /// <param name="cancelID">取消派发事件ID</param>
        /// <param name="par">事件参数</param>
        private void OpenInputBoxBase(string title, string inputTip, enUIEventID confirmID, enUIEventID cancelID, stUIEventParams par,
            string confirmStr = "确定", string cancelStr = "取消")
        {
            CUIFormScript form = CUIManager.GetInstance().OpenForm(CUIUtility.s_Form_Common_Dir + "Form_InputBox.prefab", false, false);
            GameObject root = null;
            if (  form != null )
            {
                 root = form.gameObject;
            }
            if (  root != null )
            {
                GameObject btnConfirm = root.transform.Find( "Panel/btnGroup/Button_Confirm" ).gameObject;
                btnConfirm.GetComponentInChildren<Text>().text = confirmStr;
                GameObject btnCancel = root.transform.Find( "Panel/btnGroup/Button_Cancel" ).gameObject;
                btnCancel.GetComponentInChildren<Text>().text = cancelStr;

                Text titleTxt = root.transform.Find( "Panel/title/Text" ).GetComponent<Text>();
                titleTxt.text = title;

                Text inputTipTxt = root.transform.Find( "Panel/inputText/Placeholder" ).GetComponent<Text>();
                inputTipTxt.text = inputTip;

                CUIEventScript btnConfirmEventScript = btnConfirm.GetComponent<CUIEventScript>();
                CUIEventScript btnCancelEventScript = btnCancel.GetComponent<CUIEventScript>();

                btnConfirmEventScript.SetUIEvent( enUIEventType.Click, confirmID, par );
                btnCancelEventScript.SetUIEvent( enUIEventType.Click, cancelID, par );
            }

            
        }

        /// <summary>
        /// 打开显示一段长文本信息（例如xx规则）的页面
        /// </summary>
        /// <param name="title">上方标题</param>
        /// <param name="info">下方信息</param>
        public void OpenInfoForm(string title = null, string info = null)
        {
            CUIFormScript form = OpenForm(CUIUtility.s_Form_Common_Dir + "Form_Info.prefab", false);
            DebugHelper.Assert(form != null, "CUIManager.OpenInfoForm(): form == null!!!");
            if (form == null)
            {
                return;
            }

            if (title != null)
            {
                Text txtTitle = form.GetWidget((int)enInfoFormWidgets.Title_Text).GetComponent<Text>();
                txtTitle.text = title;
            }

            if (info != null)
            {
                Text txtInfo = form.GetWidget((int)enInfoFormWidgets.Info_Text).GetComponent<Text>();
                txtInfo.text = info;
            }
        }

        /// <summary>
        /// 打开编辑页面，用于编辑一段文本
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="editContent">编辑的文本内容</param>
        /// <param name="confirmEventId">确定事件id</param>
        public void OpenEditForm(string title, string editContent, enUIEventID confirmEventId = enUIEventID.None)
        {
            CUIFormScript form = OpenForm(CUIUtility.s_Form_Common_Dir + "Form_Edit.prefab", false);
            DebugHelper.Assert(form != null, "CUIManager.OpenEditForm(): form == null!!!");
            if (form == null)
            {
                return;
            }

            if (title != null)
            {
                Text txtTitle = form.GetWidget((int) enEditFormWidgets.Title_Text).GetComponent<Text>();
                txtTitle.text = title;
            }

            if (editContent != null)
            {
                Text txtEditContent = form.GetWidget((int) enEditFormWidgets.Input_Text).GetComponent<Text>();
                txtEditContent.text = editContent;
            }

            CUIEventScript confirmEventScript = form.GetWidget((int) enEditFormWidgets.Confirm_Button).GetComponent<CUIEventScript>();
            confirmEventScript.SetUIEvent(enUIEventType.Click, confirmEventId);
        }

        /// <summary>
        /// 加载关卡的prefab，专给外围UI使用
        /// </summary>
        public void LoadUIScenePrefab(string sceneName, CUIFormScript formScript)
        {
            if (formScript == null) return;

            //避免重复添加
            if (formScript.IsRelatedSceneExist(sceneName)) return;

          //  formScript.AddRelatedScene(CUICommonSystem.GetAnimation3DOjb(sceneName), sceneName);
        }

        #region searchbox
        private const string K_SEARCH_FORM_PATH = CUIUtility.s_Form_Common_Dir + "Search/Form_Search.prefab";
        private GameObject m_searchRecommendGo = null;

        //private string m_searchEvtCallBackSingleEnable = null;
        private string m_recommendEvtCallBackSingleEnable = null;

        private Vector2 m_searchBoxOrgSizeDetla;

        public void SearchBox_OnRecommendListEnable(CUIEvent evt)
        {
            if (m_recommendEvtCallBackSingleEnable != null)
            {
                EventRouter.GetInstance().BroadCastEvent(m_recommendEvtCallBackSingleEnable, m_searchRecommendGo, evt);
            }
        }
        #endregion
    };
};