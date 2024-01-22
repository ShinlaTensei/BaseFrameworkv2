#region Header
// Date: 28/05/2023
// Created by: Huynh Phong Tran
// File name: DragDropDetector.cs
#endregion

using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Base.Helper
{
    public abstract class DraggableItem : BaseUI
    {
        [SerializeField, ValueDropdown(nameof(GetBoolValues))] protected bool m_isMoveToStartPos;

        private IEnumerable GetBoolValues() => new List<ValueDropdownItem>() { new ValueDropdownItem("True", true), new ValueDropdownItem("False", false) };

        public abstract void SetParentAfterDrag(Transform parent);
    }
}


