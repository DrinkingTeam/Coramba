using System;
using AutoMapper;

namespace Coramba.Core.AutoMapper
{
    public interface IAutoMapTypeConfigurator
    {
        void Configure(IMapperConfigurationExpression expression, Type type);
    }
}
