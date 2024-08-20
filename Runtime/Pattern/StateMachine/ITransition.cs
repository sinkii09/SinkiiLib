using SinkiiLib.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib.Pattern
{
    public interface ITransition
    {
        IState newState { get; }

        IPredicate condition { get; }
    }

    public class Transition : ITransition
    {
        public IState newState { get; }

        public IPredicate condition { get; }

        public Transition(IState state,IPredicate condition)
        {
            newState = state;
            this.condition = condition;
        }
    }
}
