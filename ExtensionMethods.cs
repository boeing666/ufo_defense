using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UfoDefense;

public static class ExtensionMethods
{
    [Flags]
    public enum TextAlignment
    {
        Center = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }

    public static void DrawString(this SpriteBatch self, SpriteFont font, ref string text, Rectangle bounds,
        TextAlignment align, Color color)
    {
        Vector2 size = font.MeasureString(text);
        Vector2 pos = bounds.Center.ToVector2();
        Vector2 origin = size * 0.5f;

        if (align.HasFlag(TextAlignment.Left))
        {
            origin.X += bounds.Width / 2 - size.X / 2;
        }

        if (align.HasFlag(TextAlignment.Right))
        {
            origin.X -= bounds.Width / 2 - size.X / 2;
        }

        if (align.HasFlag(TextAlignment.Top))
        {
            origin.Y += bounds.Height / 2 - size.Y / 2;      
        }

        if (align.HasFlag(TextAlignment.Bottom))
        {
            origin.Y -= bounds.Height / 2 - size.Y / 2;       
        }

        self.DrawString(font, text, pos, color, 0, origin, 1, SpriteEffects.None, 0);
    }
    
    public static bool IsButtonOncePressed(this KeyboardState state, KeyboardState laststate, Keys key)
    {
        return state.IsKeyDown(key) && laststate.IsKeyUp(key);
    }
    
    public static void FastRemove<T>(this List<T> list, int index)
    {
        list[index] = list[ ^ 1 ];
        list.RemoveAt(list.Count - 1);
    }
}
