#region Header
// Date: 17/08/2023
// Created by: Huynh Phong Tran
// File name: StateController.cs
#endregion

using System;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using UnityEngine;

namespace Base.Pattern
{
    public class StateController : BaseMono
    {
        [SerializeField] private string m_runningState = string.Empty;
        
        private Lazy<IDictionary<string, State>> m_states = new(new Dictionary<string, State>());
        private Queue<State>                     m_transitionQueue;

        private State m_currentState  = null;
        private State m_previousState = null;

        public event Action<State, State> OnStateChangedEvent; 

        public State CurrentState => m_currentState;

        public State PreviousState => m_previousState;

        #region Life Cycle

        protected virtual void Update()
        {
            if (CurrentState is null) return;

            bool isTransition = CheckTransition();
            
            m_transitionQueue.Clear();

            if (isTransition)
            {
                PreviousState?.OnExit();
                CurrentState.OnEnter();

                m_runningState = CurrentState.GetType().Name;
                
                OnStateChangedEvent?.Invoke(PreviousState, CurrentState);
            }
            
            float dt = Time.deltaTime;
            CurrentState.PostUpdateBehaviour(dt);
            CurrentState.UpdateBehaviour(dt);
            CurrentState.PreUpdateBehaviour(dt);
        }

        #endregion

        private bool CheckTransition()
        {
            if (!CurrentState.IsReadyToExit()) return false;

            if (m_transitionQueue.Count < 1)
            {
                return false;
            }

            State newState     = m_transitionQueue.Dequeue();
            bool  readyToEnter = newState.IsReadyToEnter();

            if (readyToEnter)
            {
                m_previousState = CurrentState;
                m_currentState  = newState;
            }
            return true;
        }

        public State GetState<T>() where T : State
        {
            if (!m_states.Value.TryGetValue(typeof(T).Name, out var result))
            {
                result                         = Activator.CreateInstance<T>();
                m_states.Value[typeof(T).Name] = result;
            }

            return result as T;
        }

        public void AddTransition<T>() where T : State
        {
            State state = GetState<T>();
            m_transitionQueue.Enqueue(state);
        }
    }
}