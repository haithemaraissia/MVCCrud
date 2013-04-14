<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<tblCategory>" %>

    <%= Html.ValidationSummary("Edit was unsuccessful. Please correct the errors and try again.") %>

    <% using (Html.BeginForm()) {%>

        <fieldset>
            <legend>Fields</legend>
            <p>
                <label for="name">name:</label>
                <%= Html.TextBox("name", Model.name) %>
                <%= Html.ValidationMessage("name", "*") %>
            </p>
            <p>
                <input type="submit" value="Save" />
                <%= Html.Hidden("id", Model.id) %>
            </p>
        </fieldset>

    <% } %>

    <div>
        <%=Html.ActionLink("Back to List", "Index") %>
    </div>


