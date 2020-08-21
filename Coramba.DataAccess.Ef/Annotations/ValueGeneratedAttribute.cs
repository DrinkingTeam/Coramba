using System;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Coramba.DataAccess.Ef.Annotations
{
    public class ValueGeneratedAttribute: Attribute
    {
        public ValueGenerated ValueGenerated { get; }

        public ValueGeneratedAttribute(ValueGenerated valueGenerated)
        {
            ValueGenerated = valueGenerated;
        }
    }
}
