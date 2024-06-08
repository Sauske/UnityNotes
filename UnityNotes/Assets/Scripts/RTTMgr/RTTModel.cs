using System;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    public class RTTModel
    {
        protected string modelPrefabPath;

        //模型
        protected GameObject model;

        //动画
        protected Animation animation;

        protected Vector3 localPos = Vector3.zero;

        protected Vector3 localScale = Vector3.one;

        protected Quaternion localRatition;

        protected GameObject parentObject;

        protected int objectLayer;

        protected bool isDestory = false;

        protected Action loadComplateAction;


        //>-----------------------------------------------------------------------------
        public string modelPath
        {
            set { modelPrefabPath = value; }
            get { return modelPrefabPath; }
        }

        //>-----------------------------------------------------------------------------

        /// <summary>
        /// 加载模型
        /// </summary>
        public virtual void LoadModel(Action completeAction)
        {
            loadComplateAction = completeAction;

            if (model != null)
            {
                if (loadComplateAction != null) loadComplateAction();
            }
            else
            {
              //  AsyncLoadGameObjectTools.ins.LoadPlayerPrefabFormPool(modelPrefabPath, (o, ud) =>
                {
                    GameObject o = new GameObject();
                    if (o == null)
                    {
                        Debug.LogErrorFormat("Load {0} failed", modelPrefabPath);
                        return;
                    }

                    if (isDestory)
                    {
                       // AsyncLoadGameObjectTools.ins.FreeObjectToPool(modelPrefabPath, o);
                        return;
                    }

                    model = o;
                    animation = model.GetComponent<Animation>();

                    model.SetActive(true);
                    SetLayer(objectLayer);
                    SetParent(parentObject);
                    SetTransform(localPos, localRatition, localScale);

                    ChangeShader();

                    PlayModelAction(PlayerAnimName.Stand, WrapMode.Loop);

                    if (loadComplateAction != null) loadComplateAction();
                }//);
            }
        }

        public GameObject GetModel()
        {
            return model;
        }

        //>-----------------------------------------------------------------------------

        /// <summary>
        /// 清空模型
        /// </summary>
        public virtual void DestoryModel()
        {
            isDestory = true;
            if (model != null)
            {
               // AsyncLoadGameObjectTools.ins.FreeObjectToPool(modelPrefabPath, model);
                animation = null;
                model = null;
            }

           // DestroyAllEffect();

            loadComplateAction = null;
        }

        //>-----------------------------------------------------------------------------

        /// <summary>
        /// 设置隐藏显示
        /// </summary>
        /// <param name="visible"></param>
        public virtual void SetVisible(bool visible)
        {
            if (model != null)
            {
                model.SetActive(visible);

                if (visible && !IsPlayingAnimation())
                {
                    PlayModelAction(PlayerAnimName.Stand, WrapMode.Loop);
                }
            }
        }

        //>-----------------------------------------------------------------------------

        public virtual void SetParent(GameObject parent)
        {
            parentObject = parent;

            if (model != null && parentObject != null)
            {
                model.transform.parent = parentObject.transform;
            }
        }

        //>-----------------------------------------------------------------------------
        /// <summary>
        /// 设置位置信息
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        public virtual void SetTransform(Vector3 pos, Quaternion rotation, Vector3 scale)
        {
            localPos = pos;
            localRatition = rotation;
            localScale = scale;

            if (model == null)
            {
                return;
            }

            model.transform.localPosition = pos;
            model.transform.localScale = scale;
            model.transform.localRotation = rotation;
        }

        //>-----------------------------------------------------------------------------
        /// <summary>
        /// 设置旋转
        /// </summary>
        public virtual void SetRotation(Quaternion rotation)
        {
            if (model == null)
            {
                localRatition = rotation;
                return;
            }
            model.transform.localRotation = rotation;
        }
        //>-----------------------------------------------------------------------------
        public virtual void SetLayer(int Layer)
        {
            objectLayer = Layer;

            if (model != null)
            {
                model.SetLayerRecursively(Layer);
            }
        }

        //>-----------------------------------------------------------------------------

        public bool IsPlayingAnimation()
        {
            if (animation == null)
            {
                return false;
            }

            return animation.isPlaying;
        }

        public float GetModelAnimLength(string actionName)
        {
            if (animation != null && animation[actionName] != null)
            {
                return animation[actionName].length;
            }
            return 0;
        }

        //>-----------------------------------------------------------------------------

        /// <summary>
        /// 播放模型某一段动画
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="wrapMode"></param>
        /// <param name="speed"></param>
        public virtual float PlayModelAction(string actionName, WrapMode wrapMode, bool addToQueue = false, float speed = 1f)
        {
            if (animation != null && animation[actionName] != null)
            {
                animation[actionName].speed = speed;
                animation[actionName].wrapMode = wrapMode;

                if (addToQueue)
                {
                    animation.PlayQueued(actionName);
                }
                else
                {
                    animation.Stop();
                    animation.PlayQueued(actionName);
                }
                return animation[actionName].length;
            }
            else
            {
                Debug.LogError("Animtion is null");
            }

            return 0;
        }

        //>-----------------------------------------------------------------------------

        /// <summary>
        /// 切换shader
        /// </summary>
        public virtual void ChangeShader()
        {
            if (model == null)
            {
                Debug.LogError("Model is already removed");
                return;
            }

            Renderer[] allRenderers = model.GetComponentsInChildren<Renderer>();
            if (allRenderers == null)
            {
                return;
            }

            for (int index = 0; index < allRenderers.Length; index++)
            {
                var allRenderer = allRenderers[index];
                if (allRenderer == null || !allRenderer.enabled || allRenderer.material == null)
                {
                    continue;
                }

                bool isCharacter = allRenderer.material.shader.name == "GreatWall/BumpSpecReflect" || allRenderer.material.shader.name.Contains("/Characters/");
                if (allRenderer.material != null && isCharacter)
                {
                    //allRenderer.material.shader = ShaderMgr.ins.Find("GreatWall/BumpSpecReflect_NoAmbient");
                }
            }

        }
    }
}
