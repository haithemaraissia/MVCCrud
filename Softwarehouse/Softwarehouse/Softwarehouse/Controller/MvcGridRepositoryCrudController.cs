using System.Linq;
using System.Web.Mvc;
using Softwarehouse.MvcCrud.JQuery;
using Softwarehouse.MvcCrud.Repository;

namespace Softwarehouse.MvcCrud.Controllers
{
    public class MvcGridRepositoryCrudController<TKeyType, TModel, TRepository, TGrid> : MvcRepositoryCrudController<TKeyType, TModel, TRepository>
        where TRepository : IMvcCrudRepository<TKeyType, TModel>, new()
        where TModel : new()
        where TGrid : GenericDataGrid<TModel>, new()
        {
            public override ActionResult Index()
            {
                ViewData.Model = new TGrid();
                return View("Index");
            }

            public virtual ActionResult GridData(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
            {
                using (TGrid grid = new TGrid { items = repository.All() })
                {
                    return grid.GenerateGridData(sidx, sord, page, rows, _search, searchField, searchOper, searchString);
                }
            }

            public virtual ActionResult GridDataFromList(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
            {
                using (TGrid grid = new TGrid { items = repository.All().ToList().AsQueryable() })
                {
                    return grid.GenerateGridData(sidx, sord, page, rows, _search, searchField, searchOper, searchString);
                }
            }
        }
}
