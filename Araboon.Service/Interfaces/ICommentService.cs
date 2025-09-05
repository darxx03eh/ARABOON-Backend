namespace Araboon.Service.Interfaces
{
    public interface ICommentService
    {
        public Task<string> AddCommentAsync(string content, int mangaId);
        public Task<string> DeleteCommentAsync(int id);
    }
}
