﻿using HK.AutoAnt.CellControllers.Gimmicks;
using HK.AutoAnt.Systems;
using HK.AutoAnt.UI;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.CellControllers.Events
{
    /// <summary>
    /// <see cref="Cell"/>の各種イベントを持つインターフェイス
    /// </summary>
    public interface ICellEvent
    {
        /// <summary>
        /// セルイベントの名前
        /// </summary>
        string EventName { get; }

        /// <summary>
        /// セルイベントに対して通知されるブローカー
        /// </summary>
        IMessageBroker Broker { get; }

        /// <summary>
        /// 原点座標
        /// </summary>
        Vector2Int Origin { get; }

        /// <summary>
        /// セルサイズ
        /// </summary>
        /// <remarks>
        /// 正方形にのみ対応しています
        /// </remarks>
        int Size { get; }

        /// <summary>
        /// ギミックのプレハブ
        /// </summary>
        GameObject GimmickPrefab { get; }

        /// <summary>
        /// ギミックのインスタンス
        /// </summary>
        GameObject Gimmick { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        /// <remarks>
        /// <see cref="isInitializingGame"/>はセーブデータから生成された場合は<c>true</c>になります
        /// 例えばセーブデータから復帰した場合はSEを再生しないなどに利用してください
        /// </remarks>
        void Initialize(Vector2Int position, bool isInitializingGame);

        /// <summary>
        /// 削除処理
        /// </summary>
        void Remove();

        /// <summary>
        /// <see cref="CellGimmickController"/>を生成する
        /// </summary>
        GameObject CreateGimmickController(Vector2Int origin);

        /// <summary>
        /// 作成可能か返す
        /// </summary>
        Constants.CellEventGenerateEvalute CanGenerate(Cell owner, int cellEventRecordId, CellMapper cellMapper);

        /// <summary>
        /// セルがクリックされた時の処理
        /// </summary>
        void OnClick(Cell owner);
    }
}
