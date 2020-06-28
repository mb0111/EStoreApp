using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EStore.API.Service.Contracts
{
    public interface ITransactionService : ICommonService
    {
        Task<EnEntityExistsStatus> TransactionExistsAsync(Guid idTransaction);

        Task<Output<TransactionModel>> GetTransactionAsync(Guid idTransaction);

        Task<Output<IEnumerable<TransactionModel>>> GetTransactionsForSellerAndByerAsync(TransactionSearchSellerByerModel transactionSearchSellerByerModel);

        Task<Output<TransactionModel>> CreateTransactionAsync(TransactionInsertModel transactionInsertModel);
    }
}
