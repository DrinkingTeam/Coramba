using System;
using System.Collections.Generic;

namespace Coramba.DataAccess.Conventions
{
    public class RepositoryConventions<T>: IRepositoryConventions<T>
    {
        private readonly IEnumerable<IRepositoryConvention<T>> _conventions;

        public RepositoryConventions(
            IEnumerable<IRepositoryConvention<T>> conventions
        )
        {
            _conventions = conventions ?? throw new ArgumentNullException(nameof(conventions));
        }

        public IEnumerable<IRepositoryConvention<T>> GetConventions()
        {
            return _conventions;
        }
    }
}
