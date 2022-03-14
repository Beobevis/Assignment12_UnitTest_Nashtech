using System.Diagnostics;

using Microsoft.AspNetCore.Mvc;

using Assignment12_UnitTest.Models;
using Assignment12_UnitTest.Services;

namespace Assignment12_UnitTest.Controllers;

public class MemberController : Controller
{
    private readonly IPersonService _personService;
    private readonly ILogger _logger;
    public MemberController(ILogger logger, IPersonService personService)
    {
        _logger = logger;
        _personService = personService;
    }
    static List<Person> persons = new List<Person>
        {
            new Person
            {
                LastName = "Phuong",
                FirstName = "Nguyen Nam",
                Gender = "Male",
                DOB = new DateTime(2001, 1, 22),
                PhoneNumber = "",
                BirthPlace = "Phu Tho",
                IsGraduated = false
            },
            new Person
            {
                LastName = "Nam",
                FirstName = "Nguyen Thanh",
                Gender = "Male",
                DOB = new DateTime(2001, 1, 20),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            },
            new Person
            {
                LastName = "Son",
                FirstName = "Do Hong",
                Gender = "Male",
                DOB = new DateTime(2000, 11, 6),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            },
            new Person
            {
                LastName = "Huy",
                FirstName = "Nguyen Duc",
                Gender = "Male",
                DOB = new DateTime(1996, 1, 26),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            },
            new Person
            {
                LastName = "Hoang",
                FirstName = "Phuong Viet",
                Gender = "Male",
                DOB = new DateTime(1999, 2, 5),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            },
            new Person
            {
                LastName = "Long",
                FirstName = "Lai Quoc",
                Gender = "Male",
                DOB = new DateTime(1997, 5, 30),
                PhoneNumber = "",
                BirthPlace = "Bac Giang",
                IsGraduated = false
            },
            new Person
            {
                LastName = "Thanh",
                FirstName = "Tran Chi",
                Gender = "Male",
                DOB = new DateTime(2000, 9, 18),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            },
            new Person
            {
                LastName = "Person",
                FirstName = "Old",
                Gender = "Male",
                DOB = new DateTime(1996, 1, 14),
                PhoneNumber = "",
                BirthPlace = "Ha Noi",
                IsGraduated = false
            }
        };
    public IActionResult Index()
    {
        var people = _personService.GetAll();
        return View(people);
    }
    public IActionResult AddPerson()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult AddPerson(Person model)
    {
        if (!ModelState.IsValid) return View();

        _personService.Create(model);

        return RedirectToAction("Index");
    }

    public IActionResult EditPerson(int index)
    {
        try
        {
            var person = _personService.GetOne(index);
            ViewBag.PersonIndex = index;
            return View(person);
        }
        catch (IndexOutOfRangeException ex)
        {
            if(ex is IndexOutOfRangeException){
            return NotFound(ex.Message);
            }
            else{ throw ex; }
        }
    }
    [HttpPost]
    public IActionResult EditPerson(int index, Person model)
    {
        if (!ModelState.IsValid) return View();
        _personService.Update(index, model);

        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult DeletePerson(int index)
    {
        try
        {
            _personService.Delete(index);
        }
        catch (IndexOutOfRangeException ex)
        {
            if( ex is IndexOutOfRangeException){ return NotFound(ex.Message);}
            else { throw ex; }
            
        }

        return RedirectToAction("Index");
    }

    public IActionResult Detail(int index)
    {
        try
        {
            var person = _personService.GetOne(index);
            ViewBag.PersonIndex = index;
            return View(person);
        }
        catch (IndexOutOfRangeException ex)
        {
            if (ex is IndexOutOfRangeException)
            {
                //return RedirectToAction("Index");
                return NotFound(ex.Message);
            }
            else { throw ex; }


        }
    }
    [HttpPost]
    public IActionResult DeleteWithResult(int index)
    {
        var deletedUserName = string.Empty;
        try
        {
            var person = _personService.GetOne(index);
            deletedUserName = person.FullName;
            //HttpContext.Session.SetString("DELETED_USER_NAME",person.FullName);
            _personService.Delete(index);

        }
        catch (System.Exception)
        {

        }
        return View("ResultDeletePage", deletedUserName);
    }
    public IActionResult Result()
    {

        var deletedUserName = HttpContext.Session.GetString("DELETED_USER_NAME");
        ViewBag.DeletedUserName = deletedUserName;
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

   
}
