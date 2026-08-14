using Application.DTO.Chapter;
using Application.Interfaces;
using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class ChapterService : IChapterService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ChapterService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ChapterDto> CreateChapterAsync(int courseId, CreateChapterDto createChapterDto)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
            if (course == null) throw new ArgumentException("Course not found");

            var chapter = new Chapter
            {
                Name = createChapterDto.Name,
                Length = createChapterDto.Length,
                CourseId = courseId
            };

            await _unitOfWork.Chapters.AddAsync(chapter);
            await _unitOfWork.SaveChangesAsync();

            return new ChapterDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Length = chapter.Length,
                CourseId = chapter.CourseId
            };
        }

        public async Task DeleteChapterAsync(int id)
        {
            var chapter = await _unitOfWork.Chapters.GetByIdAsync(id);
            if (chapter == null) throw new ArgumentException("Chapter not found");

            await _unitOfWork.Chapters.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ChapterDto>> GetAllChaptersAsync(int courseId)
        {
            if (await _unitOfWork.Courses.GetByIdAsync(courseId) == null)
                throw new ArgumentException("Course not found");

            var chapters = await _unitOfWork.Chapters.GetByCourseIdAsync(courseId);
            if (chapters == null || !chapters.Any()) return Enumerable.Empty<ChapterDto>();

            return chapters.Select(c => new ChapterDto
            {
                Id = c.Id,
                Name = c.Name,
                Length = c.Length,
                CourseId = c.CourseId
            });
        }

        public async Task<ChapterDto> GetChapterByIdAsync(int id)
        {
            var chapter = await _unitOfWork.Chapters.GetByIdAsync(id);
            if (chapter == null) throw new ArgumentException("Chapter not found");

            return new ChapterDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Length = chapter.Length,
                CourseId = chapter.CourseId
            };
        }

        public async Task<ChapterDto> UpdateChapterAsync(int id, CreateChapterDto updateChapterDto)
        {
            var chapter = await _unitOfWork.Chapters.GetByIdAsync(id);
            if (chapter == null) throw new ArgumentException("Chapter not found");

            chapter.Name = updateChapterDto.Name;
            chapter.Length = updateChapterDto.Length;

            _unitOfWork.Chapters.Update(chapter);
            await _unitOfWork.SaveChangesAsync();

            return new ChapterDto
            {
                Id = chapter.Id,
                Name = chapter.Name,
                Length = chapter.Length,
                CourseId = chapter.CourseId
            };
        }
    }
}
