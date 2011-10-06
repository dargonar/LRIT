<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
    
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <meta name="description" content="Reflect Template" />
		<meta http-equiv="X-UA-Compatible" content="IE=EmulateIE7" />
        <title>LRIT ARGENTINA DATA CENTER</title>
        <link rel="stylesheet" href="~/css/style_al.css" type="text/css" media="screen" />
        
        <!-- to choose another color scheme uncomment one of the foloowing stylesheets and wrap styl1.css into a comment -->
        <link rel="stylesheet" href="~/css/style100.css" type="text/css" media="screen" />
        
        <link rel="stylesheet" href="~/css/jquery-u.css" type="text/css" media="screen" />
        <link rel="stylesheet" href="~/css/jquery00.css" type="text/css" media="screen" />
        
        <!--Internet Explorer Trancparency fix-->
        <!--[if IE 6]>
        <script src="js/ie6pngfix.js"></script>
        <script>
          DD_belatedPNG.fix('#head, a, a span, img, .message p, .click_to_close, .ie6fix');
        </script>
        <![endif]--> 
              
        
        <script type='text/javascript' src="<%= Url.Content("~/js/jquery00.js") %>"></script>
        <script type='text/javascript' src="<%= Url.Content("~/js/jquery-u.js") %>"></script>
        <script type='text/javascript' src="<%= Url.Content("~/js/jquery01.js") %>"></script>
        <script type='text/javascript' src="<%= Url.Content("~/js/custom00.js") %>"></script>
        
    
	<meta http-equiv="content-type" content="text/html; charset=iso-8859-1" />
    
    <link rel="stylesheet" type="text/css" media="screen" href="~/css/jquery-ui-1.7.2.custom.css" />
    <link rel="stylesheet" type="text/css" media="screen" href="~/css/ui.jqgrid.css" />
    <link rel="stylesheet" type="text/css" media="screen" href="~/css/ui.datetimepicker.css" />
        
    <!-- Jquery y Jquery UI-->
    <script src="<%= Url.Content("~/js/jquery-1.3.2.min.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/js/jquery-ui-1.7.2.custom.min.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/js/ui.datetimepicker.js") %>" type="text/javascript"></script>
    
    <!-- Jqgrid -->
    <script src="<%= Url.Content("~/js/i18n/grid.locale-sp.js") %>" type="text/javascript"></script>
    <script src="<%= Url.Content("~/js/jquery.jqGrid.min.js") %>" type="text/javascript"></script>
    </head>
    
    <body class="nobackground">

        <div id="login">
            <h2 class="loginheading">LRIT ARGENTINA DATA CENTER</h2>
            <div class="icon_login ie6fix"></div>
                
        	<form id="login-form" action="<%= Url.Action("LogOn", "Account")  %>" method="post">
            <p>
            	<label for="name">Usuario</label>
            	<%= Html.TextBox("username","", new { @class = "input-medium"} ) %>
        	</p>
        	<p>
            	<label for="password">Password</label>
            	<%= Html.Password("password","",new { @class = "input-medium"}) %>
        	</p>
        	<br />
        	<p class="clearboth">
            	<input class="button" name="submit" type="submit" value="Ingresar"/>
        	</p>
            </form>
        </div>
        
        
        <% foreach (var state in ViewData.ModelState)
           {
               foreach (var error in ViewData.ModelState[state.Key].Errors)
               {%>
                 <div class="login_message message error">
                    <p><%=error.ErrorMessage%></p>
                 </div>
            <% }
           } 
        %>
    </body>
    
</html>
