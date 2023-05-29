using System;
using System.Linq;

namespace UfoDefense.Entities;

public class Invader : BaseEntity
{
    private TimeSpan _nextShootTime;
    public float ShootDecrement;
    public Action OnBottomTouched;
    public Invader? UpperInvader;
    public Invader? LowerInvader;

    public Invader()
    {
        CalcNextShotTime();
        ShootDecrement = 0.0f;
        Speed = 0.05f;
        Health = 1;
    }

    private void Shoot()
    {
        if (!CanShoot())
        {
            return;
        }
        
        Sounds?.Where( s => s.Type == SoundType.Shoot).ToList().ForEach( s => s.Sound.Play(volume: 0.1f, pitch: 0.0f, pan: 0.0f));
        var bullet = (Bullet)Globals.EntityManager.CreateEntityByName("Bullet");
        bullet.Position = this.Position;
        bullet.Parent = this;
        bullet.SetType(Bullet.From.Invader);
        CalcNextShotTime();
    }

    private bool CanShoot()
    {
        return Globals.TotalTime > _nextShootTime;
    }

    public void CalcNextShotTime()
    {
        _nextShootTime = Globals.TotalTime + TimeSpan.FromSeconds(Globals.Random.Next(1, 8)) - TimeSpan.FromSeconds(ShootDecrement);
    }

    public override void Think()
    {
        if (Globals.InvadersDirection == InvadersDirection.Down 
            && Globals.TotalTime > Globals.NextInvaderMove)
        {
            Globals.InverseInvadersDirection(); 
        }

        if( LowerInvader == null )
        {
            Shoot();
        }
        Velocity += Globals.InvadersVelocity * Speed;
    }

    public override void OnTouch(BaseEntity another)
    {

    }   
    
    public override void OnWorldTouch(Physics.WorldSide side)
    {
        if( side == Physics.WorldSide.Bottom)
        {
            OnBottomTouched?.Invoke();
            return;
        }
        
        // unrealistically, invaders can't touch top side
        if (side == Physics.WorldSide.Top)
        {
            return;
        }

        if (Globals.InvadersDirection == InvadersDirection.Down)
        {
            return;
        }
        
        if( side == Physics.WorldSide.Right && Globals.InvadersDirection == InvadersDirection.Left)
        {
            return;
        }
        
        if( side == Physics.WorldSide.Left && Globals.InvadersDirection == InvadersDirection.Right)
        {
            return;
        }

        Globals.OldInvadersDirection = Globals.InvadersDirection;
        Globals.InvadersDirection = InvadersDirection.Down;
        Globals.InvadersVelocity.X = 0;
        Globals.InvadersVelocity.Y = 1;

        Globals.NextInvaderMove = Globals.TotalTime + TimeSpan.FromSeconds(0.2);
    }   
    
    public override void OnDeath()
    {
        Sounds?.Where( s => s.Type == SoundType.Death).ToList().ForEach( s => s.Sound.Play());
        SpawnBonuses();
    }

    private void SpawnBonuses()
    {
        if( Globals.Random.Next(0, 100) < 2)
        {
            var bonus = Globals.EntityManager.CreateEntityByName("MoreLife");
            bonus.Position = this.Position;
            return;
        }   

        if( Globals.Random.Next(0, 100) < 5 )
        {
            var bonus = Globals.EntityManager.CreateEntityByName("LaserShot");
            bonus.Position = this.Position;
            return;
        }
        
        if( Globals.Random.Next(0, 100) < 10)
        {
            var bonus = Globals.EntityManager.CreateEntityByName("AttackSpeed");
            bonus.Position = this.Position;
            return;
        }
    }

    public override bool CanTakeDamage()
    {
        return true;
    }
}