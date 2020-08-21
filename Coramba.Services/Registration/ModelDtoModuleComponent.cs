using System;

namespace Coramba.Services.Registration
{
    public class ModelDtoModuleComponent
    {
        public Type InsertModelServiceType { get; set; }
        public Type UpdateModelServiceType { get; set; }
        public Type DeleteModelServiceType { get; set; }
        public Type SelectOneModelServiceType { get; set; }
        public Type SelectAllModelServiceType { get; set; }
        public Type ModelSelectorServiceType { get; set; }
        public Delegate IdGetter { get; set; }
    }
}
