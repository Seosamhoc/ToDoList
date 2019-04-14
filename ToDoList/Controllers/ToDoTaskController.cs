using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    [Authorize]
    public class ToDoTaskController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _applicationDbContext;

        public ToDoTaskController(UserManager<ApplicationUser> userManager
            , ApplicationDbContext applicationDbContext
            )
        {
            _userManager = userManager;
            _applicationDbContext = applicationDbContext;
        }
        
        [HttpGet]
        public async Task<string> GetCurrentUserId()
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            return usr?.Id;
        }

        private Task<ApplicationUser> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);

        // GET: ToDoTask
        public async Task<ActionResult> Index()
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            List<ToDoTask> toDoTasks = _applicationDbContext.ToDoTask.Where(c => c.TaskOwnerId == usr.Id).ToList();
            return View("ToDoTaskList", toDoTasks);
        }

        // GET: ToDoTask/Details/5
        public async Task<ActionResult> Details(int id)
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            ToDoTask toDoTask = _applicationDbContext.ToDoTask.FirstOrDefault(c => c.Id == id);
            if (toDoTask != null && toDoTask.TaskOwnerId == usr.Id)
            {
                return View("ToDoTask", toDoTask);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: ToDoTask/Create
        public ActionResult Create()
        {
            return View("ToDoTaskCreate");
        }

        // POST: ToDoTask/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ToDoTask newTask)
        {
            try
            {
                ApplicationUser usr = await GetCurrentUserAsync();
                newTask.LastEdited = DateTime.Now;
                newTask.TaskOwner = usr;
                _applicationDbContext.ToDoTask.Add(newTask);
                _applicationDbContext.Users.First(c => c.Id == usr.Id).ToDoTasks.Add(newTask);
                _applicationDbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ToDoTask/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            ToDoTask toDoTask = _applicationDbContext.ToDoTask.FirstOrDefault(c => c.Id == id);
            if (toDoTask != null && toDoTask.TaskOwnerId == usr.Id)
            {
                return View("ToDoTaskEdit", toDoTask);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: ToDoTask/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, ToDoTask taskEdits)
        {
            try
            {
                ApplicationUser usr = await GetCurrentUserAsync();
                ToDoTask toDoTaskToEdit = _applicationDbContext.ToDoTask.FirstOrDefault(c => c.Id == id);
                if (toDoTaskToEdit != null && toDoTaskToEdit.TaskOwnerId == usr.Id)
                {
                    toDoTaskToEdit.LastEdited = DateTime.Now;
                    toDoTaskToEdit.TaskDescription = taskEdits.TaskDescription;
                    toDoTaskToEdit.TaskName = taskEdits.TaskName;
                    toDoTaskToEdit.Completed = taskEdits.Completed;
                    _applicationDbContext.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("ToDoTaskEdit", id);
            }
        }

        // GET: ToDoTask/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            ApplicationUser usr = await GetCurrentUserAsync();
            ToDoTask toDoTask = _applicationDbContext.ToDoTask.FirstOrDefault(c => c.Id == id);
            if (toDoTask != null && toDoTask.TaskOwnerId == usr.Id)
            {
                return View("ToDoTaskDelete", toDoTask);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: ToDoTask/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, ToDoTask toDoTask)
        {
            try
            {
                ApplicationUser usr = await GetCurrentUserAsync();
                ToDoTask toDoTaskToDelete = _applicationDbContext.ToDoTask.FirstOrDefault(c => c.Id == id);
                if (toDoTaskToDelete.TaskOwnerId == usr.Id)
                {
                    _applicationDbContext.ToDoTask.Remove(toDoTaskToDelete);
                    _applicationDbContext.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View("ToDoTaskDelete", id);
            }
        }
    }
}