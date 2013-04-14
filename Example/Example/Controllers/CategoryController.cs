using Softwarehouse.MvcCrud.Controllers;
using Example.Models;

namespace Example.Controllers
{
    //Long way round just to demo how it would work with a repository that didnt implement the interface.
    public class CategoryController : GenericCrudController<int, tblCategory>
    {
        //Create instance of some kind of data interface, in this case a LinqToSqlRepository
        readonly CategoryRepository repository = new CategoryRepository();
        public CategoryController()
        {
            //Point the delegates to the appropriate functions.
            CreateItem = item => repository.Create(item);
            SaveItem = item => repository.Save(item);
            DeleteItem = id => repository.Delete(id);
            AllItems = () => repository.AllOrdered();
            SingleItem = id => repository.Single(id);
        }
    }
}
