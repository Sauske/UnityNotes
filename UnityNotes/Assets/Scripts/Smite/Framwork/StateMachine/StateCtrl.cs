//==================================================================================
///
/// @arong
/// @2017.7.18
//==================================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Framework
{
    public class GameStateAttribute : AutoRegisterAttribute { }
    
    public class StateCtrl : Singleton<StateCtrl>
    {
        private StateMachine _stateMachine = new StateMachine();

        public void Initialize()
        {
            _stateMachine.RegisterStateByAttributes<GameStateAttribute>(typeof(GameStateAttribute).Assembly);
        }

        public override void Init()
        {
            base.Init();
            Initialize();
        }


        public override void UnInit()
        {
            base.UnInit();
            _stateMachine.Clear();
            _stateMachine = null;
        }

        public void GotoState(string name)
        {
            DebugHelper.CustomLog(string.Format("StateCtrl Goto State {0}", name));

            _stateMachine.ChangeState(name);
        }

        public IState GetCurrentState()
        {
            return _stateMachine.Top();
        }
    }
}