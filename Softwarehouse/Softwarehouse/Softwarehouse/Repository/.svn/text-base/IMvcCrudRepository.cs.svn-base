//Interface for use with the MvcRepositoryCrudController class
//Developed by Matthew Hood - 2009

using System.Linq;

namespace Softwarehouse.MvcCrud.Repository
{
    public interface IMvcCrudRepository<TKeyType, TModel> where TModel : new()
    {
        string primaryKey { get; set; }
        string orderBy { get; set; }
        IQueryable<TModel> All();
        IQueryable<TModel> AllOrdered();
        TModel Single(TKeyType id);
        void Create(TModel t);
        void Save(TModel t);
        void Delete(TKeyType id);
    } 
}
