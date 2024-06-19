using System;
using System.Collections;
using System.Collections.Generic;
using Base.CustomType;
using Base.Helper;
using Base.Pattern;
using Cysharp.Threading.Tasks;
using EnhancedUI.EnhancedScroller;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Base.System
{
    [Serializable]
    public class InventorySlotCellData
    {
        [field: SerializeField] public float Size { get; set; }
        [field: SerializeField] public string ItemId { get; set; }
    }
    public abstract class InventorySlotEnhanced : EnhancedScrollerCellView, IPointerClickHandler
    {
        private InventorySlotCellData m_cellData;

        protected InventorySlotCellData CellData => m_cellData;
        
        public void SetData(InventorySlotCellData cellData)
        {
            m_cellData = cellData;
            if (m_cellData == null) return;
            
            PopulateData(cellData);
            PopulateDataAsync(cellData).Forget();
        }

        protected virtual void PopulateData(InventorySlotCellData cellData)
        {
            
        }

        protected virtual async UniTask PopulateDataAsync(InventorySlotCellData cellData)
        {
            await UniTask.Yield();
        }

        public abstract void SelectSlot(int selectedIndex);

        public void OnPointerClick(PointerEventData eventData)
        {
            SignalLocator.Get<InventorySlotClickSignal>().Dispatch(dataIndex, cellIndex);
        }
    }
}

