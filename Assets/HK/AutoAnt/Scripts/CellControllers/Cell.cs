﻿using HK.AutoAnt.CellControllers.Events;
using HK.AutoAnt.Constants;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.CellControllers
{
    /// <summary>
    /// セルの中枢となるクラス
    /// </summary>
    public sealed class Cell : MonoBehaviour, IClickableObject
    {
        public Vector2Int Id { get; private set; }

        public CellType Type { get; private set; }

        private ICellClickEvent clickEvent = null;

        public Transform CachedTransform{ get; private set; }

        void Awake()
        {
            this.CachedTransform = this.transform;
        }

        public Cell Initialize(Vector2Int id, CellType cellType, CellSpec cellSpec)
        {
            this.Id = id;
            this.Type = cellType;

            this.CachedTransform.position = new Vector3(id.x * cellSpec.Interval, 0.0f, id.y * cellSpec.Interval);
            this.CachedTransform.localScale = cellSpec.Scale;

            return this;
        }

        public void OnClickDown()
        {
        }

        public void OnClickUp()
        {
            if(this.clickEvent == null)
            {
                return;
            }

            this.clickEvent.Do();
        }
    }
}
