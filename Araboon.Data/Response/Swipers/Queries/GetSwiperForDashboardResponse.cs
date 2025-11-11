namespace Araboon.Data.Response.Swipers.Queries
{
    public class GetSwiperForDashboardResponse
    {
        public int Id { get; set; }
        public string? Note { get; set; }
        public string Url { get; set; }
        public string CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public string Link { get; set; }
    }
}
