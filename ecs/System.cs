using System;

namespace Ecs
{
    public interface IStartable
    {
        void Start(QueryResult result);
    }

    public interface IUpdateable
    {
        void Update(QueryResult result, float dt);
    }

    public interface IDrawable
    {
        void Draw(QueryResult result);
    }

    public class System
    {
        private EntityWorld world;

        public readonly Query Query;
        public CompiledQuery CompiledQuery { get; private set; }

        private QueryResult result;

        public System(Query query)
        {
            Query = query;
        }

        public void CompileQuery(EntityWorld world)
        {
            CompiledQuery = new CompiledQuery(Query, world);
        }
    }
}