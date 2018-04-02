using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CreativeSpore.TiledImporter
{
    [XmlRoot("tileset")]
    public class TmxTileset
    {

        [XmlAttribute("source")]
        public string Source { get; set; }

        [XmlAttribute("firstgid")]
        public int FirstGId {get; set;}

        [XmlAttribute("name")]
        public string Name {get; set;}

        [XmlAttribute("tilewidth")]
        public int TileWidth {get; set;}

        [XmlAttribute("tileheight")]
        public int TileHeight { get; set; }

        [XmlAttribute("spacing")]
        public int Spacing { get; set; }

        [XmlAttribute("margin")]
        public int Margin { get; set; }

        [XmlAttribute("tilecount")]
        public int TileCount { get; set; }

        [XmlAttribute("columns")]
        public int Columns { get; set; }

        [XmlElement(Order = 0, ElementName = "image")]
        public TmxImage Image { get; set; }

        [XmlElement(Order = 1, ElementName = "tile")]
        public List<TmxTile> TilesWithProperties { get; set; }

        public TmxTileset()
        {
            TilesWithProperties = new List<TmxTile>();
        }
    }
}
