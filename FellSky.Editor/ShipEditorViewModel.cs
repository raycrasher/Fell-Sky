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

        public Dictionary<string, Graphics.Sprite> Sprites { get; set; }
        public SpriteSheet CurrentSpriteSheet { get; set; }

        public Dictionary<string, List<JsonSprite>> HullSprites { get; set; }
        public object PropertyObject { get; set; }

        public XnaColor DefaultColor { get; set; } = XnaColor.White;
        public XnaColor TrimColor { get; set; } = XnaColor.CornflowerBlue;
        public XnaColor BaseColor { get; set; } = XnaColor.Gold;
        public XnaColor BackgroundColor { get; set; } = new XnaColor(5, 10, 20);
        public XnaColor GridColor { get; set; } = new XnaColor(30, 40, 50);

        public Ship Ship { get; set; }
        public ContentManager Content { get; set; }
        public GameServiceContainer Services { get; set; }
        public EntityWorld World { get; set; }

        public List<Entity> SelectedPartEntities { get; set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public ICommand AddHull { get { return new DelegateCommand(o => AddHullToShip((JsonSprite)o)); } }

        public Entity ShipEntity { get; private set; }
        public Entity CameraEntity { get; private set; }
        public Entity GridEntity { get; private set; }
        public Entity TransformEntity { get; private set; }

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

            CreateNewShip();            

            _transformSystem = World.SystemManager.GetSystem<MouseControlledTransformSystem>();
            SelectedPartEntities = World.SystemManager.GetSystem<BoundingBoxSelectionSystem>().SelectedEntities;
            host.KeyUp += HandleKeyboardInput;
        }

        private void HandleKeyboardInput(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.R:
                    TransformSelectedParts();
                    _transformSystem.Mode = MouseControlledTransformMode.Rotate;
                    break;
                case Key.T:
                    TransformSelectedParts();
                    _transformSystem.Mode = MouseControlledTransformMode.Translate;
                    break;
                case Key.S:
                    //TransformSelectedItems();
                    _transformSystem.Mode = MouseControlledTransformMode.Scale;
                    break;
                case Key.Delete:
                    DeleteSelectedParts();
                    break;
                case Key.Up:
                    TranslateSelectedParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10));
                    break;
                case Key.Down:
                    TranslateSelectedParts(new Vector2(0, Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10));
                    break;
                case Key.Left:
                    TranslateSelectedParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? -1 : -10, 0));
                    break;
                case Key.Right:
                    TranslateSelectedParts(new Vector2(Keyboard.IsKeyDown(Key.LeftShift) ? 1 : 10, 0));
                    break;
            }
        }

        private void TransformSelectedParts()
        {
            foreach(var entity in SelectedPartEntities)
            {
                if (!entity.HasComponent<MouseControlledTransform>()) {
                    entity.AddComponent(new MouseControlledTransform());
                    entity.Refresh();
                }
            }
        }

        private void TranslateSelectedParts(Vector2 offset)
        {
            foreach (var xform in SelectedPartEntities.Select(e => e.GetComponent<Transform>()).Where(t => t != null))
            {
                xform.Position += offset;
            }
        }

        private void DeleteSelectedParts()
        {
            foreach(var part in SelectedPartEntities.Select(e => e.Components.OfType<ShipPart>().First()))
            {
                Ship.RemovePart(part);
            }
        }

        private void OnMouseButtonDown(Microsoft.Xna.Framework.Point arg1, int arg2)
        {
            ActionsNextFrame.Add(ClearSelection);
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



        private void AddHullToShip(JsonSprite sprite)
        {
            ClearSelection();

            var pos = _host.PointToScreen(new System.Windows.Point(_host.ActualWidth / 2, _host.ActualHeight / 2));
            _mouse.ScreenPosition = new Vector2((float)pos.X, (float)pos.Y);
            _transformSystem.Mode = MouseControlledTransformMode.Translate;
            _transformSystem.Origin = Vector2.Zero;
            var hull = new Hull(sprite.Id, Vector2.Zero, 0, Vector2.One, new Vector2(sprite.OriginX ?? sprite.W/2, sprite.OriginY ?? sprite.H/2), XnaColor.White);
            Ship.Hulls.Add(hull);
            var entity = World.CreateEntity();
            entity.AddComponent(hull);
            
            entity.AddComponent(hull.Transform);
            entity.AddComponent(new MouseControlledTransform());
            entity.AddComponent(new ChildEntity(ShipEntity));

            var select = new BoundingBoxSelector(hull.BoundingBox) { IsEnabled = false };
            entity.AddComponent(select);

            var bb = hull.BoundingBox;
            bb.Inflate(2,2);
            
            var drawbounds = new DrawBoundingBoxComponent(bb);
            entity.AddComponent(drawbounds);
            select.SelectedChanged += (s, e) => drawbounds.IsEnabled = select.IsSelected;

            entity.Refresh();
            SelectedPartEntities.Add(entity);
        }

        private void ClearSelection()
        {
            foreach (var entity in SelectedPartEntities)
            {
                entity.GetComponent<BoundingBoxSelector>().IsSelected = false;
                entity.GetComponent<BoundingBoxSelector>().IsEnabled = true;
                entity.GetComponent<DrawBoundingBoxComponent>().IsEnabled = false;
                entity.RemoveComponent<MouseControlledTransform>();
                entity.Refresh();
            }
            SelectedPartEntities.Clear();
        }

        public void CreateNewShip()
        {
            Ship = new Ship();
            ShipEntity = World.CreateEntity();
            ShipEntity.AddComponent(Ship);
            ShipEntity.AddComponent(new ShipSpriteComponent(Ship));
            ShipEntity.AddComponent(new Transform());
            ShipEntity.Refresh();
            
            Artemis.System.EntitySystem.BlackBoard.SetEntry("PlayerShip", Ship);
            Artemis.System.EntitySystem.BlackBoard.SetEntry("PlayerShipEntity", ShipEntity);
            PropertyObject = Ship;
        }

        public ICommand Quit => new DelegateCommand(o => Application.Current.Shutdown());
    }
}
