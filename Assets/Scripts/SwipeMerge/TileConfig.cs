using System;
using UnityEngine;

namespace SwipeMerge
{
    public class TileConfig : ScriptableObject
    {
        public GameObject tileSource;

        [Header("Colors")]
        public TileColor _2;
        public TileColor _4;
        public TileColor _8;
        public TileColor _16;
        public TileColor _32;
        public TileColor _64;
        public TileColor _128;
        public TileColor _256;
        public TileColor _512;
        public TileColor _1024;
        public TileColor _2048;
        public TileColor outRange;

        public TileColor GetColor(int value) => value switch
        {
            2 => this._2,
            4 => this._4,
            8 => this._8,
            16 => this._16,
            32 => this._32,
            64 => this._64,
            128 => this._128,
            256 => this._256,
            512 => this._512,
            1024 => this._1024,
            2048 => this._2048,
            > 2048 => this.outRange,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    [Serializable]
    public struct TileColor
    {
        public Color background;
        public Color foreground;
    }
}
