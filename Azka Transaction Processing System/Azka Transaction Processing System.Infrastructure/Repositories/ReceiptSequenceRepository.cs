using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using Azka_Transaction_Processing_System.Infrastructure.Presistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Repositories
{
    internal class ReceiptSequenceRepository : IReceiptSequenceRepository
    {
        private readonly TPSDbContext _context;

        public ReceiptSequenceRepository(TPSDbContext context)
        {
            _context = context;
        }

        public async Task<ReceiptSequence?> GetForUpdateAsync(ReceiptPrefixEnum prefix, DateOnly date, CancellationToken cancellationToken = default)
        {
            return await _context.ReceiptSequences
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM ReceiptSequences WITH (UPDLOCK, ROWLOCK)
                    WHERE Prefix = {(int)prefix}
                    AND [Date] = {date}")
                .AsTracking()
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task AddAsync(ReceiptSequence sequence, CancellationToken cancellationToken = default)
        {
            await _context.ReceiptSequences.AddAsync(sequence, cancellationToken);
        }

        public void Update(ReceiptSequence sequence)
        {
            _context.ReceiptSequences.Update(sequence);
        }
    }
}
