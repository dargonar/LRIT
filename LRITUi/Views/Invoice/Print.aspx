<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<DataCenterDataAccess.Invoice>" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<HTML>
<HEAD>
	<META HTTP-EQUIV="CONTENT-TYPE" CONTENT="text/html; charset=windows-1252">
	<TITLE></TITLE>
	<META NAME="GENERATOR" CONTENT="LibreOffice 3.3  (Win32)">
	<META NAME="CREATED" CONTENT="20110615;18325223">
	<META NAME="CHANGED" CONTENT="20110615;18361246">
	<script src="<%= Url.Content("~/js/jquery-1.3.2.min.js") %>" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).ready(function() {
            window.print();
        });
    </script>
	
	<STYLE TYPE="text/css">
	<!--
		@page { margin-right: 0.5in; margin-top: 0.5in; margin-bottom: 0.51in }
		P { margin-bottom: 0.08in; direction: ltr; color: #000000; line-height: 110%; widows: 2; orphans: 2 }
		P.western { font-family: "Tahoma", sans-serif; font-size: 8pt; so-language: en-US }
		P.cjk { font-family: "Times New Roman", serif; font-size: 8pt }
		P.ctl { font-family: "Times New Roman", serif; font-size: 9pt; so-language: ar-SA }
		H1 { margin-top: 0in; margin-bottom: 0in; direction: ltr; color: #808080; line-height: 110%; text-align: right; widows: 2; orphans: 2; page-break-after: auto }
		H1.western { font-family: "Tahoma", sans-serif; font-size: 20pt; so-language: en-US }
		H1.cjk { font-family: "Times New Roman", serif; font-size: 20pt }
		H1.ctl { font-family: "Times New Roman", serif; font-size: 9pt; so-language: ar-SA; font-weight: normal }
		H2 { margin-top: 0in; margin-bottom: 0in; direction: ltr; color: #000000; line-height: 110%; widows: 2; orphans: 2; page-break-after: auto }
		H2.western { font-family: "Tahoma", sans-serif; font-size: 8pt; so-language: en-US }
		H2.cjk { font-family: "Times New Roman", serif; font-size: 8pt }
		H2.ctl { font-family: "Times New Roman", serif; font-size: 8pt; so-language: ar-SA; font-weight: normal }
		H3 { margin-top: 0in; margin-bottom: 0.13in; direction: ltr; color: #000000; line-height: 110%; widows: 2; orphans: 2; page-break-after: auto }
		H3.western { font-family: "Tahoma", sans-serif; font-size: 8pt; so-language: en-US; font-style: italic; font-weight: normal }
		H3.cjk { font-family: "Times New Roman", serif; font-size: 8pt; font-style: italic; font-weight: normal }
		H3.ctl { font-family: "Times New Roman", serif; font-size: 9pt; so-language: ar-SA; font-weight: normal }
		A:link { so-language: zxx }
	-->
	</STYLE>
</HEAD>
<BODY LANG="en-US" TEXT="#000000" DIR="LTR">
<CENTER>
	<TABLE WIDTH=720 BORDER=0 CELLPADDING=7 CELLSPACING=0 STYLE="page-break-before: always">
		<COL WIDTH=347>
		<COL WIDTH=345>
		<TR VALIGN=TOP>
			<TD ROWSPAN=2 WIDTH=347 HEIGHT=39>
				<P STYLE="margin-top: 0.1in; margin-bottom: 0in">PREFECTURA NAVAL ARGENTINA</P>
				<H3 CLASS="western">Protecci&oacute;n de las aguas y el comercio desde 1810</H3>
				<P CLASS="western" STYLE="margin-bottom: 0in">Edificio Guardacostas Avenida E. Madero 235</P>
				<P CLASS="western" STYLE="margin-bottom: 0in">Ciudad de Buenos Aires (C1106ACC) Republica Argentina</P>
				<P CLASS="western">Phone <B>+54-11-4318-7400</B></P>
			</TD>
			<TD WIDTH=345>
				<H1 CLASS="western">INVOICE</H1>
			</TD>
		</TR>
		<TR>
			<TD WIDTH=345 VALIGN=BOTTOM>
				<P ALIGN=RIGHT STYLE="margin-bottom: 0in"><FONT SIZE=1 STYLE="font-size: 8pt">Invoice
				#<%=Model.invoiceNumber%></FONT></P>
				<P ALIGN=RIGHT><FONT SIZE=1 STYLE="font-size: 8pt">Date: <SDFIELD TYPE=DATETIME SDNUM="1033;1033;MMMM D, YYYY"><%=Model.isueDate.ToLongDateString()%></SDFIELD></FONT></P>
			</TD>
		</TR>
	</TABLE>
</CENTER>
<P CLASS="western" STYLE="margin-bottom: 0in"><BR>
</P>
<CENTER>
	<TABLE WIDTH=720 BORDER=0 CELLPADDING=7 CELLSPACING=0>
		<COL WIDTH=346>
		<COL WIDTH=346>
		<TR VALIGN=TOP>
			<TD WIDTH=346 HEIGHT=82>
				<H2 CLASS="western">To:</H2>
				<P CLASS="western" STYLE="margin-bottom: 0in"><%=Model.Contract.name%></P>
				<P CLASS="western">[Phone]</P>
			</TD>
			<TD WIDTH=346>
				<H2 CLASS="western">For:</H2>
				<P CLASS="western" STYLE="margin-bottom: 0in">LRIT Messages</P>
				<P CLASS="western">Contract: #<%=Model.Contract.id %></P>
			</TD>
		</TR>
	</TABLE>
</CENTER>
<P CLASS="western" STYLE="margin-bottom: 0in"><BR>
</P>
<P CLASS="western" STYLE="margin-bottom: 0in"><BR>
</P>
<CENTER>
	<TABLE WIDTH=721 BORDER=1 BORDERCOLOR="#000000" CELLPADDING=3 CELLSPACING=0 RULES=COLS>
		<COL WIDTH=510>
		<COL WIDTH=198>
		<TBODY >
			<TR style="border: solid 1px;">
				<TD WIDTH=510 HEIGHT=12>
					<P ALIGN=CENTER><FONT SIZE=1 STYLE="font-size: 12pt"><B>DESCRIPTION</B></FONT></P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=CENTER><FONT SIZE=1 STYLE="font-size: 12pt"><B>AMOUNT</B></FONT></P>
				</TD>
			</TR>
		</TBODY>
		<TBODY>
			<TR>
				<TD WIDTH=510 HEIGHT=13 align="center">
					<P><FONT SIZE=1 STYLE="font-size: 10pt">Messages transmitted between <B><%=Model.dateFrom.ToString("yyyy-MM-dd")%></B> and <B><%=Model.dateTo.ToString("yyyy-MM-dd")%></B></FONT>
					</P>
				</TD>
				<TD WIDTH=198 align="center">
					<P ALIGN=CENTER><%=Model.currency%>&nbsp;<%=Model.amount.ToString(".00")%></P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P CLASS="western"><BR>
					</P>
				</TD>
				<TD WIDTH=198>
					<P ALIGN=RIGHT><BR>
					</P>
				</TD>
			</TR>
		</TBODY>
		<TBODY>
			<TR>
				<TD WIDTH=510 HEIGHT=13>
					<P ALIGN=RIGHT><FONT SIZE=1 STYLE="font-size: 8pt">TOTAL</FONT></P>
				</TD>
				<TD WIDTH=198>
				    <P ALIGN=CENTER><%=Model.currency%>&nbsp;<%=Model.amount.ToString(".00")%></P>
				</TD>
			</TR>
		</TBODY>
	</TABLE>
</CENTER>
<P CLASS="western" STYLE="margin-bottom: 0in"><BR>
</P>
<CENTER>
	<TABLE WIDTH=720 BORDER=0 CELLPADDING=7 CELLSPACING=0>
		<COL WIDTH=706>
		<TR>
			<TD WIDTH=706 HEIGHT=133 VALIGN=BOTTOM>
				<P CLASS="western" STYLE="margin-bottom: 0in">Make all checks
				payable to PNA</P>
				<P CLASS="western" STYLE="margin-bottom: 0in">Payment is due
				within 30 days.</P>
				<P CLASS="western" STYLE="margin-bottom: 0in">If you have any
				questions concerning this invoice, contact <B>Ricardo Javier Rial +54-11-4318-7400</B></P>
				<P CLASS="western"><BR>
				</P>
			</TD>
		</TR>
	</TABLE>
</CENTER>
<P CLASS="western" STYLE="margin-bottom: 0in"><BR>
</P>
</BODY>
</HTML>