using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace Softwarehouse.MvcCrud.JQuery
{
    public abstract class GenericDataGrid<TModel> : Controller
        where TModel : new()
    {
        public IQueryable<TModel> items;
        protected string gridName;
        protected string gridHeading;
        public int gridWidth = 0;
        protected int gridMinWidth = 400;

        protected string jsonUrl;
        protected string primaryKey;
        protected string orderBy;
        protected string sortOrder;
        protected string extraGridSettings = "";
        protected string addUrl = "";
        protected string editUrl = "";
        protected string deleteUrl = "";

        public List<DataGridActionButton<TModel>> actionButtons;
        protected string actionButtonsHeading = "";
        private int actionButtonsWidth{
            get{
                return (actionButtons.Count * 20) + (string.IsNullOrEmpty(checkboxGroupName) ? 0 : 18);
            }
        }

        public List<DataGridNavButton> navButtons;

        private string _checkboxGroupName = "";
        public string checkboxGroupName
        {
            get
            {
                return _checkboxGroupName;
            }
            set
            {
                _checkboxGroupName = value;
                navButtons.Add(new DataGridNavButton { classTag = "ui-icon-check", onclick = string.Format("function(){{ $(\"INPUT[name='{0}']\").each(function(){{ $(this).attr('checked', $(this).is(':checked') ? '' : 'checked'); }}); }}", checkboxGroupName), title = "Invert Selection", position = DataGridNavButton.eNavButtonPosition.last });
            }
        }
        protected string rowList = "20, 30, 50, 100";
        protected int rows = 20;
        protected string themeName = "redmond";
        protected string locale = "en";
        protected string decimalFormat = "N02";
        protected Func<TModel, bool> deleteExecuteCheck = (e => true);
        protected Func<TModel, bool> editExecuteCheck = (e => true);

        public string dateTimeFormat = "dd-MMM-yyyy";

        public DataGridCol[] data_rows;

        //Short version specific to mvccrud

        public GenericDataGrid(string controllerName, string primaryKey, string orderBy, DataGridCol[] dataColumns)
            : this(
              controllerName + "Grid"
            , controllerName
            , string.Format("/{0}/GridData", controllerName)
            , string.Format("/{0}/Create", controllerName)
            , string.Format("/{0}/Edit/@0", controllerName)
            , string.Format("/{0}/Delete/@0", controllerName)
            , primaryKey, orderBy, dataColumns)
        { }

        //Legacy for old versions
        public GenericDataGrid(string gridName, string gridHeading, string jsonUrl, string editUrl, string deleteUrl, string primaryKey, string orderBy, DataGridCol[] dataColumns)
            : this(gridName, gridHeading, jsonUrl, "", editUrl, deleteUrl, primaryKey, orderBy, dataColumns)
        {}

        public GenericDataGrid(string gridName, string gridHeading, string jsonUrl, string addUrl, string editUrl,string deleteUrl, string primaryKey, string orderBy, DataGridCol[] dataColumns)
        {
            this.gridName = gridName;
            this.gridHeading = gridHeading;

            this.jsonUrl = jsonUrl;
            this.addUrl = addUrl;
            this.editUrl = editUrl;
            this.deleteUrl = deleteUrl;

            this.data_rows = dataColumns;

            this.primaryKey = primaryKey;

            string[] sorting = orderBy.Split(' ');
            this.orderBy = sorting[0];
            sortOrder = (sorting.Length > 1) ? sorting[1] : "asc";
            this.actionButtons = new List<DataGridActionButton<TModel>>();
        }

        private void SetupButtons()
        {
            if(!string.IsNullOrEmpty(editUrl))
                actionButtons.Add(new DataGridActionButton<TModel>
                                      {
                                          title = "Edit",
                                          classTag = "ui-icon ui-icon-pencil",
                                          href = editUrl,
                                          executeCheck = editExecuteCheck,
                                          parameters = new [] { primaryKey }
                                      });
            if(!string.IsNullOrEmpty(deleteUrl))
                actionButtons.Add(new DataGridActionButton<TModel>
                {
                    title = "Delete",
                    classTag = "ui-icon ui-icon-trash",
                    href = deleteUrl,
                    onclick = "event.returnValue = confirm('Are you sure you want to delete this item?');",
                    executeCheck = deleteExecuteCheck,
                    parameters = new[] { primaryKey }
                });
            navButtons = new List<DataGridNavButton>();

            if (!string.IsNullOrEmpty(addUrl))
                navButtons.Add(new DataGridNavButton { classTag = "ui-icon-plus", onclick = string.Format("function(){{ document.location.href = '{0}'; }}", addUrl), title = "Create New", position = DataGridNavButton.eNavButtonPosition.first });

        }

        public ActionResult GenerateGridData(string sidx, string sord, int page, int rows, bool _search, string searchField, string searchOper, string searchString)
        {
            SetupButtons();
            //Generate Variables
            int pageIndex = Convert.ToInt32(page) - 1;
            int pageSize = rows;
            int totalRecords = items.Count();
            int totalPages = (int)Math.Ceiling((float)totalRecords / pageSize);
          
            if (_search)
            {
                //check type of string
                Type final_type = getFinalType(searchField);

                object searchObject;
                if (final_type == typeof (string))
                {
                    searchObject = searchString;
                }
                else
                {
                    MethodInfo parse = final_type.GetMethod("Parse", new [] { typeof(string) });
                    searchObject = parse.Invoke(null, new object[] { searchString });
                }

                switch (searchOper)
                {
                    case ("eq"):
                        items = items.Where(string.Format("{0} == @0", searchField), searchObject);
                        break;
                    case ("ne"):
                        items = items.Where(string.Format("{0} != @0", searchField), searchObject);
                        break;
                    case ("lt"):
                        items = items.Where(string.Format("{0} < @0", searchField), searchObject);
                        break;
                    case ("le"):
                        items = items.Where(string.Format("{0} <= @0", searchField), searchObject);
                        break;
                    case ("gt"):
                        items = items.Where(string.Format("{0} > @0", searchField), searchObject);
                        break;
                    case ("ge"):
                        items = items.Where(string.Format("{0} >= @0", searchField), searchObject);
                        break;
                    case ("cn"):
                        //Could not get contains working in Dynamic Linq so done manually in code.
                        List<TModel> results = new List<TModel>();
                        foreach(TModel item in items)
                        {
                            try
                            {
                                object tmp = getObjectValue(searchField, item);
                                if (tmp.ToString().Contains((string)searchObject))
                                {
                                    results.Add(item);
                                }
                            }
                            catch (Exception)
                            {}
                        }

                        items = results.AsQueryable();
                        break;
                }
            }

            items = items
                .OrderBy(string.Format("{0} {1}", sidx, sord))
                .Skip(pageIndex * pageSize)
                .Take(pageSize);

            //Generate JSON
            var jsonData = new
            {
                total = totalPages,
                page,
                records = totalRecords,
                rows = (items.Select(p => new
                {
                    //id column from repository
                    i = typeof(TModel).GetProperty(primaryKey).GetValue(p, null),
                    cell = getCells(p)
                })).ToArray()
            };
            return Json(jsonData);
        }

        //Breaks string down and gets value
        private object getObjectValue(string searchField, TModel item)
        {
            //hack for tblcategory.name
            string[] parts = searchField.Split('.');

            //Set first part
            PropertyInfo c = typeof(TModel).GetProperty(parts[0]);
            object tmp = c.GetValue(item, null);

            //loop through if there is more than one depth to the . eg tblCategory.name
            for (int j = 1; j < parts.Length; j++)
            {
                c = tmp.GetType().GetProperty(parts[j]);
                tmp = c.GetValue(tmp, null);
            }

            return tmp;
        }

        protected virtual string actionCell(TModel line)
        {
            // initialize the string and add a checkbox to the row if necessary
            string actionButtonString = actionButtons.Count > 0 || !string.IsNullOrEmpty(checkboxGroupName) ? "" : null;

            if (!string.IsNullOrEmpty(checkboxGroupName)){
                actionButtonString = string.Format("<input type='checkbox' style='vertical-align:middle;' name='{0}' title='Click to select this row' />", checkboxGroupName);
            }

            PropertyInfo c = typeof (TModel).GetProperty(primaryKey);
            foreach (DataGridActionButton<TModel> button in actionButtons.Where(a => a.executeCheck.Invoke(line)))
                actionButtonString += string.Format("<a href=\"{0}\" onclick=\"{1}\" title=\"{2}\" class=\"{3}\" style=\"display:inline-block;{4}\" {5}>&nbsp;</a>",
                                 buildHref(button, line),
                                 button.onclick,
                                 button.title,
                                 button.classTag,
                                 button.styleTag,
                                 button.extras);
            return actionButtonString;
        }

        //Used to replace {0} etc in href of buttons
        private string buildHref(DataGridActionButton<TModel> button, TModel line)
        {
            int c = 0;
            string href = button.href;
            foreach(string parameter in button.parameters)
            {
                try
                {
                    object value =  getObjectValue(parameter, line);
                    href = href.Replace("@" + c, value.ToString());
                }
                catch (Exception)
                {
                }
                c++;
            }
            return href;
        }

        private string[] getCells(TModel p)
        {
            List<string> result = new List<string>();

            string a = actionCell(p);
            if (a != null)
            {
                result.Add(a);
            }

            foreach(string column in data_rows.Select(r => r.value))
            {
                try
                {
                    //hack for tblcategory.name
                    string[] parts = column.Split('.');

                    //Set first part
                    PropertyInfo c = typeof(TModel).GetProperty(parts[0]);
                    object tmp = c.GetValue(p, null);
                    
                    //loop through if there is more than one depth to the . eg tblCategory.name
                    for (int j = 1; j < parts.Length; j++)
                    {
                        c = tmp.GetType().GetProperty(parts[j]);
                        tmp = c.GetValue(tmp, null);
                    }

                    if (tmp.GetType() == typeof(DateTime))
                    {
                        result.Add(((DateTime)tmp).ToString(dateTimeFormat));
                    }
                    else if (tmp.GetType() == typeof(float))
                    {
                        result.Add(((float)tmp).ToString(decimalFormat));
                    }
                    else if (tmp.GetType() == typeof(double))
                    {
                        result.Add(((double)tmp).ToString(decimalFormat));
                    }
                    else if (tmp.GetType() == typeof(decimal))
                    {
                        result.Add(((decimal)tmp).ToString(decimalFormat));
                    }
                    else
                    {
                        result.Add(tmp.ToString());
                    }
                }
                catch (Exception)
                {
                    result.Add("");
                }
            }
            return result.ToArray();
        }

        public string renderIncludes
        {
            get
            {
                return
                    string.Format(@"<link href=""/Scripts/css/ui.jqgrid.css"" rel=""stylesheet"" type=""text/css"" />
                    <link href=""/Scripts/css/{0}/jquery-ui-1.7.1.custom.css"" rel=""stylesheet"" type=""text/css"" />
                    <script src=""/Scripts/jquery-1.3.2.min.js"" type=""text/javascript""></script>
                    <script src=""/Scripts/i18n/grid.locale-{1}.js"" type=""text/javascript""></script>
                    <script src=""/Scripts/jquery.jqGrid.min.js"" type=""text/javascript""></script>", themeName, locale);
            }
        }

        public string renderGrid
        {
            get
            {
                SetupButtons();
                if (gridWidth == 0)
                {
                    foreach (DataGridCol col in this.data_rows)
                        gridWidth += col.width;
                    gridWidth += actionButtonsWidth;
                }

                return
                    string.Format(
                        @"<script type=""text/javascript"">
                    jQuery(document).ready(function() {{
                        jQuery('#{0}').jqGrid({{
                            url: '{1}',
                            datatype: 'json',
                            mtype:'POST',
                            colNames: [{2}],
                            colModel: [{3}],
                            rowNum: {9},
                            rowList: [{8}],
                            imgpath: ""/Scripts/css/{10}/images"",
                            pager: jQuery('#{0}_pager'),
                            sortname: '{5}',
                            viewrecords: true,
                            sortorder: '{6}',
                            caption: '{4}',
                            width: {12}
                            {7}
                        }}).navGrid('#{0}_pager', {{ edit: false, add: false, del: false, search: true, refresh: true  }}){11};
                    }}); 
                </script>
                <table id=""{0}"" width=""100%"" class=""scroll"" cellpadding=""0"" cellspacing=""0""></table>
                <div id=""{0}_pager"" class=""scroll"" style="" text-align:center;""></div>",
                        gridName, 
                        jsonUrl, 
                        headingsString, 
                        columnsString, 
                        gridHeading, 
                        orderBy, 
                        sortOrder,
                        string.IsNullOrEmpty(extraGridSettings) ? "" : "," + extraGridSettings, 
                        rowList, 
                        rows, 
                        themeName, 
                        navButtonsString,
                        gridWidth < gridMinWidth ? gridMinWidth : gridWidth);
            }
        }

        public string headingsString
        {
            get
            {
                string result = "";
                string delimiter = "";

                if (actionButtons.Count > 0)
                {
                    result += string.Format("{0}'{1}'", delimiter, actionButtonsHeading);
                    delimiter = ", ";
                }

                foreach(string s in data_rows.Select(r => r.heading))
                {
                    result += string.Format("{0}'{1}'", delimiter, s);
                    delimiter = ", ";
                }
                return result;
            }
        }

        public string columnsString
        {
            get
            {
                string result = "";
                string delimiter = "";

                if (actionButtons.Count > 0 || !string.IsNullOrEmpty(checkboxGroupName))
                {
                    result += string.Format("{0}{{ name: 'actionButtons', index: 'actionButtons', width:{1}, sortable:false, search:false }}", delimiter, actionButtonsWidth);
                    delimiter = ", ";
                }

                foreach (DataGridCol row in data_rows)
                {
                    result += string.Format("{0}{{ name: '{1}', index: '{1}', width:{2} {3} {4} }}"
                        , delimiter, row.id, row.width
                        , string.Format(", searchoptions: {{ sopt: [{0}] }}", string.IsNullOrEmpty(row.sopt) ? "'eq', 'ne', 'lt', 'le', 'gt', 'ge', 'cn'" : row.sopt)
                        , string.IsNullOrEmpty(row.extraArguments) ? "" : (string.Format(", {0}", row.extraArguments)));
                    delimiter = ", ";
                }
                return result;
            }
        }

        public string navButtonsString
        {
            get
            {
                string result = string.Empty;
                foreach (DataGridNavButton button in navButtons)
                {
                    if (button.style == DataGridNavButton.eNavButtonStyle.Button)
                    {
                        result += string.Format(@".navButtonAdd('#{0}_pager', {{ caption: '{1}', buttonicon: '{2}', onClickButton: {3}, position: '{4}', cursor: '{5}', title: '{6}', id: '{7}' }})", gridName, button.caption, button.classTag, button.onclick, button.position, button.cursor, button.title, button.id);
                    }
                    else
                    {
                        result += string.Format(@".navSeparatorAdd('#{0}_pager', {{ sepclass: '{1}', sepcontent: '{2}' }})", gridName, button.classTag, button.caption);
                    }
                }
                return result;
            }
        }

        private static Type getFinalType(string input)
        {
            //hack for tblcategory.name
            string[] parts = input.Split('.');

            //Set first part
            PropertyInfo c = typeof(TModel).GetProperty(parts[0]);

            //loop through if there is more than one depth to the . eg tblCategory.name
            for (int j = 1; j < parts.Length; j++)
            {
                c = c.PropertyType.GetProperty(parts[j]);
            }
            return c.PropertyType;

        }
    }

    public struct DataGridActionButton<TModel>
        where TModel : new()
    {
        public string classTag { get; set; }
        public string styleTag { get; set; }
        public string onclick { get; set; }
        public string href { get; set; }
        public string title { get; set; }
        public Func<TModel, bool> executeCheck { get; set; }
        public string extras { get; set; }
        public string[] parameters { get; set; }
    }

    public struct DataGridNavButton
    {
        public enum eNavButtonPosition{
            first,
            last
        }
        public enum eNavButtonStyle{
            Button,
            Separator
        }
        public string classTag { get; set; }
        public string onclick { get; set; }
        public string caption { get; set; }
        public eNavButtonPosition position { get; set; }
        public string cursor { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public eNavButtonStyle style { get; set; }
    }

    public class DataGridCol
    {
        public string value;
        public string id;
        public string heading;
        public string extraArguments;
        public int width;
        public string sopt;

        public DataGridCol(string value, string id, string heading, int width, string extraArguments, string sopt )
        {
            this.value = value;
            this.id = id;
            this.heading = heading;
            this.width = width;
            this.extraArguments = extraArguments;
        }

        public DataGridCol(string value, string id, string heading) : this(value, id, heading, 55, "", "") { }
        public DataGridCol(string value, string id, string heading, int width) : this(value, id, heading, width, "", "") { }
        public DataGridCol(string value, string id, string heading, int width, string extraArguments) : this(value, id, heading, width, extraArguments, "") { }

        public DataGridCol(string value, string heading) : this(value, value, heading, 55, "", "") { }
        public DataGridCol(string value, string heading, int width) : this(value, value, heading, width, "", "") { }
        public DataGridCol(string value, string heading, int width, string extraArguments) : this(value, value, heading, width, extraArguments, "") { }
    }
}
