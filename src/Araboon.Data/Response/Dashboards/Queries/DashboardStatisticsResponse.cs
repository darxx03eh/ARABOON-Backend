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
        public double Percentage { get; set; }
    }

    public class TopCategories
    {
        public string Name { get; set; }
        public int TotalMangasCount { get; set; }
    }

    public class MangaPercentage
    {
        public double ActivePercentage { get; set; }
        public double InActivePercentage { get; set; }
    }
}