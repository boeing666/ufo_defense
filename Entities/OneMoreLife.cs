using Microsoft.Xna.Framework;

namespace UfoDefense.Entities;

public class OneMoreLife : BaseEntity
{
    public OneMoreLife()
    {
        Speed = 0.25f;
    }

    public override void Think()
    {
        Velocity += new Vector2( 0, 1 ) * Speed;
    }

    public override void OnTouch(BaseEntity another)
    {
        if( another is not Player )
        {
            return;
        }

        Globals.GameLogic.Lives += 1;
        Kill();
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