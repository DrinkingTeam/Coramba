using System.ComponentModel.DataAnnotations.Schema;
using Coramba.DataAccess.Ef.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Coramba.DataAccess.Forge.PropertiesDb
{
    [Table("_objects_eav")]
    [EfRepository]
    [DbContext(typeof(ForgePropertiesDbContext))]
    public class ForgePropertyObjectEav
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("entity_id")]
        public int EntityId { get; set; }

        [Column("attribute_id")]
        public int AttributeId { get; set; }

        [Column("value_id")]
        public int ValueId { get; set; }

        public ForgePropertyObject Entity { get; set; }
        public ForgePropertyObjectAttribute Attribute { get; set; }
        public ForgePropertyObjectValue Value { get; set; }
    }
}
