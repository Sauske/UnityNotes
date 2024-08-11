using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace SFramework
{

    public static class ModuleMgr
    {
        private static List<IModule> moduleList = new List<IModule>();
        private static List<IUpdate> updateList = new List<IUpdate>();
        private static List<ILogin> loginList = new List<ILogin>();

        public static void AddModule(IModule module)
        {
            moduleList.Add(module);
        }

        public static void RemoveModule(IModule module)
        {
            module.Dispose();
            moduleList.Remove(module);
        }

        public static void AddUpdate(IUpdate update)
        {
            updateList.Add(update);
        }

        public static void RemoveUpdate(IUpdate update)
        {
            updateList.Remove(update);
        }

        public static void AddLogin(ILogin login)
        {
            loginList.Add(login);
        }

        public static void RemoveLogin(ILogin login)
        {
            loginList.Remove(login);
        }

        public static void OnLogin()
        {
            for (int idx = 0; idx < loginList.Count; idx++)
            {
                loginList[idx].OnLogin();
            }
        }

        public static void OnLogout()
        {
            for (int idx = 0; idx < loginList.Count; idx++)
            {
                loginList[idx].OnLogout();
            }
        }

        public static void OnUpdate(float delta)
        {
            for(int idx = 0; idx < moduleList.Count;idx++)
            {
                moduleList[idx].OnUpdate(delta);
            }

            for (int idx = 0; idx < updateList.Count; idx++)
            {
                updateList[idx].OnUpdate(delta);
            }
        }
    }
}
