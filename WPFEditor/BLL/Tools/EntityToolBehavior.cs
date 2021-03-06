﻿using MegaMan.Common.Entities;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class EntityToolBehavior : IToolBehavior
    {
        private EntityInfo _entity;

        public EntityToolBehavior(EntityInfo entity)
        {
            _entity = entity;
        }

        public void Click(ScreenCanvas canvas, Point location)
        {
            var placement = new Common.EntityPlacement() {
                entity = _entity.Name,
                direction = Common.Direction.Unknown,
                screenX = location.X,
                screenY = location.Y
            };

            canvas.Screen.AddEntity(placement);

            canvas.Screen.Stage.PushHistoryAction(new AddEntityAction(placement, canvas.Screen));
        }

        public void Move(ScreenCanvas canvas, Point location)
        {

        }

        public void Release(ScreenCanvas canvas, Point location)
        {

        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {
            var i = canvas.Screen.FindEntityAt(location);
            if (i >= 0)
            {
                var e = canvas.Screen.GetEntity(i);
                canvas.Screen.RemoveEntity(e);
                canvas.Screen.Stage.PushHistoryAction(new RemoveEntityAction(e, canvas.Screen));
            }
        }
    }
}
