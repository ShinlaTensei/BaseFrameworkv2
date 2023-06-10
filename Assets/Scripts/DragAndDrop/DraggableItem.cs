#region Header
// Date: 28/05/2023
// Created by: Huynh Phong Tran
// File name: DragDropDetector.cs
#endregion

using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public abstract class DraggableItem : BaseUI
    {
        [SerializeField, Dropdown("m_boolValue")] protected bool m_isMoveToStartPos;

        private List<bool> m_boolValue = new List<bool> { true, false };

        public abstract void SetParentAfterDrag(Transform parent);
    }
}


