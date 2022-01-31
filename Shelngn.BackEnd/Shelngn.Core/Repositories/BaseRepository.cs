namespace Shelngn.Repositories
{
    public abstract class BaseRepository
    {
        private readonly IUnitOfWork injectedUnitOfWork;
        private IUnitOfWork? unitOfWork;

        protected BaseRepository(IUnitOfWork injectedUnitOfWork)
        {
            this.injectedUnitOfWork = injectedUnitOfWork;
        }

        protected IUnitOfWork UnitOfWork => unitOfWork ?? injectedUnitOfWork;

        internal void Initialize(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
    }
}
