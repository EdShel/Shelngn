namespace Shelngn.Repositories
{
    public interface IRepositoryFactory
    {
        T Create<T>(IUnitOfWork unitOfWork) where T : class;
    }
}
