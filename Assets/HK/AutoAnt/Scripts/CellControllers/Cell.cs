﻿using HK.AutoAnt.Constants;
using HK.AutoAnt.Systems;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.CellControllers
{
    /// <summary>
    /// セルの中枢となるクラス
    /// </summary>
    public sealed class Cell : MonoBehaviour, IClickableObject
    {
        [SerializeField]
        private BoxCollider boxCollider = null;

        [SerializeField]
        private Transform scalableObject = null;

        public int RecordId { get; private set; }

        public Vector2Int Position { get; private set; }

        public int Group { get; private set; }

        public CellType Type { get; private set; }

        private CellMapper cellMapper;

        public Transform CachedTransform{ get; private set; }

        void Awake()
        {
            this.CachedTransform = this.transform;
        }

        public Cell Initialize(int recordId, Vector2Int position, int group, CellType cellType, CellMapper cellMapper)
        {
            this.RecordId = recordId;
            this.Position = position;
            this.Group = group;
            this.Type = cellType;
            this.cellMapper = cellMapper;

            var constants = GameSystem.Instance.Constants.Cell;
            this.CachedTransform.position = new Vector3(position.x * (constants.Scale.x + constants.Interval), 0.0f, position.y * (constants.Scale.z + constants.Interval));
            this.scalableObject.localScale = constants.Scale;
            this.boxCollider.center = new Vector3(0.0f, constants.Scale.y / 2.0f, 0.0f);
            this.boxCollider.size = constants.Scale;

            this.cellMapper.Add(this);

            return this;
        }

        public bool HasEvent => this.cellMapper.HasEvent(this);

        public void ClearEvent()
        {
            Assert.IsTrue(this.HasEvent);

            this.cellMapper.Remove(this.cellMapper.CellEvent.Map[this.Position]);
        }

        public void OnClickDown()
        {
        }

        public void OnClickUp()
        {
            if(!this.HasEvent)
            {
                return;
            }

            this.cellMapper.CellEvent.Map[this.Position].OnClick(this);
        }

        /// <summary>
        /// フィールドに存在するセルを返す
        /// </summary>
        public static Cell GetCell(Ray ray)
        {
            var hitInfo = default(RaycastHit);
            if (Physics.Raycast(ray, out hitInfo))
            {
                return hitInfo.collider.GetComponent<Cell>();
            }

            return null;
        }
    }
}
