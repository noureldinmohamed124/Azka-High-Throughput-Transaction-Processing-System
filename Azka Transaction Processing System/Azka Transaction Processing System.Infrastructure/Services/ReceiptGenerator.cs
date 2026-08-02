using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Abstractions.Services;
using Azka_Transaction_Processing_System.Application.Common.DTOs;
using Azka_Transaction_Processing_System.Application.Common.Extensions;
using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using Azka_Transaction_Processing_System.Infrastructure.Presistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Services
{
    public class ReceiptGenerator : IReceiptGenerator
    {
        private readonly TPSDbContext _context;
        private readonly IReceiptSequenceRepo _receiptSequenceRepo;

        public ReceiptGenerator(TPSDbContext context, IReceiptSequenceRepo repository)
        {
            _context = context;
            _receiptSequenceRepo = repository;
        }

        public async Task<ReceiptNumberResult> GenerateAsync(TransactionTypeEnum prefix, int receiptUserId, CancellationToken cancellationToken = default)
        {
            
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            var sequence = await _receiptSequenceRepo.GetForUpdateAsync(prefix, today, cancellationToken);

            if (sequence is null)
            {
                sequence = new ReceiptSequence
                {
                    Prefix = prefix,
                    Date = today,
                    LastSequence = 1
                };

                await _receiptSequenceRepo.AddAsync(sequence);
            }
            else
            {
                sequence.LastSequence++;

                _receiptSequenceRepo.Update(sequence);
            }


            return new ReceiptNumberResult
            {
                ReceiptNumber = $"{prefix.ToCode()}-" + $"{today:yyyyMMdd}-" + $"{receiptUserId}-" + $"{sequence.LastSequence:D6}",
                Sequence = sequence.LastSequence,
                Date = today,
            };
        }

        
    }
}
