using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using AutoMapper.Configuration;

namespace Coramba.Core.AutoMapper
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class AutoMapCreateAttribute: Attribute, IAutoMapTypeConfigurator
    {
        public Type SourceType { get; }
        public Type DestinationType { get; }

        public bool ReverseMap { get; set; }

        /// <summary>
        /// If set to true, construct the destination object using the service locator.
        /// </summary>
        public bool ConstructUsingServiceLocator { get; set; }

        /// <summary>
        /// For self-referential types, limit recurse depth.
        /// </summary>
        public int MaxDepth { get; set; }

        /// <summary>
        /// If set to true, preserve object identity. Useful for circular references.
        /// </summary>
        public bool PreserveReferences { get; set; }

        /// <summary>
        /// If set to true, disable constructor validation.
        /// </summary>
        public bool DisableCtorValidation { get; set; }

        /// <summary>
        /// If set to true, include this configuration in all derived types' maps.
        /// </summary>
        public bool IncludeAllDerived { get; set; }

        /// <summary>
        /// Skip normal member mapping and convert using a <see cref="ITypeConverter{TSource,TDestination}"/> instantiated during mapping.
        /// </summary>
        public Type TypeConverter { get; set; }

        public AutoMapCreateAttribute(Type sourceType, Type destinationType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            DestinationType = destinationType ?? throw new ArgumentNullException(nameof(destinationType));
        }

        private void ApplyConfiguration(IMappingExpression mappingExpression)
        {
            if (ReverseMap)
            {
                mappingExpression.ReverseMap();
            }

            if (ConstructUsingServiceLocator)
            {
                mappingExpression.ConstructUsingServiceLocator();
            }

            if (MaxDepth > 0)
            {
                mappingExpression.MaxDepth(MaxDepth);
            }

            if (PreserveReferences)
            {
                mappingExpression.PreserveReferences();
            }

            if (DisableCtorValidation)
            {
                mappingExpression.DisableCtorValidation();
            }

            if (IncludeAllDerived)
            {
                mappingExpression.IncludeAllDerived();
            }

            if (TypeConverter != null)
            {
                mappingExpression.ConvertUsing(TypeConverter);
            }

            foreach (var memberInfo in DestinationType.GetMembers(BindingFlags.Public | BindingFlags.Instance))
            {
                foreach (var memberConfigurationProvider in memberInfo.GetCustomAttributes().OfType<IMemberConfigurationProvider>())
                {
                    mappingExpression.ForMember(memberInfo.Name, cfg => memberConfigurationProvider.ApplyConfiguration(cfg));
                }
            }
        }

        public void Configure(IMapperConfigurationExpression expression, Type type)
        {
            var profile = new NamedProfile($"{SourceType.FullName}-{DestinationType.FullName}");

            var mappingExpression = (MappingExpression)profile.CreateMap(SourceType, DestinationType);

            ApplyConfiguration(mappingExpression);

            expression.AddProfile(profile);
        }
    }
}
