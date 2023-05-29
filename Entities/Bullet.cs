using Microsoft.Xna.Framework;

namespace UfoDefense.Entities;

public class Bullet : BaseEntity
{
    public Vector2 baseVelocity;

    public enum From
    {
        Player,
        Invader
    }
    
    private From _type;

    public Bullet()
    {
        Speed = 1.0f;
    }
    
    public void SetType(From type)
    {
        _type = type;
        if( _type == From.Invader )
        {
            Speed = 0.5f;
        }

        baseVelocity = new Vector2(0, _type == From.Player ? -1 : 1);
    }
    
    public override void Think()
    {
        Velocity += baseVelocity * Speed;
    }

    public override void OnTouch(BaseEntity another)
    {
        if (another == Parent)
        {
            return;
        }
        
        Invader invader = null;
        if( another is not Player )
        {
            invader = another as Invader;
            if( invader == null )
            {
                return;
            }
        }
        
        if( invader != null && _type == From.Invader )
        {
            return;
        }

        TakeDamage(1);

        if (another.CanTakeDamage())
        {
            another.TakeDamage(1);
        }
    }
    
    public override void OnDeath()
    {
        if( Parent is Player player )
        {
            player.AllowToShoot();
        }
    }
    
    public override void OnWorldTouch(Physics.WorldSide side)
    {
        Kill();
    } 

    public override bool CanTakeDamage()
    {
        return true;
    }
}