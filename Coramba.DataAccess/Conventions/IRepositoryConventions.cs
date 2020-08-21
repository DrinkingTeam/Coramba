using System.Collections.Generic;

namespace Coramba.DataAccess.Conventions
{
    public interface IRepositoryConventions<T>
    {
        IEnumerable<IRepositoryConvention<T>> GetConventions();
    }
}
