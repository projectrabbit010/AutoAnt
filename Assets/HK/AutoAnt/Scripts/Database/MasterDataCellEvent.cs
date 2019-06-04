﻿using System;
using System.Collections.Generic;
using System.Globalization;
using HK.AutoAnt.CellControllers;
using HK.AutoAnt.CellControllers.Events;
using HK.AutoAnt.Constants;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.Database
{
    /// <summary>
    /// セルイベントのマスターデータ
    /// </summary>
    [CreateAssetMenu(menuName = "AutoAnt/Database/CellEvent")]
    public sealed class MasterDataCellEvent : MasterDataBase<MasterDataCellEvent.Record>
    {
        [Serializable]
        public class Record : IRecord
        {
            private int cachedId = int.MinValue;
            public int Id
            {
                get
                {
                    if(this.cachedId == int.MinValue)
                    {
                        if(!int.TryParse(this.eventData.name, out this.cachedId))
                        {
                            Assert.IsTrue(false, $"{typeof(CellEvent).Name}の{this.eventData.name}を数値に変換出来ませんでした");
                        }
                    }

                    return this.cachedId;
                }
            }

            [SerializeField]
            private CellEvent eventData = null;
            public CellEvent EventData => this.eventData;
        }
    }
}
