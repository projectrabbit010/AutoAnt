﻿using System;
using System.Collections.Generic;
using HK.Framework.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.Database
{
    /// <summary>
    /// 住宅のレベルに紐づくパラメータのマスターデータ
    /// </summary>
    [CreateAssetMenu(menuName = "AutoAnt/Database/HousingLevelParameter")]
    public sealed class MasterDataHousingLevelParameter : MasterDataBase<MasterDataHousingLevelParameter.Record>
    {
        [Serializable]
        public class Record : IRecord, IRecordLevel
        {
            [SerializeField]
            private int id = 0;
            public int Id => this.id;

            [SerializeField]
            private int level = 0;
            public int Level => this.level;

            /// <summary>
            /// 増加する人口の量
            /// </summary>
            [SerializeField]
            private double population = 1;
            public double Population => this.population;

#if UNITY_EDITOR
            public Record(SpreadSheetData.HousingLevelParameterData data)
            {
                this.id = data.Id;
                this.level = data.Level;
                this.population = data.Population;
            }
#endif
        }
    }
}
