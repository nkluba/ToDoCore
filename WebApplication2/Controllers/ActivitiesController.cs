using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Activities
        // Optionally accepts a search string and a filter ("All", "Active", "Done")
        public async Task<IActionResult> Index(string searchString, string filter = "All")
        {
            var activitiesQuery = from a in _context.Activities
                                  select a;

            // Filter by search string
            if (!string.IsNullOrEmpty(searchString))
            {
                activitiesQuery = activitiesQuery.Where(a => a.Description.Contains(searchString));
            }

            // Filter by status
            if (filter == "Active")
            {
                activitiesQuery = activitiesQuery.Where(a => !a.IsDone);
            }
            else if (filter == "Done")
            {
                activitiesQuery = activitiesQuery.Where(a => a.IsDone);
            }

            // Order activities by creation date (newest first)
            var activityList = await activitiesQuery.OrderByDescending(a => a.CreatedAt).ToListAsync();

            // Calculate summary counts for header display
            ViewBag.TotalCount = activityList.Count;
            ViewBag.DoneCount = activityList.Count(a => a.IsDone);
            ViewBag.ActiveCount = activityList.Count(a => !a.IsDone);

            // Persist filter state in the view
            ViewBag.SearchString = searchString;
            ViewBag.Filter = filter;

            return View(activityList);
        }

        // POST: Activities/Create
        // Adds a new activity. The view should have a form posting to this action.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                activity.CreatedAt = DateTime.Now;
                activity.IsImportant = false;
                activity.IsDone = false;
                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // If model state is invalid, return to the Index view with current activities.
            var activities = await _context.Activities.OrderByDescending(a => a.CreatedAt).ToListAsync();
            return View("Index", activities);
        }

        // POST: Activities/MarkImportant/5
        // Toggles the "important" status of an activity.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkImportant(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                activity.IsImportant = !activity.IsImportant;
                _context.Update(activity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Activities/MarkDone/5
        // Toggles the "done" status of an activity.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkDone(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                activity.IsDone = !activity.IsDone;
                _context.Update(activity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Activities/Delete/5
        // Deletes an activity.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
