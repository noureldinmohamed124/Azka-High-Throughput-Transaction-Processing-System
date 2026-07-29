using Azka_Transaction_Processing_System.Application.Abstractions.Repositories;
using Azka_Transaction_Processing_System.Application.Abstractions.Services;
using Azka_Transaction_Processing_System.Application.Common.DTOs;
using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Domain.Entities;
using Azka_Transaction_Processing_System.Domain.Enums;
using Azka_Transaction_Processing_System.Infrastructure.Presistance;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Services
{
    internal class ReceiptGenerator : IReceiptGenerator
    {
        private readonly TPSDbContext _context;
        private readonly IReceiptSequenceRepository _repository;

        public ReceiptGenerator(TPSDbContext context, IReceiptSequenceRepository repository)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<ReceiptNumberResult> GenerateAsync(ReceiptPrefixEnum prefix, int receiptUserId, CancellationToken cancellationToken = default)
        {
            const int MaxRetries = 3;

            for (int attempt = 1; attempt <= MaxRetries; attempt++)
            {
                await using var transaction =
                    await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var today = DateOnly.FromDateTime(DateTime.UtcNow);

                    var sequence = await _repository.GetForUpdateAsync(
                        prefix,
                        today,
                        cancellationToken);

                    if (sequence is null)
                    {
                        sequence = new ReceiptSequence
                        {
                            Prefix = prefix,
                            Date = today,
                            LastSequence = 1
                        };

                        await _repository.AddAsync(sequence, cancellationToken);
                    }
                    else
                    {
                        sequence.LastSequence++;

                        _repository.Update(sequence);
                    }

                    await _context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return new ReceiptNumberResult
                    {
                        ReceiptNumber =
                            $"{prefix.ToCode()}-" +
                            $"{today:yyyyMMdd}-" +
                            $"{receiptUserId}-" +
                            $"{sequence.LastSequence:D6}",

                        Sequence = sequence.LastSequence,

                        Date = today
                    };
                }
                catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
                {
                    await transaction.RollbackAsync(cancellationToken);

                    // Another request created today's sequence row first.
                    // Retry and the next iteration will find the row.
                    continue;
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }

            throw new BusinessRuleException(
                "Failed to generate a unique receipt number after multiple attempts.");
        }

        private static bool IsUniqueConstraintViolation(DbUpdateException exception)
        {
            return exception.InnerException is SqlException sql
                && (sql.Number == 2601 || sql.Number == 2627);
        }
    }
}
