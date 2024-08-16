//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Framework
{
    
    public class ClassEnumerator
    {
        protected ListView<Type> _results = new ListView<Type>();

        public ListView<Type> Results { get { return _results; } }

        private Type AttributeType;
        private Type InterfaceType;

        public ClassEnumerator(Type InAttributeType,Type InInterfaceType,Assembly InAssembly,
            bool bIgnoreAbstract = true,bool bInheritAttribute = false,bool bShouldCrossAssembly = false)
        {
            AttributeType = InAttributeType;
            InterfaceType = InInterfaceType;

            try
            {
                if (bShouldCrossAssembly)
                {


                    Assembly[] Assemblys = AppDomain.CurrentDomain.GetAssemblies();
                    if (Assemblys != null)
                    {
                        for (int i = 0; i < Assemblys.Length; i++)
                        {
                            var a = Assemblys[i];
                            CheckInAssembly(a,bIgnoreAbstract,bInheritAttribute);
                        }
                    }
                }
                else
                {
                    CheckInAssembly(InAssembly, bIgnoreAbstract, bInheritAttribute);
                }
            }
            catch (Exception ex)
            {
                DebugHelper.LogError("Error is enumerate classes:" + ex.Message);
            }
        }

        protected void CheckInAssembly(Assembly InAssembly,bool bInIgnoreAbtrack,bool bInInheritAttribute)
        {
            Type[] types = InAssembly.GetTypes();

            if (types != null)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    var t = types[i];
                    if (InterfaceType == null || InterfaceType.IsAssignableFrom(t))
                    {
                        if (!bInIgnoreAbtrack || (bInIgnoreAbtrack && !t.IsAbstract))
                        {
                            if (t.GetCustomAttributes(AttributeType, bInInheritAttribute).Length > 0)
                            {
                                _results.Add(t);
                            }
                        }
                    }
                }
            }
        }
    }
}