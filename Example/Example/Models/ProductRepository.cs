using System;
using System.Data.Linq;
using Softwarehouse.MvcCrud.JQuery;
using Softwarehouse.MvcCrud.Repository;

namespace Example.Models
{
    public class ProductRepository : LinqToSqlRepository<int, tblProduct>
    {
        public ProductRepository()
            : this(new ExampleDataContext())
        { }

        public ProductRepository(DataContext db)
            : base(db, "id", "name")
        { }
    }

    //Custom model binder for more detailed error messages
    public class ProductModelBinder : AutomatedModelBinder<tblProduct>
    {
        public override void ExtraBindings()
        {
            parseFromForm("category_id", "No category ID");
            parseFromForm("name", "No name", true); //cannot be blank
            parseFromForm("price", "No price");
            parseFromForm("alwaysNull", "Balls");
            parseFromForm("start_date", "Invalid Date");
        }
    }


    public class productGrid : GenericDataGrid<tblProduct>
    {
        public productGrid()
            : base("Product", "id", "name"
            , new[]
                  {
                      new DataGridCol("id", "Id", 50), 
                      new DataGridCol("name", "Name", 100), 
                      new DataGridCol("tblCategory.name", "Category Name", 150), 
                      new DataGridCol("start_date", "Start Date", 100)
                  })
        {
            base.editExecuteCheck = editExecuteCheck;
            actionButtons.Add(new DataGridActionButton<tblProduct>
                                  {
                                    title = "Other Edit",
                                    classTag = "ui-icon ui-icon-arrowreturnthick-1-n",
                                    href = "/Product/Edit/@0",
                                    parameters = new[] { "id" },
                                    executeCheck = x => !(x.price > 0)
                                  });
        }

        //Example of a possible editExecuteCheck, ie can only edit items with a price > 0
        private static Func<tblProduct, bool> editExecuteCheck = p => p.price > 0;
    } 
}