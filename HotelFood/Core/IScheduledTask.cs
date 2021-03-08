using System.Threading;
using System.Threading.Tasks;

namespace HotelFood.Core
{
    public interface IScheduledTask
    {
        string Schedule { get; }
        Task ExecuteAsync(CancellationToken cancellationToken);

        bool IsRunning { get; }
    }
}