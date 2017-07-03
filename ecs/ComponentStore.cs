using System.Collections.Generic;

namespace Ecs
{
    public class ComponentStore
    {
        public int CompId { get; private set; }
        public int CompCount { get { return components.Count; } }

        private List<IComponent> components;
        private Dictionary<int, int> entityMapper;

        public ComponentStore(int compId)
        {
            CompId = compId;
            components = new List<IComponent>();
            entityMapper = new Dictionary<int, int>();
        }

        public void Add(Entity entity, IComponent component)
        {
            int compId = components.Count;
            components.Add(component);
            entityMapper.Add(entity.Id, compId);
        }

        public void Remove(Entity entity)
        {
            if (entityMapper.TryGetValue(entity.Id, out var compId))
            {
                components.RemoveAt(compId);
            }
        }

        public IComponent Get(Entity entity)
        {
            if (entityMapper.TryGetValue(entity.Id, out var compId))
            {
                return components[compId];
            }
            return null;
        }
    }
}