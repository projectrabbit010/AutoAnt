﻿using HK.AutoAnt.Extensions;
using HK.AutoAnt.GameControllers;
using HK.AutoAnt.Systems;
using HK.AutoAnt.UserControllers;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.CellControllers.Events
{
    /// <summary>
    /// 住宅のセルイベント
    /// </summary>
    /// <remarks>
    /// - やっていること
    ///     - 人口を増やす
    /// </remarks>
    [CreateAssetMenu(menuName = "AutoAnt/Cell/Event/Housing")]
    public sealed class Housing : CellEventBlankGimmick, IAddTownPopulation
    {
        /// <summary>
        /// ベース人口増加量
        /// </summary>
        public int BasePopulationAmount = 0;

        /// <summary>
        /// 保持している人口
        /// </summary>
        [HideInInspector]
        public int CurrentPopulation = 0;

        /// <summary>
        /// レベル
        /// </summary>
        /// <remarks>
        /// 人口増加の変動に利用しています
        /// </remarks>
        public int Level = 1;

        private GameSystem gameSystem;

        void IAddTownPopulation.Add(Town town)
        {
            // FIXME: 正式な計算式を適用する
            var result = Mathf.FloorToInt((this.BasePopulationAmount * (this.Level / 10.0f)) * (town.Popularity.Value / 1000.0f));
            this.CurrentPopulation += result;
            town.AddPopulation(result);
        }

        public override void Initialize(Vector2Int position, GameSystem gameSystem)
        {
            base.Initialize(position, gameSystem);
            this.gameSystem = gameSystem;
            this.gameSystem.User.Town.AddPopulation(this.CurrentPopulation);
            this.gameSystem.UserUpdater.AddTownPopulations.Add(this);
        }

        public override void Remove(GameSystem gameSystem)
        {
            base.Remove(gameSystem);
            gameSystem.UserUpdater.AddTownPopulations.Remove(this);
            gameSystem.User.Town.AddPopulation(-this.CurrentPopulation);
        }

        public override void OnClick(Cell owner)
        {
            var levelUpCostRecord = this.gameSystem.MasterData.LevelUpCost.Records.Get(this.Id, this.Level);
            if(levelUpCostRecord == null)
            {
                Debug.Log($"Id = {this.Id}は既にレベルMAX");
                return;
            }

            if(!levelUpCostRecord.Cost.IsEnough(this.gameSystem.User, this.gameSystem.MasterData.Item))
            {
                Debug.Log($"Id = {this.Id}, Level = {this.Level}の必要な素材が足りない");
                return;
            }

            levelUpCostRecord.Cost.Consume(this.gameSystem.User, this.gameSystem.MasterData.Item);
            this.Level++;
            Debug.Log($"LevelUp -> {this.Level}");
        }
    }
}
