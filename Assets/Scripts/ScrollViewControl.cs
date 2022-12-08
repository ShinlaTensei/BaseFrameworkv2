using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ScrollViewControl : MonoBehaviour
{
    [SerializeField] private Camera currentCam;
    [SerializeField] private RectTransform viewPortTransform;
    [SerializeField] private Transform contentParent;
    [SerializeField] private Transform prefab;

    private List<RectTransform> _listItem;
    
    private void Start()
    {
        _listItem = new List<RectTransform>();
        for (int i = 0; i < 10; ++i)
        {
            var child = Instantiate(prefab, contentParent) as RectTransform;
            _listItem.Add(child);
        }
    }

    private void Update()
    {
        var spacing = viewPortTransform.rect.height / 5f;
        for (int i = 0; i < contentParent.childCount; ++i)
        {
            Vector3 localPosition = contentParent.localPosition;
            float value = 1f - Mathf.Clamp(Mathf.Abs(localPosition.y - spacing * (float)i) / spacing / 3f, 0f, 0.5f);
            value = Mathf.Clamp(value, .5f, 1f);
            _listItem[i].localScale = new Vector3(value, value, value);
        }
    }
}
