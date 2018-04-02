/*
-------------------------------------------------------
| Tilemap Group builder from PyxelEdit

| Export from PyxelExit:
| 1) Export tilemap
| 2) Export tileset (use same name as expororted tilemap name)

| Import to unity
| 1) Create empty gameObject
| 2) add PyxelEdit component
| 3) set properties for slice when you have others as default
| 4) from context menu press 'Generate PyxelEdit tilemap'

AndyGFX - CubesTeam - https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:2125

-------------------------------------------------------
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;
using CubesTeam.Boomlagoon.JSON;

using System.IO;

namespace CubesTeam
{
    public class PyxelEdit : MonoBehaviour
    {
        public Vector2 sliceOffset = Vector2.zero;
        public Vector2 slicePadding = Vector2.zero;
        public int orderInLayer = -10;
        public int layerDistance = 10;
        public TextAsset tilemapJSON;
    }
}