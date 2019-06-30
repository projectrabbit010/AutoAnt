﻿using HK.AutoAnt.Database;
using HK.AutoAnt.Extensions;
using HK.AutoAnt.GameControllers;
using HK.AutoAnt.Systems;
using HK.AutoAnt.UserControllers;
using UnityEngine;
using UnityEngine.Assertions;
using HK.AutoAnt.EffectSystems;

namespace HK.AutoAnt.CellControllers.Events
{
    /// <summary>
    /// 道路のセルイベント
    /// </summary>
    /// <remarks>
    /// - やっていること
    ///     - 周りのセルイベントの効果を上昇させる
    /// </remarks>
    [CreateAssetMenu(menuName = "AutoAnt/Cell/Event/Road")]
    public sealed class Road : CellEvent, ILevelUpEvent
    {
        /// <summary>
        /// レベル
        /// </summary>
        /// <remarks>
        /// 人口増加の変動に利用しています
        /// </remarks>
        public int Level { get; set; } = 1;

        private GameSystem gameSystem;

        public override void Initialize(Vector2Int position, GameSystem gameSystem, bool isInitializingGame)
        {
            base.Initialize(position, gameSystem, isInitializingGame);
            this.gameSystem = gameSystem;
        }

        public override void Remove(GameSystem gameSystem)
        {
            base.Remove(gameSystem);
        }

        public override void OnClick(Cell owner)
        {
            if(this.CanLevelUp())
            {
                this.LevelUp();
            }
        }

        public bool CanLevelUp()
        {
            return this.CanLevelUp(this.gameSystem);
        }

        public void LevelUp()
        {
            this.LevelUp(this.gameSystem);
            this.gameSystem.User.History.GenerateCellEvent.Add(this.Id, this.Level - 1);
        }
    }
}