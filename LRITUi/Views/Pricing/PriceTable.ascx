<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<DataCenterDataAccess.Price>" %>
<table style="width:100%"  cellspacing="0" id="table_auto">
    <tbody><tr>
    <th class="nobg">&nbsp;</th>
    <th scope="col">Precio</th>
    </tr>
    <tr>
    <th class="spec">PositionReport</th>
    <td><%=Model.currency%>&nbsp;<%=Model.PositionReport%></td>
    </tr>
    <tr>
    <th class="spec">Poll</th>
    <td><%=Model.currency%>&nbsp;<%=Model.Poll%></td>
    </tr>
    <tr>
    <th class="spec">PeriodicRateChange</th>
    <td><%=Model.currency%>&nbsp;<%=Model.PeriodicRateChange%></td>
    </tr>
    <tr>
    <th class="spec">ArchivePositionReport</th>
    <td><%=Model.currency%>&nbsp;<%=Model.ArchivePositionReport%></td>
    </tr>
    </tbody>
</table>
