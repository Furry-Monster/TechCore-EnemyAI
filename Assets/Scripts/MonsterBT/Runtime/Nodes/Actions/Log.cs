using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public class Log : Action
    {
        public string LogMessage;

        protected override void OnInitialize() { }

        protected override void OnRun()
        {

        }

        protected override NodeState OnTick()
        {
            throw new System.NotImplementedException();
        }
    }
}