using System.Collections;

namespace Ecs
{
    public class Entity
    {
        public readonly BitArray ComponentMask;

        private EntityWorld world;

        public Entity(EntityWorld world, int id)
        {
            this.world = world;
            ComponentMask = new BitArray(world.ComponentCount, false);
            Id = id;
        }

        public int Id { get; set; }

        public void AddComponent<T>(T component) where T : IComponent
        {
            world.AddComponent(this, component);
            int compIndex = world.GetIdOfComponent(typeof(T));
            ComponentMask.Set(compIndex, true);
        }

        public void RemoveComponent<T>(T component) where T : IComponent
        {
            world.RemoveComponent(this, component);
            int compIndex = world.GetIdOfComponent(typeof(T));
            ComponentMask.Set(compIndex, false);
        }
    }
}