using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace MegaMan.Common
{
    public class Tileset : List<Tile>
    {
        private Dictionary<string, TileProperties> properties;
        public IEnumerable<TileProperties> Properties { get { return properties.Values; } }

        public FilePath SheetPath { get; set; }

        public FilePath FilePath { get; set; }

        public int TileSize { get; set; }

        public Tileset()
        {
            properties = new Dictionary<string, TileProperties>();
            properties["Default"] = TileProperties.Default;
        }

        public void ChangeSheetPath(string path)
        {
            SheetPath = FilePath.FromAbsolute(path, FilePath.BasePath);
        }

        public Tile AddTile()
        {
            var sprite = new TileSprite(this);
            var tile = new Tile(this.Count, sprite);
            base.Add(tile);
            return tile;
        }

        public TileProperties GetProperties(string name)
        {
            if (properties.ContainsKey(name)) return properties[name];
            return TileProperties.Default;
        }

        public void AddProperties(TileProperties properties)
        {
            this.properties[properties.Name] = properties;
        }

        public void DeleteProperties(TileProperties properties)
        {
            foreach (var tile in this.Where(t => t.Properties == properties))
            {
                tile.Properties = TileProperties.Default;
            }

            this.properties.Remove(properties.Name);
        }

        public void Save(string path)
        {
            if (FilePath == null)
            {
                FilePath = FilePath.FromAbsolute(path, Directory.GetParent(path).FullName);
            }
            else
            {
                FilePath = FilePath.FromAbsolute(path, FilePath.BasePath);
            }

            XmlTextWriter writer = new XmlTextWriter(FilePath.Absolute, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Tileset");

            if (SheetPath != null)
                writer.WriteAttributeString("tilesheet", SheetPath.Relative);

            writer.WriteAttributeString("tilesize", TileSize.ToString());

            writer.WriteStartElement("TileProperties");
            foreach (TileProperties properties in this.properties.Values)
            {
                if (properties.Name == "Default" && properties == TileProperties.Default)
                    continue;
                properties.Save(writer);
            }
            writer.WriteEndElement();

            foreach (Tile tile in this)
            {
                writer.WriteStartElement("Tile");
                writer.WriteAttributeString("id", tile.Id.ToString());
                writer.WriteAttributeString("name", tile.Name);
                writer.WriteAttributeString("properties", tile.Properties.Name);

                tile.Sprite.WriteTo(writer);

                writer.WriteEndElement();   // end Tile
            }
            writer.WriteEndElement();

            writer.Close();
        }
        
        public static Tileset Load(FilePath path)
        {
            var tileset = new Tileset();

            tileset.FilePath = path;

            var doc = XDocument.Load(path.Absolute);
            var reader = doc.Element("Tileset");
            if (reader == null)
                throw new Exception("The specified tileset definition file does not contain a Tileset tag.");

            var sheetPath = FilePath.FromRelative(reader.Attribute("tilesheet").Value, path.BasePath);
            tileset.ChangeSheetPath(sheetPath.Absolute);

            int size;
            if (!int.TryParse(reader.Attribute("tilesize").Value, out size))
                throw new Exception("The tileset definition does not contain a valid tilesize attribute.");
            tileset.TileSize = size;

            var propParent = reader.Element("TileProperties");
            if (propParent != null)
            {
                foreach (XElement propNode in propParent.Elements("Properties"))
                {
                    var prop = new TileProperties(propNode);
                    tileset.AddProperties(prop);
                }
            }

            foreach (XElement tileNode in reader.Elements("Tile"))
            {
                int id = int.Parse(tileNode.Attribute("id").Value);
                string name = tileNode.Attribute("name").Value;

                var spriteNode = tileNode.Element("Sprite");
                if (spriteNode == null)
                    throw new GameXmlException(tileNode, "All Tile tags must contain a Sprite tag.");

                var sprite = Sprite.FromXml(spriteNode);
                var tileSprite = new TileSprite(tileset, sprite);

                Tile tile = new Tile(id, tileSprite);

                string propName = "Default";
                XAttribute propAttr = tileNode.Attribute("properties");
                if (propAttr != null)
                    propName = propAttr.Value;

                tile.Properties = tileset.GetProperties(propName);

                tile.Sprite.Play();
                tileset.Add(tile);
            }

            return tileset;
        }
        
    }
}
