using Artemis;
using Microsoft.Xna.Framework;
using PropertyChanged;
using System;
using FellSky.Systems;
using FellSky.EntityFactories;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FellSky.Services;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using FellSky.Framework;
using Microsoft.Xna.Framework.Content;
using FellSky.Components;

namespace FellSky.Editor
{
    [ImplementPropertyChanged]
    class ParticleEditorViewModel
    {
        public bool IsContinuousMode { get; set; }
        public ParticleEmitterComponent Emitter { get; set; }

        private D3D11Host Host = null;
        private EntityWorld World;
        private GameServiceContainer Services;
        private MouseService _mouse;
        private KeyboardService _keyboard;
        private TimerService _timer;
        private GameTime _gameTime = new GameTime();
        private SpriteBatch SpriteBatch;
        private ContentManager Content;
        private SpriteManagerService SpriteManager;

        internal void Render(TimeSpan obj)
        {
            Host.GraphicsDevice.Clear(Color.Black);
            World.Update(obj.Milliseconds);
            World.Draw();
        }

        internal void Initialize(D3D11Host host)
        {
            Services = new GameServiceContainer();

            ServiceLocator.Initialize(Services);

            Artemis.System.EntitySystem.BlackBoard.SetEntry("GraphicsDevice", host.GraphicsDevice);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("ServiceProvider", Services);

            Services.AddService<IGraphicsDeviceService>(host);
            Services.AddService(host.GraphicsDevice);

            _mouse = new MouseService(host);
            Services.AddService<IMouseService>(_mouse);
            _mouse.ButtonDown += OnMouseButtonDown;
            _keyboard = new KeyboardService(host);
            Services.AddService<IKeyboardService>(_keyboard);
            _timer = new TimerService();
            Services.AddService<ITimerService>(_timer);
            Services.AddService(new FastSpriteBatch(host.GraphicsDevice));

            _timer.LastFrameUpdateTime = _gameTime;
            _timer.LastFrameRenderTime = _gameTime;

            SpriteBatch = new SpriteBatch(host.GraphicsDevice);
            Services.AddService(SpriteBatch);

            Content = new ContentManager(Services);
            Content.RootDirectory = Environment.CurrentDirectory;

            SpriteManager = new SpriteManagerService(Content);
            Services.AddService<ISpriteManagerService>(SpriteManager);

            SpriteManager.LoadSpriteSheet("Textures/Hulls.json");

            Host = host;
            World = new EntityWorld(false, false, false);
            int drawDepth = 0;
            World.CreateComponentPool<Transform>(200, 200);
            World.SystemManager.SetSystem(new GridRendererSystem(), Artemis.Manager.GameLoopType.Draw, drawDepth++);
            World.InitializeAll();
            World.CreateCamera(Constants.ActiveCameraTag, Host.GraphicsDevice);
            Emitter = new ParticleEmitterComponent();
        }

        private void OnMouseButtonDown(Point arg1, int arg2)
        {
            
        }
    }
}
