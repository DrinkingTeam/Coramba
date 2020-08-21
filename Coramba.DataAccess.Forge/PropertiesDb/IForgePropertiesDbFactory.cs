namespace Coramba.DataAccess.Forge.PropertiesDb
{
    public interface IForgePropertiesDbFactory
    {
        ForgePropertiesDbContext Create(string filename);
    }
}
