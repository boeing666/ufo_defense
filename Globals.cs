using System;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace UfoDefense;

public enum InvadersDirection
{
    Left,
    Right,
    Down
}

static class Globals
{
    public static GameTime Time;
    public static TimeSpan TotalTime;
    public static GameWindow Window;
    public static ContentManager Content;
    public static GraphicsDevice Graphics;
    public static EntitiesManager EntityManager;
    public static Random Random;
    public static Logic GameLogic;

    public static bool IsPaused;
    
    public static InvadersDirection OldInvadersDirection;
    public static InvadersDirection InvadersDirection;
    public static Vector2 InvadersVelocity;
    public static TimeSpan NextInvaderMove;
    
    public static Size ScreenSize;

    public static void Init(GameWindow window, ContentManager content, GraphicsDevice graphics,
        EntitiesManager entityManager, Logic logic)
    {
        Random = new Random();
        GameLogic = logic;
        Window = window;
        Content = content;
        Graphics = graphics;
        EntityManager = entityManager;
        ScreenSize = new Size(graphics.Viewport.Width, graphics.Viewport.Height);
    }

    public static void PrepareGame()
    {
        IsPaused = false;
        InvadersDirection = InvadersDirection.Right;
        OldInvadersDirection = InvadersDirection.Right;
        InvadersVelocity = new Vector2(1, 0);
        NextInvaderMove = TimeSpan.Zero;
    }

    public static Rectangle GetScreenRect()
    {
        return new Rectangle(0, 0, ScreenSize.Width, ScreenSize.Height);
    }
    
    public static void Update(GameTime gameTime)
    {
        Time = gameTime;
        TotalTime += gameTime.ElapsedGameTime;
    }
    
    public static void InverseInvadersDirection()
    {
        switch (OldInvadersDirection)
        {
            case InvadersDirection.Left: ChangeInvadersDirection(InvadersDirection.Right); break;
            case InvadersDirection.Right: ChangeInvadersDirection(InvadersDirection.Left); break;
        }
    }
    
    private static void ChangeInvadersDirection(InvadersDirection direction)
    {
        OldInvadersDirection = InvadersDirection;
        InvadersDirection = direction;
        switch (direction)
        {
            case InvadersDirection.Down:
                InvadersVelocity.X = 0;
                InvadersVelocity.Y = 1;
                break;
            case InvadersDirection.Left:
                InvadersVelocity.X = -1;
                InvadersVelocity.Y = 0;
                break;
            case InvadersDirection.Right:
                InvadersVelocity.X = 1;
                InvadersVelocity.Y = 0;
                break;
        }
    }
}