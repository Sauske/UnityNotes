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
    [GameStateAttribute]
    public class StateDemo : BaseState
    {
        public override void OnStateEnter()
        {
            base.OnStateEnter();
        }

        public override void OnStateLeave()
        {
            base.OnStateLeave();
        }

        public override void OnStateOverride()
        {
            base.OnStateOverride();
        }

        public override void OnStateResume()
        {
            base.OnStateResume();
        }
    }
}