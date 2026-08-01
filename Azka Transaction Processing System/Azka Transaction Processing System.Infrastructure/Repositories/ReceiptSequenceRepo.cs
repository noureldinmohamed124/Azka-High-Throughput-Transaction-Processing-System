using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Repositories
{
    public class ReceiptSequenceRepo : IReceiptSequenceRepo
    {
        public Task AddAsync(ReceiptSequence sequence, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<ReceiptSequence?> GetForUpdateAsync(ReceiptPrefixEnum prefix, DateOnly date, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Update(ReceiptSequence sequence)
        {
            throw new NotImplementedException();
        }
    }
}
