using Application.DTO.Chapter;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IChapterService
    {
        Task<IEnumerable<ChapterDto>> GetAllChaptersAsync(int courseId);
        Task<ChapterDto> GetChapterByIdAsync(int id);
        Task<ChapterDto> CreateChapterAsync(int courseId, CreateChapterDto createChapterDto);
        Task<ChapterDto> UpdateChapterAsync(int id, CreateChapterDto updateChapterDto);
        Task DeleteChapterAsync(int id);
    }
}
