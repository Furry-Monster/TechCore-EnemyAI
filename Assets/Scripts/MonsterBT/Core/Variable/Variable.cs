using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public abstract class Variable
    {
        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        public abstract object GetValue();
        public abstract void SetValue(object value);
    }
}