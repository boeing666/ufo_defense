using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using UfoDefense.Entities;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = System.Numerics.Vector2;

namespace UfoDefense;

public class Logic
{
    public int Score;
    public int HighScore;
    public int Level;
    public int Lives;
    
    private int _invadersColumns = 11;
    private int _invadersRows = 5;

    private SpriteFont _font;
    
    private bool GameOver;
    public Action OnGameEnd;
    public Action OnLevelCompleted;
    
    public void Init()
    {
        Globals.EntityManager.OnEntityDeleted += OnEntityDeleted;
    }

    public void GameStart()
    {
        LoadProgress();
        SpawnPlayer();
        InitLevel();
    }

    public void SpawnPlayer()
    {
        var screen = Globals.ScreenSize;
        var player = (Player)Globals.EntityManager.CreateEntityByName("Player");
        player.Death += OnPlayerDeath;
        player.Position = new Vector2(screen.Width/2, screen.Height - 32);      
    }
    
    public void SaveProgress()
    {
        using (FileStream fs = new FileStream("save.bin", FileMode.Create))
        {
            fs.Write(BitConverter.GetBytes(Score));
            fs.Write(BitConverter.GetBytes(HighScore));
            fs.Write(BitConverter.GetBytes(Level));
            fs.Write(BitConverter.GetBytes(Lives));
            fs.Close();
        }
    }
    
    public void LoadProgress()
    {
        if (!File.Exists("save.bin"))
        {
            Lives = 3;
            Score = 0;
            Level = 0;
            return;
        }

        using (FileStream fs = new FileStream("save.bin", FileMode.Open))
        {
            byte[] buffer = new byte[16]; // 4 bytes per variable, 4 variables
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            Score = BitConverter.ToInt32(buffer, 0);
            HighScore = BitConverter.ToInt32(buffer, 4);
            Level = BitConverter.ToInt32(buffer, 8);
            Lives = BitConverter.ToInt32(buffer, 12);
        }
        File.Delete("save.bin");
    }
    
    private void OnEntityDeleted(BaseEntity entity)
    {
        if( GameOver )
        {
            return;
        }
  
        if (entity is Invader invader)
        {
            OnInvaderDeath(invader);
        }
    }

    private void OnPlayerDeath()
    {
        if (--Lives > 0)
        {
            SpawnPlayer();
        }
        else
        {
            GameEnd();
        }
    }
    
    private void OnInvaderTouchBottom()
    {
        GameEnd();
    }

    public void StartNextLevel()
    {
        Globals.EntityManager.ClearEntities();
        SpawnPlayer();
        InitLevel();
    }
    
    public void InitContent()
    {
        _font = Globals.Content.Load<SpriteFont>("fonts/Pixeboy");
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        string score = $"Score: {Score}";
        string highscore = $"HighScore: {HighScore}";
        string level = $"Level: {Level}";
        string lives = $"Lives: {Lives}";

        var bounds = Globals.GetScreenRect();
        bounds.Y += 8;
        spriteBatch.Begin();
        spriteBatch.DrawString(_font, ref score, bounds, 
            ExtensionMethods.TextAlignment.Left | ExtensionMethods.TextAlignment.Top, Color.White);      
        spriteBatch.DrawString(_font, ref highscore, bounds, 
            ExtensionMethods.TextAlignment.Top | ExtensionMethods.TextAlignment.Right, Color.White);        
        spriteBatch.DrawString(_font, ref level, bounds, 
            ExtensionMethods.TextAlignment.Bottom | ExtensionMethods.TextAlignment.Right, Color.White);        
        spriteBatch.DrawString(_font, ref lives, bounds, 
            ExtensionMethods.TextAlignment.Left | ExtensionMethods.TextAlignment.Bottom, Color.White);
        spriteBatch.End();
    }

    private void OnInvaderDeath(Invader invader)
    {
        if (invader.UpperInvader != null)
        {
            invader.UpperInvader.CalcNextShotTime();
            invader.UpperInvader.LowerInvader = null;
        }
        
        int invaders = 0;
        foreach( var ent in Globals.EntityManager.Entities)
        {
            if (ent is not Invader)
            {
                continue;
            }
            invaders += 1;
            ent.Speed += 0.002f;
        }

        Score += 10;
        if( Score > HighScore )
        { 
            HighScore = Score;
        }

        if( invaders <= 1 )
        {
            Level += 1;
            OnLevelCompleted?.Invoke();
        }
    }

    private void InitLevel()
    {
        Globals.PrepareGame();
        GameOver = false;

        int invaderPosX = 70;
        int offset = ( (Globals.ScreenSize.Width - invaderPosX) / 11 ) - 2 ;
        Vector2 invaderPos = new Vector2( invaderPosX, 100 );

        Invader? upInvader;
        for( int i = 0; i < _invadersColumns; i++)
        {
            upInvader = null;
            for( int j = 0; j < _invadersRows; j++ )
            {
                var invader = (Invader)Globals.EntityManager.CreateEntityByName("Invader");
                invader.Position = invaderPos;
                invader.OnBottomTouched += OnInvaderTouchBottom;
                invader.Speed += (Level * 0.01f);
                invader.ShootDecrement += (Level * 0.01f);
                invader.ActiveTexture = j;
                invaderPos.Y += 50;
 
                if( upInvader != null )
                {
                    invader.UpperInvader = upInvader;
                    upInvader.LowerInvader = invader;
                }
                
                upInvader = invader;
            }
            invaderPos.Y = 100;
            invaderPos.X += offset;
        }
    }

    private void GameEnd()
    {
        GameOver = true;
        OnGameEnd?.Invoke();
        Globals.EntityManager.ClearEntities();
    }
}