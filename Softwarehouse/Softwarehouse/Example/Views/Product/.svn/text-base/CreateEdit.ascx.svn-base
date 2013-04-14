<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Example.Models.tblProduct>" %>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="category_id">category:</label>
                <%= Html.DropDownList("category_id", (SelectList)ViewData["Categories"]) %>
                <%= Html.ValidationMessage("category_id", "*") %>
            </p>
            <p>
                <label for="name">name:</label>
                <%= Html.TextBox("name", Model.name) %>
                <%= Html.ValidationMessage("name", "*") %>
            </p>
            <p>
                <label for="price">price:</label>
                <%= Html.TextBox("price", String.Format("{0:F}", Model.price)) %>
                <%= Html.ValidationMessage("price", "*") %>
            </p>
            <p>
                <label for="start_date">start_date:</label>
                <%= Html.TextBox("start_date", Model.start_date.ToString("dd-MMM-yyyy")) %>
                <%= Html.ValidationMessage("start_date", "*")%>
            </p>
            <p>
                <input type="submit" value="Save" />
                <%= Html.Hidden("id", Model.id) %>
                <input type="hidden" name="alwaysNull" value="1" />
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>


