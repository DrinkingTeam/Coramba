namespace Coramba.Services.Crud
{
    public class CrudModelServiceContext<TModelDto>
    {
        public IInsertModelService<TModelDto> InsertModelService { get; }
        public IUpdateModelService<TModelDto> UpdateModelService { get; }
        public IDeleteModelService<TModelDto> DeleteModelService { get; }
        public ISelectOneModelService<TModelDto> SelectOneModelService { get; }
        public ISelectAllModelService<TModelDto> SelectAllModelService { get; }

        public CrudModelServiceContext(
            IInsertModelService<TModelDto> insertModelService,
            IUpdateModelService<TModelDto> updateModelService,
            IDeleteModelService<TModelDto> deleteModelService,
            ISelectOneModelService<TModelDto> selectOneModelService,
            ISelectAllModelService<TModelDto> selectAllModelService)
        {
            InsertModelService = insertModelService;
            UpdateModelService = updateModelService;
            DeleteModelService = deleteModelService;
            SelectOneModelService = selectOneModelService;
            SelectAllModelService = selectAllModelService;
        }
    }
}