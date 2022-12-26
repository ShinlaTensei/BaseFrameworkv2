using System;
using System.Collections;
using System.Collections.Generic;
using Base.Module;
using Base.Pattern;
using UnityEngine;

namespace Base.Services
{
    [Serializable]
    public class LocalizeData
    {
        public string Key;
        public string Data;
    }

    public class LocalizeStructure : IBlueprintData
    {
        
    }
    
    public class BlueprintLocalization : BaseBlueprint<LocalizeStructure>, IService<LocalizeData>
    {
        public void UpdateData(LocalizeData data)
        {
            
        }

        public void Init()
        {
            
        }

        public override void Load()
        {
            throw new NotImplementedException();
        }

        public override void LoadDummyData()
        {
            throw new NotImplementedException();
        }
    }
}

