using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SinkiiLib.Pattern
{
    public class StateMachine
    {
        StateNode current;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();

        public void Update()
        {
            var transtion = GetTransition();
            if (transtion != null)
            {
                ChangeState(transtion.newState);
            }
            current.State?.OnUpdate();
        }
        public void FixedUpdate()
        {
            current.State?.OnFixedUpdate();
        }
        public void SetState(IState state)
        {
            current = nodes[state.GetType()];
            current.State?.OnEnter();
        }

        void ChangeState(IState state)
        {
            if (state == current.State) return;

            var prevState = current.State;
            var nextState = nodes[state.GetType()].State;

            prevState.OnExit();
            nextState.OnEnter();
            current = nodes[state.GetType()];
        }
        public void AddTransition(IState from, IState to, IPredicate condition)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }
        public void AddAnyTransition(IState to, IPredicate condition)
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }
        ITransition GetTransition()
        {
            foreach (var transition in anyTransitions)
            {
                if(transition.condition.Evaluate()) return transition;
            }
            foreach (var transition in current.Transitions)
            {
                if(transition.condition.Evaluate()) return transition;
            }
            return null;
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());

            if (node == null)
            {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }

            return node;
        }
    }
}
