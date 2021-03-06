﻿using HK.AutoAnt.Extensions;
using HK.AutoAnt.Systems;
using HK.Framework.Text;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.UI
{
    /// <summary>
    /// ヘッダーの人口UIを制御するクラス
    /// </summary>
    public sealed class HeaderPopulationController : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI value = null;

        private double cachedPopulation;

        void Start()
        {
            this.UpdateValue(GameSystem.Instance.User.Town.Population.Value);

            GameSystem.Instance.UpdateAsObservable()
                .SubscribeWithState(this, (_, _this) =>
                {
                    var population = GameSystem.Instance.User.Town.Population.Value;
                    if(_this.cachedPopulation == population)
                    {
                        return;
                    }

                    _this.cachedPopulation = population;
                    _this.UpdateValue(population);
                })
                .AddTo(this);
        }

        private void UpdateValue(double value)
        {
            this.value.text = value.ToReadableString("###.00");
        }
    }
}
