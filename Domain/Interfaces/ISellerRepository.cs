using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISellerRepository
    {
        Task<Seller?> GetByUserIdAsync(Guid userId);
        Task<Seller?> GetByIdAsync(Guid id);
        Task AddAsync(Seller seller);
        Task SaveAsync();
    }
}
