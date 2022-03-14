using Assignment12_UnitTest.Models;

namespace Assignment12_UnitTest.Services;

public interface IPersonService{
  
    public List<Person> GetAll();
    public Person GetOne(int index);
    public void Create(Person person);
   
    public void Update(int index, Person person);
    
    public void Delete(int index);
}