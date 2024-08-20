using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib.Pattern
{
    public interface IPredicate 
    {
        bool Evaluate();
    }

    public class FuncPredicate : IPredicate
    {
        readonly Func<bool> func;
        public FuncPredicate(Func<bool> func)
        {
            this.func = func;
        }
        public bool Evaluate()
        {
            return func.Invoke();
        }
    }
}
