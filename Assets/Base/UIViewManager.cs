using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Pattern;
using UnityEngine;

namespace Base
{
    public enum UICanvasType
    {
        None            = 0,
        RootCanvas      = 1,
        ViewCanvas      = 2,
        TopViewCanvas   = 3,
        OverlayCanvasUI = 4,
        RetryCanvasUI   = 5,
        UIOverlayLayout = 6,
        CampaignCanvasUI = 7,
    }
    public class UIViewManager : BaseMono, IService
    {
        private Dictionary<string, UIView> _uiViewPool = new Dictionary<string, UIView>();
    }
}

