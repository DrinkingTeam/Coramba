using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Coramba.DataAccess.Ef.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Coramba.DataAccess.Forge.PropertiesDb
{
    [Table("_objects_val")]
    [EfRepository]
    [DbContext(typeof(ForgePropertiesDbContext))]
    public class ForgePropertyObjectValue
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("value")]
        public byte[] Value { get; set; }

        public List<ForgePropertyObjectEav> Eavs { get; set; }
    }
}
