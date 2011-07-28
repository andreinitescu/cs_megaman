﻿using System.Drawing;

namespace MegaMan.Engine
{
    public interface IHandleGameEvents
    {
        void StartHandler();
        void StopHandler();

        void GameInputReceived(GameInputEventArgs e);
        void GameRender(GameRenderEventArgs e);
    }

    public abstract class Component
    {
        public GameEntity Parent { get; set; }

        public abstract Component Clone();

        public abstract void Start();

        public abstract void Stop();

        public abstract void Message(IGameMessage msg);

        protected abstract void Update();

        public abstract void RegisterDependencies(Component component);

        public abstract void LoadXml(System.Xml.Linq.XElement xmlNode);
    }

    public interface IPositioned
    {
        PointF Position { get; }

        void SetPosition(PointF pos);
        void Offset(float x, float y);
    }

    public interface IMovement
    {
        bool CanMove { get; set; }
        bool Flying { get; set; }
        Direction Direction { get; }
        float VelocityX { get; set; }
        float VelocityY { get; set; }
    }
}
