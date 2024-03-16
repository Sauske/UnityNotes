namespace SFramework
{

    public abstract class StateNode : IStateNode
    {
        public StateMachine Machine;

        public virtual void OnCreate(StateMachine machine)
        {
            this.Machine = machine;
        }

        public virtual void OnEnter() { }

        public virtual void OnUpdate() { }

        public virtual void OnExit() { }
    }
}
