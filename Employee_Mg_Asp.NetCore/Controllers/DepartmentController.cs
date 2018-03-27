using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Employee_Mg_Asp.NetCore.Data;
using Employee_Mg_Asp.NetCore.Models;
using Microsoft.AspNetCore.Identity;

namespace Employee_Mg_Asp.NetCore.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public DepartmentController(UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: Department
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewData["Role"] = user.PhoneNumber;
            var DepartmentLists = await _context.DepartmentMaster.Where(dep => dep.IsDelete == 0).ToListAsync();
            return View(DepartmentLists);

        }

        // GET: Department/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }

            if (id == null)
            {
                return NotFound();
            }

            var departmentMaster = await _context.DepartmentMaster
                .SingleOrDefaultAsync(m => m.Department_Id == id);
            if (departmentMaster == null)
            {
                return NotFound();
            }

            return View(departmentMaster);
        }

        // GET: Department/Create
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            if(user.PhoneNumber.ToString().Equals("GuestUser"))
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Department_Id,Department_Name")] DepartmentMaster departmentMaster)
        {
            if (ModelState.IsValid)
            {
                if (_context.DepartmentMaster.Any(name => name.Department_Name.Equals(departmentMaster.Department_Name) && name.IsDelete == 0))
                {
                    ModelState.AddModelError(string.Empty, "Department is already exists");
                }
                else
                {
                    departmentMaster.IsDelete = 0;
                    _context.Add(departmentMaster);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(departmentMaster);
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            if (user.PhoneNumber.ToString().Equals("GuestUser"))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var departmentMaster = await _context.DepartmentMaster.SingleOrDefaultAsync(m => m.Department_Id == id);
            if (departmentMaster == null)
            {
                return NotFound();
            }
            return View(departmentMaster);
        }

        // POST: Department/Edit/5
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Department_Id,Department_Name")] DepartmentMaster departmentMaster)
        {
            //if (id != departmentMaster.Department_Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.DepartmentMaster.Any(name => name.Department_Name.Equals(departmentMaster.Department_Name) && name.Department_Id != departmentMaster.Department_Id && name.IsDelete == 0))
                    {
                        ModelState.AddModelError(string.Empty, "Department is already exists");
                        return View(departmentMaster);
                    }
                    else
                    {
                        departmentMaster.IsDelete = 0;
                        _context.Update(departmentMaster);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentMasterExists(departmentMaster.Department_Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(departmentMaster);
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            if (user.PhoneNumber.ToString().Equals("GuestUser"))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }

            var departmentMaster = await _context.DepartmentMaster
                .SingleOrDefaultAsync(m => m.Department_Id == id);
            if (departmentMaster == null)
            {
                return NotFound();
            }

            return View(departmentMaster);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departmentMaster = await _context.DepartmentMaster.SingleOrDefaultAsync(m => m.Department_Id == id);
            departmentMaster.IsDelete = 1;
            _context.Update(departmentMaster);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentMasterExists(int id)
        {
            return _context.DepartmentMaster.Any(e => e.Department_Id == id);
        }
    }
}
