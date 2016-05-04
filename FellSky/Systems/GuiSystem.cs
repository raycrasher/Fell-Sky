using FellSky.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Artemis;
using FellSky.Services;
using LibRocketNet;

namespace FellSky.Systems
{
    class GuiSystem : Artemis.System.EntitySystem
    {
        const string PreviousDisplayPropertyAttributeName = "__previousDisplayProp";
        private IGuiService _service;
        private ElementDocument _document;

        public GuiSystem(IGuiService service): base(Aspect.All(typeof(GuiComponent)))
        {
            _service = service;
        }

        public override void LoadContent()
        {
            _document = _service.Context.LoadDocument(Properties.Settings.Default.GuiDocument);
            _document.Show();
            base.LoadContent();
        }

        public override void UnloadContent()
        {
            _document.Close();
            _document.Dispose();
            base.UnloadContent();
        }

        public override void OnAdded(Entity entity)
        {
            var component = entity.GetComponent<GuiComponent>();
            if (component.Element == null) return;
            if(component.Element.ParentNode == null)
            {
                _document.AppendChild(component.Element);
            }
            base.OnAdded(entity);
        }

        public override void OnRemoved(Entity entity)
        {
            var component = entity.GetComponent<GuiComponent>();
            if (component.Element == null) return;
            component.Element.ParentNode.RemoveChild(component.Element);
            base.OnRemoved(entity);
        }

        public override void OnDisabled(Entity entity)
        {
            var component = entity.GetComponent<GuiComponent>();
            if (component.Element == null) return;
            component.Element.SetAttribute(PreviousDisplayPropertyAttributeName, component.Element.GetPropertyString("display"));
            component.Element.SetProperty("display", "none");
            
            base.OnDisabled(entity);
        }

        public override void OnEnabled(Entity entity)
        {
            var component = entity.GetComponent<GuiComponent>();
            if (component.Element == null) return;
            var attr = component.Element.GetAttributeString(PreviousDisplayPropertyAttributeName, "block");
            component.Element.SetProperty("display", attr);
            base.OnEnabled(entity);
        }
    }
}
