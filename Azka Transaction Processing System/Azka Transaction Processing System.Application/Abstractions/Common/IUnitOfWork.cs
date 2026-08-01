using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Common
{
    public interface IUnitOfWork
    {
        Task BeginTransactionAsync();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        Task CommitTransactionAsync();

        Task RollbackTransactionAsync();
    }
}
