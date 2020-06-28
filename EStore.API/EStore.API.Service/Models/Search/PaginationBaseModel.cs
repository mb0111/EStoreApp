namespace EStore.API.Service.Models.Search
{
    public class PaginationBaseModel
    {
        private int? pageNumber;
        private int? pageSize;

        public int? PageNumber
        {
            get
            {
                return pageNumber;
            }
            set
            {
                pageNumber = value;
            }
        }
        public int? PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
            }
        }
    }
}
