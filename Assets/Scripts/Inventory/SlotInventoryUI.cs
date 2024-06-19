using System.Collections;
using System.Collections.Generic;
using Base.Core;
using Base.Logging;
using Base.System;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class SlotInventoryUI : InventorySlotEnhanced
{
    [SerializeField] private Image m_itemImage;
    protected override void PopulateData(InventorySlotCellData cellData)
    {
        InventoryUISample uiSample = BaseContextRegistry.TryGetOrCreateContext(CoreContext.GLOBAL_CONTEXT).Get<InventoryUISample>();
        if (uiSample.InventoryItems.TryGetValue(cellData.ItemId, out InventoryItemData itemData))
        {
            m_itemImage.sprite = itemData.itemImage;
        }
    }

    public override void SelectSlot(int selectedIndex)
    {
        PDebug.InfoFormat("[SlotInventoryUI] Selected: {0}", selectedIndex == cellIndex);
    }
}
