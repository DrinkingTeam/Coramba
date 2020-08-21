namespace Coramba.Services.Crud
{
    public interface ICrudModelService<TModelDto>:
        IInsertModelService<TModelDto>,
        IUpdateModelService<TModelDto>,
        IDeleteModelService<TModelDto>,
        ISelectOneModelService<TModelDto>,
        ISelectAllModelService<TModelDto>
    {
    }
}
