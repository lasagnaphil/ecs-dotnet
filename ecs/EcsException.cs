using System;

namespace Ecs
{
    public class EcsException : Exception
    {
        public EcsException(string message) : base(message) { }
    }

}