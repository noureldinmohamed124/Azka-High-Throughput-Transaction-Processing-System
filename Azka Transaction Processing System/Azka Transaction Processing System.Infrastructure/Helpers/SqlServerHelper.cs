using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Infrastructure.Helpers
{
    internal static class SqlServerHelper
    {
        private const string ReceiptSequenceConstraint = "IX_ReceiptSequence_Prefix_Date";

        public static bool IsReceiptSequenceConflict(DbUpdateException exception)
        {
            if (exception.InnerException is not SqlException sqlException)
                return false;

            if (sqlException.Number != 2601 && sqlException.Number != 2627)
                return false;

            return sqlException.Message.Contains(ReceiptSequenceConstraint, StringComparison.OrdinalIgnoreCase);
        }
    }
}
