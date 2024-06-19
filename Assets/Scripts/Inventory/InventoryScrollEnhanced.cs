using System;
using System.Collections;
using System.Collections.Generic;
using Base.Core;
using Base.Helper;
using Base.Logging;
using Base.Pattern;
using EnhancedUI.EnhancedScroller;
using UniRx;
using UnityEngine;

namespace Base.System
{
    /// <summary>
    /// The signal notify when the slot was clicked, parameter: dataIndex, cellIndex
    /// </summary>
    public class InventorySlotClickSignal : Signal<int, int> {}
    
    [RequireComponent(typeof(EnhancedScroller))]
    [DisallowMultipleComponent]
    public class InventoryScrollEnhanced : BaseMono, IEnhancedScrollerDelegate
    {
        [SerializeField] private EnhancedScroller m_scroller;
        [SerializeField] private InventorySlotEnhanced m_slotPrefab;
        
        private IList<InventorySlotCellData> m_slotDataList = new List<InventorySlotCellData>();
        private IDictionary<int, IDisposable> m_compositeDisposable = new Dictionary<int, IDisposable>();

        private IntReactiveProperty m_selectedCellIndex;

        private IntReactiveProperty SelectedCellIndex
        {
            get
            {
                if (m_selectedCellIndex == null)
                {
                    m_selectedCellIndex = new IntReactiveProperty(-1);
                }

                return m_selectedCellIndex;
            }
        }
        
        /// <summary>
        /// This action will be fired when the item slot was clicked.
        /// Note: This will be released when the object is killed.
        /// Arguments:
        /// - dataIndex(int)
        /// - cellIndex(int)
        /// </summary>
        public Action<int, int> SlotClickedAction;

        private void Awake()
        {
            m_scroller.Delegate = this;
        }

        protected override void Start()
        {
            base.Start();
            m_scroller.cellViewWillRecycle += OnCellViewWillRecycle;
            SignalLocator.Get<InventorySlotClickSignal>().Subscribe(OnInventorySlotClicked);
        }

        private void OnDestroy()
        {
            SignalLocator.Get<InventorySlotClickSignal>().UnSubscribe(OnInventorySlotClicked);
            m_scroller.cellViewWillRecycle -= OnCellViewWillRecycle;
            SelectedCellIndex.Dispose();
            SlotClickedAction = null;
        }

        private void OnCellViewWillRecycle(EnhancedScrollerCellView cellView)
        {
            if (m_compositeDisposable.TryGetValue(cellView.dataIndex, out IDisposable disposable))
            {
                disposable.Dispose();
                m_compositeDisposable.Remove(cellView.dataIndex);
            }
        }

        private void OnInventorySlotClicked(int dataIndex, int cellIndex)
        {
            SlotClickedAction?.Invoke(dataIndex, cellIndex);
            SelectedCellIndex.Value = cellIndex;
        }

        public void ReloadData(float scrollPositionFactor = 0f) => m_scroller.ReloadData(scrollPositionFactor);

        public void AddData(InventorySlotCellData cellData)
        {
            m_slotDataList.Add(cellData);
        }

        public void RemoveData(int index) => m_slotDataList.RemoveAt(index);

        public int GetNumberOfCells(EnhancedScroller scroller)
        {
            return m_slotDataList.Count;
        }

        public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
        {
            return m_slotDataList[dataIndex].Size;
        }

        public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
        {
            EnhancedScrollerCellView cellView = scroller.GetCellView(m_slotPrefab);
            if (cellView is InventorySlotEnhanced slotEnhanced)
            {
                m_compositeDisposable[dataIndex] = SelectedCellIndex.Subscribe(slotEnhanced.SelectSlot);
                slotEnhanced.SetData(m_slotDataList[dataIndex]);
            }

            return cellView;
        }

        public InventorySlotCellData GetDataAtIndex(int dataIndex)
        {
            if (dataIndex >= m_slotDataList.Count)
            {
                PDebug.ErrorFormat("[InventoryScroll] The index:{0} is out of range !!!", dataIndex);
                return null;
            }

            return m_slotDataList[dataIndex];
        }

        public InventorySlotEnhanced GetCellViewAtDataIndex(int dataIndex)
        {
            EnhancedScrollerCellView cellView = m_scroller.GetCellViewAtDataIndex(dataIndex);
            
            if (cellView == null)
            {
                PDebug.ErrorFormat("[InventoryScroll] No CellView found at index:{0}", dataIndex);
                return null;
            }

            if (cellView is InventorySlotEnhanced slotEnhanced)
            {
                return slotEnhanced;
            }
            
            return null;
        }

        public InventorySlotEnhanced GetCellAtIndex(int cellIndex)
        {
            EnhancedScrollerCellView cellView = m_scroller.GetCellViewAtCellIndex(cellIndex);
            if (cellView == null)
            {
                PDebug.ErrorFormat("[InventoryScroll] No CellView found at index:{0}", cellIndex);
                return null;
            }

            if (cellView is InventorySlotEnhanced slotEnhanced)
            {
                return slotEnhanced;
            }
            
            return null;
        }
    }
}

