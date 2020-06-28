using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EStore.API.DTO.Transaction
{
    public class TransactionSearchSellerByerDTO
    {
        [Required]
        public Guid IdSeller { get; set; }

        [Required]
        public Guid IdByer { get; set; }
    }
}
