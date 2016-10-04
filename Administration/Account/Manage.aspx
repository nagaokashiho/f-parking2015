<%@  Title="ユーザー管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Manage.aspx.cs" Inherits="Administration.Account.Manage" %>

<%@ Import Namespace="Administration.Models" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <p class="text-danger"><asp:Literal runat="server" ID="ErrorMessage" ViewStateMode="Disabled" /></p>

    <section id="userForm">
        <div class="form-horizontal">
            <h4>ユーザーを管理します。</h4>
            <hr />
            <div class="container">
                <asp:GridView ID="UserGridView" runat="server" AutoGenerateColumns="False"
                    ItemType="Administration.Models.ApplicationUser" DataKeyNames="Id"
                    SelectMethod="SelectUser" UpdateMethod="UpdateUser" DeleteMethod="DeleteUser"
                    OnRowDataBound="UserGridView_RowDataBound">
                    <Columns>
                        <asp:TemplateField ShowHeader="False">
                            <ItemTemplate>
                                <asp:Button ID="EditButton" runat="server" CausesValidation="False" CssClass="btn btn-default"
                                    CommandName="Edit" Text="編集" />
                                <asp:Button ID="DeleteButton" runat="server" CausesValidation="False" CssClass="btn btn-default"
                                    CommandName="Delete" Text="削除"
                                    OnClientClick="return confirm('削除してよいですか？');" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:Button ID="UpdateButton" runat="server" CausesValidation="True" CssClass="btn btn-default"
                                    CommandName="Update" Text="更新"
                                    ValidationGroup="UpdateUser" />
                                <asp:Button ID="CancelButton" runat="server" CausesValidation="False" CssClass="btn btn-default"
                                    CommandName="Cancel" Text="キャンセル" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="電子メール">
                            <ItemTemplate>
                                <%#: Item.Email %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email"
                                    Text='<%# BindItem.Email %>' />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Email" ValidationGroup="UpdateUser"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="電子メール フィールドは必須です。" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="パスワード" ItemStyle-Width="300">
                            <EditItemTemplate>
                                入力用 :
                                <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                                確認用 :
                                <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
                                <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword" ValidationGroup="UpdateUser"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="パスワードと確認のパスワードが一致しません。" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="名前">
                            <ItemTemplate>
                                <%#: Item.DispUserName %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="DispUserName" CssClass="form-control"
                                    Text='<%# BindItem.DispUserName %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="管理者" ItemStyle-Width="50" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:CheckBox ID="IsAdmin" runat="server" Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="IsAdmin" runat="server" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>

            <hr />

            <h4>ユーザーを新たに登録します。</h4>
            <asp:ValidationSummary runat="server" CssClass="text-danger" ValidationGroup="CreateUser" />
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="Email" CssClass="col-md-2 control-label">電子メール</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="Email" CssClass="form-control" TextMode="Email" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Email"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="電子メール フィールドは必須です。"
                        ValidationGroup="CreateUser" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="Password" CssClass="col-md-2 control-label">パスワード</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="Password" TextMode="Password" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Password"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="パスワード フィールドは必須です。"
                        ValidationGroup="CreateUser" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="ConfirmPassword" CssClass="col-md-2 control-label">パスワードの確認入力</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="ConfirmPassword" TextMode="Password" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ConfirmPassword"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="パスワードの確認入力フィールドは必須です。"
                        ValidationGroup="CreateUser" />
                    <asp:CompareValidator runat="server" ControlToCompare="Password" ControlToValidate="ConfirmPassword"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="パスワードと確認のパスワードが一致しません。"
                        ValidationGroup="CreateUser" />
                </div>
            </div>
            <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="DispUserName" CssClass="col-md-2 control-label">名前</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="DispUserName" CssClass="form-control" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                <asp:Button runat="server" OnClick="CreateUser_Click" Text="登録" CssClass="btn btn-default"
                    ValidationGroup="CreateUser" />
                </div>
            </div>
        </div>
    </section>
</asp:Content>
