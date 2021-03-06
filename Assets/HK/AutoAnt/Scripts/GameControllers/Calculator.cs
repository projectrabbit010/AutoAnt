﻿using HK.AutoAnt.Systems;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.GameControllers
{
    /// <summary>
    /// ゲームに関する様々な計算式を定義するクラス
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// セルを開拓するために必要なお金を返す
        /// </summary>
        public static double DevelopCost(double cost, Vector2Int position)
        {
            return cost * position.sqrMagnitude;
        }

        /// <summary>
        /// 人口増加量を返す
        /// </summary>
        /// <param name="basePopulation">ベースの増加量</param>
        /// <param name="popularity">人気度</param>
        /// <param name="popularityRate">人気度の係数</param>
        public static double AddPopulation(double basePopulation, double popularity, float popularityRate, float rate, float deltaTime)
        {
            return ((basePopulation * (popularity / popularityRate)) * rate) * deltaTime;
        }

        /// <summary>
        /// 加算出来る税金を返す
        /// </summary>
        public static double Tax(double economic, float deltaTime)
        {
            // 人口は小数点以下は切り取り
            economic = System.Math.Floor(economic);
            
            return (economic * 10.0d) * deltaTime;
        }
    }
}
