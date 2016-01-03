using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FellSky.Graphics;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.Windows.Input;
using FellSky.Ships;
using Microsoft.Xna.Framework.Content;
using Artemis;
using XnaColor = Microsoft.Xna.Framework.Color;
using WpfColor = System.Windows.Media.Color;
using Microsoft.Xna.Framework;
using FellSky.Ships.Parts;
using FellSky.Framework;
using System.Windows;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class ShipEditorViewModel
    {
        private D3D11Host _host;

        [PropertyChanged.ImplementPropertyChanged]
        public class SpriteSheet
        {
            public JsonSpriteSheet SpriteDefinitions { get; set; }
            public BitmapImage Image { get; set; }
        }

        public Camera2D Camera { get; set; }

        public Dictionary<string, Sprite> Sprites { get; set; }
        public SpriteSheet CurrentSpriteSheet { get; set; }

        public Dictionary<string, List<JsonSprite>> HullSprites { get; set; }
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
            _mouse = new MouseService(host);

            _mouse.ButtonDown += OnMouseButtonDown;

            Artemis.System.EntitySystem.BlackBoard.SetEntry("GraphicsDevice", _host.GraphicsDevice);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("ServiceProvider", Services);
           
            Environment.CurrentDirectory = Path.GetFullPath(Properties.Settings.Default.DataFolder);
            
            Services.AddService<IGraphicsDeviceService>(host);
            Services.AddService(host.GraphicsDevice);
            Services.AddService<Framework.IMouseService>(_mouse);
            SpriteBatch = new SpriteBatch(host.GraphicsDevice);
            Services.AddService(SpriteBatch);

            Content = new ContentManager(Services);
            Content.RootDirectory = Environment.CurrentDirectory;
            LoadHullSprites("textures/hulls.json");

            Artemis.System.EntitySystem.BlackBoard.SetEntry("ContentManager", Content);

            World = new EntityWorld(false, true, false);
            World.InitializeAll(typeof(FellSky.Game).Assembly, GetType().Assembly);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("World", World);

            CameraEntity = World.CreateEntityFromTemplate("Camera");
            Camera = CameraEntity.GetComponent<Camera2D>();

            Artemis.System.EntitySystem.BlackBoard.SetEntry(Camera2D.PlayerCameraName, Camera);
            GridEntity = World.CreateEntityFromTemplate("Grid", new Vector2(50,50), GridColor);

            World.CreateEntityFromTemplate("GenericDrawable", Entities.GenericDrawableTemplate.Circle(Vector2.Zero, 10, 10, XnaColor.Red));

            _transformSystem = World.SystemManager.GetSystem<MouseControlledTransformSystem>();
            
            host.KeyUp += HandleKeyboardInput;

            EditorService = new ShipEditorService(_mouse, World);
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



        private void OnMouseButtonDown(Microsoft.Xna.Framework.Point arg1, int arg2)
        {
            ActionsNextFrame.Add(EditorService.ClearSelection);
            _transformSystem.Mode = null;
        }

        internal void Render(TimeSpan timespan)
        {
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
            sheet.SpriteDefinitions = Graphics.SpriteManager.AddSpriteSheetFromFile(Content, sheetfile);
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
            var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
            _mouse.ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
            EditorService.AddHull((JsonSprite)o);
        });

        public ICommand DeletePartsCommand => new DelegateCommand(o => EditorService.DeleteParts());
        public ICommand MirrorLateralCommand => new DelegateCommand(o => EditorService.MirrorLateralOnSelected());
        public ICommand RotatePartsCommand => new DelegateCommand(o => EditorService.RotateParts());
    }
}
