using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using Microsoft.Xna.Framework.Content;
using Artemis;
using XnaColor = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework;
using FellSky.Framework;
using System.Windows;
using FellSky.Components;
using FellSky.Systems;
using FellSky.Services;
using FellSky.EntityFactories;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class ShipEditorViewModel
    {
        private D3D11Host _host;
        public const string CameraTag = "EditorCamera";

        [PropertyChanged.ImplementPropertyChanged]
        public class SpriteSheet
        {
            public Framework.SpriteSheet SpriteDefinitions { get; set; }
            public BitmapImage Image { get; set; }
        }

        public CameraComponent Camera { get; set; }

        public Dictionary<string, Sprite> Sprites { get; set; }
        public SpriteSheet CurrentSpriteSheet { get; set; }

        public Dictionary<string, List<Sprite>> HullSprites { get; set; }
        public object PropertyObject { get; set; }

        public XnaColor BackgroundColor { get; set; } = new XnaColor(5, 10, 20);
        public XnaColor GridColor { get; set; } = new XnaColor(30, 40, 50);
        
        public ShipEditorService EditorService { get; private set; }

        public ContentManager Content { get; set; }
        public GameServiceContainer Services { get; set; }
        public EntityWorld World { get; set; }

        //public List<Entity> SelectedPartEntities { get; set; }
        
        public SpriteBatch SpriteBatch { get; private set; }
        public bool IsSnap { get; set; }
        public bool IsGridVisible { get; set; }
        public int GridSize {
            get { return ((int?) GridEntity?.GetComponent<GridComponent>()?.GridSize.X) ?? 10;  }
            set
            {
                var grid = GridEntity?.GetComponent<GridComponent>();
                if (grid == null) return;
                grid.GridSize = new Vector2(value, value);
            }
        }
        
        public Entity CameraEntity { get; private set; }
        public Entity GridEntity { get; private set; }
        
        private MouseService _mouse;
        private MouseControlledTransformSystem _transformSystem;

        private List<Action> ActionsNextFrame { get; } = new List<Action>();

        internal void Initialize(D3D11Host host)
        {
            Services = new GameServiceContainer();

            _host = host;

            Artemis.System.EntitySystem.BlackBoard.SetEntry("GraphicsDevice", _host.GraphicsDevice);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("ServiceProvider", Services);
           
            Environment.CurrentDirectory = Path.GetFullPath(Properties.Settings.Default.DataFolder);
            
            Services.AddService<IGraphicsDeviceService>(host);
            Services.AddService(host.GraphicsDevice);

            _mouse = new MouseService(host);
            Services.AddService<IMouseService>(_mouse);
            _mouse.ButtonDown += OnMouseButtonDown;

            SpriteBatch = new SpriteBatch(host.GraphicsDevice);
            Services.AddService(SpriteBatch);

            SpriteManager = new SpriteManagerService();
            Services.AddService<ISpriteManagerService>(SpriteManager);


            Content = new ContentManager(Services);
            Content.RootDirectory = Environment.CurrentDirectory;
            LoadHullSprites("textures/hulls.json");

            Artemis.System.EntitySystem.BlackBoard.SetEntry("ContentManager", Content);

            World = new EntityWorld(false,false, false);
                
            World.SystemManager.SetSystem(new GridRendererSystem(SpriteBatch, CameraTag), Artemis.Manager.GameLoopType.Draw, 1);
            World.SystemManager.SetSystem(new ShipRendererSystem(SpriteBatch, CameraTag), Artemis.Manager.GameLoopType.Draw, 2);
            World.SystemManager.SetSystem(new BoundingBoxRendererSystem(SpriteBatch, CameraTag), Artemis.Manager.GameLoopType.Draw, 3);
            World.SystemManager.SetSystem(new GenericDrawableRendererSystem(SpriteBatch, host.GraphicsDevice, CameraTag), Artemis.Manager.GameLoopType.Draw, 4);

            _transformSystem = new MouseControlledTransformSystem(_mouse, CameraTag);
            World.SystemManager.SetSystem(_transformSystem, Artemis.Manager.GameLoopType.Update, 1);
            World.SystemManager.SetSystem(new ShipUpdateSystem(), Artemis.Manager.GameLoopType.Update, 2);
            World.SystemManager.SetSystem(new BoundingBoxSelectionSystem(_mouse, CameraTag), Artemis.Manager.GameLoopType.Update, 3);
            World.SystemManager.SetSystem(new CameraUiDraggingSystem(CameraTag, _mouse), Artemis.Manager.GameLoopType.Update, 4);

            World.InitializeAll();

            Services.AddService(new GenericDrawableFactory(World));
            Services.AddService(new ShipEntityFactory(World, SpriteManager, null));
            Services.AddService(new CameraEntityFactory(World));
            Services.AddService(new GridEntityFactory(World));
            

            CameraEntity = Services.GetService<CameraEntityFactory>().CreateCamera();
            CameraEntity.Tag = CameraTag;
            Camera = CameraEntity.GetComponent<CameraComponent>();

            GridEntity = Services.GetService<GridEntityFactory>().CreateGrid(new Vector2(50,50), GridColor);

            Services.GetService<GenericDrawableFactory>().CreateCircle(Vector2.Zero, 10, 8, XnaColor.Red);
            
            host.KeyUp += HandleKeyboardInput;

            EditorService = new ShipEditorService(Services, World, CameraTag);
            CreateNewShipCommand.Execute(null);
        }

        private void HandleKeyboardInput(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.R:
                    EditorService.RotateParts();
                    break;
                case Key.T:
                    EditorService.TranslateParts();
                    break;
                case Key.S:
                    EditorService.ScaleParts();
                    break;
                case Key.Delete:
                    EditorService.DeleteParts();
                    break;
                case Key.Up:
                    EditorService.OffsetParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10));
                    break;
                case Key.Down:
                    EditorService.OffsetParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10));
                    break;
                case Key.Left:
                    EditorService.OffsetParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10, 0));
                    break;
                case Key.Right:
                    EditorService.OffsetParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10, 0));
                    break;
                case Key.M:
                    EditorService.MirrorLateralOnSelected();
                    break;
            }
        }
        
        private void OnMouseButtonDown(Microsoft.Xna.Framework.Point pos, int button)
        {
            ActionsNextFrame.Add(EditorService.ClearSelection);
            if (button == 2) _transformSystem.CancelTransform();
            else if (button == 0) _transformSystem.ApplyTransform();
        }

        internal void Render(TimeSpan timespan)
        {
            _mouse.Update();

            foreach (var a in ActionsNextFrame) a();
            ActionsNextFrame.Clear();

            Camera.ScreenSize = new Vector2((float)_host.ActualWidth, (float)_host.ActualHeight);
            _host.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
            _host.GraphicsDevice.Clear(BackgroundColor);
            World.Update();
            World.Draw();
        }
              
        private void LoadHullSprites(string sheetfile)
        {
            var sheet = new SpriteSheet();
            sheet.SpriteDefinitions = SpriteManager.AddSpriteSheetFromFile(Content, sheetfile);
            CurrentSpriteSheet = sheet;
            HullSprites = CurrentSpriteSheet.SpriteDefinitions.Sprites
                .Where(s => s.Type == "hull")
                .GroupBy(s => s.Subtype)
                .ToDictionary(s => s.Key, s => s.ToList());
            sheet.Image = TextureToImage(Content.Load<Texture2D>(sheet.SpriteDefinitions.Texture));            
        }

        private BitmapImage TextureToImage(Texture2D tex)
        {
            var image = new BitmapImage();
            using(var stream=new MemoryStream())
            {
                tex.SaveAsPng(stream, tex.Width, tex.Height);
                image.BeginInit();
                image.StreamSource = stream;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();
                image.Freeze();
            }
            
            return image;
        }

        public ICommand CreateNewShipCommand => new DelegateCommand(o =>
        {
            EditorService.CreateNewShip();
            PropertyObject = EditorService.Ship;
        });
        public ICommand Quit => new DelegateCommand(o => Application.Current.Shutdown());
        public ICommand AddHull => new DelegateCommand(o =>
        {
            ActionsNextFrame.Add(() => {
                var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
                _mouse.ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
                EditorService.AddHull((Sprite)o);
            });
        });

        public ICommand DeletePartsCommand => new DelegateCommand(o => EditorService.DeleteParts());
        public ICommand MirrorLateralCommand => new DelegateCommand(o => EditorService.MirrorLateralOnSelected());
        public ICommand RotatePartsCommand => new DelegateCommand(o => EditorService.RotateParts());
        public ICommand SaveShipCommand => new DelegateCommand(o => {
            ActionsNextFrame.Add(() =>
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    AddExtension = true,
                    DefaultExt = ".ship.json"
                };
                if (dialog.ShowDialog() == true)
                    EditorService.SaveShip(dialog.FileName);
            });
        });
        public ICommand LoadShipCommand => new DelegateCommand(o => {
            ActionsNextFrame.Add(() =>
            {
                var dialog = new Microsoft.Win32.OpenFileDialog
                {
                    AddExtension = true,
                    DefaultExt = ".ship.json"
                };
                if (dialog.ShowDialog() == true)
                {
                    EditorService.LoadShip(dialog.FileName);
                }
                PropertyObject = EditorService.Ship;
            });
        });

        

        public SpriteManagerService SpriteManager { get; private set; }
    }
}
