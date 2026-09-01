using Application.DTO.Chapter;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterService _chapterService;

        public ChapterController(IChapterService chapterService)
        {
            _chapterService = chapterService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{courseId}")]
        public async Task<ActionResult<ChapterDto>> CreateChapter([FromRoute] int courseId, CreateChapterDto dto) 
        {
            var chapter = await _chapterService.CreateChapterAsync(courseId, dto);
            return CreatedAtAction(nameof(CreateChapter), new { id = chapter.Id }, chapter);
        }

        [Authorize]
        [HttpGet("course/{courseId}")]
        public async Task<ActionResult<IEnumerable<ChapterDto>>> GetAllChaptersForCourse([FromRoute] int courseId)
        {
            var chapters = await _chapterService.GetAllChaptersAsync(courseId);
            return Ok(chapters);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ChapterDto>> GetChapterById([FromRoute] int id)
        {
            var chapter = await _chapterService.GetChapterByIdAsync(id);
            return Ok(chapter);
        }

        [Authorize]
        [HttpGet("by-name")]
        public async Task<ActionResult<ChapterDto>> GetChapterByName([FromQuery] string name)
        {
            var chapter = await _chapterService.GetChapterByNameAsync(name);
            return Ok(chapter);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ChapterDto>> UpdateChapter([FromRoute] int id, [FromBody] CreateChapterDto dto)
        {
            return Ok(await _chapterService.UpdateChapterAsync(id, dto));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteChapter([FromRoute] int id)
        {
            await _chapterService.DeleteChapterAsync(id);
            return Ok();
        }
    }
}
