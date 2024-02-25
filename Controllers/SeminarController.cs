using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SeminarHub.Data;
using SeminarHub.Data.DataConstants;
using SeminarHub.Data.DataModels;
using SeminarHub.Models;
using SeminarHub.Models.ModelConstants;
using System.Drawing;
using System.Globalization;
using System.Runtime.Serialization;
using System.Security.Claims;

namespace SeminarHub.Controllers
{
    [Authorize]
    public class SeminarController : Controller
    {
        private readonly SeminarHubDbContext context;

        public SeminarController(SeminarHubDbContext _context)
        {
            context = _context;
        }
        public async Task<IActionResult> All()
        {
            var seminars = await context.Seminars
                .AsNoTracking()
                .Select(e => new SeminarInformationViewModel(
                    e.Id,
                    e.Lecturer,
                    e.Topic,
                    e.DateAndTime,
                    e.Category.Name,
                    e.Organizer.UserName
                    ))
                .ToListAsync();

            return View(seminars);
        }

        [HttpPost]
        public async Task<IActionResult> Join(int id)
        {
            var seminars = await context.Seminars
                .Where(s => s.Id == id)
                .Include(sp => sp.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminars == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            if (!seminars.SeminarsParticipants.Any(p => p.ParticipantId == userId))
            {
                seminars.SeminarsParticipants.Add(new SeminarParticipant()
                {
                    SeminarId = seminars.Id,
                    ParticipantId = userId
                });

                await context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            string userId = GetUserId();

            var model = await context.SeminarParticipants
                .Where(ep => ep.ParticipantId == userId)
                .AsNoTracking()
                .Select(ep => new SeminarInformationViewModel(
                    ep.SeminarId,
                    ep.Seminar.Topic,
                    ep.Seminar.Lecturer,
                    ep.Seminar.DateAndTime,
                    ep.Seminar.Category.Name,
                    ep.Seminar.Organizer.UserName
                    ))
                .ToListAsync();

            return View(model);
        }

        public async Task<IActionResult> Leave(int id)
        {
            var seminars = await context.Seminars
                .Where(e => e.Id == id)
                .Include(e => e.SeminarsParticipants)
                .FirstOrDefaultAsync();

            if (seminars == null)
            {
                return BadRequest();
            }

            string userId = GetUserId();

            var seminarsParticipants = seminars.SeminarsParticipants
                .FirstOrDefault(ep => ep.ParticipantId == userId);

            if (seminarsParticipants == null)
            {
                return BadRequest();
            }

            seminars.SeminarsParticipants.Remove(seminarsParticipants);

            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Joined));
        }

        [HttpGet]
        public async Task<IActionResult> Add() 
        {
            var model = new SeminarFormViewModel();
            
            model.Categories = await GetCategories();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(SeminarFormViewModel model) 
        {
            DateTime dateAndTime = DateTime.Now;

            if (!DateTime.TryParseExact(
                model.DateAndTime,
                ModelConstants.DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState
                    .AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be: {ModelConstants.DateTimeFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            var seminar = new Seminar()
            {
                Topic = model.Topic,
                Lecturer = model.Lecturer,
                Details = model.Details,
                OrganizerId = GetUserId(),
                DateAndTime = dateAndTime,
                Duration = model.Duration,
                CategoryId = model.CategoryId
            };

            await context.Seminars.AddAsync(seminar);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var seminar = await context.Seminars
                .FindAsync(id);

            if (seminar == null)
            {
                return BadRequest();
            }

            if (seminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var model = new SeminarFormViewModel()
            {
                Topic = seminar.Topic,
                Lecturer = seminar.Lecturer,
                Details = seminar.Details,
                DateAndTime = seminar.DateAndTime.ToString(ModelConstants.DateTimeFormat),
                Duration = seminar.Duration,
                CategoryId = seminar.CategoryId    
            };

            model.Categories = await GetCategories();

            return View(model);
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(SeminarFormViewModel model, int id)
        {
            var seminar = await context.Seminars
                .FindAsync(id);

            if (seminar == null)
            {
                return BadRequest();
            }

            if (seminar.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            DateTime dateAndTime = DateTime.Now;

            if (!DateTime.TryParseExact(
                model.DateAndTime,
                ModelConstants.DateTimeFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateAndTime))
            {
                ModelState
                    .AddModelError(nameof(model.DateAndTime), $"Invalid date! Format must be: {ModelConstants.DateTimeFormat}");
            }

            if (!ModelState.IsValid)
            {
                model.Categories = await GetCategories();

                return View(model);
            }

            seminar.DateAndTime = dateAndTime;
            seminar.Topic = model.Topic;
            seminar.Lecturer = model.Lecturer;
            seminar.Details = model.Details;
            seminar.Duration = model.Duration;
            seminar.CategoryId = model.CategoryId;
           
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Details(int id)
        {
            var model = await context.Seminars
                .Where(s => s.Id == id)
                .AsNoTracking()
                .Select(sd => new SeminarsDetailsViewModel()
                {
                    Id = sd.Id,
                    Topic = sd.Topic,
                    Lecturer = sd.Lecturer,
                    Details = sd.Details,
                    Duration = sd.Duration,
                    DateAndTime = sd.DateAndTime.ToString(ModelConstants.DateTimeFormat),
                    Category = sd.Category.Name
                })
                .FirstOrDefaultAsync();

            if (model == null)
            {
                return BadRequest();
            }

            return View(model);
        }        
      
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var seminarToBeDeleted = await context.Seminars.FindAsync(id);

            if (seminarToBeDeleted == null)
            {
                return BadRequest();
            }

            if (seminarToBeDeleted.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            var model = new SeminarDeletionViewModel()
            {
                Id = seminarToBeDeleted.Id,
                Topic = seminarToBeDeleted.Topic,
                DateAndTime = seminarToBeDeleted.DateAndTime.ToString(ModelConstants.DateTimeFormat)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(SeminarDeletionViewModel model)
        {
            var seminarToBeDeleted = await context.Seminars.FindAsync(model.Id);

            if (seminarToBeDeleted == null)
            {
                return BadRequest();
            }

            if (seminarToBeDeleted.OrganizerId != GetUserId())
            {
                return Unauthorized();
            }

            context.Seminars.Remove(seminarToBeDeleted);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        }
        
        private async Task<IEnumerable<CategoryViewModel>> GetCategories()
        {
            return await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }
    }
}
