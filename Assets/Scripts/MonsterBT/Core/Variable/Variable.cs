using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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

    public class ReflectionVariable : Variable
    {
        private FieldInfo fieldInfo;
        private object target;

        public ReflectionVariable(FieldInfo fieldInfo, object target = null)
        {
            this.fieldInfo = fieldInfo;
            this.target = target;
            Name = fieldInfo.Name;
        }

        public override object GetValue()
        {
            return fieldInfo.GetValue(target);
        }

        public override void SetValue(object value)
        {
            fieldInfo.SetValue(target, value);
        }
    }
}