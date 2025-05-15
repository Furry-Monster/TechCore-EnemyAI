using System.Collections.Generic;

namespace MonsterBT
{
    public abstract class Composite : BehaviorTreeNode
    {
        private List<BehaviorTreeNode> children;

        protected override void OnInitialize()
        {
            children?.ForEach(e => e.Initialize());
        }

        protected override void OnBoot()
        {
            children?.ForEach(e => e.Boot());
        }

        public override void Dispose()
        {
            children?.ForEach(e => e.Dispose());

            base.Dispose();
        }

        #region children ops
        public void AddChild(BehaviorTreeNode nodeToAdd)
        {
            children.Add(nodeToAdd);
        }

        public void SetChild(int index, BehaviorTreeNode nodeToSet)
        {
            children[index] = nodeToSet;
        }

        public void RemoveChild(int index)
        {
            children.RemoveAt(index);
        }

        public BehaviorTreeNode GetChild(int index)
        {
            return children[index];
        }

        public List<BehaviorTreeNode> GetChildren()
        {
            return children;
        }

        public int GetChildCount()
        {
            return children.Count;
        }
        #endregion
    }
}