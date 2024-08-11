using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UMI
{
    /// <summary>
    /// 配置表的初始化部分
    /// </summary>
    public class InitUnitConfig : InitUnit
    {
        protected override async Task OnInit()
        {
            await Task.WhenAll(LoadExcelJsonConfig());//, ServerConfig.Instance.Init());

            //ServerConfigData serverData = ServerConfig.Instance.GetConfig();
            // HttpFileMgr.Instance.SetHttpHost(serverData.authUrl, serverData.downFileHost);
            // ShareNetCtrl.SetHttpHost(serverData.authUrl);
        }

        /// <summary>
        /// 加载json格式的配置
        /// </summary>
        private async Task LoadExcelJsonConfig()
        {
            const string CONFIG_ADDRESSABLE_PATH = "Config/excel.txt";

            System.Diagnostics.Stopwatch st = new System.Diagnostics.Stopwatch();
            st.Start();
            TextAsset tx = await Addressables.LoadAssetAsync<TextAsset>(CONFIG_ADDRESSABLE_PATH).Task;
            Debug.LogFormat("LoadExcelJson assert cost {0}ms", st.ElapsedMilliseconds);
            //CsJsonDataLoader.InitJsonData(tx.text);

            //GameParamConfig.Init();
            Addressables.Release(tx);
            //UINotifyCtrl.SetDefaultBtnStr();            

            st.Stop();
            Debug.LogFormat("LoadExcelJsonConfig total cost {0}ms", st.ElapsedMilliseconds);
        }

    }
}