using System;
using System.Collections.Generic;

namespace Ecs
{
    public class QueryResult
    {
        private Entity entity;
        private EntityWorld world;
        private System system;

        public QueryResult(Entity entity, EntityWorld world, System system)
        {
            this.entity = entity;
            this.world = world;
            this.system = system;
        }

        public T Get<T>() where T : class, IComponent
        {
            return world.FindComponent<T>(entity);
            /*
            Type compType = typeof(T);
            bool exists = system.Query.BaseComps.Exists(c => c == compType);
            if (exists)
            {
                return world.FindComponent<T>(entity);
            }
            return null;
            */
        }
    }
}