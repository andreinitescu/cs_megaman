﻿using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    public interface IUndoableAction
    {
        void Execute();
        IUndoableAction Reverse();
    }

    public class TileChange
    {
        private ScreenDocument screen;
        private readonly int tileX;
        private readonly int tileY;
        private readonly int oldTileId;
        private readonly int newTileId;

        public TileChange(ScreenDocument screen, int tx, int ty, int oldId, int newId)
        {
            this.screen = screen;
            this.tileX = tx;
            this.tileY = ty;
            this.oldTileId = oldId;
            this.newTileId = newId;
        }

        public TileChange Reverse()
        {
            return new TileChange(screen, tileX, tileY, newTileId, oldTileId);
        }

        public void Apply()
        {
            screen.ChangeTile(tileX, tileY, newTileId);
        }
    }

    public class DrawAction : IUndoableAction
    {
        private readonly List<TileChange> changes;
        private readonly string name;

        public DrawAction(string name, IEnumerable<TileChange> changes)
        {
            this.name = name;
            this.changes = new List<TileChange>(changes);
        }

        public override string ToString()
        {
            return name;
        }

        public void Execute()
        {
            foreach (TileChange change in changes)
            {
                change.Apply();
            }
        }

        public IUndoableAction Reverse()
        {
            List<TileChange> ch = new List<TileChange>(changes.Count);
            ch.AddRange(changes.Select(change => change.Reverse()));
            return new DrawAction(name, ch);
        }
    }

    public class AddEntityAction : IUndoableAction
    {
        private readonly EntityPlacement entity;
        private readonly ScreenDocument screen;

        public AddEntityAction(EntityPlacement entity, ScreenDocument screen)
        {
            this.entity = entity;
            this.screen = screen;
        }

        public void Execute()
        {
            screen.AddEntity(entity);
        }

        public IUndoableAction Reverse()
        {
            return new RemoveEntityAction(entity, screen);
        }
    }

    public class RemoveEntityAction : IUndoableAction
    {
        private readonly EntityPlacement entity;
        private readonly ScreenDocument screen;

        public RemoveEntityAction(EntityPlacement entity, ScreenDocument screen)
        {
            this.entity = entity;
            this.screen = screen;
        }

        public void Execute()
        {
            screen.RemoveEntity(entity);
        }

        public IUndoableAction Reverse()
        {
            return new AddEntityAction(entity, screen);
        }
    }
}
