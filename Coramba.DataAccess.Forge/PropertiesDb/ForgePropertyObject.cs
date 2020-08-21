using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Coramba.DataAccess.Ef.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Coramba.DataAccess.Forge.PropertiesDb
{
    [Table("_objects_id")]
    [EfRepository]
    [DbContext(typeof(ForgePropertiesDbContext))]
    public class ForgePropertyObject
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("external_id")]
        public byte[] ExternalId { get; set; }

        [Column("viewable_id")]
        public byte[] ViewableId { get; set; }

        public List<ForgePropertyObjectEav> Eavs { get; set; }
    }
}
