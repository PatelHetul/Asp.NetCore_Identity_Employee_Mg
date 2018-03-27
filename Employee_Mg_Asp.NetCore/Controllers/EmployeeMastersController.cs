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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Net.Http.Headers;

namespace Employee_Mg_Asp.NetCore.Controllers
{
    public class EmployeeMastersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHostingEnvironment _hostingEnvironment;

        public EmployeeMastersController(UserManager<ApplicationUser> userManager,
          SignInManager<ApplicationUser> signInManager, ApplicationDbContext context, IHostingEnvironment hostingEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: EmployeeMasters
        public async Task<IActionResult> Index(string searchText, string sortOrder, string currentFilter, int? page)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            ViewData["Role"] = user.PhoneNumber;
            ViewData["Email"] = user.Email;
            ViewData["CurrentSort"] = sortOrder;
            ViewBag.idSortParm = String.IsNullOrEmpty(sortOrder) ? "Id" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";

            if (searchText != null)
            {
                page = 1;
            }
            else
            {
                searchText = currentFilter;
            }

            ViewBag.currentFilter = searchText;
            var employeeMasters = _context.EmployeeMaster.Include(e => e.DepartmentMaster).Where(e => e.IsDelete == 0 && e.DepartmentMaster.IsDelete == 0);
            if (!String.IsNullOrEmpty(searchText))
            {
                employeeMasters = employeeMasters.Where(s => s.Employee_Name.Contains(searchText) || s.Email.Contains(searchText) || s.Address.Contains(searchText) || s.JoiningDate.ToString().Contains(searchText) || s.DepartmentMaster.Department_Name.Contains(searchText));
            }
            switch (sortOrder)
            {
                case "Id":
                    employeeMasters = employeeMasters.OrderBy(s => s.Employee_Id);
                    break;
                case "name_desc":
                    employeeMasters = employeeMasters.OrderByDescending(s => s.Employee_Name);
                    break;
                case "Email":
                    employeeMasters = employeeMasters.OrderBy(s => s.Email);
                    break;
                case "email_desc":
                    employeeMasters = employeeMasters.OrderByDescending(s => s.Email);
                    break;
                default:
                    employeeMasters = employeeMasters.OrderBy(s => s.Employee_Name);
                    break;
            }
            ViewBag.path = Path.Combine(_hostingEnvironment.WebRootPath, "Emp_IMg");
            int pageSize = 3;
            return View(await PaginatedList<EmployeeMaster>.CreateAsync(employeeMasters.AsNoTracking(), page ?? 1, pageSize));
        }


        // GET: EmployeeMasters/Details/5
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

            var employeeMaster = await _context.EmployeeMaster
                .Include(e => e.DepartmentMaster)
                .SingleOrDefaultAsync(m => m.Employee_Id == id);
            ViewBag.image = employeeMaster.Image;
            ViewBag.path = Path.Combine(_hostingEnvironment.WebRootPath, "Emp_IMg");
            if (employeeMaster == null)
            {
                return NotFound();
            }

            return View(employeeMaster);
        }

        // GET: EmployeeMasters/Create
        public async Task<IActionResult> Create()
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
            ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
            return View();
        }

        // POST: EmployeeMasters/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Employee_Id,Employee_Name,Department_Id,JoiningDate,Address,Email,MobileNo,Image")] EmployeeMaster employeeMaster)
        {
            if (ModelState.IsValid)
            {
                if (_context.EmployeeMaster.Any(name => name.Email.Equals(employeeMaster.Email) && name.IsDelete == 0))
                {
                    ModelState.AddModelError(string.Empty, "Employee is already exists");
                    ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
                    return View(employeeMaster);
                }

                else
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var Image in files)
                    {
                        if (Image != null && Image.Length > 0)
                        {
                            var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                            var fileExt = System.IO.Path.GetExtension(Image.FileName).Substring(1);
                            if (!supportedTypes.Contains(fileExt))
                            {
                                string ErrorMessage = "File Extension Is InValid - Only Upload Image File";
                                ModelState.AddModelError(string.Empty, ErrorMessage);
                                ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
                                return View(employeeMaster);
                            }
                            var file = Image;
                            var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "Emp_IMg");

                            if (file.Length > 0)
                            {
                                var fileName = ContentDispositionHeaderValue.Parse
                                    (file.ContentDisposition).FileName.ToString().Trim('"');

                                System.Console.WriteLine(fileName);
                                using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                    employeeMaster.Image = file.FileName;
                                }


                            }
                        }
                    }
                    //employeeMaster.Department_Id = 2;
                    employeeMaster.IsDelete = 0;
                    _context.Add(employeeMaster);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
            return View(employeeMaster);
        }

        // GET: EmployeeMasters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("LogIn", "Account");
            }
            var employeeMaster = await _context.EmployeeMaster.SingleOrDefaultAsync(m => m.Employee_Id == id);
            if (employeeMaster == null)
            {
                return NotFound();
            }
            if (user.PhoneNumber.ToString().Equals("GuestUser") && !user.Email.ToString().Equals(employeeMaster.Email))
            {
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return NotFound();
            }


            ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
            return View(employeeMaster);
        }

        // POST: EmployeeMasters/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Employee_Id,Employee_Name,Department_Id,JoiningDate,Address,Email,MobileNo,Image")] EmployeeMaster employeeMaster)
        {
            //if (id != employeeMaster.Employee_Id)
            //{
            //    return NotFound();
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.EmployeeMaster.Any(name => name.Email.Equals(employeeMaster.Email) && name.Employee_Id != employeeMaster.Employee_Id && name.IsDelete == 0))
                    {
                        ModelState.AddModelError(string.Empty, "Employee is already exists");
                        ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
                        return View(employeeMaster);
                    }
                    else
                    {
                        var files = HttpContext.Request.Form.Files;
                        foreach (var Image in files)
                        {
                            if (Image != null && Image.Length > 0)
                            {
                                var supportedTypes = new[] { "jpg", "jpeg", "png", "gif", "bmp" };
                                var fileExt = System.IO.Path.GetExtension(Image.FileName).Substring(1);
                                if (!supportedTypes.Contains(fileExt))
                                {
                                    string ErrorMessage = "File Extension Is InValid - Only Upload Image File";
                                    ModelState.AddModelError(string.Empty, ErrorMessage);
                                    ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
                                    return View(employeeMaster);
                                }

                                var file = Image;
                                var uploads = Path.Combine(_hostingEnvironment.WebRootPath, "Emp_IMg");

                                if (file.Length > 0)
                                {
                                    var fileName = ContentDispositionHeaderValue.Parse
                                        (file.ContentDisposition).FileName.ToString().Trim('"');

                                    System.Console.WriteLine(fileName);
                                    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
                                    {
                                        await file.CopyToAsync(fileStream);
                                        employeeMaster.Image = file.FileName;
                                    }


                                }
                            }
                        }
                        //  employeeMaster.Department_Id = 2;
                        employeeMaster.IsDelete = 0;
                        _context.Update(employeeMaster);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeMasterExists(employeeMaster.Employee_Id))
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
            ViewData["Department_Id"] = new SelectList(_context.DepartmentMaster.Where(s => s.IsDelete == 0), "Department_Id", "Department_Name");
            return View(employeeMaster);
        }

        // GET: EmployeeMasters/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            var employeeMaster = await _context.EmployeeMaster
                .Include(e => e.DepartmentMaster)
                .SingleOrDefaultAsync(m => m.Employee_Id == id);
            if (user.PhoneNumber.ToString().Equals("GuestUser") && !user.Email.ToString().Equals(employeeMaster.Email))
            {
                return RedirectToAction("Index");
            }
            ViewBag.image = employeeMaster.Image;
            ViewBag.path = Path.Combine(_hostingEnvironment.WebRootPath, "Emp_IMg");
            if (employeeMaster == null)
            {
                return NotFound();
            }

            return View(employeeMaster);
        }

        // POST: EmployeeMasters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employeeMaster = await _context.EmployeeMaster.SingleOrDefaultAsync(m => m.Employee_Id == id);
            try
            {
                employeeMaster.IsDelete = 1;
                _context.Update(employeeMaster);
                //  _context.EmployeeMaster.Remove(employeeMaster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                string message = string.Format("<b>Message:</b> {0}<br /><br />", ex.Message);
                message += string.Format("<b>StackTrace:</b> {0}<br /><br />", ex.StackTrace.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>Source:</b> {0}<br /><br />", ex.Source.Replace(Environment.NewLine, string.Empty));
                message += string.Format("<b>TargetSite:</b> {0}", ex.TargetSite.ToString().Replace(Environment.NewLine, string.Empty));
                ModelState.AddModelError(string.Empty, message);
            }
            return View(employeeMaster);
        }

        private bool EmployeeMasterExists(int id)
        {
            return _context.EmployeeMaster.Any(e => e.Employee_Id == id);
        }
    }
}
