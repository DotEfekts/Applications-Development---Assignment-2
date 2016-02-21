<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="UnitAttempts.aspx.cs" Inherits="Students_UnitAttempts" %>
<%@ Register Src="~/RecordsDataControl.ascx" TagPrefix="uc1" TagName="RecordsDataControl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
    <asp:DropDownList ID="filterBox" runat="server" AutoPostBack="True"></asp:DropDownList>
    <uc1:RecordsDataControl runat="server" ID="RecordsDataControl" />
</asp:Content>

