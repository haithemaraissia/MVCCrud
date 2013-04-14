using System.Linq;
using Example.Models;
using System.Web.Mvc;
using Softwarehouse.MvcCrud.Controllers;

namespace Example.Controllers
{
    public class ProductController : MvcGridRepositoryCrudController<int, tblProduct, ProductRepository, productGrid>
    {
        //Overloaded method to fill the select list before loading the view.
        protected override void CreateEditSetup(tblProduct item)
        {
            CategoryRepository categoryRepository = new CategoryRepository(repository.db);
            ViewData["Categories"] = new SelectList(categoryRepository.AllOrdered(), "id", "name", item.id);
            ViewData["ComponentGrid"] = new ComponentGrid(item.id);
        }

        //normally none of this would be needed its only to handle the components part.
        #region Components
 
        public ActionResult ComponentAdd(tblComponent component)
        {
            if (ModelState.IsValid)
            {
                ComponentRepository componentRepository = new ComponentRepository(repository.db);
                componentRepository.Create(component);
                return RedirectToAction("Edit/" + component.product_id);
            }
            return Edit(component.product_id);
        }

        public ActionResult ComponentRemove(int id)
        {
            ComponentRepository componentRepository = new ComponentRepository(repository.db);
            int product_id = componentRepository.Single(id).product_id;
            componentRepository.Delete(id);
            return RedirectToAction("Edit/" + product_id);
        }

        //Needs to handle showing only the items for a specific product.
        public ActionResult ComponentGridData(int id, string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            ComponentRepository componentRepository = new ComponentRepository(repository.db);
            using (ComponentGrid grid = new ComponentGrid(id) { items = Enumerable.Where(componentRepository.All(), s => s.product_id == id).AsQueryable() })
            {
                return grid.GenerateGridData(sidx, sord, page, rows, _search, searchField, searchOper, searchString);
            }
        }
        
        #endregion
    }
}
