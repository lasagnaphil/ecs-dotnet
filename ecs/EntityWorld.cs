using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Ecs
{
    public static class BitArrayExtensions
    {
        public static string ToDebugString(this BitArray bitArray)
        {
            string str = "";
            foreach (bool bit in bitArray)
            {
                if (bit) str += "1";
                else str += "0";
            }
            return str;
        }

        public static bool Compare(this BitArray bitArray, BitArray otherBitArray)
        {
            if (bitArray.Length != otherBitArray.Length) return false;
            for (int i = 0; i < bitArray.Length; i++)
            {
                if (bitArray.Get(i) != otherBitArray.Get(i)) return false;
            }
            return true;
        }
    }
    public class EntityWorld
    {
        private List<Entity> entities;
        private List<System> systems;
        private ComponentStore[] componentStores;
        private List<Type> componentTypes;
        private Dictionary<Type, int> componentIdDict;

        public readonly int ComponentCount;
        public int EntityCount { get { return entities.Count; } }

        public EntityWorld()
        {
            entities = new List<Entity>();
            systems = new List<System>();
            componentIdDict = new Dictionary<Type, int>();

            // Get the list of components using reflection
            Type baseCompType = typeof(IComponent);
            componentTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => baseCompType.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract && p != baseCompType)
                .ToList();

            ComponentCount = componentTypes.Count;

            componentStores = new ComponentStore[ComponentCount];

            for (int i = 0; i < componentTypes.Count; i++)
            {
                Type type = componentTypes[i];
                componentIdDict.Add(type, i);
                componentStores[i] = new ComponentStore(i);
            }
        }

        public Entity CreateEntity()
        {
            Entity entity = new Entity(this, EntityCount);
            entities.Add(entity);
            return entity;
        }
        public void RemoveEntity(Entity entity) => entities.Remove(entity);

        public void AddSystem(System system) => systems.Add(system);
        public void RemoveSystem(System system) => systems.Remove(system);

        public void AddComponent<T>(Entity entity, T component) where T : IComponent
        {
            int compTypeId = componentIdDict[typeof(T)];
            componentStores[compTypeId].Add(entity, component);
        }

        public void RemoveComponent<T>(Entity entity, T component) where T : IComponent
        {
            int compTypeId = componentIdDict[typeof(T)];
            componentStores[compTypeId].Remove(entity);
        }

        public T FindComponent<T>(Entity entity) where T : IComponent
        {
            int compTypeId = componentIdDict[typeof(T)];
            return (T) componentStores[compTypeId].Get(entity);
        }

        public int GetIdOfComponent(Type type)
        {
            if (componentIdDict.TryGetValue(type, out var compId)) return compId;
            else throw new EcsException($"No component id ({compId}) found for component {type.Name}");
        }

        public Type GetComponentType(int id)
        {
            if (id < 0 || id >= componentTypes.Count)
                throw new EcsException($"No component type of id {id} found");
            return componentTypes[id];
        }

        public void Start()
        {
            foreach (var system in systems)
            {
                system.CompileQuery(this);
                Console.WriteLine($"Finished compiling system {system.GetType().Name}");
            }
            foreach (var system in systems)
            {
                if (system is IStartable startable)
                {
                    foreach (var entity in entities)
                    {
                        bool found = SearchQuery(entity, system, out var result);
                        if (found)
                        {
                            startable.Start(result);
                            //Console.WriteLine($"Finished starting system {system.GetType().Name}");
                        }
                    }
                }
            }
        }

        public void Update(float dt)
        {
            foreach (var system in systems)
            {
                foreach (var entity in entities)
                {
                    if (system is IUpdateable updateable)
                    {
                        bool found = SearchQuery(entity, system, out var result);
                        if (found)
                        {
                            updateable.Update(result, dt);
                            //Console.WriteLine($"Finished updating system {system.GetType().Name}");
                        }
                    }
                }
            }
        }

        public void Draw()
        {
            foreach (var system in systems)
            {
                foreach (var entity in entities)
                {
                    if (system is IDrawable drawable)
                    {
                        bool found = SearchQuery(entity, system, out var result);
                        if (found)
                        {
                            drawable.Draw(result);
                            //Console.WriteLine($"Finished drawing system {system.GetType().Name}");
                        }
                    }
                }
            }
        }

        public bool SearchQuery(Entity entity, System system, out QueryResult queryResult)
        {
            /*
            Console.WriteLine($"In system {system.GetType().Name}");
            Console.WriteLine($"Mask of entity: {entity.ComponentMask.ToDebugString()}");
            Console.WriteLine($"Mask of query: {system.CompiledQuery.BaseBits.ToDebugString()}");
            */

            BitArray andResult = new BitArray(entity.ComponentMask);
            andResult.And(system.CompiledQuery.BaseBits);
            if (andResult.Compare(system.CompiledQuery.BaseBits))
            {
                queryResult = new QueryResult(entity, this, system);
                return true;
            }

            queryResult = null;
            return false;
        }

    }
}