﻿using System.ComponentModel;

namespace EStore.API.DTO.Header
{
    [DisplayName("PaginationMetadata")]
    public class PaginationMetadataDTO
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }
    }
}
