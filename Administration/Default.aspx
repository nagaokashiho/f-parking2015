<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Administration._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <div class="jumbotron">
        <div class="container">
            <a runat="server" href="~/WhatsNews/index.aspx" class="btn btn-primary btn-lg">Whta's New 管理 &raquo;</a>
            <a runat="server" href="~/Rooms/index.aspx" class="btn btn-primary btn-lg">貸会議室　管理 &raquo;</a>
        </div>
    </div>

</asp:Content>
