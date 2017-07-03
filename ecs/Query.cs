using System;
using System.Collections;
using System.Collections.Generic;

namespace Ecs
{
    public class Query
    {
        public readonly List<Type> BaseComps;
        public readonly List<Type[]> IncludedComps;
        public readonly List<Type[]> ExcludedComps;

        public Query()
        {
            BaseComps = new List<Type>();
            IncludedComps = new List<Type[]>();
            ExcludedComps = new List<Type[]>();
        }

        public static Query All(params Type[] andTypes)
        {
            Query query = new Query();
            query.BaseComps.AddRange(andTypes);
            return query;
        }

        public Query Include(params Type[] orTypes)
        {
            IncludedComps.Add(orTypes);
            return this;
        }

        public Query Exclude(params Type[] orTypes)
        {
            ExcludedComps.Add(orTypes);
            return this;
        }
    }

    public class CompiledQuery
    {
        public readonly BitArray BaseBits;
        public readonly List<BitArray> IncludedBits;
        public readonly List<BitArray> ExcludedBits;

        public CompiledQuery(Query query, EntityWorld world)
        {
            int bitLength = world.ComponentCount;
            BaseBits = new BitArray(bitLength);
            for (int i = 0; i < bitLength; i++)
            {
                Type compType = world.GetComponentType(i);
                bool flag = query.BaseComps.Contains(compType);
                BaseBits.Set(i, flag);
            }
            IncludedBits = new List<BitArray>();
            ExcludedBits = new List<BitArray>();
        }
    }
}