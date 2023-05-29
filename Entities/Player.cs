using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace UfoDefense.Entities;

public class Player : BaseEntity
{
    private TimeSpan _nextShootTime;
    public Action Death;
    private TimeSpan _endOfGodMode;
    private TimeSpan _endFastShootTime;
    private bool _godMode;
    private bool _megaBullet;

    public Player()
    {
        TurnOnGodMode();
        AllowToShoot();
        _megaBullet = false;
        Speed = 0.3f;
    }
    
    public void TurnOnGodMode()
    {
        _endOfGodMode = Globals.TotalTime + TimeSpan.FromSeconds(3);
        _godMode = true;
        Color = Color.Red;
    }   
    
    public void TurnOnMegaBullet()
    {
        _megaBullet = true;
    }    
    
    public void TurnOnFastShoot()
    {
        AllowToShoot();
        _endFastShootTime = Globals.TotalTime + TimeSpan.FromSeconds(5);
    }
    
    private bool CanShoot()
    {
        return Globals.TotalTime > _nextShootTime;
    }

    private void CalcNextShotTime()
    {
        var addTime = TimeSpan.FromSeconds(9999);
        if (_endFastShootTime > Globals.TotalTime)
        {
            addTime = TimeSpan.FromSeconds(0.6);
        }
        _nextShootTime = Globals.TotalTime + addTime;
    }
    
    public void AllowToShoot()
    {
        _nextShootTime = TimeSpan.Zero;
    }

    public override void Think()
    {
        if (_godMode && Globals.TotalTime > _endOfGodMode)
        {
            Color = Color.White;
            _godMode = false;
        }

        KeyboardState state = Keyboard.GetState();

        int dir = 0;
        if ( state.IsKeyDown(Keys.A) )
        {
            dir = -1;
        }
        if ( state.IsKeyDown(Keys.D) )
        {
            dir = dir == -1 ? 0 : 1;
        }

        if (dir != 0)
        {
            Velocity.X += Speed * dir;
        }
        
        if ( state.IsKeyDown(Keys.Space)  )
        {
            Shoot();
        }
    }
    
    public void Shoot()
    {
        if (!CanShoot())
        {
            return;
        }
        
        Sounds?.Where( s => s.Type == SoundType.Shoot).ToList().ForEach( s => s.Sound.Play());
        var bullet = (Bullet)Globals.EntityManager.CreateEntityByName("Bullet");
        bullet.Position = this.Position;
        bullet.Parent = this;
        bullet.SetType(Bullet.From.Player);
        if (_megaBullet)
        {
            bullet.Health = 5;
            bullet.Color = Color.Red;
            _megaBullet = false;
        }
        CalcNextShotTime( );
    }

    public override void OnTouch(BaseEntity another)
    {
        
    }
    
    public override void OnWorldTouch(Physics.WorldSide side)
    {
        switch (side)
        {
            case Physics.WorldSide.Left:
            {
                if( Velocity.X < 0 )
                {
                    Velocity.X = 0;
                }
                break;
            }
            case Physics.WorldSide.Right:
            {
                if (Velocity.X > 0)
                {
                    Velocity.X = 0;
                }
                break;
            }
        }
    }   
    
    public override void OnDeath()
    {
        Sounds?.Where( s => s.Type == SoundType.Death).ToList().ForEach( s => s.Sound.Play(volume: 0.1f, pitch: 0.0f, pan: 0.0f));
        Death?.Invoke();
    }

    public override bool CanTakeDamage()
    {
        return !_godMode;
    }
}