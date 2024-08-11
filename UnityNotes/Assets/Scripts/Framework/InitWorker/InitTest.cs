using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace UMI
{

    public class InitTest : MonoBehaviour
    {
        private InitUnitWorker mInitWorker;      // 初始化工作类

        // Start is called before the first frame update
        void Start()
        {
            mInitWorker = new InitUnitWorker();
            mInitWorker.AddUnit(new InitUnitConfig());
            mInitWorker.AddUnit(new InitUnitMgr());
            mInitWorker.AddUnit(new InitUnitPlugin());
            mInitWorker.DoInit(OnInitSuccess);
        }

        // Update is called once per frame
        void Update()
        {
            if (!mInitWorker.InitFinished)
            {
                return;
            }
        }
        private void LateUpdate()
        {
            if (!mInitWorker.InitFinished)
            {
                return;
            }
        }

        private void OnInitSuccess()
        {

        }
    }
}
