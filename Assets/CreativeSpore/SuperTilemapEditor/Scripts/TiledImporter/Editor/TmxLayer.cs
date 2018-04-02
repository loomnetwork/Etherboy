using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CreativeSpore.TiledImporter
{
    [XmlRoot("Layer")]
    public class TmxLayer
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("width")]
        public int Width { get; set; }

        [XmlAttribute("height")]
        public int Height { get; set; }

        [XmlAttribute("opacity")]
        public float Opacity { get; set; }

        [XmlAttribute("visible")]
        public bool Visible { get; set; }

        [XmlArray(Order = 0, ElementName = "data")]
        [XmlArrayItem("tile")] 
        public List<TmxLayerTile> Tiles { get; set; }

        public TmxLayer()
        {
            Tiles = new List<TmxLayerTile>();
            Opacity = 1f;
            Visible = true;
        }
    }
}
