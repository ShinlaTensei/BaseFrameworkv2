#region Header
// Date: 30/05/2023
// Created by: Huynh Phong Tran
// File name: DraggableUI.cs
#endregion

using System;
using Base.Logging;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Base.Helper
{
    public class DraggableUI : DraggableItem, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [SerializeField, Range(1f, 10f)] 
        private float m_scaleFactor = 1f;

        private Vector3         m_originalScale = Vector3.one;
        private Vector3         m_posOffset     = Vector3.zero;
        private Image           m_image;
        private RectTransform   m_parentAfter;
        private RectTransform   m_parentBefore;

        private void Awake()
        {
            m_image = GetComponent<Image>();
            if (Parent)
            {
                m_parentBefore = (RectTransform)Parent;
            }
        }

        private void Snapping(RectTransform parent)
        {
            Sprite sprite = m_image.sprite;

            Vector4 padding = DataUtility.GetPadding(sprite);

            padding = new Vector4(padding.x * Scale.x, padding.y * Scale.y, padding.z * Scale.x, padding.w * Scale.y);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (Parent == null) return;
            
            Vector3 movePos = Parent.InverseTransformPoint(eventData.position);
            CacheRectTransform.anchoredPosition = movePos + m_posOffset;
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            var localScale = CacheRectTransform.localScale;
            m_originalScale = localScale;
            CacheRectTransform.SetScale(localScale.x * m_scaleFactor, localScale.y * m_scaleFactor, localScale.z * m_scaleFactor);
            m_image.raycastTarget = false;
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            CacheRectTransform.SetScale(m_originalScale.x, m_originalScale.y, m_originalScale.z);
            m_image.raycastTarget = true;
            if (m_parentAfter)
            {
                CacheRectTransform.SetParent(m_parentAfter);
                Snapping(m_parentAfter);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (Parent == null) return;
            
            if (m_parentAfter)
            {
                Parent          = m_parentBefore;
                m_parentAfter   = null;
            }
            
            Vector3 startPos = Parent.InverseTransformPoint(eventData.pressPosition);
            CacheRectTransform.SetAsLastSibling();
            if (m_isMoveToStartPos)
            {
                CacheRectTransform.anchoredPosition = startPos;
            }
            else
            {
                m_posOffset = (Vector3)CacheRectTransform.anchoredPosition - startPos;
            }
        }

        public override void SetParentAfterDrag(Transform parent)
        {
            m_parentAfter = (RectTransform)parent;
        }
    }
}