using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Coramba.Common;
using Coramba.Common.Reflection;
using Coramba.DataAccess.Common;
using Coramba.DataAccess.Conventions;
using Coramba.DataAccess.Repositories;

namespace Coramba.DataAccess.Annotations
{
    public class RepositoryFillNewAnnotationsConvention<T> : IRepositoryNewConvention<T>
    {
        internal static void Fill(T model, ActionFlags actionFlags, RepositoryOperationContext context)
        {
            typeof(T)
                .GetPropertyDescriptorsCached<IDefaultValueGetter>()
                .ForEach(p =>
                {
                    object value = null;
                    var defaultValueGetter = p
                        .Attributes
                        .OfType<IDefaultValueGetter>()
                        .FirstOrDefault(g => g.TryGetValue(model, actionFlags, p, context, out value));

                    if (defaultValueGetter != null)
                    {
                        p.SetValue(model, value);
                        return;
                    }

                    if (actionFlags.HasFlag(ActionFlags.New))
                    {
                        var a = p.Attributes.OfType<DefaultValueAttribute>().FirstOrDefault();
                        if (a != null)
                            p.SetValue(model, a.Value);
                    }
                });
        }

        public async Task ApplyAsync(T entity, RepositoryOperationContext context)
        {
            Fill(entity, ActionFlags.New, context);

            if (entity is IEntityNewListener<T> listener)
                await listener.OnNewAsync(entity, context);
        }
    }
}
