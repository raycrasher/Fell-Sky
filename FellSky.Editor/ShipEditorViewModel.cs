using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.Integration;
using FellSky.Graphics;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace FellSky.Editor
{
    [PropertyChanged.ImplementPropertyChanged]
    public class ShipEditorViewModel
    {
        [PropertyChanged.ImplementPropertyChanged]
        public class SpriteSheet
        {
            public SpriteManager.JsonSpriteSheet SpriteDefinitions { get; set; }
            public System.Windows.Media.Imaging.BitmapImage Image { get; set; }
        }

        public ShipEditorRenderer Renderer { get; set; }
        public Thread RendererThread { get; private set; }
        public Dictionary<string, Graphics.Sprite> Sprites { get; set; }
        public List<SpriteSheet> SpriteSheets { get; } = new List<SpriteSheet>();
        public SpriteSheet CurrentSpriteSheet { get; set; }

        public Dictionary<string, SpriteManager.JsonSprite[]> HullSprites { get; set; }

        public void InitializeRenderer(System.Windows.Forms.Control host)
        {
            Renderer = new ShipEditorRenderer(host);
            RendererThread = new Thread(Renderer.Run);
            Environment.CurrentDirectory = Path.GetFullPath( Properties.Settings.Default.DataFolder);
            Renderer.Content.RootDirectory= Environment.CurrentDirectory;

            Renderer.Load += () => AddSpriteSheet("textures/hulls.json");
            RendererThread.Start();
            Renderer.LoadEvent.WaitOne();
            CurrentSpriteSheet = SpriteSheets[0];
            
            HullSprites = SpriteSheets[0].SpriteDefinitions.sprites.Where(s => s.type == "hull").GroupBy(s => s.subtype).ToDictionary(s => s.Key, s => s.ToArray());
        }
        
        private void AddSpriteSheet(string sheetfile)
        {
            var sheet = new SpriteSheet();
            sheet.SpriteDefinitions = Graphics.SpriteManager.AddSpriteSheetFromFile(Renderer.Content, sheetfile);
            sheet.Image = TextureToImage(Renderer.Content.Load<Texture2D>(sheet.SpriteDefinitions.texture));
            SpriteSheets.Add(sheet);

        }

        private BitmapImage TextureToImage(Texture2D tex)
        {
            var image = new System.Windows.Media.Imaging.BitmapImage();
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
    }

    public class SpriteToIntRectConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sprite = value as SpriteManager.JsonSprite;
            if (sprite == null) return null;
            return new System.Windows.Int32Rect(sprite.x, sprite.y, sprite.w, sprite.h);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
