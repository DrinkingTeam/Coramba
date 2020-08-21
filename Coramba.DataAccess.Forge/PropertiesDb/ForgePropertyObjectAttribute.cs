using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Coramba.DataAccess.Ef.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Coramba.DataAccess.Forge.PropertiesDb
{
    [Table("_objects_attr")]
    [EfRepository]
    [DbContext(typeof(ForgePropertiesDbContext))]
    public class ForgePropertyObjectAttribute
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("data_type")]
        public int? DataType { get; set; }

        [Column("data_type_context")]
        public string DataTypeContext { get; set; }

        [Column("display_name")]
        public string DisplayName { get; set; }

        [Column("flags")]
        public int? Flags { get; set; }

        [Column("display_precision")]
        public int? DisplayPrecision { get; set; }

        public List<ForgePropertyObjectEav> Eavs { get; set; }
    }
}
