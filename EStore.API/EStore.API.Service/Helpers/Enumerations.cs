namespace EStore.API.Service.Helpers
{
    public enum EnRole
    {
        System = 1,
        Admin = 2,
        Seller = 3,
        Byer = 4
    }

    public enum EnPurchaseStatus
    {
        StandBy = 1,
        Confirmed = 2,
        Cancelled = 3,
        Rejected = 4
    }

    public enum EnActiveStatus
    {
        False = 0,
        True = 1
    }

    public enum EnResultStatus
    {
        None,
        BadRequest,
        NotFound,
        Success,
        Error
    }

    public enum EnEntityExistsStatus
    {
        None,
        BadRequest,
        Found,
        NotFound
    }
}
