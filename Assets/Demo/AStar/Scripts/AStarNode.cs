// ReSharper disable CheckNamespace

using System;

namespace AStar
{
    public enum AStarNodeType
    {
        None,
        Source,
        Target,
        Obstacle,
    }

    public class AStarNode
    {
        public readonly int X;
        public readonly int Y;

        public float G = 0.0f;
        public float H = 0.0f;
        public float F => G + H;

        public AStarNode Parent;
        public AStarNodeType Type = AStarNodeType.None;

        public AStarNode(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}