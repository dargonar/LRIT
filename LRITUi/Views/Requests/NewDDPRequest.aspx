<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeadInfo" runat="server">

<script language="javascript" type="text/javascript">
  $(document).ready(function() {
    $("#updateType").change(function() {
      if ($(this).val() == 4)
        $("#archived").show();
      else
        $("#archived").hide();
    });

    $("#archivedTimestamp").datetimepicker({ dateFormat: 'yy-mm-dd' });
  });
</script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="Contenido" runat="server">
    <div class="content_block">
    <h2>Nueva Solicitud DDP</h2>

    <% using (Html.BeginForm("CreateAndSendDDPRequest", "Requests"))
       {%>

            <% if (ViewData["error"] != null)
               {
                 %>    
                 <div class="message error">
                        <p><strong><%=ViewData["error"]%></strong></p>
                 </div>
                 <%
               } 
            %>
            <p>
                <label for="AreaIndex">Update Type</label>
                <%= Html.DropDownList("updateType", new SelectList(new[] { 
                                            new { @id = 0, @value = "0 - Incremental regular update" }, 
                                            new { @id = 1, @value = "1 - Incremental immediate update" }, 
                                            new { @id = 2, @value = "2 - All incremental updates (regular and immediate)" }, 
                                            new { @id = 3, @value = "3 - Full DDP" }, 
                                            new { @id = 4, @value = "4 - Archived DDP" } }, "id", "value", ViewData["updateType"]), new {@id="updateType" })%>
            </p>

            <% if (ViewData["updateType"].ToString() != "4")
               { %>
              <div id="archived" style="display:none">
            <%}
               else
               { %>
              <div id="archived">
            <%} %>
            
            <p>
                <label for="Lat">Archived DDP Timestamp</label>
                <%= Html.TextBox("archivedTimestamp", ViewData["archivedTimestamp"], new { @class = "time input-small", @id = "archivedTimestamp" })%>
            </p>
            
            <p>
                <label for="Long">Archived DDP Version</label>
                <%= Html.TextBox("archivedVersion", ViewData["archivedVersion"], new { @class = "time input-small", @id = "archivedVersion" })%>
            </p>          
            </div>
            <p>
                <input class="button" type="submit" value="Enviar" />
            </p>
    <% } %>
   
   </div>
</asp:Content>



