#region Header
// Date: 31/05/2023
// Created by: Huynh Phong Tran
// File name: DroppableUI.cs
#endregion

using Base.Pattern;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Base.Helper
{
    public class DroppableUI : BaseUI, IDropHandler
    {
        [SerializeField] private DraggableUI m_draggableUI;
        protected override void Start()
        {
            base.Start();
            
            PoolSystem.CreatePool(m_draggableUI);
            PoolSystem.Preload<DraggableUI>(50, 5);
        }

        public void OnDrop(PointerEventData eventData)
        {
            Debug.Log("TestDrop");
            GameObject dropObj = eventData.pointerDrag;
            if (dropObj)
            {
                dropObj.GetComponent<DraggableUI>().SetParentAfterDrag(CacheRectTransform);
            }
        }
    }
}