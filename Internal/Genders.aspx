<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="Genders.aspx.cs" Inherits="Internal_Genders" %>
<%@ Register Src="~/RecordsDataControl.ascx" TagPrefix="uc1" TagName="RecordsDataControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
    <uc1:RecordsDataControl runat="server" ID="RecordsDataControl" />
</asp:Content>

