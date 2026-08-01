using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Repositories
{
    public interface IReceiptSequenceRepo
    {
        Task<ReceiptSequence?> GetForUpdateAsync(ReceiptPrefixEnum prefix, DateOnly date, CancellationToken cancellationToken = default);
        Task AddAsync(ReceiptSequence sequence, CancellationToken cancellationToken = default);
        void Update(ReceiptSequence sequence);
    }
}
