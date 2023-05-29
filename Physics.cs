using Microsoft.Xna.Framework;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace UfoDefense;

public class Physics
{
    public enum WorldSide
    {
        Left = 0,
        Right,
        Top,
        Bottom
    }
    
    /// <summary>
    /// Метод, вычисляющий столкновения объектов и выход за границы мира
    /// </summary>
    public void Update()
    {
        var entities = Globals.EntityManager.Entities;
        var gameTime = Globals.Time;
        var screenSize = Globals.ScreenSize;

        foreach (var entity in entities)
        {
            Rectangle bbox = entity.GetBBox();

            if (bbox.Left < 0)
            {
                entity.OnWorldTouch(WorldSide.Left);
            }

            if( bbox.Top < 0)
            {
                entity.OnWorldTouch(WorldSide.Top);
            }

            if (bbox.Right > screenSize.Width)
            {
                entity.OnWorldTouch(WorldSide.Right);
            }    

            if (bbox.Bottom > screenSize.Height)
            {
                entity.OnWorldTouch(WorldSide.Bottom);
            }

            entity.Position += entity.Velocity * gameTime.ElapsedGameTime.Milliseconds;

            foreach (var another in entities)
            {
                if (another == entity)
                {
                    continue;
                }

                Rectangle anbbox = another.GetBBox();
                if (bbox.Intersects(anbbox))
                {
                    entity.OnTouch(another);
                }
            }

            entity.Velocity = Vector2.Zero;
        }
    }
}