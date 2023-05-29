using Microsoft.Xna.Framework;

namespace UfoDefense.Entities;

public class AttackSpeed : BaseEntity
{
    public AttackSpeed()
    {
        Health = 111;
        Speed = 0.25f;
    }
    
    public override void Think()
    {
        Velocity += new Vector2( 0, 1 ) * Speed;
    }

    public override void OnTouch(BaseEntity another)
    {
        if( another is Player player )
        {
            player.TurnOnFastShoot();
            Kill();
        }
    }

    public override void OnWorldTouch(Physics.WorldSide side)
    {
        Kill();
    }

    public override void OnDeath()
    {
        
    }

    public override bool CanTakeDamage()
    {
        return false;
    }
}