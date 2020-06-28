using AutoMapper;
using EStore.API.DAL.Data;
using EStore.API.Service.Contracts;
using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utf8Json;

namespace EStore.API.Service.Services
{
    public class TransactionService : ITransactionService
    {
        #region Private Property

        private readonly string ServiceName = nameof(TransactionService);
        private readonly EStoreContext _dbContext;
        private readonly IPurchaseService _purchaseService;
        private readonly IUserProductService _userProductService;
        private readonly IMapper _mapper;
        private readonly ILogger<TransactionService> _logger;

        #endregion

        #region Constructor
        public TransactionService(EStoreContext dbContext,
                                  IPurchaseService purchaseService,
                                  IUserProductService userProductService,
                                  IMapper mapper,
                                  ILogger<TransactionService> logger)
        {
            this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this._purchaseService = purchaseService ?? throw new ArgumentNullException(nameof(purchaseService));
            this._userProductService = userProductService ?? throw new ArgumentNullException(nameof(userProductService));
            this._mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Common Service

        public async Task AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            _logger.LogInformation($"{ServiceName}. Adding an object of type {entity.GetType().Name} to context");
            await _dbContext.Set<TEntity>().AddAsync(entity);
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            _logger.LogInformation($"{ServiceName}. Removing an object of type {entity.GetType().Name} from context");
            _dbContext.Set<TEntity>().Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            _logger.LogInformation($"{ServiceName}. Attempting to save the changes in the context");
            try
            {
                return (await _dbContext.SaveChangesAsync()) > 0;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"{ServiceName} error while SaveChanges");
                return false;
            }
        }

        #endregion

        #region Service Implementation

        public async Task<EnEntityExistsStatus> TransactionExistsAsync(Guid idTransaction)
        {
            _logger.LogInformation($"Checking if {nameof(Transaction)} with IdTransaction: {idTransaction} exists");
            if (idTransaction.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while checking if {nameof(Transaction)} exists. {nameof(Transaction.IdTransaction)} cannot be empty Guid");
                return EnEntityExistsStatus.BadRequest;
            }
            Transaction transaction = await FindTransactionAsync(idTransaction);
            return transaction == null ? EnEntityExistsStatus.NotFound : EnEntityExistsStatus.Found;
        }

        public async Task<Output<TransactionModel>> GetTransactionAsync(Guid idTransaction)
        {
            _logger.LogInformation($"Searching for {typeof(Transaction).Name} with IdTransaction: {idTransaction}");
            if (idTransaction.IsEmptyGuid())
            {
                _logger.LogError($"Validation error while searching for {typeof(Transaction).Name} with IdTransaction {Guid.Empty}");
                return new Output<TransactionModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdTransaction {idTransaction} is not valid"
                };
            }
            Transaction transaction = await FindTransactionAsync(idTransaction);
            if (transaction != null)
            {
                return new Output<TransactionModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<TransactionModel>(transaction)
                };
            }
            _logger.LogInformation($"No {typeof(Transaction).Name} found with IdTransaction: {idTransaction}");
            return new Output<TransactionModel>()
            {
                Status = EnResultStatus.NotFound,
                Message = $"No result found"
            };
        }

        public async Task<Output<IEnumerable<TransactionModel>>> GetTransactionsForSellerAndByerAsync(TransactionSearchSellerByerModel transactionSearchSellerByerModel)
        {
            _logger.LogInformation($"Searching for {typeof(Transaction).Name} with IdSeller: {transactionSearchSellerByerModel.IdSeller} and IdByer: {transactionSearchSellerByerModel.IdByer}");
            var transaction = await _dbContext.Transaction.Include(t => t.IdPurchaseNavigation).ThenInclude(t => t.IdUserProductNavigation).Where(t => t.IdPurchaseNavigation.InsertBy == transactionSearchSellerByerModel.IdByer && t.IdPurchaseNavigation.IdUserProductNavigation.IdUser == transactionSearchSellerByerModel.IdSeller).ToListAsync();
            if (!transaction.IsNullOrEmpty())
            {
                return new Output<IEnumerable<TransactionModel>>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<IEnumerable<TransactionModel>>(transaction)
                };
            }
            _logger.LogInformation($"No {typeof(Transaction).Name} found for IdSeller: {transactionSearchSellerByerModel.IdSeller} and IdByer: {transactionSearchSellerByerModel.IdByer}");
            return new Output<IEnumerable<TransactionModel>>()
            {
                Status = EnResultStatus.NotFound,
                Message = $"No {typeof(Transaction).Name} found for IdSeller: {transactionSearchSellerByerModel.IdSeller} and IdByer: {transactionSearchSellerByerModel.IdByer}"
            };
        }

        public async Task<Output<TransactionModel>> CreateTransactionAsync(TransactionInsertModel transactionInsertModel)
        {
            if (await _purchaseService.PurchaseExistsAsync(transactionInsertModel.IdPurchase) != EnEntityExistsStatus.Found)
            {
                _logger.LogWarning($"IdPurchase {transactionInsertModel.IdPurchase} is not valid");
                return new Output<TransactionModel>()
                {
                    Status = EnResultStatus.BadRequest,
                    Message = $"IdPurchase {transactionInsertModel.IdPurchase} is not valid"
                };
            }
            Transaction transaction = _mapper.Map<Transaction>(transactionInsertModel);
            await AddAsync(transaction);
            if (await this.SaveChangesAsync())
            {
                return new Output<TransactionModel>()
                {
                    Status = EnResultStatus.Success,
                    Message = $"Ok",
                    Result = _mapper.Map<TransactionModel>(transaction)
                };
            }
            _logger.LogError($"Error while saving new {typeof(Transaction).Name}. {typeof(Transaction).Name} insert parameters: {JsonSerializer.ToJsonString(transactionInsertModel)}");
            return new Output<TransactionModel>()
            {
                Status = EnResultStatus.Error,
                Message = $"Save could not be completed. Please try again later"
            };
        }

        #endregion

        #region Private

        private async Task<Transaction> FindTransactionAsync(Guid idTransaction)
        {
            return await _dbContext.Transaction.AsNoTracking().FirstOrDefaultAsync(t => t.IdTransaction == idTransaction);
        }

        #endregion

    }
}
