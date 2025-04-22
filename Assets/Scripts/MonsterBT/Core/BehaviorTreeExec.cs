using System;

namespace MonsterBT
{
    /// <summary>
    /// ÐÐÎªÊ÷Ö´ÐÐÆ÷
    /// </summary>
    public class BehaviorTreeExec : IDisposable
    {

        private BehaviorTree currentTree;

        public BehaviorTree CurrentTree
        {
            get => currentTree;
            set => currentTree = value;
        }

        public void Boot() => currentTree.Enter.Initalize(currentTree, this);

        public void Tick()
        {
            currentTree.Enter.Execute();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}