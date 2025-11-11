using Araboon.Data.Entities;
using Araboon.Data.Response.Swipers.Queries;
using Microsoft.AspNetCore.Http;

namespace Araboon.Service.Interfaces
{
    public interface ISwiperService
    {
        public Task<(string, IList<Swiper>?)> GetSwiperForHomePageAsync();
        public Task<string> ActivateSwiperToggleAsync(int id);
        public Task<string> DeleteExistingSwiperAsync(int id);
        public Task<(string, IList<Swiper>?, SwiperMetaDataResponse?)> GetSwiperForDashboardAsync();
        public Task<(string, Swiper?)> AddNewSwiperAsync(IFormFile image, string? note = null);
        public Task<(string, string?)> UploadNewSwiperImageAsync(int id, IFormFile image);
        public Task<string> UpdateSwiperNoteAsync(int id, string note);
    }
}
