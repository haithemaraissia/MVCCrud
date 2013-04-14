using System.Data.Linq;
using Softwarehouse.MvcCrud.Repository;

namespace Example.Models
{
    public class CategoryRepository : LinqToSqlRepository<int, tblCategory>
    {
        public CategoryRepository()
            : this(new ExampleDataContext())
        { }

        public CategoryRepository(DataContext db)
            : base(db, "id", "name")
        { }
    }
}