namespace Coramba.DataAccess.Queries.Universal.Conditions
{
    public interface IUniversalFilterCondition
    {
        UniversalFilterWhereClause GetWhereClause(int parameterIndex);
    }
}
