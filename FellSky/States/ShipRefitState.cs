using FellSky.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibRocketNet;
using Microsoft.Xna.Framework;
using Artemis;

namespace FellSky.States
{
    public class ShipRefitState: GameState
    {
        public enum EditorMode
        {
            Weapons, Modules, Armor, Build
        }

        public const string RefitScreenGuiDocument = "Gui/RefitScreen.xml";

        private readonly GuiService GuiService;
        private Element MenuItem_Armor;
        private Element MenuItem_Modules;
        private Element MenuItem_Weapons;
        private Element MenuItem_Build;

        public EditorMode Mode { get; private set; }

        public ElementDocument Document { get; private set; }
        public static ShipRefitState Instance { get; private set; }
        public EntityWorld World { get; private set; }

        public ShipRefitState()
        {
            GuiService = ServiceLocator.Instance.GetService<GuiService>();
        }

        public override void LoadContent()
        {
            World = new EntityWorld();
            Instance = this;
            if(Document == null)
            {
                Document = GuiService.LoadDocument(RefitScreenGuiDocument);
                Core.ScriptEvent += (o,e) => Instance?.HandleScriptEvent(o, e);
            }
            
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            Instance = null;
        }

        public override void Update(GameTime gameTime)
        {
            World.Update();
        }

        public override void Draw(GameTime gameTime)
        {
            World.Draw();
        }

        private void SetMode(EditorMode mode)
        {
            if (mode == Mode) return;
            Mode = mode;
            Document.GetElementById("availablePartsList")?.SetProperty("display", "none");
        }

        public void HandleScriptEvent(object sender, ScriptEventArgs args)
        {
            switch (args.Script)
            {
                case "Refit_ChangeMode_Weapons":
                    SetMode(EditorMode.Weapons);
                    break;
                case "Refit_ChangeMode_Armor":
                    SetMode(EditorMode.Armor);
                    break;
                case "Refit_ChangeMode_Modules":
                    SetMode(EditorMode.Modules);
                    break;
                case "Refit_ChangeMode_Build":
                    SetMode(EditorMode.Build);
                    break;
            }
            
        }
    }
}
