using System;

namespace MonsterBT
{
    /// <summary>
    /// ÐÐÎªÊ÷Ö´ÐÐÆ÷
    /// </summary>
    public class BehaviorTreeExec : IDisposable
    {

        private readonly BehaviorTree currentTree;
        public BehaviorTree CurrentTree => currentTree;

        private readonly BehaviorTreeComp BTComp;
        public BehaviorTreeComp BTComponent => BTComp;

        private readonly BlackboardComp BBComp;
        public BlackboardComp BBComponent => BBComp;

        public BehaviorTreeExec(BehaviorTreeComp bt, BlackboardComp bb)
        {
            currentTree ??= new();
            BTComp = bt;
            BBComp = bb;
        }

        public void Boot() => currentTree.Enter?.Initalize(currentTree, this, BTComp.gameObject);

        public void Tick()
        {
            currentTree.Enter?.Execute();
        }

        public void Halt()
        {
            throw new NotImplementedException();
        }

        public void Dispose() => currentTree.Dispose();
    }
}