using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SFramework
{
    public class StateMachine1Demo : MonoBehaviour
    {
        StateMachine sm = null;
        // Start is called before the first frame update
        void Start()
        {
            sm = new StateMachine(this);
            sm.AddNode<State1>();
            sm.AddNode<State2>();
            sm.Run<State1>();
        }

        // Update is called once per frame
        void Update()
        {
            if(sm != null)
            {
                sm.Update();
            }
        }
    }

    public class State1:StateNode
    {
        public override void OnCreate(StateMachine machine)
        {

        }
        public override void OnEnter()
        {
            Logger.Log("Sate1");
        }
        public override void OnUpdate()
        {

        }
        public override void OnExit()
        {
            Machine.ChangeState<State2>();
        }
    }

    public class State2 : StateNode
    {
        public override void OnCreate(StateMachine machine)
        {

        }
        public override void OnEnter()
        {
            Logger.Log("State2");
        }
        public override void OnUpdate()
        {

        }
        public override void OnExit()
        {
            
        }
    }
}
