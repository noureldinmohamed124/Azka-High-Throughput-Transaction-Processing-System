using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azka_Transaction_Processing_System.Application.Abstractions.Common
{
    public interface IGenericRepo<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}
