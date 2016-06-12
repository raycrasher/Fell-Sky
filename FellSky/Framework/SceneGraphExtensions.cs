using Artemis;
using FellSky.Components;
using FellSky.Systems.SceneGraphRenderers;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FellSky
{
    public static class SceneGraphExtensions
    {
        public static void AddChild(this Entity parent, Entity child, int? index = null)
        {
            if (parent == null) throw new ArgumentException(nameof(parent));
            if (child == null) throw new ArgumentNullException(nameof(child));

            var parentComponent = parent.GetComponent<SceneGraphComponent>();
            if (parentComponent == null)
            {
                parentComponent = new SceneGraphComponent();
            }


            var childComponent = child.GetComponent<SceneGraphComponent>();
            if (childComponent == null)
            {
                childComponent = new SceneGraphComponent();
            }

            if (childComponent.Parent != null)
                throw new InvalidOperationException("Cannot attach a scene graph with a parent.");

            parent.AddComponent(parentComponent);
            if (index != null)
            {
                parentComponent.Children.Insert(index.Value, child);
            }
            else {
                parentComponent.Children.Add(child);
            }
            child.AddComponent(childComponent);
            childComponent.Parent = parent;
        }

        public static void RemoveChild(this Entity parent, Entity child)
        {
            if (parent == null) throw new ArgumentException(nameof(parent));
            if (child == null) throw new ArgumentNullException(nameof(child));

            var parentComponent = parent.GetComponent<SceneGraphComponent>();
            if (parent == null)
                throw new InvalidOperationException("Cannot remove child - parent is invalid."); 

            var childComponent = child.GetComponent<SceneGraphComponent>();
            if (childComponent == null)
                throw new InvalidOperationException("Cannot remove child - child is invalid.");

            if (childComponent.Parent != parent)
                throw new InvalidOperationException("Cannot remove child - parent is not the same.");

            childComponent.Parent = null;
            parentComponent.Children.Remove(child);
        }

        public static Entity GetParent(this Entity entity) => entity.GetComponent<SceneGraphComponent>()?.Parent;
        public static List<Entity> GetChildren(this Entity entity) => entity.GetComponent<SceneGraphComponent>()?.Children;

        public static void GetWorldMatrix(this Entity entity, out Matrix matrix)
        {
            var thisMatrix = entity.GetComponent<Transform>()?.Matrix ?? Matrix.Identity;
            var parent = entity.GetParent();
            if (parent != null) {
                Matrix parentMatrix;
                parent.GetWorldMatrix(out parentMatrix);
                matrix = thisMatrix * parentMatrix;
            }
            matrix = thisMatrix;
        }

        public static void DeleteWithChildren(this Entity entity)
        {
            var component = entity.GetComponent<SceneGraphComponent>();
            if (component != null)
            {
                if (component.Parent != null)
                {
                    component.Parent.GetChildren().Remove(entity);
                    component.Parent = null;
                }
                for(int i=component.Children.Count - 1; i>=0; i--)
                {
                    component.Children[i].DeleteWithChildren();
                }
            }
            entity.Delete();
        }

        public static void AddSceneGraphRendererComponent<T>(this Entity entity)
            where T : ISceneGraphRenderer
        {
            entity.AddComponent<ISceneGraphRenderableComponent<T>>(new SceneGraphRenderableComponent<T>());
        }
    }
}
