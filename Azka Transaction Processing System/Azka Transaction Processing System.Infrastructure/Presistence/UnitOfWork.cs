using Azka_Transaction_Processing_System.Application.Abstractions.Common;
using Azka_Transaction_Processing_System.Application.Common;
using Azka_Transaction_Processing_System.Application.Exceptions;
using Azka_Transaction_Processing_System.Infrastructure.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Presistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TPSDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(TPSDbContext context)
        {
            _context = context;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction is not null) return;

            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateException ex) when (SqlServerHelper.IsReceiptSequenceConflict(ex))
            {
                throw new DuplicateReceiptSequenceException("");
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("No active transaction.");

            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();

            _transaction = null;
        }

        public async Task RollbackTransactionAsync()
        {

            if (_transaction == null)
                return;

            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            //if (_transaction is null)
            //    throw new InvalidOperationException("No active transaction.");

            //await _transaction.RollbackAsync();
            //await _transaction.DisposeAsync();

            //_transaction = null;
        }

        public void ClearChanges()
        {
            _context.ChangeTracker.Clear();
        }


    }
}
