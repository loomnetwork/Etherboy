using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CreativeSpore.TiledImporter
{
    [XmlRoot("map")]
    public class TmxMap
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("orientation")]
        public string Orientation { get; set; }

        [XmlAttribute("tilewidth")]
        public string TileWidth { get; set; }

        [XmlAttribute("tileheight")]
        public string TileHeight { get; set; }

        [XmlElement(Order = 0, ElementName="tileset")]
        public List<TmxTileset> Tilesets { get; set; }

        [XmlElement(Order = 1, ElementName = "layer")]
        public List<TmxLayer> Layers { get; set; }

        public TmxMap()
        {
            Tilesets = new List<TmxTileset>();
            Layers = new List<TmxLayer>();
        }

        internal void FixExportedTilesets(string relativePath)
        {
            XMLSerializer objSerializer = new XMLSerializer();
            for(int i = 0; i < Tilesets.Count; ++i)
            {
                TmxTileset tmxTileset = Tilesets[i];
                if(!string.IsNullOrEmpty(tmxTileset.Source))
                {
                    int firstGid = tmxTileset.FirstGId;
                    Tilesets[i] = tmxTileset = objSerializer.LoadFromXMLFile<TmxTileset>( Path.Combine( relativePath, tmxTileset.Source));
                    tmxTileset.FirstGId = firstGid;
                    if (tmxTileset.TileCount == 0)
                    {
                        int horTiles = System.Convert.ToInt32(Math.Round((float)(tmxTileset.Image.Width - 2 * tmxTileset.Margin) / (tmxTileset.TileWidth + tmxTileset.Spacing)));
                        int verTiles = System.Convert.ToInt32(Math.Round((float)(tmxTileset.Image.Height - 2 * tmxTileset.Margin) / (tmxTileset.TileHeight + tmxTileset.Spacing)));
                        tmxTileset.Columns = horTiles;
                        tmxTileset.TileCount = horTiles * verTiles;
                    }                    
                }
                
                //if tileset is made of a collection of sprites, tile count needs to include padding tiles (tiles that were removed)
                if(tmxTileset.Image == null)
                {
                    TmxTile tmxTile = tmxTileset.TilesWithProperties[tmxTileset.TilesWithProperties.Count - 1];
                    if (tmxTile.Image != null)
                        tmxTileset.TileCount = tmxTile.Id + 1;
                }
            }
        }
    }
}
