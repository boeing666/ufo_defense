using System;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace UfoDefense;

public enum ItemType
{
    None = 0,
    StartGame,
    RestartGame,
    ResumeGame,
    Exit
}

public class MenuItem
{
    public ItemType Type { get; }
    public string Text { get; }

    private Action _callback;
    public MenuItem(ItemType type, string text, Action callback)
    {
        this.Type = type;
        this.Text = text;
        this._callback = callback;
    }

    public void Invoke() => _callback();
}

public class GameMenu
{
    private int _activeItemIndex;
    private MenuItem[] _activeMenu;
    
    private MenuItem[] _allMenuItems;
    private MenuItem[] _mainMenu;

    private SpriteFont _fontGameName;
    private SpriteFont _fontMenu;
    
    private KeyboardState _lastKeyboardState;
    private bool _draw;
    
    public bool GameLoosed;

    public Action OnStartGame;
    public Action OnResumeGame;
    public Action OnPauseGame;
    public Action OnExitGame;

    public GameMenu()
    {
        _allMenuItems = new MenuItem[]
        {
            new MenuItem(ItemType.StartGame, "Start Game", () => OnStartGame?.Invoke()),
            new MenuItem(ItemType.ResumeGame, "Resume Game", () => OnResumeGame?.Invoke()),
            new MenuItem(ItemType.Exit, "Exit", () => OnExitGame?.Invoke()),
        };

        BuildMenu(ref _mainMenu, ItemType.StartGame, ItemType.Exit);

        _activeMenu = _mainMenu;
        GameLoosed = false;
        _draw = true;
    }

    public void InitContent()
    {
        _fontMenu = Globals.Content.Load<SpriteFont>("fonts/PixeboyMenuItems");
        _fontGameName = Globals.Content.Load<SpriteFont>("fonts/PixeboyMenu");
    }

    private void BuildMenu(ref MenuItem[] array, params ItemType[] numbers)
    {
        array = new MenuItem[numbers.Length];
        for (int i = 0; i < numbers.Length; i++)
        {
            array[i] = GetMenuItem(numbers[i]);
        }
    }

    public void Update()
    {
        KeyboardState state = Keyboard.GetState();

        if (state.IsButtonOncePressed(_lastKeyboardState,Keys.Escape))
        {
            OnPauseGame?.Invoke();
        }
        
        if (!_draw)
        {
            _lastKeyboardState = state;
            return;   
        }
        
        if (state.IsButtonOncePressed(_lastKeyboardState,Keys.W))
        {
            _activeItemIndex = NormalizeIndex(_activeItemIndex - 1, _activeMenu.Length);
        }

        if (state.IsButtonOncePressed(_lastKeyboardState, Keys.S))
        {
            _activeItemIndex = NormalizeIndex(_activeItemIndex + 1, _activeMenu.Length);
        }

        if (state.IsButtonOncePressed(_lastKeyboardState, Keys.Space))
        {
            _activeMenu[_activeItemIndex].Invoke();
        }

        _lastKeyboardState = state;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        if (!_draw)
        {
            return;       
        }

        var title = Globals.Window.Title;
        
        var bounds = Globals.GetScreenRect();
        spriteBatch.Begin();
        
        bounds.Y = 250;
        spriteBatch.DrawString(_fontGameName, ref title, bounds, 
            ExtensionMethods.TextAlignment.Center | ExtensionMethods.TextAlignment.Top, Color.White);
        bounds.Y = 0;

        if (GameLoosed || Globals.IsPaused)
        {
            bounds.Y = 330;
            string text = "You lose";
            if (Globals.IsPaused) {
                text = "Game paused";
            }
            spriteBatch.DrawString(_fontGameName, ref text, bounds, 
                ExtensionMethods.TextAlignment.Center | ExtensionMethods.TextAlignment.Top, Color.White);
            bounds.Y = 0;     
        }

        for (int i = 0; i < _activeMenu.Length; i++)
        {
            MenuItem item = _activeMenu[i];

            string text = item.Text;
            if (_activeItemIndex == i)
            {
                text = $">{text}";
            }
            spriteBatch.DrawString(_fontMenu, ref text, bounds, ExtensionMethods.TextAlignment.Center, Color.White);
            bounds.Y += 60;
        }
        spriteBatch.End();
    }

    public void Hide()
    {
        _draw = false;
    }
    
    public void Show()
    {
        _draw = true;
    }
    
    private int NormalizeIndex(int index, int maxindex)
    {
        int r = index % maxindex;
        return r < 0 ? r + maxindex : r;
    }
    private MenuItem GetMenuItem(ItemType type)
    {
        return _allMenuItems.FirstOrDefault( item => item.Type == type );
    }
}