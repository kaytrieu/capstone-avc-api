using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AVC.Extensions
{
    public class NotFoundException : Exception
    {
        public NotFoundException()
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }

        public NotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class PermissionDeniedException : Exception
    {
        public PermissionDeniedException()
        {
        }

        public PermissionDeniedException(string message)
            : base(message)
        {
        }

        public PermissionDeniedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class ConflictEntityException : Exception
    {
        public ConflictEntityException()
        {
        }

        public ConflictEntityException(string message)
            : base(message)
        {
        }

        public ConflictEntityException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
