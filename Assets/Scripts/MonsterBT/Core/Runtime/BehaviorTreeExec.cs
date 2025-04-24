using System;

namespace MonsterBT
{
    /// <summary>
    /// ÐÐÎªÊ÷Ö´ÐÐÆ÷
    /// </summary>
    public class BehaviorTreeExec : IDisposable
    {

        private BehaviorTree currentTree;
        public BehaviorTree CurrentTree => currentTree;

        private BehaviorTreeComp component;
        public BehaviorTreeComp Component => component;

        public BehaviorTreeExec(BehaviorTreeComp comp)
        {
            currentTree ??= new();
            component = comp;
        }

        public void Boot() => currentTree.Enter?.Initalize(currentTree, this, component.gameObject);

        public void Tick()
        {
            currentTree.Enter?.Execute();
        }

        public void Halt()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}