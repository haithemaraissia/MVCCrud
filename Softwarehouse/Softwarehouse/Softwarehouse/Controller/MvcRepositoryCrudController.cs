//Generic class to link a IMvcCrudRepository to a GenericCrudController
//Developed by Matthew Hood - 2009

using Softwarehouse.MvcCrud.Repository;

namespace Softwarehouse.MvcCrud.Controllers
{
    public class MvcRepositoryCrudController<TKeyType, TModel ,TRepository> : GenericCrudController<TKeyType, TModel> 
        where TRepository : IMvcCrudRepository<TKeyType, TModel>, new()
        where TModel : new() 
    {
        public TRepository repository = new TRepository();
        public MvcRepositoryCrudController()
        {
            CreateItem = item => repository.Create(item);
            SaveItem = item => repository.Save(item);
            DeleteItem = id => repository.Delete(id);
            AllItems = () => repository.AllOrdered();
            SingleItem = id => repository.Single(id);
        }
    }
}
