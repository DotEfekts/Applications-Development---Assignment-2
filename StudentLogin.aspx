<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.master" AutoEventWireup="true" CodeFile="StudentLogin.aspx.cs" Inherits="StudentLogin" %>
<%@ Register Src="~/RecordsDataView.ascx" TagPrefix="uc1" TagName="RecordsDataView" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Main" Runat="Server">
    <div id="login" class="editor" runat="server">
        <table>
            <tr>
                <td><label class="bold">Student Email:</label></td>
                <td><input type="text" name="usernameinput" /></td>
            </tr>
            <tr>
                <td><label class="bold">Password:</label></td>
                <td><input type="password" name="passwordinput" /></td>
            </tr>
            <tr>
                <td colspan="2" class="rowbuttons left"><input type="submit" value="Login" /></td>
            </tr>
            <tr>
                <td colspan="2"><asp:Label id="invalidup" class="invalid" runat="server"></asp:Label></td>
            </tr>
        </table>
       
    </div>
    <div id="data" runat="server">
        <div class="editor">
            <table>
                <tr>
                    <td><label class="bold">Student Name:</label></td>
                    <td colspan="99"><label id="stunam" runat="server"></label></td>
                </tr>
                <tr>
                    <td><label class="bold">Student Number:</label></td>
                    <td colspan="99"><label id="stunum" runat="server"></label></td>
                </tr>
              </table>
        </div>
        <uc1:RecordsDataView runat="server" ID="RecordsDataViewCourses" /><br />
        <uc1:RecordsDataView runat="server" ID="RecordsDataViewUnits" />
    </div>
</asp:Content>

