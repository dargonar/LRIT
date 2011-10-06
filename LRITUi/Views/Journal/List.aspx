<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Mensajes enviados/recibidos formato RAW
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Contenido" runat="server">
    <h2>Mensajes enviados/recibidos</h2>
    <div id="container_id" style="max-height: 200px; height:200px; overflow-y:scroll"></div>
    </br>
    <h2>Contenido del mensaje</h2>
    <strong><span id="fname">&nbsp</span></strong>
    </br>
    <textarea cols="100" rows="100" id="dlgcontent" style="width:100%"></textarea>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeadInfo" runat="server">
<script type="text/javascript">
    $(document).ready(function() {
        $('#container_id').fileTree(
            {
                root: 'z:/', 
                script:virtualdir + '/Journal/Handler',
            },
            function(file) {
                $.ajax({
                    url: virtualdir + '/Journal/GetContent?file=' + file,
                    success: function(cont) {
                        $("#fname").html(file);
                        $("#dlgcontent").text(cont);
                    }
                });
            });
    });
</script>

</asp:Content>
