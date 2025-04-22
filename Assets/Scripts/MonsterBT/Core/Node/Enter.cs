using System;

namespace MonsterBT
{
    public class Enter : BehaviorTreeNode
    {
        private BehaviorTreeNode child;

        private BehaviorTreeNode Child
        {
            get => child;
            set => child = value;
        }

        protected override void OnInitialize()
        {
            child.Initalize(Tree, Exec);
        }

        protected override NodeState DoExecute()
        {
            var state = child.Execute();

            return state;
        }

        public override void Dispose()
        {
            base.Dispose();

            child.Dispose();
        }
    }
}