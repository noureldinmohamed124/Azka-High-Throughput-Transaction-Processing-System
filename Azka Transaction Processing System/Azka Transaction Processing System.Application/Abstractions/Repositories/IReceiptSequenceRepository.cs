using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Repositories
{
    public interface IReceiptSequenceRepository
    {
        Task<ReceiptSequence?> GetAsync(ReceiptPrefixEnum prefix, DateOnly date);

        Task AddAsync(ReceiptSequence sequence);

        void Update(ReceiptSequence sequence);
    }
}
