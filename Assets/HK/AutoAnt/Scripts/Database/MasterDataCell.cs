﻿using System;
using System.Collections.Generic;
using HK.AutoAnt.CellControllers;
using HK.AutoAnt.CellControllers.Events;
using HK.AutoAnt.Constants;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.Database
{
    /// <summary>
    /// セルのマスターデータ
    /// </summary>
    [CreateAssetMenu(menuName = "AutoAnt/Database/Cell")]
    public sealed class MasterDataCell : MasterDataBase<MasterDataCell.Record>
    {
        [Serializable]
        public class Record : IRecord
        {
            [SerializeField]
            private int id = 0;
            public int Id => this.id;

            [SerializeField]
            private CellType cellType = CellType.Grassland;
            public CellType CellType => this.cellType;

            [SerializeField]
            private Cell prefab = null;
            public Cell Prefab => this.prefab;

            [SerializeField]
            private List<CellEvent> clickEvents = new List<CellEvent>();
            public List<CellEvent> ClickEvents => this.clickEvents;
        }
    }
}