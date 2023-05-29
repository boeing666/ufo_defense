using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace UfoDefense.Entities;

public abstract class BaseEntity
{
    public Color Color;
    public Vector2 Position;
    public Vector2 Velocity;
    
    public BaseEntity? Parent;
    public float Speed;
    public int Health;
    
    public int ActiveTexture; 
    public Texture2D[] Textures;
    public Sounds[]? Sounds;

    public bool DeleteSelf;
    
    public BaseEntity()
    {
        Color = Color.White;
    }    
    
    ~BaseEntity()
    {
        Textures = null;
        Sounds = null;
        Parent = null;
    }

    public bool IsAlive()
    {
        return Health > 0;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D texture = GetActiveTexture();
        Vector2 drawpos = new Vector2(Position.X - texture.Width / 2,
            Position.Y - texture.Height);

        spriteBatch.Begin();
        spriteBatch.Draw(Textures[ActiveTexture], drawpos, Color);
        spriteBatch.End();
    }
    
    public Texture2D GetActiveTexture()
    {
        return Textures[ActiveTexture];
    }
    
    public Rectangle GetBBox()
    {
        Texture2D texture = GetActiveTexture();
        return new Rectangle((int)Position.X - (texture.Width / 2),
            (int)Position.Y - texture.Height, texture.Width, texture.Height);
    }

    public void Kill()
    {
        OnDeath();
        DeleteSelf = true;
    }

    public void TakeDamage( int damage )
    {
        if( --Health <= 0 )
        {
            Kill();
        }
    }

    public abstract void Think();
    public abstract void OnTouch(BaseEntity another);
    public abstract void OnWorldTouch(Physics.WorldSide side);
    public abstract void OnDeath();
    public abstract bool CanTakeDamage();
}