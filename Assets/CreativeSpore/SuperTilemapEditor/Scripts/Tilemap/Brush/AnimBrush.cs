using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace CreativeSpore.SuperTilemapEditor
{

    public class AnimBrush : TilesetBrush
    {
        public uint AnimFPS = 4;

        [Serializable]
        public class TileAnimFrame
        {
            /// <summary>
            /// Contains the tileData for this frame
            /// </summary>
            public uint tileId; // NOTE: now contains tileData, not just the id
            public Vector2 UVOffset;
            // Idea for animation improvements
            // public float time; //<= 0 means, one per frame, > 0 is the time to stay
            // OR
            // public int frames; //<= 0 means, one per frame, > 0 is the number of frames to stay
        }
        public List<TileAnimFrame> AnimFrames = new List<TileAnimFrame>();

        #region IBrush

        public override uint PreviewTileData()
        {
            if (AnimFrames.Count > 0)
            {
                int animIdx = (int)(Time.realtimeSinceStartup * AnimFPS) % AnimFrames.Count;
                return AnimFrames[animIdx].tileId;
            }
            return Tileset.k_TileId_Empty;
        }

        public override uint Refresh(STETilemap tilemap, int gridX, int gridY, uint tileData)
        {
            if (m_animTileIdx < AnimFrames.Count)
            {
                return (tileData & ~Tileset.k_TileDataMask_TileId) | (uint)AnimFrames[m_animTileIdx].tileId;
            }
            return tileData;
        }

        public override bool IsAnimated()
        {
            return true;
        }

        private int m_animTileIdx = 0;
        public override Rect GetAnimUV( )
        {
            if (AnimFrames.Count > 0)
            {
                int animIdx = (int)(Time.realtimeSinceStartup * AnimFPS) % AnimFrames.Count;
                TileAnimFrame animFrame = AnimFrames[animIdx];
                uint tileData = animFrame.tileId;
                int tileId = (int)(tileData & Tileset.k_TileDataMask_TileId);
                Rect uv = tileId != Tileset.k_TileId_Empty ? Tileset.Tiles[tileId].uv : default(Rect);
                uv.position += animFrame.UVOffset;                
                return uv;
            }
            return default(Rect);
        }

        public override int GetAnimFrameIdx()
        {
            return (int)(Time.realtimeSinceStartup * AnimFPS) % AnimFrames.Count;
        }

        public override uint GetAnimTileData()
        {
            if (AnimFrames.Count > 0)
            {
                int animIdx = (int)(Time.realtimeSinceStartup * AnimFPS) % AnimFrames.Count;
                TileAnimFrame animFrame = AnimFrames[animIdx];
                return animFrame.tileId;
            }
            return Tileset.k_TileData_Empty;
        }

        #endregion
    }
}