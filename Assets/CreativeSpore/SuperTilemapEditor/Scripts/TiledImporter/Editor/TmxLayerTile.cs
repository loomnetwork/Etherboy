using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CreativeSpore.TiledImporter
{
    [XmlRoot("LayerTile")]
    public class TmxLayerTile
    {
        [XmlAttribute("gid")]
        public uint GId { get; set; }
    }
}
