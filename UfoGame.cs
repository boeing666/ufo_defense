using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace UfoDefense;

public class UfoGame : Game
{
    private GraphicsDeviceManager _graphics;
    private Logic _logic;
    private GameMenu _menu;
    private Physics _physics;
    private SpriteBatch _spriteBatch;
    private EntitiesManager _entityManager;
    private Texture2D _background;

    public UfoGame()
    {
        _graphics = new GraphicsDeviceManager(this);    
        _graphics.PreferredBackBufferWidth = 700;
        _graphics.PreferredBackBufferHeight = 900;
        _graphics.ApplyChanges();
        
        Content.RootDirectory = "Content";  
        Window.Title = "UFO DEFENSE";
    }

    protected override void Initialize()
    {
        // Задний фон игры
        _background = Content.Load<Texture2D>("images/background");
        
        // Создание всех объектов для игры
        _entityManager = new EntitiesManager();
        _logic = new Logic();
        _menu = new GameMenu();
        _physics = new Physics();
        
        // Сохранение ссылок на объекты в глобальном классе
        Globals.Init( Window, Content, GraphicsDevice, _entityManager, _logic);
        _logic.Init();

        // Слушаем события меню и логики игры
        _menu.OnStartGame += OnStartGame;
        _menu.OnResumeGame += OnResumeGame;
        _menu.OnPauseGame += OnPauseGame;
        _menu.OnExitGame += OnExitGame;
        
        _logic.OnGameEnd += OnGameEnd;
        _logic.OnLevelCompleted += OnLevelCompleted;
        
        Exiting += OnExiting;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _logic.InitContent();
        _menu.InitContent();
        _entityManager.InitContent();
    }
    
    /// <summary>
    /// Метод, вызывается при закрытии окна игры
    /// </summary>
    private void OnExiting(Object sender, EventArgs args)
    {
        // Сохраняем прогресс игры
        _logic.SaveProgress();
    }
    
    /// <summary>
    /// Метод, вызывается, когда начинается игра 
    /// </summary>
    private void OnStartGame()
    {
        // Прячем меню при старте игры
        _menu.Hide();
        // Если игра была на паузе, то снимаем ее и не вызываем старт логики игры
        if (Globals.IsPaused)
        {
            Globals.IsPaused = false;
            return;
        }
        _menu.GameLoosed = false;
        _logic.GameStart();
    }
    
    /// <summary>
    /// Метод, вызывается, когда игра приостанавливается
    /// </summary>
    private void OnPauseGame()
    {
        Globals.IsPaused = true;
        _menu.Show();
    }
    
    /// <summary>
    /// Метод, вызывается, когда игра закончилась
    /// </summary>
    private void OnGameEnd()
    {
        _menu.GameLoosed = true;
        _menu.Show();
    }
   
    /// <summary>
    /// Метод, вызывается, когда уровень пройден
    /// </summary>
    private void OnLevelCompleted()
    {
        _logic.StartNextLevel();
    }
    
    /// <summary>
    /// Метод, вызывается, когда игра снимается с паузы
    /// </summary>
    private void OnResumeGame()
    {
        Globals.IsPaused = false;
        _menu.Hide();
    }

    /// <summary>
    /// Метод, вызывается при нажатии кнопки выхода из меню игры
    /// </summary>
    private void OnExitGame()
    {
        _logic.SaveProgress();
        Exit();
    }
    
    protected override void Update(GameTime gameTime)
    {
        _menu.Update();
        // На паузе работает только меню
        if (Globals.IsPaused)
        {
            return;
        }

        // Свой счетчик времени, чтобы реализовать паузу
        Globals.Update(gameTime);
        _entityManager.Update();
        _physics.Update();      

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // Сначала рисуем ззадний фон, затем объекты на карте, и уже после этого меню и текста
        _spriteBatch.Begin();
        _spriteBatch.Draw(_background, new Vector2(0,0), Color.White);
        _spriteBatch.End();
        
        _entityManager.Draw(_spriteBatch);
        _logic.Draw(_spriteBatch);
        _menu.Draw(_spriteBatch);
        base.Draw(gameTime);
    }
}
