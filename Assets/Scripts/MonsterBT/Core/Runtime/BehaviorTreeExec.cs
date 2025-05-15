using System;

namespace MonsterBT
{
    /// <summary>
    /// 行为树执行器
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

        public void Initalize() => currentTree.Enter?.Initalize(currentTree, this, BTComp.gameObject);

        public void Update() => currentTree.Enter?.Update();

        public void Halt() => currentTree.Enter?.Halt();

        public void Dispose() => currentTree.Dispose();
    }
}