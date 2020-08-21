using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Conventions
{
    public static class RepositoryConventionsExtensions
    {
        public static async Task<T> ApplyAsync<TConvention, T>(
            this IEnumerable<TConvention> conventions,
            T obj,
            Func<TConvention, T, Task<T>> conventionAction,
            RepositoryOperationContext context)
        {
            foreach (var convention in conventions)
                if (context.IsConventionEnabled(convention.GetType()))
                    obj = await conventionAction(convention, obj);
            return obj;
        }

        private static async Task ApplyAsync<TConvention>(
            this IEnumerable<TConvention> conventions,
            Func<TConvention, Task> conventionAction,
            RepositoryOperationContext context)
        {
            foreach (var convention in conventions)
                if (context.IsConventionEnabled(convention.GetType()))
                    await conventionAction(convention);
        }

        public static async Task ApplyAsync<TConvention, T>(this IEnumerable<TConvention> conventions, T entity, RepositoryOperationContext context)
            where TConvention: IRepositoryModelConvention<T>
        {
            await conventions.ApplyAsync(async c => await c.ApplyAsync(entity, context), context);
        }
    }
}
