namespace Coramba.Services.Crud
{
    public class CrudModelService<TModelDto> : CrudModelService<TModelDto, object>
    {
        public CrudModelService(CrudModelServiceContext<TModelDto> context)
            : base(context)
        {
        }
    }
}