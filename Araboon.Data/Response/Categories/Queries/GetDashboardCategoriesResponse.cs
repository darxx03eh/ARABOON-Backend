namespace Araboon.Data.Response.Categories.Queries
{
    public class GetDashboardCategoriesResponse : CategoriesResponse
    {
        public int Id { get; set; }
        public bool IsActive { get; set; }
        public int AvailableMangaCounts { get; set; }
    }
}
