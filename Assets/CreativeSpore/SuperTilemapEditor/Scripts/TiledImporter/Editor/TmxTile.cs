using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CreativeSpore.TiledImporter
{
    [XmlRoot("Tile")]
    public class TmxTile
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlArray(Order = 0, ElementName = "properties")]
        [XmlArrayItem("property")]
        public List<TmxTileProperty> Properties { get; set; }

        [XmlElement(Order = 1, ElementName = "image")]
        public TmxImage Image { get; set; }

        public TmxTile()
        {
            Properties = new List<TmxTileProperty>();
        }
    }
}
