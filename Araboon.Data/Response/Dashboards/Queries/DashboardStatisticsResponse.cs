namespace Araboon.Data.Response.Dashboards.Queries
{
    public class DashboardStatisticsResponse
    {
        public DashboardStatistics Categories { get; set; }
        public DashboardStatistics Mangas { get; set; }
        public DashboardStatistics Users { get; set; }
        public DashboardStatistics Banners { get; set; }
        public IList<TopCategories> TopCategories { get; set; }
        public MangaPercentage MangaPercentage { get; set; }
    }
    public class DashboardStatistics
    {
        public int TotalCounts { get; set; }
        public bool IsRise { get; set; }
        public string Percentage { get; set; }
    }
    public class TopCategories
    {
        public string Name { get; set; }
        public int TotalMangasCount { get; set; }
    }
    public class MangaPercentage
    {
        public string ActivePercentage { get; set; }
        public string InActivePercentage { get; set; }
    }
}
