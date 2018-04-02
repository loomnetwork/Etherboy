using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CubesTeam
{
    [System.Serializable]
    public class PyxelEditTilemap
    {
        public int tileswide = 0;
	    public int tileshigh = 0;
	    public int tilewidth = 0;
	    public int tileheight = 0;
	    public LayerRefs[] layers;
        
        
    }

    [System.Serializable]
    public class LayerRefs
    {
	    public string name = "";	    
	    public int number = 0;
	    public TileRefs[] tiles;
	    
    }

    [System.Serializable]
    public class TileRefs
    {
        public int rot = 0;
        public int tile = 0;
        public bool flipX = false;
        public int y = 0;
        public int index = 0;
        public int x = 0;
    }
}