﻿using MegaMan.Common;
using MegaMan.IO.Xml;
using System.Xml.Linq;

namespace MegaMan.Editor.Bll
{
    public class Entity
    {
        // for now, this class is only used for placement
        public string Name { get; private set; }
        public Sprite MainSprite { get; private set; }

        public Entity(XElement xmlNode, string basePath)
        {
            Name = xmlNode.RequireAttribute("name").Value;

            // find the primary sprite
            var spriteNode = xmlNode.Element("Sprite");

            if (spriteNode != null)
            {
                // if it doesn't have a tilesheet, use the first for the entity
                var sheetNode = xmlNode.Element("Tilesheet");

                if (sheetNode == null)
                {
                    MainSprite = GameXmlReader.LoadSprite(spriteNode, basePath);
                }
                else
                {
                    string sheetPath = System.IO.Path.Combine(basePath, sheetNode.Value);
                    MainSprite = GameXmlReader.LoadSprite(spriteNode);
                }

                MainSprite.Play();
                //Program.AnimateTick += Program_FrameTick;
            }
        }

        void Program_FrameTick()
        {
            MainSprite.Update();
        }
    }
}
