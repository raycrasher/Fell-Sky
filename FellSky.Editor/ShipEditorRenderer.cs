using Artemis;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using FellSky.Graphics;
using FellSky.Mechanics.Ships;

namespace FellSky.Editor
{
    public class ShipEditorRenderer: Microsoft.Xna.Framework.Game
    {
        private IntPtr _deviceWindowHandle;

        public GraphicsDeviceManager GraphicsManager { get; private set; }
        public Point Size { get; set; }
        public volatile bool QuitFlag = false;
        private bool _hidePhantomWindow=false;
        private System.Windows.Forms.Control _host;
        private volatile bool _hasResized=false;

        public readonly AutoResetEvent LoadEvent = new AutoResetEvent(false);
        private Entity _cameraEntity;
        private Entity _shipEntity;

        public event Action Load;
        public EntityWorld World { get; set; }
        public ConcurrentBag<Action> ExecuteNextUpdate { get; } = new ConcurrentBag<Action>();
        public Camera2D Camera { get; private set; }
        public Ship Ship { get; private set; }

        public ShipEditorRenderer(System.Windows.Forms.Control host)
        {
            _host = host;
            GraphicsManager = new GraphicsDeviceManager(this);
            _deviceWindowHandle = host.Handle;
            GraphicsManager.PreparingDeviceSettings += PrepareDeviceSettings;
            GraphicsManager.IsFullScreen = false;
            GraphicsManager.PreferredDepthStencilFormat = Microsoft.Xna.Framework.Graphics.DepthFormat.None;
            Camera = new Graphics.Camera2D();
            ApplyGraphicsChanges();
            this.IsMouseVisible = true;
            host.Resize += Host_Resize;
        }

        private void Host_Resize(object sender, EventArgs e)
        {
            _hasResized = true;
        }

        public void ApplyGraphicsChanges()
        {
            Size = new Point(_host.Width, _host.Height);
            GraphicsManager.PreferredBackBufferWidth = Size.X;
            GraphicsManager.PreferredBackBufferHeight = Size.Y;
            Camera.ScreenSize = new Vector2(_host.Width, _host.Height);
            GraphicsManager.ApplyChanges();
            _hasResized = false;
        }

        protected override void LoadContent()
        {
            Artemis.System.EntitySystem.BlackBoard.SetEntry(Camera2D.PlayerCameraName, Camera);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("GraphicsDevice", GraphicsDevice);
            World = new EntityWorld(false,true,false);
            //World.SystemManager.SetSystem(new Systems.ShipRendererSystem(), Artemis.Manager.GameLoopType.Draw, 10, Artemis.Manager.ExecutionType.Synchronous);
            World.InitializeAll(typeof(FellSky.Game).Assembly, GetType().Assembly);
            _cameraEntity = World.CreateEntity();
            _cameraEntity.AddComponent(Camera);
            _cameraEntity.AddComponentFromPool<Transform>();
            _cameraEntity.Refresh();

            CreateNewShip();

            HidePhantomWindow();
            
            base.LoadContent();
            Load?.Invoke();
            LoadEvent.Set();
        }

        private void CreateNewShip()
        {
            Ship = new Ship();
            _shipEntity = World.CreateEntity();
            _shipEntity.AddComponent(Ship);
            _shipEntity.AddComponent(new FellSky.EntityComponents.ShipSpriteComponent(Ship));
            _shipEntity.AddComponentFromPool<Transform>();
            _shipEntity.Refresh();
        }

        protected override void Update(GameTime gameTime)
        {
            while (!ExecuteNextUpdate.IsEmpty)
            {
                Action a;
                ExecuteNextUpdate.TryTake(out a);
                a();
            }
            
            if(!_hidePhantomWindow) HidePhantomWindow();
            if (_hasResized) ApplyGraphicsChanges();

            if (QuitFlag) Exit();
            base.Update(gameTime);
        }

        internal void AddHullToShip(SpriteManager.JsonSprite o)
        {
            Ship.Hulls.Add(new Hull(o.id, Vector2.Zero, 0, Vector2.One, new Vector2(o.origin_x,o.origin_y), Color.White));
        }

        private void HidePhantomWindow()
        {
            var form = (System.Windows.Forms.Form) System.Windows.Forms.Form.FromHandle(Window.Handle);
            form.Opacity = 0;
            form.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            form.ShowInTaskbar = false;
            _hidePhantomWindow = true;
            World.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.FromNonPremultiplied(0,30,60,255));
            World.Draw();
            base.Draw(gameTime);
        }

        private void PrepareDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = _deviceWindowHandle;
        }
    }
}
