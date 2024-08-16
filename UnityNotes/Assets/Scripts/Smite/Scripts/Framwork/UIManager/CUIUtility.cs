//==================================================================================
/// UI工具类
/// @neoyang
/// @2015.03.20
//==================================================================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text.RegularExpressions;

namespace Framework
{
    //字体大小种类
    public enum enFontSize
    {
        enTitleNomal = 22,
    };

    //Form渲染模式
    public enum enFormRenderMode
    {
        Overlay,
        Camera
    };

    public static class CUIUtility
    {
        //用于隐藏GameObject的Layer
        public const int c_hideLayer = 31;

        //UI层
        public const int c_uiLayer = 5;

        //Default层
        public const int c_defaultLayer = 0;

        //UI_BottomBg层
        public const int c_UIBottomBg = 18;

        //public const enFormRenderMode c_formRenderMode = enFormRenderMode.Camera;

        //UI Form基准尺寸
        public const int c_formBaseWidth = 960;
        public const int c_formBaseHeight = 640;


        //UI资源路径相关
        public const string s_Form_Battle_Dir = "UGUI/Form/Battle/";
        public const string s_Form_System_Dir = "UGUI/Form/System/";
        public const string s_Form_Common_Dir = "UGUI/Form/Common/";

        public const string s_Sprite_Battle_Dir = "UGUI/Sprite/Battle/";
        public const string s_Sprite_System_Dir = "UGUI/Sprite/System/";
        public const string s_Sprite_Common_Dir = "UGUI/Sprite/Common/";
        public const string s_Sprite_Dynamic_Dir = "UGUI/Sprite/Dynamic/";
        public const string s_Sprite_Dynamic_Quality_Dir = "UGUI/Sprite/Dynamic/Quality/";   

		public static string s_Form_Activity_Dir = s_Form_System_Dir + "OpActivity/";
		public static string s_Sprite_Activity_Dir = s_Sprite_System_Dir + "OpActivity/";
        public static string s_Sprite_HeroInfo_Dir = s_Sprite_Dynamic_Dir + "HeroInfo/";

        public static string s_IDIP_Form_Dir = s_Form_System_Dir + "IDIPNotice/";

        public static string s_Animation3D_Dir = "UGUI/Animation/";
        public static string s_Particle_Dir = "UGUI/Particle/";

        public static string s_heroSceneBgPath = "UIScene_HeroInfo";
        public static string s_heroSelectBgPath = "UIScene_HeroSelect";
        public static string s_recommendHeroInfoBgPath = "UIScene_Recommend_HeroInfo";
        public static string s_lotterySceneBgPath = "UIScene_Lottery";
        public static string s_battleResultBgPath = "UIScene_BattleResult";

        public static string s_Sprite_Dynamic_Icon_Dir = s_Sprite_Dynamic_Dir + "Icon/";
        //public static string s_Sprite_Dynamic_Bust_Dir = s_Sprite_Dynamic_Dir + "Bust/";
        public static string s_Sprite_Dynamic_BustHero_Dir = s_Sprite_Dynamic_Dir + "BustHero/";
        public static string s_Sprite_Dynamic_BustCircle_Dir = s_Sprite_Dynamic_Dir + "BustCircle/";
        public static string s_Sprite_Dynamic_BustCircleSmall_Dir = s_Sprite_Dynamic_Dir + "BustCircleSmall/";
        public static string s_Sprite_Dynamic_BustHeroLarge_Dir = s_Sprite_Dynamic_Dir + "BustHeroLarge/";
        public static string s_Sprite_Dynamic_ActivityPve_Dir = s_Sprite_Dynamic_Dir + "ActivityPve/";
        public static string s_Sprite_Dynamic_FucUnlock_Dir = s_Sprite_Dynamic_Dir + "FunctionUnlock/";
        public static string s_Sprite_Dynamic_Dialog_Dir = s_Sprite_Dynamic_Dir + "Dialog/";
        public static string s_Sprite_Dynamic_Dialog_Dir_Head = s_Sprite_Dynamic_Dialog_Dir + "Heads/";
        public static string s_Sprite_Dynamic_Dialog_Dir_Portrait = s_Sprite_Dynamic_Dialog_Dir + "Portraits/";
        public static string s_Sprite_Dynamic_Map_Dir = s_Sprite_Dynamic_Dir + "Map/";
        public static string s_Sprite_Dynamic_Talent_Dir = s_Sprite_Dynamic_Dir + "Skill/";     //天赋和技能合用目录，策划保证id分段不重复
        public static string s_Sprite_Dynamic_Adventure_Dir = s_Sprite_Dynamic_Dir + "Adventure/";
        public static string s_Sprite_Dynamic_Task_Dir = s_Sprite_Dynamic_Dir + "Task/";
        public static string s_Sprite_Dynamic_Skill_Dir = s_Sprite_Dynamic_Dir + "Skill/";
        public static string s_Sprite_Dynamic_PvPTitle_Dir = s_Sprite_Dynamic_Dir + "PvPTitle/";
        public static string s_Sprite_Dynamic_GuildHead_Dir = s_Sprite_Dynamic_Dir + "GuildHead/";
        public static string s_Sprite_Dynamic_Guild_Dir = s_Sprite_Dynamic_Dir + "Guild/";
        public static string s_Sprite_Dynamic_Profession_Dir = s_Sprite_Dynamic_Dir + "Profession/";
        public static string s_Sprite_Dynamic_Pvp_Settle_Dir = s_Sprite_System_Dir + "PvpIcon/";
        public static string s_Sprite_Dynamic_Pvp_Settle_Large_Dir = s_Sprite_System_Dir + "PvpIconLarge/";
        public static string s_Sprite_Dynamic_Achieve_Dir = s_Sprite_Dynamic_Dir + "Achieve/";
        public static string s_Sprite_Dynamic_Purchase_Dir = s_Sprite_Dynamic_Dir + "Purchase/";
        public static string s_Sprite_Dynamic_BustPlayer_Dir = s_Sprite_Dynamic_Dir + "BustPlayer/";
        public static string s_Sprite_Dynamic_AddedSkill_Dir = s_Sprite_Dynamic_Dir + "AddedSkill/";
        public static string s_Sprite_Dynamic_Proficiency_Dir = s_Sprite_Dynamic_Dir + "HeroProficiency/";
        public static string s_Sprite_Dynamic_PvpEntry_Dir = s_Sprite_Dynamic_Dir + "PvpEntry/";
        public static string s_Sprite_Dynamic_SkinQuality_Dir = s_Sprite_Dynamic_Dir + "SkinQuality/";
        public static string s_Sprite_Dynamic_ExperienceCard_Dir = s_Sprite_Dynamic_Dir + "ExperienceCard/";
        public static string s_Sprite_Dynamic_PvpAchievementShare_Dir = s_Sprite_Dynamic_Dir + "PvpShare/";
        public static string s_Sprite_Dynamic_UnionBattleBaner_Dir = s_Sprite_Dynamic_Dir + "UnionBattleBaner/";
        public static string s_Sprite_Dynamic_Nobe_Dir = s_Sprite_Dynamic_Dir + "Nobe/";
        public static string s_Sprite_Dynamic_Newbie_Dir = s_Sprite_Dynamic_Dir + "Newbie/";
        public static string s_Sprite_Dynamic_SkinFeature_Dir = s_Sprite_Dynamic_Dir + "SkinFeature/";
        public static string s_Sprite_Dynamic_Signal_Dir = s_Sprite_Dynamic_Dir + "Signal/";
        public static string s_Sprite_Dynamic_Mall_Dir = s_Sprite_Dynamic_Dir + "Mall/";

        public static string s_Sprite_System_Equip_Dir = s_Sprite_System_Dir + "Equip/";
        public static string s_Sprite_System_BattleEquip_Dir = s_Sprite_System_Dir + "BattleEquip/";
        public static string s_Sprite_System_Honor_Dir = s_Sprite_System_Dir + "Honor/";      //局外玩家资料里的荣誉称号
        public static string s_Sprite_System_HeroSelect_Dir = s_Sprite_System_Dir + "HeroSelect/";
        public static string s_Sprite_System_Qualifying_Dir = s_Sprite_System_Dir + "Qualifying/";
        public static string s_Sprite_System_Burn_Dir = s_Sprite_System_Dir + "BurnExpedition/";
        public static string s_Sprite_System_Mall_Dir = s_Sprite_System_Dir + "Mall/";
        public static string s_Sprite_System_Ladder_Dir = s_Sprite_System_Dir + "Ladder/";
        public static string s_Sprite_System_ShareUI_Dir = s_Sprite_System_Dir + "ShareUI/";
        public static string s_Sprite_System_Lobby_Dir = s_Sprite_System_Dir + "LobbyDynamic/";
        public static string s_Sprite_System_Wifi_Dir = s_Sprite_System_Dir + "Wifi/";

        public static string s_battleSignalPrefabDir = s_Sprite_Battle_Dir + "Signal/";

        public static Color s_Color_White = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
        public static Color s_Color_White_HalfAlpha = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f,125.0f / 255.0f);
        public static Color s_Color_White_FullAlpha = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f, 0);
        public static Color s_Color_Grey = new Color(80.0f / 255.0f, 80.0f / 255.0f, 80.0f / 255.0f);
        public static Color s_Color_GrayShader = new Color(0, 1, 1);            //shader用黑白颜色
        public static Color s_Color_Full = new Color(1, 1, 1, 1);
        public static Color s_Color_DisableGray = new Color((float) (100.0/255.0), (float) (100.0/255.0), (float) (100.0/255.0), (float) (255.0/255.0));

        public static Color s_Text_Color_White = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);
        public static Color s_Text_Color_Disable = new Color(154.0f / 255.0f, 153.0f / 255.0f, 153.0f / 255.0f);
        public static Color s_Text_Color_Vip_Chat_Self = new Color(255.0f / 255.0f, 228.0f / 255.0f, 0.0f / 255.0f);        // 自己看到自己的vip颜色
        public static Color s_Text_Color_Vip_Chat_Other = new Color(198.0f / 255.0f, 166.0f / 255.0f, 80.0f / 255.0f);      // 其他人看到的自己的 vip颜色

        // 亲密度颜色值
        static public Color Intimacy_Full = new Color(255.0f / 255.0f, 24.0f / 255.0f, 0.0f / 255.0f);
        static public Color Intimacy_High = new Color(255.0f / 255.0f, 24.0f / 255.0f, 255.0f / 255.0f);
        static public Color Intimacy_Mid = new Color(255.0f / 255.0f, 133.0f / 255.0f, 34.0f / 255.0f);
        static public Color Intimacy_Low = new Color(255.0f / 255.0f, 223.0f / 255.0f, 46.0f / 255.0f);
        static public Color Intimacy_Freeze = new Color(206.0f / 255.0f, 207.0f / 255.0f, 225.0f / 255.0f);

        // 显示自己名字的金黄色
        public static Color s_Text_Color_Self = new Color(242.0f / 255.0f, 201.0f / 255.0f, 77.0f / 255.0f);
        public static Color s_Text_Color_Camp_1 = new Color(103.0f / 255.0f, 154.0f / 255.0f , 247.0f / 255.0f);
        public static Color s_Text_Color_Camp_2 = new Color(219.0f / 255.0f, 46.0f / 255.0f, 72.0f / 255.0f);
        public static Color s_Text_Color_CommonGray = new Color(179.0f / 255.0f, 180.0f / 255.0f, 182.0f / 255.0f);
        public static Color s_Text_Color_MyHeroName = new Color(224.0f / 255.0f, 186.0f / 255.0f, 34.0f / 255.0f);
        public static Color s_Text_OutLineColor_MyHeroName = new Color(30.0f / 255.0f, 22.0f / 255.0f, 6.0f / 255.0f);
        //文本颜色值相关
        public static Color s_Text_Color_ListElement_Normal = new Color(126.0f / 255.0f, 136.0f / 255.0f, 162.0f / 255.0f);
        public static Color s_Text_Color_ListElement_Select = new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f);

        public static Color s_Text_Color_Hero_Name_Active = new Color(236.0f / 255.0f, 217.0f / 255.0f, 164.0f / 255.0f);
        public static Color s_Text_Color_Hero_Name_DeActive = new Color(102.0f / 255.0f, 93.0f / 255.0f, 69.0f / 255.0f);

        public static Color s_Text_Color_Camp_Allies = new Color(90.0f / 255.0f, 140.0f / 255.0f, 213.0f / 255.0f);
        public static Color s_Text_Color_Camp_Enemy = new Color(175.0f / 255.0f, 41.0f / 255.0f, 60.0f / 255.0f);

        //button颜色相关
        public static Color s_Color_Button_Disable = new Color(98.0f / 255.0f, 98.0f / 255.0f, 98.0f / 255.0f, 230.0f / 255.0f);

        //setting toggle未选中时显色
        //public static Color s_Color_Toggle_Text_Unselected = new Color(107.0f / 255.0f, 117.0f / 255.0f, 140.0f / 255.0f);

        // 血量低时头像颜色
        public static Color s_Color_EnemyHero_Button_PINK = new Color(255.0f / 255.0f, 144.0f / 255.0f, 144.0f / 255.0f);


        //进阶颜色（颜色代表星级）相关
        public static Color[] s_Text_Color_Hero_Advance = new Color[5]
        {
            new Color(255.0f / 255.0f, 255.0f / 255.0f, 255.0f / 255.0f),
            new Color(99.0f / 255.0f, 230.0f / 255.0f, 61.0f / 255.0f),
            new Color(30.0f / 255.0f, 164.0f / 255.0f, 233.0f / 255.0f),
            new Color(195.0f / 255.0f, 86.0f / 255.0f, 210.0f / 255.0f),
            new Color(242.0f / 255.0f, 119.0f / 255.0f, 18.0f / 255.0f),  
        };

        //能够购买和不能够购买
        public static Color s_Text_Color_Can_Afford = Color.white;
        public static Color s_Text_Color_Can_Not_Afford = new Color(173.0f / 255.0f, 58.0f / 255.0f, 52.0f / 255.0f);

        //正则表达式，用于剔除emoji表情
        private static readonly Regex s_regexEmoji = new Regex(@"\ud83c[\udf00-\udfff]|\ud83d[\udc00-\udeff]|\ud83d[\ude80-\udeff]");

        //UI默认材质Shader名称
        public static string s_ui_defaultShaderName = "Sprites/Default";

        //UI_Gray材质Sprite保存路径
        public static string s_ui_graySpritePath = s_Sprite_Dynamic_BustHero_Dir + "gray";

        //材质拷贝的时候需要额外拷贝的参数变量名
        public static string[] s_materianlParsKey = {"_StencilComp","_Stencil","_StencilOp","_StencilWriteMask","_StencilReadMask","_ColorMask"};

        // 死亡回放
        public static Color[] s_Text_Skill_HurtType_Color = new Color[4]
        {
            new Color(0xd0 / 255.0f, 0x6c/ 255.0f, 0x6c / 255.0f), //物理
            new Color(0x96 / 255.0f, 0x98 / 255.0f, 0xf5 / 255.0f), // 魔法
            new Color(0xef / 255.0f, 0xc1 / 255.0f, 0x36 / 255.0f), // 真实
            new Color(0x96 / 255.0f, 0xf5 / 255.0f, 0xd5 / 255.0f), // 混合伤害
        };

        public static Color[] s_Text_SkillName_And_HurtValue_Color = new Color[4]
        {
            new Color(0xd4 / 255.0f, 0x9f/ 255.0f, 0x9f / 255.0f), //物理
            new Color(0x9a / 255.0f, 0x9b / 255.0f, 0xc9 / 255.0f), // 魔法
            new Color(0xcb / 255.0f, 0xbc / 255.0f, 0x91 / 255.0f), // 真实
            new Color(0x8b / 255.0f, 0xb2 / 255.0f, 0xab / 255.0f), // 混合伤害
        };

        public static Color[] s_Text_Total_Damage_Text_Color = new Color[2]
        {
            new Color(0xee/255.0f, 0xe2/255.0f, 0xa9/255.0f), // 最高
            new Color(0x81/255.0f, 0x90/255.0f, 0xe2/255.0f), // 一般
        };

        public static Color[] s_Text_Total_Damage_Value_Color = new Color[2]
        {
            new Color(0xee/255.0f, 0xe0/255.0f, 0xc9/255.0f), // 最高
            new Color(0xc9/255.0f, 0xdb/255.0f, 0xee/255.0f), // 一般
        };

        public static Color[] s_Text_Total_Damage_Text_Outline_Color = new Color[2]
        {
            new Color(0x3a/255.0f, 0x1c/255.0f, 0x12/255.0f), // 最高
            new Color(0x12/255.0f, 0x1c/255.0f, 0x3a/255.0f), // 一般
        };


        //--------------------------------------------------
        /// 返回固定宽度文本适配后的尺寸
        /// @text           : 文本组件
        /// @content        : 文本内容
        /// @fixedWidth     : 文本内容固定宽度
        //--------------------------------------------------
        public static Vector2 GetFixedTextSize(UnityEngine.UI.Text text, string content, float fixedWidth)
        {
            return Vector2.zero;
        }

        //--------------------------------------
        /// 遍历获取UI组件
        /// @go在非active状态下依然有效
        //--------------------------------------
        public static T GetComponentInChildren<T>(GameObject go) where T : Component
        {
            if( go == null )
            {
                return null;
            }

            T t = go.GetComponent<T>();

            if (t != null)
            {
                return t;
            }

            var trans = go.transform;
            var Count = trans.childCount;

            for (int i = 0; i < Count; i++)
            {
                t = GetComponentInChildren<T>(trans.GetChild(i).gameObject);

                if (t != null)
                {
                    return t;
                }
            }

            return null;
        }

        //--------------------------------------
        /// 遍历获取UI组件数组
        /// @go在非active状态下依然有效
        //--------------------------------------
        public static void GetComponentsInChildren<T>(GameObject go, T[] components, ref int count) where T : Component
        {
            T t = go.GetComponent<T>();

            if (t != null)
            {
                components[count] = t;
                count++;
            }

            for (int i = 0; i < go.transform.childCount; i++)
            {
                GetComponentsInChildren<T>(go.transform.GetChild(i).gameObject, components, ref count);
            }
        }

        //--------------------------------------
        /// 字符串替换
        //--------------------------------------
        public static string StringReplace(string scrStr, params string[] values)
        {
            string res = string.Format(scrStr,values);

            /*
            for (int i = 0; i < values.Length; i++)
            {
                res = res.Replace(i.ToString() + "%", values[i]);
            }
            */
            return res;
        }

        //--------------------------------------
        /// 屏幕坐标转换为世界坐标
        /// @camera
        /// @screenPoint
        /// @z
        //--------------------------------------
        public static Vector3 ScreenToWorldPoint(Camera camera, Vector2 screenPoint, float z)
        {
            return ((camera == null) ? new Vector3(screenPoint.x, screenPoint.y, z) : camera.ViewportToWorldPoint(new Vector3(screenPoint.x / Screen.width, screenPoint.y / Screen.height, z)));
        }

        //--------------------------------------
        /// 世界坐标转换为屏幕坐标
        /// @camera
        /// @worldPoint
        //--------------------------------------
        public static Vector2 WorldToScreenPoint(Camera camera, Vector3 worldPoint)
        {
            return ((camera == null) ? new Vector2(worldPoint.x, worldPoint.y) : (Vector2)camera.WorldToScreenPoint(worldPoint));
        }

        //--------------------------------------
        /// 设置GameObject Layer
        /// @gameObject
        /// @layer
        //--------------------------------------
        public static void SetGameObjectLayer(GameObject gameObject, int layer)
        {
            gameObject.layer = layer;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                SetGameObjectLayer(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }

        //--------------------------------------
        /// 范围内取值
        /// @value
        /// @min
        /// @max
        //--------------------------------------
        public static float ValueInRange(float value, float min, float max)
        {
            if (value < min)
            {
                return min;
            }
            else if (value > max)
            {
                return max;
            }
            else
            {
                return value;
            }
        }

        //重置节点大小，让其自动适配Form节点scale的影响
        public static void ResetUIScale(GameObject target)
        {
            Vector3 targetScale = target.transform.localScale;
            Transform father = target.transform.parent;

            target.transform.SetParent(null);
            target.transform.localScale = targetScale;
            target.transform.SetParent(father);
        }

        //--------------------------------------
        /// 从字符串中移除emoji
        /// @str
        //--------------------------------------
        public static string RemoveEmoji(string str)
        {
            return s_regexEmoji.Replace(str, "");
        }

        //--------------------------------------
        /// 统一获取sprite prefeb资源接口
        /// @如果找不到资源也会用一个替图资源，防止空指针
        //--------------------------------------
        public static GameObject GetSpritePrefeb(string prefebPath, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false)
        {
            GameObject prefab = null;// CResourceManager.GetInstance().GetResource(prefebPath, typeof(GameObject), enResourceType.UISprite, needCached, unloadBelongedAssetBundleAfterLoaded).m_content as GameObject;

            if (prefab == null)
            {
#if !SGAME_PROFILE && UNITY_EDITOR
                DebugHelper.LogError("图片资源缺失：error sprite path:" + prefebPath);
#endif
                prefab = null;// CResourceManager.GetInstance().GetResource(CUIUtility.s_Sprite_Dynamic_Icon_Dir + "0", typeof(GameObject), enResourceType.UISprite, true, true).m_content as GameObject;
            }

            return prefab;
        }

        //获取材质裁参数
        public static float[] GetMaterailMaskPars(Material tarMat)
        {
            float[] res = new float[s_materianlParsKey.Length];

            for (int i = 0; i < s_materianlParsKey.Length; i++)
            {
                res[i] = tarMat.GetFloat(s_materianlParsKey[i]);
            }

            return res;
        }

        //设置材质裁剪参数
        public static void SetMaterailMaskPars(float[] pars, Material tarMat)
        {
            for (int i = 0; i < s_materianlParsKey.Length; i++)
            {
                tarMat.SetFloat(s_materianlParsKey[i],pars[i]);
            }
        }

        //--------------------------------------
        /// 设置Image Sprite
        /// @image
        /// @prefab
        //--------------------------------------
        public static void SetImageSprite(Image image, GameObject prefab, bool isShowSpecMatrial = false)
        {
            if (image == null)
            {
                return;
            }

            if (prefab == null)
            {
                image.sprite = null;
                return;
            }

            SpriteRenderer spriteRender = prefab.GetComponent<SpriteRenderer>();

            if (spriteRender != null)
            {
                image.sprite = spriteRender.sprite;

                isShowSpecMatrial = false; //临时关闭功能

                //如果需要重置材质，并且目标材质不是默认的sprite材质
                if (isShowSpecMatrial
                    && spriteRender.sharedMaterial != null
                    && spriteRender.sharedMaterial.shader != null
                    && !spriteRender.sharedMaterial.shader.name.Equals(s_ui_defaultShaderName))
                {
                    //缓存裁剪信息
                    float[] pars = GetMaterailMaskPars(image.material);

                    image.material = spriteRender.sharedMaterial;
                    image.material.shaderKeywords = spriteRender.sharedMaterial.shaderKeywords;  
       
                    //写回裁剪信息
                    SetMaterailMaskPars(pars, image.material);
                }
                //如果要用特殊材质，但是又没有制作特殊材质，必须要设置为空，不然之前挂上去的材质不会清理干净
                //比如英雄列表有个英雄用了动态材质，然后进入英雄查看再改成不是动态材质皮肤，再返回，就会看到没有材质的半身像还挂着之前的动态材质
                else if (isShowSpecMatrial)
                {
                    image.material = null;
                }
            }

            if (image is Image2)
            {
                var settings = prefab.GetComponent<SGameSpriteSettings>();
                var image2 = image as Image2;

                image2.alphaTexLayout = (settings != null) ? settings.layout : ImageAlphaTexLayout.None;
            }
        }

        //--------------------------------------
        /// 设置Image Sprite
        /// @image
        /// @prefabPath
        /// @formScript
        /// @loadSync : 是否同步加载
        //--------------------------------------
        public static void SetImageSprite(Image image, string prefabPath, CUIFormScript formScript, bool loadSync = true, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false, bool isShowSpecMatrial = false)
        {
            if (image == null)
            {
                return;
            }

            if (loadSync)
            {
                SetImageSprite(image, GetSpritePrefeb(prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded),isShowSpecMatrial);
            }
            else
            {
                //加载前设置替图alpha为0
                image.color = new Color(image.color.r, image.color.g, image.color.b,0);

                //放到加载队列s
                formScript.AddASyncLoadedImage(image, prefabPath, needCached, unloadBelongedAssetBundleAfterLoaded,isShowSpecMatrial);
            }
        }

        //--------------------------------------
        /// 设置Image Sprite
        /// @image
        /// @targetImage
        //--------------------------------------
        public static void SetImageSprite(Image image, Image targetImage)
        {
            if (image == null)
            {
                return;
            }

            if (targetImage == null)
            {
                image.sprite = null;
                return;
            }

            image.sprite = targetImage.sprite;

            if (image is Image2)
            {
                var image2 = image as Image2;
                image2.alphaTexLayout = ImageAlphaTexLayout.None;

                if (targetImage is Image2)
                {
                    var targetimage2 = targetImage as Image2;
                    image2.alphaTexLayout = targetimage2.alphaTexLayout;
                }
            }
        }

        public static void SetImageGrey(Graphic graphic, bool isSetGrey)
        {
            SetImageGrey(graphic, isSetGrey, Color.white);
        }

        private static void SetImageGrey(Graphic graphic, bool isSetGrey, Color defaultColor)
        {
            if (graphic != null)
            {
                graphic.color = isSetGrey ? s_Color_Grey : defaultColor;
            }
        }


        //为image挂上gray材质
        public static void SetImageGrayMatrial(Image image)
        {
            //临时关闭功能
            /*
            GameObject pb = GetSpritePrefeb(s_ui_graySpritePath);
            SpriteRenderer spriteRender = pb.GetComponent<SpriteRenderer>();
            if (spriteRender != null)
            {
                if (image.material == null || !image.material.shader.name.Equals(spriteRender.sharedMaterial.shader.name))
                {
                    //缓存裁剪信息
                    float[] pars = GetMaterailMaskPars(image.material);

                    image.material = spriteRender.sharedMaterial;
                    image.material.shaderKeywords = spriteRender.sharedMaterial.shaderKeywords;

                    //写回裁剪信息
                    SetMaterailMaskPars(pars, image.material);
                }
            }
            */
        }

        //--------------------------------------------------
        /// 查找FormScript
        /// @transform
        //--------------------------------------------------
        public static CUIFormScript GetFormScript(Transform transform)
        {
            if (transform == null)
            {
                return null;
            }

            CUIFormScript formScript = transform.gameObject.GetComponent<CUIFormScript>();
            if (formScript != null)
            {
                return formScript;
            }

            return GetFormScript(transform.parent);
        }

    //--------------------------------------
    /// Image扩展函数
    //--------------------------------------
        public static void SetSprite(this Image image, GameObject prefab, bool isShowSpecMatrial = false)
        {
            CUIUtility.SetImageSprite(image, prefab,isShowSpecMatrial);
        }

        public static void SetSprite(this Image image, string prefabPath, CUIFormScript formScript, bool loadSync = true, bool needCached = false, bool unloadBelongedAssetBundleAfterLoaded = false, bool isShowSpecMatrial = false)
        {
            CUIUtility.SetImageSprite(image, prefabPath, formScript, loadSync, needCached, unloadBelongedAssetBundleAfterLoaded,isShowSpecMatrial);
        }

        public static void SetSprite(this Image image, Image targetImage)
        {
            CUIUtility.SetImageSprite(image, targetImage);
        }

        public static void SetSprite(this Image image, Sprite sprite, ImageAlphaTexLayout imageAlphaTexLayout)
        {
            if (image is Image2)
            {
                (image as Image2).alphaTexLayout = imageAlphaTexLayout;
            }

            image.sprite = sprite;
        }

        public static void CustomFillAmount(this Image image, float value)
        {
            if (image != null && image.fillAmount != value)
            {
                image.fillAmount = value;
            }
        }

        public static void CustomSetActive(this GameObject go, bool IsActive)
        {
            if (go == null)
            {
                Debug.Log("CustomSetActive GameObject is Null");
                return;
            }

            go.SetActive(IsActive);
        }

        public static void SetText(this Text text, string content)
        {
            if (text == null)
            {
                Debug.Log("SetText Text is Null:" + content);
                return;
            }
            text.text = content;
        }
    }
}
