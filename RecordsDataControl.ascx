<%@ Control Language="C#" AutoEventWireup="true" CodeFile="RecordsDataControl.ascx.cs" Inherits="RecordsDataControl" %>
<%@ Register Src="~/RecordsDataView.ascx" TagPrefix="uc1" TagName="RecordsDataView" %>
<%@ Register Src="~/RecordsDataEdit.ascx" TagPrefix="uc1" TagName="RecordsDataEdit" %>

<uc1:RecordsDataView runat="server" ID="RecordsDataView" />
<uc1:RecordsDataEdit runat="server" ID="RecordsDataEdit" />
