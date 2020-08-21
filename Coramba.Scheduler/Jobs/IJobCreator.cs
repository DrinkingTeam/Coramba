using System.Threading.Tasks;

namespace Coramba.Scheduler.Jobs
{
    public interface IJobCreator
    {
        Task<JobCreationInstruction> CreateAsync();
    }
}
