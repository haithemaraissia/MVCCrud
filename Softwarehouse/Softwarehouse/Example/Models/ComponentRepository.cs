using System.Data.Linq;
using Softwarehouse.MvcCrud.JQuery;
using Softwarehouse.MvcCrud.Repository;

namespace Example.Models
{
    public class ComponentRepository : LinqToSqlRepository<int, tblComponent>
    {
        public ComponentRepository()
            : this(new ExampleDataContext())
        { }

        public ComponentRepository(DataContext db)
            : base(db, "id", "name")
        { }
    }

    //Custom model binder for more detailed error messages
    public class ComponentModelBinder : AutomatedModelBinder<tblComponent>
    {
        public override void ExtraBindings()
        {
            parseFromForm("product_id", "No product ID");
            parseFromForm("name", "Please enter a name", true); //cannot be blank
        }
    }


    public class ComponentGrid : GenericDataGrid<tblComponent>
    {
        //Customise the constructor to handle passing through the product_id
        // Common problem is to not do /Product/ComponentGridData and only write it as Product/ComponentGridData this will not work cos we are in /Product/Edit already
        public ComponentGrid(int product_id)
            : base("component_list", "Components", "/Product/ComponentGridData/" + product_id, "", "/Product/ComponentRemove/@0", "id", "name"
            , new[] { new DataGridCol("name", "Name", 150) } )
        {
        }
    }

}