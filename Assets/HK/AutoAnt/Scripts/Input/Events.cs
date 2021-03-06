﻿using HK.Framework.EventSystems;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.InputControllers
{
    /// <summary>
    /// 入力のイベント群
    /// </summary>
    public static class Events
    {
        /// <summary>
        /// クリック時のイベント
        /// </summary>
        public class Click : Message<Click, ClickData>
        {
            public ClickData Data { get { return this.param1; } }
        }

        /// <summary>
        /// クリックアップ時のイベント
        /// </summary>
        public class ClickUp : Message<ClickUp, ClickData>
        {
            public ClickData Data { get { return this.param1; } }
        }

        /// <summary>
        /// クリックダウン時のイベント
        /// </summary>
        public class ClickDown : Message<ClickDown, ClickData>
        {
            public ClickData Data { get { return this.param1; } }
        }

        /// <summary>
        /// ドラッグ時のイベント
        /// </summary>
        public class Drag : Message<Drag, DragData>
        {
            public DragData Data { get { return this.param1; } }
        }

        public class Scroll : Message<Scroll, ScrollData>
        {
            public ScrollData Data { get { return this.param1; } }
        }

        public class ClickData
        {
            private static readonly ClickData cache = new ClickData();
            public static ClickData Get(int buttonId, Vector2 position)
            {
                cache.ButtonId = buttonId;
                cache.Position = position;

                return cache;
            }

            public int ButtonId { get; private set; }

            public Vector2 Position { get; private set; }
        }

        public class DragData
        {
            private static readonly DragData cache = new DragData();
            public static DragData Get(Vector3 deltaPosition)
            {
                cache.DeltaPosition = deltaPosition;

                return cache;
            }

            public Vector3 DeltaPosition { get; private set; }
        }

        public class ScrollData
        {
            private static readonly ScrollData cache = new ScrollData();
            public static ScrollData Get(float amount)
            {
                cache.Amount = amount;

                return cache;
            }

            public float Amount { get; private set; }
        }
    }
}
