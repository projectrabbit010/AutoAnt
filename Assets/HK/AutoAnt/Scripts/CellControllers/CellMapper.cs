﻿using System.Collections.Generic;
using HK.AutoAnt.CellControllers.Events;
using HK.AutoAnt.SaveData.Serializables;
using UnityEngine;
using UnityEngine.Assertions;

namespace HK.AutoAnt.CellControllers
{
    /// <summary>
    /// <see cref="Cell"/>の様々な情報をマッピングするクラス
    /// </summary>
    public sealed class CellMapper
    {
        private readonly Element<Vector2Int, Cell> cell = new Element<Vector2Int, Cell>();
        public IReadonlyElement<Vector2Int, Cell> Cell => this.cell;

        private readonly Element<Vector2Int, ICellEvent> cellEvent = new Element<Vector2Int, ICellEvent>();
        public IReadonlyElement<Vector2Int, ICellEvent> CellEvent => this.cellEvent;

        public void Add(Cell cell)
        {
            Assert.IsNotNull(cell);

            this.cell.Add(cell.Position, cell);
        }

        public void Add(ICellEvent cellEvent, Vector2Int position)
        {
            Assert.IsNotNull(cellEvent);

            this.cellEvent.AddListOnly(cellEvent);

            Assert.IsTrue(this.cell.Map.ContainsKey(position), $"position = {position}にセルが無いのにイベントが登録されました");

            for (var y = 0; y < cellEvent.Size; y++)
            {
                for (var x = 0; x < cellEvent.Size; x++)
                {
                    var p = position + new Vector2Int(x, y);
                    this.cellEvent.AddMapOnly(p, cellEvent);
                }
            }
        }

        public void Remove(Cell cell)
        {
            Assert.IsNotNull(cell);

            this.cell.Remove(cell.Position);
        }

        public void Remove(ICellEvent cellEvent)
        {
            Assert.IsNotNull(cellEvent);

            this.cellEvent.RemoveListOnly(cellEvent);

            for (var y = 0; y < cellEvent.Size; y++)
            {
                for (var x = 0; x < cellEvent.Size; x++)
                {
                    var p = cellEvent.Origin + new Vector2Int(x, y);
                    this.cellEvent.RemoveMapOnly(p);
                }
            }
        }

        public bool HasEvent(Cell cell)
        {
            Assert.IsNotNull(cell);

            return this.cellEvent.Map.ContainsKey(cell.Position);
        }

        /// <summary>
        /// <paramref name="positions"/>に存在する全てのセルを返す
        /// </summary>
        public Cell[] GetCells(List<Vector2Int> positions)
        {
            Assert.IsNotNull(positions);

            var result = new Cell[positions.Count];
            for (var i = 0; i < positions.Count; i++)
            {
                var position = positions[i];
                Assert.IsTrue(this.Cell.Map.ContainsKey(position));
                result[i] = this.Cell.Map[position];
            }

            return result;
        }

        /// <summary>
        /// 指定した範囲でセルが配置されていない座標を返す
        /// </summary>
        public Vector2Int[] GetEmptyPositions(Vector2Int origin, int range)
        {
            return Vector2IntUtility.GetRange(origin, range, (p) => !this.cell.Map.ContainsKey(p)).ToArray();
        }

        /// <summary>
        /// <paramref name="group"/>に一致する全てのセルを返す
        /// </summary>
        public List<Cell> GetCellFromGroup(int group)
        {
            var result = new List<Cell>();
            foreach(var c in this.cell.List)
            {
                if(c.Group != group)
                {
                    continue;
                }

                result.Add(c);
            }

            return result;
        }

        public SerializableCellMapper GetSerializable()
        {
            var result = new SerializableCellMapper();
            foreach(var c in this.Cell.List)
            {
                result.Cells.Add(new SerializableCell() { RecordId = c.RecordId, Position = c.Position, Group = c.Group });
            }
            foreach(var e in this.CellEvent.List)
            {
                result.CellEvents.Add(e);
            }

            return result;
        }

        public void Deserialize(SerializableCellMapper serializableData, CellGenerator cellGenerator, CellEventGenerator cellEventGenerator)
        {
            Assert.IsNotNull(serializableData);
            Assert.IsNotNull(cellGenerator);
            Assert.IsNotNull(cellEventGenerator);
            
            foreach(var c in serializableData.Cells)
            {
                cellGenerator.Generate(c.RecordId, c.Position, c.Group);
            }
            foreach(var e in serializableData.CellEvents)
            {
                cellEventGenerator.GenerateOnDeserialize((CellEvent)e);
            }
        }

        public interface IReadonlyElement<Key, Value>
        {
            /// <summary>
            /// 全要素を持つリスト
            /// </summary>
            /// <remarks>
            /// 全検索などはこれを利用してください
            /// </remarks>
            IReadOnlyList<Value> List { get; }

            /// <summary>
            /// 紐づけされたマップ
            /// </summary>
            /// <remarks>
            /// <see cref="cell"/>の場合は座標と紐付けられているため、座標で検索したい場合はこれを利用してください
            /// </remarks>
            IReadOnlyDictionary<Key, Value> Map { get; }
        }

        public class Element<Key, Value> : IReadonlyElement<Key, Value>
        {
            private readonly List<Value> list = new List<Value>();

            private readonly Dictionary<Key, Value> map = new Dictionary<Key, Value>();

            public IReadOnlyList<Value> List => this.list;

            public IReadOnlyDictionary<Key, Value> Map => this.map;

            public void Add(Key key, Value value)
            {
                this.AddListOnly(value);
                this.AddMapOnly(key, value);
            }

            public void AddListOnly(Value value)
            {
                Assert.IsFalse(this.list.Contains(value), $"{value}は既に登録されています");
                this.list.Add(value);
            }

            public void AddMapOnly(Key key, Value value)
            {
                Assert.IsFalse(this.map.ContainsKey(key), $"{key}は既に登録されています");
                this.map.Add(key, value);
            }

            public void Remove(Key key)
            {
                var value = this.map[key];
                Assert.IsTrue(this.map.ContainsKey(key), $"{key}は登録されていません");

                this.RemoveListOnly(value);
                this.RemoveMapOnly(key);
            }

            public void RemoveListOnly(Value value)
            {
                Assert.IsTrue(this.list.Contains(value), $"{value}は登録されていません");
                this.list.Remove(value);
            }

            public void RemoveMapOnly(Key key)
            {
                Assert.IsTrue(this.map.ContainsKey(key), $"{key}は登録されていません");

                this.map.Remove(key);
            }
        }
    }
}
