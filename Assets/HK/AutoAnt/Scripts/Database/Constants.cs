﻿using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.Database
{
    /// <summary>
    /// Inspectorで編集出来る定数
    /// </summary>
    [CreateAssetMenu(menuName = "AutoAnt/Database/Constants")]
    public sealed class Constants : ScriptableObject
    {
        [SerializeField]
        private ConstantsGameSystem gameSystem = null;
        public ConstantsGameSystem GameSystem => this.gameSystem;
        
        [SerializeField]
        private ConstantsCell cell = null;
        public ConstantsCell Cell => this.cell;

        [SerializeField]
        private ConstantsHousing housing = null;
        public ConstantsHousing Housing => this.housing;

        [SerializeField]
        private ConstantsColor color = null;
        public ConstantsColor Color => this.color;
    }
}
