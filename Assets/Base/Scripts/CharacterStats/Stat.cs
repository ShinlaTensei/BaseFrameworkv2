using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    [System.Serializable]
    public class Stat
    {
        [SerializeField] private float baseValue;
        [SerializeField] private float statValue;
        [SerializeField] private bool isUpgradable;
        [SerializeField] private float increment;
        [SerializeField] private bool isDecrement;
        [SerializeField] private float decrement;

        private int _countChange = 0;

        public float StatValue => baseValue + (_countChange >= 0 ? _countChange * increment : _countChange * decrement);

        public Stat() { }

        public Stat(Stat source)
        {
            baseValue = source.baseValue;
            //statValue = source.statValue;
            isUpgradable = source.isUpgradable;
            increment = source.increment;
        }

        public void Upgrade()
        {
            if (isUpgradable)
            {
                statValue += increment;
                _countChange++;
            }
        }

        public void Downgrade()
        {
            if (isDecrement)
            {
                statValue -= decrement;
                _countChange--;
            }
        }

        public void Reset()
        {
            _countChange = 0;
        }

        public void InitCountChange(int value) => _countChange = value;
    }
}

