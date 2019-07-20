﻿using HK.AutoAnt.CellControllers.Events;
using HK.AutoAnt.Extensions;
using HK.Framework.Text;
using TMPro;
using UnityEngine;

namespace HK.AutoAnt.UI
{
    /// <summary>
    /// アンロックされたセルイベントを表示するポップアップ
    /// </summary>
    public sealed class UnlockCellEventPopup : TweenPopup
    {
        [SerializeField]
        private TextMeshProUGUI message = null;

        [SerializeField]
        private Transform gimmickViewParent = null;

        [SerializeField]
        private StringAsset.Finder format = null;

        public void Initialize(CellEvent cellEvent)
        {
            this.message.text = this.format.Format(cellEvent.EventName);
            var gimmick = Instantiate(cellEvent.GimmickPrefab, this.gimmickViewParent, false).transform;
            gimmick.localPosition = Vector3.zero;
            gimmick.localRotation = Quaternion.identity;
            gimmick.gameObject.SetLayerRecursive(Layers.Id.UI);
        }
    }
}
