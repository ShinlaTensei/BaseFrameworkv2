using System;
using System.Collections;
using System.Collections.Generic;
using Base.Core;
using Base.CustomType;
using Base.Helper;
using Base.System;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItemData
{
    public string itemId;
    public Sprite itemImage;
}
public class InventoryUISample : BaseUI
{
    [SerializeField] private InventoryScrollEnhanced m_inventoryScrollEnhanced;
    [SerializeField] private SerializedDictionary<string, InventoryItemData> m_inventoryItems = new SerializedDictionary<string, InventoryItemData>();
    [SerializeField] private List<InventorySlotCellData> m_slotCellData = new List<InventorySlotCellData>();

    public SerializedDictionary<string, InventoryItemData> InventoryItems => m_inventoryItems;

    protected override void Awake()
    {
        base.Awake();
        RegisterContext(CoreContext.GLOBAL_CONTEXT, this);
    }

    protected override void Start()
    {
        m_inventoryScrollEnhanced.SlotClickedAction = SlotClickedAction;
        foreach (var cellData in m_slotCellData)
        {
            m_inventoryScrollEnhanced.AddData(cellData);
        }
        
        m_inventoryScrollEnhanced.ReloadData();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        UnRegisterContext(CoreContext.GLOBAL_CONTEXT, this);
    }

    private void SlotClickedAction(int dataIndex, int cellIndex)
    {
        
    }
}
