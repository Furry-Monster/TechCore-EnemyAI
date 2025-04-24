using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public class Blackboard : IDisposable
    {
        private List<Variable> Shared = new();

        public void Dispose()
        {
            Shared.Clear();
        }
    }
}