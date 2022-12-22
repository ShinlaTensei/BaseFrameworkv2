using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base
{
    public class MouseInputAction : MonoBehaviour, InputAction
    {
        private Vector3 _prevMousePos;
        private bool m_lock;

        public InputPhase Phase
        {
            get
            {
                if (Lock) return InputPhase.None;
                
                if (EventSystem.current != null && !EventSystem.current.IsPointerOverGameObject())
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        return InputPhase.Began;
                    }
                    if (Input.GetMouseButton(0))
                    {
                        return InputPhase.Moved;
                    }
                    if (Input.GetMouseButtonUp(0))
                    {
                        return InputPhase.Ended;
                    }
                }
                

                return InputPhase.None;
            }
        }

        public Vector3 Position
        {
            get
            {
                _prevMousePos = Input.mousePosition;
                return Input.mousePosition;
            }
        }

        public Vector3 DeltaPosition
        {
            get
            {
                Vector3 deltaPos = Input.mousePosition - _prevMousePos;
                _prevMousePos = Input.mousePosition;
                return deltaPos;
            }
        }

        public bool Lock => m_lock;

        public void SetLock(bool isLock)
        {
            m_lock = isLock;
        }

        public Touch Touch => Input.GetTouch(0);
    }
}

