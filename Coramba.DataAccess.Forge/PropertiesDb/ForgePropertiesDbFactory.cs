using Coramba.DataAccess.Ef;
using Coramba.DataAccess.Ef.DbContexts;

namespace Coramba.DataAccess.Forge.PropertiesDb
{
    public class ForgePropertiesDbFactory : IForgePropertiesDbFactory
    {
        private readonly IDbContextFactory<ForgePropertiesDbContext> _factory;

        public ForgePropertiesDbFactory(IDbContextFactory<ForgePropertiesDbContext> factory)
        {
            _factory = factory;
        }

        public ForgePropertiesDbContext Create(string filename)
        {
            return _factory.Create(dbContext =>
            {
                dbContext.Filename = filename;
            });
        }
    }
}