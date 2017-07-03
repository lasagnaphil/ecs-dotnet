using System;
using Ecs;

namespace Test
{
    class Position : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    class Velocity : IComponent
    {
        public float X { get; set; }
        public float Y { get; set; }
    }

    class Speakable : IComponent
    {
        public string Message { get; set; }
    }

    class SpeakSystem : Ecs.System, IStartable
    {
        public SpeakSystem() : base(Query.All(typeof(Position), typeof(Velocity))) { }

        public void Start(QueryResult result)
        {
            var speakable = result.Get<Speakable>();
            Console.WriteLine(speakable.Message);
        }
    }

    class UpdatePositionSystem : Ecs.System, IStartable, IUpdateable
    {
        public UpdatePositionSystem() : base(Query.All(typeof(Position), typeof(Velocity))) { }

        public void Start(QueryResult result)
        {
            var pos = result.Get<Position>();
            var vel = result.Get<Velocity>();
            Print(pos, vel);

        }
        public void Update(QueryResult result, float dt)
        {
            var pos = result.Get<Position>();
            var vel = result.Get<Velocity>();
            pos.X += vel.X * dt;
            pos.Y += vel.Y * dt;
            Print(pos, vel);
        }

        private void Print(Position pos, Velocity vel)
        {
            Console.WriteLine($"Position: X = {pos.X}, Y = {pos.Y}, Velocity: X = {vel.X}, Y = {vel.Y}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            EntityWorld world = new EntityWorld();
            for (int i = 0; i < 10; i++)
            {
                Entity testEntity = world.CreateEntity();
                testEntity.AddComponent(new Position() {X = 1.0f, Y = 2.0f});
                testEntity.AddComponent(new Velocity() {X = 10.0f, Y = 10.0f});
                testEntity.AddComponent(new Speakable() {Message = "Hello World!"});
            }
            world.AddSystem(new SpeakSystem());
            world.AddSystem(new UpdatePositionSystem());

            world.Start();
            world.Update(1.0f);
            world.Update(1.0f);

            Console.ReadKey();
        }
    }
}
