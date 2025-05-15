using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterBT
{
    public abstract class SharedVariable : ICloneable
    {
        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }

    public abstract class ExposedVariable : ICloneable
    {
        private string name;
        public string Name
        {
            get => name;
            set => name = value;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}