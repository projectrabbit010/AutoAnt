﻿using HK.AutoAnt.Database;
using HK.Framework.EventSystems;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.Events
{
    /// <summary>
    /// 建設モードへ切り替えをリクエストするイベント
    /// </summary>
    public sealed class RequestBuildingMode : Message<RequestBuildingMode, MasterDataCellEvent.Record>
    {
        /// <summary>
        /// 建設したいセルイベントのレコード
        /// </summary>
        public MasterDataCellEvent.Record BuildingCellEventRecord => this.param1;
    }
}
