using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UMI
{
    public class LevelToolTest : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            TextAsset ta = Resources.Load<TextAsset>("SampleScene_config");
            LevelCfg cfg = JsonMapper.ToObject<LevelCfg>(ta.text);
            Debug.Log(cfg);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
