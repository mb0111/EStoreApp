using EStore.API.Service.Helpers;
using EStore.API.Service.Models.Abstraction;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using UserProductEntity = EStore.API.DAL.Data.UserProduct;

namespace EStore.API.Service.Models.Search
{
    public class UserProductSearchCriteria : SearchCriteriaFilter<UserProductEntity>
    {
        public Guid? IdUser { get; set; }

        public Guid? IdProduct { get; set; }

        public int? MinQuantity { get; set; }

        public int? MaxQuantity { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        public EnRole Role { get; set; }

        private bool? UserProductIsActive => (int)EnActiveStatus.True == 1;

        public override IQueryable<UserProductEntity> ApplySearchCriteriaFilter(IQueryable<UserProductEntity> source)
        {
            source = source.Include(p => p.IdProductNavigation).Where((UserProductEntity x) => (!IdUser.HasValue || (IdUser.HasValue && IdUser.Value.IsEmptyGuid()) || x.IdUser == IdUser.Value)
                                                                                            && (!IdProduct.HasValue || (IdProduct.HasValue && IdProduct.Value.IsEmptyGuid()) || x.IdProduct == IdProduct.Value)
                                                                                            && (!MinQuantity.HasValue || x.Quantity >= MinQuantity.Value)
                                                                                            && (!MaxQuantity.HasValue || x.Quantity <= MaxQuantity.Value)
                                                                                            && (!MinPrice.HasValue || x.Price >= MinPrice.Value)
                                                                                            && (!MaxPrice.HasValue || x.Price <= MaxPrice.Value)
                                                                                            && (x.IsActive == UserProductIsActive)
                                                                     );
            // A byer can only see products created by Sellers
            if (Role.IsByer())
                source = source.Include(u => u.IdUserNavigation).Where((UserProductEntity x) => x.IdUserNavigation.UserRole.Any(r => r.IdRole == (int)EnRole.Seller));
            return source;
        }
    }
}
