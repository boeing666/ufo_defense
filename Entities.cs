using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using UfoDefense.Entities;

namespace UfoDefense;

public enum SoundType
{
    Death = 0,
    Shoot
}

public class Sounds
{
    public SoundEffect Sound;
    public SoundType Type;
}

public class EntityInfo
{
    public Texture2D[] textures;
    public Sounds[] soundEffect;
    public Func<BaseEntity> createEntity;
}

public class EntitiesManager
{
    public readonly List<BaseEntity> Entities;
    public Action<BaseEntity> OnEntityDeleted;
    
    private readonly Dictionary<string, EntityInfo> _entitiesFactory;
    private readonly List<BaseEntity> _entitiesToAdd;
    private readonly List<int> _entitiesToRemove;

    public EntitiesManager()
    {
        _entitiesFactory = new Dictionary<string, EntityInfo>();
        Entities = new List<BaseEntity>(128);
        _entitiesToAdd = new List<BaseEntity>(32);
        _entitiesToRemove = new List<int>(32);
    }
    
    public void InitContent()
    {
        _entitiesFactory["Player"] = new EntityInfo()
        {
            textures = new Texture2D[] { Globals.Content.Load<Texture2D>("images/Ship") },
            soundEffect = new Sounds[]
            {
                new Sounds { Sound = Globals.Content.Load<SoundEffect>("sounds/playerDeath"), Type = SoundType.Death },
                new Sounds { Sound = Globals.Content.Load<SoundEffect>("sounds/playerShoot"), Type = SoundType.Shoot },
            },
            createEntity = () => new Player()
        };
        _entitiesFactory["Bullet"] = new EntityInfo()
        {
            textures = new Texture2D[] { Globals.Content.Load<Texture2D>("images/Bullet") },
            soundEffect = null,
            createEntity = () => new Bullet()
        };
        _entitiesFactory["AttackSpeed"] = new EntityInfo()
        {
            textures = new Texture2D[] { Globals.Content.Load<Texture2D>("images/BonusSpeed") },
            soundEffect = null,
            createEntity = () => new AttackSpeed()
        };
        _entitiesFactory["LaserShot"] = new EntityInfo()
        {
            textures = new Texture2D[] { Globals.Content.Load<Texture2D>("images/BonusLaser") },
            soundEffect = null,
            createEntity = () => new LaserShot()
        };
        _entitiesFactory["MoreLife"] = new EntityInfo()
        {
            textures = new Texture2D[] { Globals.Content.Load<Texture2D>("images/BonusLife") },
            soundEffect = null,
            createEntity = () => new OneMoreLife()
        };
        _entitiesFactory["Invader"] = new EntityInfo()
        {
            textures = new Texture2D[]
            {
                Globals.Content.Load<Texture2D>("images/Invader1"),
                Globals.Content.Load<Texture2D>("images/Invader2"),
                Globals.Content.Load<Texture2D>("images/Invader3"),
                Globals.Content.Load<Texture2D>("images/Invader4"),
                Globals.Content.Load<Texture2D>("images/Invader5"),
            },
            soundEffect = new Sounds[]
            {
                new Sounds { Sound = Globals.Content.Load<SoundEffect>("sounds/InvaderDeath"), Type = SoundType.Death },
                new Sounds { Sound = Globals.Content.Load<SoundEffect>("sounds/InvaderShoot"), Type = SoundType.Shoot },
            },
            createEntity = () => new Invader()
        };
    }

    public BaseEntity CreateEntityByName(string name)
    {
        if (!_entitiesFactory.TryGetValue(name, out var entityInfo))
        {
            return null;
        }

        var entity = entityInfo.createEntity();
        entity.Textures = entityInfo.textures;
        entity.Sounds = entityInfo.soundEffect;

        _entitiesToAdd.Add( entity );

        return entity;
    }
    
    public void Update()
    {
        RemoveDeadEntities();
        CompleteActions();
        foreach (var entity in Entities)
        {
            entity.Think();
        }
        CompleteActions();
    }

    private void RemoveDeadEntities()
    {
        for( int i = Entities.Count - 1; i >= 0; i--)
        {
            if (Entities[i].DeleteSelf)
            {
                RemoveEntity(i);
            }
        }
    }

    private void CompleteActions()
    {
        if (_entitiesToAdd.Count != 0)
        {
            Entities.AddRange(_entitiesToAdd);
            _entitiesToAdd.Clear();
        }

        if (_entitiesToRemove.Count != 0)
        {
            foreach( var index in _entitiesToRemove)
            {
                OnEntityDeleted?.Invoke(Entities[index]);
                Entities.FastRemove(index);
            }
            _entitiesToRemove.Clear();
        }
    }

    private void RemoveEntity(int index)
    {
        _entitiesToRemove.Add(index);
    }

    public void ClearEntities()
    {
        for( int i = Entities.Count - 1; i >= 0; i--)
        {
            Entities[i].DeleteSelf = true;
        }
    }
    
    public void Draw( SpriteBatch spriteBatch)
    {
        foreach (var entity in Entities)
        {
            entity.Draw(spriteBatch);
        }
    }
}