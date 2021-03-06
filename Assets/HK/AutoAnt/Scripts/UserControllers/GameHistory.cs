﻿using System;
using HK.AutoAnt.Constants;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.UserControllers
{
    /// <summary>
    /// ゲーム関連の履歴
    /// </summary>
    public sealed class GameHistory
    {
        /// <summary>
        /// プレイ時間
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// 最後にゲームを終了した時間
        /// </summary>
        public DateTime LastDateTime { get; set; } = DateTime.MinValue;

        /// <summary>
        /// ゲームを離れた理由
        /// </summary>
        public GameLeftCase GameLeftCase { get; set; }
    }
}
