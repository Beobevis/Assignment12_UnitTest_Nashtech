using Assignment12_UnitTest.Controllers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;


using Microsoft.AspNetCore.Mvc;

using System;
using System.Linq;
using System.Collections.Generic;
using Assignment12_UnitTest.Models;
using Assignment12_UnitTest.Services;

namespace MVC.Test;

public class MemberControllerTest
{

    private Mock<ILogger<MemberController>> _loggerMock;

    private Mock<IPersonService> _personServiceMock;

    static List<Person> _people = new List<Person>{
        new Person{
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
    };

    [SetUp]
    public void Setup()
    {
        _loggerMock = new Mock<ILogger<MemberController>>();
        _personServiceMock = new Mock<IPersonService>();
        //Setup
        _personServiceMock.Setup(x => x.GetAll()).Returns(_people);

    }

    [Test]
    public void Index_ReturnViewResult_WithAListOfMembers()
    {

        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);
        var expectedCount = _people.Count;
        //Act
        var result = controller.Index();

        //Assert
        Assert.IsInstanceOf<ViewResult>(result, "Invalid return type");

        var view = result as ViewResult;
        Assert.IsAssignableFrom<List<Person>>(view?.ViewData.Model, " Invalid view data Model");

        var model = view?.ViewData.Model as List<Person>;
        Assert.IsNotNull(model, "View Data model must not be null!");
        Assert.AreEqual(expectedCount, model?.Count, " Model count is not equal to the expected count!");

        // var firstPerson = model?.First();
        // Assert.AreEqual("Nguyen Duc Huy", firstPerson.FullName, "First person is not equal");
    }

    [Test]
    public void Detail_ValidIndex_ReturnViewData_WithAPerson()
    {
        //Setup
        const int index = 2;
        _personServiceMock.Setup(x => x.GetOne(index)).Returns(_people[index - 1]);
        var ExpectedPerson = _people[index - 1];

        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);

        //Act
        var result = controller.Detail(index);

        //Assert
        Assert.IsInstanceOf<ViewResult>(result, "Invalid return type");

        var view = result as ViewResult;
        Assert.IsAssignableFrom<Person>(view?.ViewData.Model, " Invalid view data Model");

        var model = view?.ViewData.Model as Person;
        Assert.IsNotNull(model, "View Data model must not be null!");
        Assert.AreEqual(ExpectedPerson, model, " Model count is not equal to the expected!");
    }

    [Test]
    public void Detail_InvalidIndex_ReturnsNotFoundObject_WithStringMessage()
    {
        const int index = 15;
        const string message = "Index out of range.";
        _personServiceMock.Setup(x => x.GetOne(index)).Throws(new IndexOutOfRangeException(message));
        // var ExpectedPerson = _people[index - 1];

        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);

        //Act
        var result = controller.Detail(index);

        //Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result, "Invalid return type");
        var view = result as NotFoundObjectResult;
        Assert.IsNotNull(view, "View must not be Null");
        Assert.IsInstanceOf<string>(view?.Value, "Invalid data type");
        Assert.AreEqual(message, view?.Value?.ToString(), "Not Equals");
        // var view = result as ViewResult;
        // Assert.IsAssignableFrom<Person>(view.ViewData.Model, " Invalid view data Model");

        // var model = view.ViewData.Model as Person;
        // Assert.IsNotNull(model, "View Data model must not be null!");
        // Assert.AreEqual(ExpectedPerson, model, " Model count is not equal to the expected!");
    }

    [Test]
    public void Detail_InvalidIndex_Throws_Exception()
    {
        //Setup 
        const int index = -1;
        const string message = "Index must be greater than zero.";
        _personServiceMock.Setup(x => x.GetOne(index)).Throws(new ArgumentException(message));
        // var ExpectedPerson = _people[index - 1];

        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);

        //Act
        //var result = controller.Detail(index);

        //Assert
        var exception = Assert.Throws<ArgumentException>(() => controller.Detail(index));
        Assert.IsNotNull(exception, "exception must not be Null");
        Assert.AreEqual(message, exception?.Message, "Not Equals");
    }

    [Test]
    public void Add_ValidIndex_ReturnsViewData_WithErrorInModelState()
    {
        const string key = "ERROR";
        const string message = "Invalid model!!";

        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);
        controller.ModelState.AddModelError(key, message);

        //Act
        var result = controller.AddPerson(null);

        //Assert
        Assert.IsInstanceOf<ViewResult>(result, "Invalid input type");

        var view = (ViewResult)result;
        var modelState = view.ViewData.ModelState;

        Assert.IsFalse(modelState.IsValid, "Invalid model state");
        Assert.AreEqual(1, modelState.ErrorCount, "");
        modelState.TryGetValue(key, out var value);
        var error = value;

    }

    [Test]
    public void Add_ValidIndex_ReturnsRedirectToActionIndex()
    {
        var person = new Person
        {
            LastName = "Huy111",
            FirstName = "Nguyen Duc",
            Gender = "Male",
            DOB = new DateTime(1996, 1, 26),
            PhoneNumber = "",
            BirthPlace = "Ha Noi",
            IsGraduated = false
        };

        _personServiceMock.Setup(x => x.Create(person)).Callback(() => _people.Add(person));
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);
        var expectedCount = _people.Count + 1;
        //Act
        var result = controller.AddPerson(person);

        //Assert
        Assert.IsInstanceOf<RedirectToActionResult>(result, "Invalid return type!");
        var view = result as RedirectToActionResult;
        // Assert.IsAssignableFrom<Person>(view.ViewData.Model," Invalid");
        Assert.AreEqual("Index", view?.ActionName, "Invalid action name!");

        var actual = _people.Count;
        Assert.AreEqual(expectedCount, actual, "ERROR!");

        Assert.AreEqual(person, _people.Last(), "Not Equals");
    }

    [Test]
    public void Edit_InvalidIndex_ThrowsException()
    {
        const int index = 15;
        const string message = "Index out of range.";
        _personServiceMock.Setup(x => x.GetOne(index)).Throws(new IndexOutOfRangeException(message));
        // var ExpectedPerson = _people[index - 1];

        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);

        //Act
        var result = controller.EditPerson(index);

        //Assert
        Assert.IsInstanceOf<NotFoundObjectResult>(result, "Invalid return type");
        var view = result as NotFoundObjectResult;
        Assert.IsNotNull(view, "View must not be Null");
        Assert.IsInstanceOf<string>(view?.Value, "Invalid data type");
        Assert.AreEqual(message, view?.Value?.ToString(), "Not Equals");
    }
    [Test]
    public void Edit_ValidIndex_ReturnsViewData_WithErrorInModelState()
    {
        const string key = "ERROR";
        const string message = "Invalid model!!";
        //Setup
        const int index = 2;
        _personServiceMock.Setup(x => x.GetOne(index)).Returns(_people[index - 1]);
        var ExpectedPerson = _people[index - 1];
        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);
        controller.ModelState.AddModelError(key, message);


        //Act
        var result = controller.EditPerson(index);

        //Assert
        Assert.IsInstanceOf<ViewResult>(result, "Invalid input type");

        var view = result as ViewResult;
        Assert.IsAssignableFrom<Person>(view?.ViewData.Model, " Invalid view data Model");

        var model = view?.ViewData.Model as Person;
        Assert.IsNotNull(model, "View Data model must not be null!");
        Assert.AreEqual(ExpectedPerson, model, " Model count is not equal to the expected!");
    }

    // [Test]
    // public void Edit_ValidIndex_ReturnRedirectToActionIndex()
    // {
    //     int index = 1;
    //     var person = new Person
    //     {
    //         LastName = "Huy111",
    //         FirstName = "Nguyen Duc",
    //         Gender = "Male",
    //         DOB = new DateTime(1996, 1, 26),
    //         PhoneNumber = "",
    //         BirthPlace = "Ha Noi",
    //         IsGraduated = false
    //     };
    //     _personServiceMock.Setup(x => x.Update(index,person)).Callback(() => _people.EditPerson(person));
    //     var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);
    //     var expectedCount = _people.Count + 1;
    //     //Act
    //     var result = controller.EditPerson(index,person);

    //     //Assert
    //     Assert.IsInstanceOf<RedirectToActionResult>(result, "Invalid return type!");
    //     var view = result as RedirectToActionResult;
    //     // Assert.IsAssignableFrom<Person>(view.ViewData.Model," Invalid");
    //     Assert.AreEqual("Index", view?.ActionName, "Invalid action name!");

    //     var actual = _people.Count;
    //     Assert.AreEqual(expectedCount, actual, "ERROR!");

    //     Assert.AreEqual(person, _people.Last(), "Not Equals");
    [Test]
    public void Delete_InvalidIndex_Throws_Exception(){
        const int index = -1;
        const string message = "Index must be greater than zero.";
        _personServiceMock.Setup(x => x.GetOne(index)).Throws(new ArgumentException(message));
        // var ExpectedPerson = _people[index - 1];

        //Arrange
        var controller = new MemberController(_loggerMock.Object, _personServiceMock.Object);

        //Act
        //var result = controller.Detail(index);

        //Assert
        var exception = Assert.Throws<ArgumentException>(() => controller.Detail(index));
        Assert.IsNotNull(exception, "exception must not be Null");
        Assert.AreEqual(message, exception?.Message, "Not Equals");   
    }

    [TearDown]
    public void TearDown()
    {
        // _loggerMock = null;
        // _personServiceMock = null;
    }
}

