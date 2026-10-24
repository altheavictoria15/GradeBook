using Microsoft.AspNetCore.Http;
using GradeBook.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradeBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]   
    public class GradesController : ControllerBase
    {
        private static readonly List<Grade> _grades = new()
    {
        new Grade { Id = 1, StudentUsername = "juan",  Subject = "IPT",        Score = 92 },
        new Grade { Id = 2, StudentUsername = "juan",  Subject = "Networking", Score = 85 },
        new Grade { Id = 3, StudentUsername = "maria", Subject = "IPT",        Score = 95 }
    };
        private static int _nextId = 4;

        [HttpGet("me")]
        public IActionResult MyGrades()
        {
            var me = User.Identity!.Name;                       
            return Ok(_grades.Where(g => g.StudentUsername == me));
        }

        [Authorize(Roles = "Instructor")]
        [HttpGet]
        public IActionResult AllGrades() => Ok(_grades);

        [Authorize(Roles = "Instructor")]
        [HttpGet("{username}")]
        public IActionResult StudentGrades(string username)
            => Ok(_grades.Where(g => g.StudentUsername == username));

        public record GradeInput(string StudentUsername, string Subject, int Score);

        [Authorize(Roles = "Instructor")]
        [HttpPost]
        public IActionResult Post(GradeInput input)
        {
            var g = new Grade
            {
                Id = _nextId++,
                StudentUsername = input.StudentUsername,
                Subject = input.Subject,
                Score = input.Score
            };
            _grades.Add(g);
            return CreatedAtAction(nameof(StudentGrades), new { username = g.StudentUsername }, g);
        }
    }
}
