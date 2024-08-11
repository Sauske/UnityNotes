using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace UMI.FrameCommand
{
    public class FrameSynchr : Singleton<FrameSynchr>
    {

        public override void OnInitialize()
        {
            FrameCommandFactory.PrepareRegisterCommand();

            RigisterCommand();
        }

        /// <summary>
        /// 反射注册命令
        /// </summary>
        private void RigisterCommand()
        {
            // 注册帧命令构造器
            // 解析网络来的cmdtype后构造对应的FrameCommand
            var TestAssembly = typeof(FrameSynchr).Assembly;

            Type[] Types = TestAssembly.GetTypes();

            for (int ti = 0; Types != null && ti < Types.Length; ++ti)
            {
                var t = Types[ti];

                object[] Attributes = t.GetCustomAttributes(typeof(FrameCommandClassAttribute), true);

                for (int i = 0; i < Attributes.Length; ++i)
                {
                    // test in this type
                    FrameCommandClassAttribute Attr = Attributes[i] as FrameCommandClassAttribute;

                    if (Attr != null)
                    {
                        var creator = GetCreator(t);

                        if (creator != null)
                        {
                            FrameCommandFactory.RegisterCommandCreator(Attr.ID, t, creator);
                        }
                    }
                }
          

                object[] Attributes2 = t.GetCustomAttributes(typeof(FrameCSSYNCCommandClassAttribute), true);

                for (int i = 0; i < Attributes2.Length; ++i)
                {
                    // test in this type
                    FrameCSSYNCCommandClassAttribute Attr = Attributes2[i] as FrameCSSYNCCommandClassAttribute;

                    if (Attr != null)
                    {
                        var creator = GetCSSyncCreator(t);

                        if (creator != null)
                        {
                            FrameCommandFactory.RegisterCSSyncCommandCreator(Attr.ID, t, creator);
                        }
                    }
                }

                //object[] SCAttributes = t.GetCustomAttributes(typeof(FrameSCSYNCCommandClassAttribute), true);

                //for (int i = 0; i < SCAttributes.Length; ++i)
                //{
                //    // test in this type
                //    FrameSCSYNCCommandClassAttribute Attr = SCAttributes[i] as FrameSCSYNCCommandClassAttribute;

                //    if (Attr != null)
                //    {
                //        FrameCommandFactory.RegisterSCSyncCommandCreator(Attr.ID, t, null);
                //    }
                //}
            }
        }

        private CreatorDelegate GetCreator(Type InType)
        {
            MethodInfo[] Methods = InType.GetMethods();

            for (int m = 0; Methods != null && m < Methods.Length; ++m)
            {
                var Method = Methods[m];

                // only find static functions
                if (Method.IsStatic)
                {
                    object[] Attributes = Method.GetCustomAttributes(typeof(FrameCommandCreatorAttribute), true);

                    for (int i = 0; i < Attributes.Length; ++i)
                    {
                        FrameCommandCreatorAttribute Attr = Attributes[i] as FrameCommandCreatorAttribute;

                        if (Attr != null)
                        {
                            return (CreatorDelegate)(object)Delegate.CreateDelegate(typeof(CreatorDelegate), Method);
                        }
                    }
                }
            }

            return default(CreatorDelegate);
        }

        private CreatorCSSyncDelegate GetCSSyncCreator(Type InType)
        {
            MethodInfo[] Methods = InType.GetMethods();

            for (int m = 0; Methods != null && m < Methods.Length; ++m)
            {
                var Method = Methods[m];

                // only find static functions
                if (Method.IsStatic)
                {
                    object[] Attributes = Method.GetCustomAttributes(typeof(FrameCommandCreatorAttribute), true);

                    for (int i = 0; i < Attributes.Length; ++i)
                    {
                        FrameCommandCreatorAttribute Attr = Attributes[i] as FrameCommandCreatorAttribute;

                        if (Attr != null)
                        {
                            return (CreatorCSSyncDelegate)(object)Delegate.CreateDelegate(typeof(CreatorCSSyncDelegate), Method);
                        }
                    }
                }
            }

            return default(CreatorCSSyncDelegate);
        }
    }
}
