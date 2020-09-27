using System;
using System.Collections.Generic;

namespace Coramba.Services.Registration
{
    public class ModelDtoModuleComponent
    {
        public Type InsertModelServiceType { get; set; }
        public Type UpdateModelServiceType { get; set; }
        public Type DeleteModelServiceType { get; set; }
        public Type SelectOneModelServiceType { get; set; }
        public Type SelectAllModelServiceType { get; set; }
        public Dictionary<Type, Type> SelectModelServiceTypes { get; set; } = new Dictionary<Type, Type>();
        public Type ModelSelectorServiceType { get; set; }
        public Delegate IdGetter { get; set; }
    }
}
