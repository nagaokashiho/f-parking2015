<%@ Title="What's New 管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Administration.WhatsNews.index" %>

<%@ Import Namespace="Administration.Models" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" ViewStateMode="Disabled" />
    </p>

    <section id="newsForm">
        <div class="form-horizontal">
            <h4>What's New を管理します。</h4>
            <hr />
            <div class="form-group">
                <asp:GridView ID="NewsGridView" runat="server" AutoGenerateColumns="False"
                    ItemType="Administration.Models.WhatsNew" DataKeyNames="id"
                    SelectMethod="SelectNews" UpdateMethod="UpdateNews" DeleteMethod="DeleteNews"
                    OnRowDataBound="NewsGridView_RowDataBound" AllowPaging="True" PageSize="5">
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
                                    ValidationGroup="UpdateNews" />
                                <asp:Button ID="CancelButton" runat="server" CausesValidation="False" CssClass="btn btn-default"
                                    CommandName="Cancel" Text="キャンセル" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="掲載日付">
                            <ItemTemplate>
                                <%#: Item.PublicationDate.ToString("yyyy/MM/dd") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtPublicationDate" CssClass="form-control datepicker" TextMode="Date"
                                    Text='<%# Bind("PublicationDate", "{0:yyyy/MM/dd}") %>' />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPublicationDate" ValidationGroup="UpdateNews"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="掲載日付は必須です。" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="No" ItemStyle-Width="40">
                            <ItemTemplate>
                                <%#: Item.No %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtNo" CssClass="form-control" TextMode="Number"
                                    Text='<%# BindItem.No  %>' />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNo" ValidationGroup="UpdateNews"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="Noは必須です。" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="タイトル">
                            <ItemTemplate>
                                <%#: Item.Title %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtTitle" CssClass="form-control"
                                    Text='<%# BindItem.Title %>' />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle" ValidationGroup="UpdateNews"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="タイトルは必須です。" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="本文" ItemStyle-Width="300">
                            <ItemTemplate>
                                <%# Item.Message.Replace("\r\n", "<br />") %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtMessage" CssClass="form-control"
                                    Text='<%# BindItem.Message %>' TextMode="MultiLine" Rows="3" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMessage" ValidationGroup="UpdateNews"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="本文は必須です。" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="URL">
                            <ItemTemplate>
                                <%#: Item.Url %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtUrl" CssClass="form-control" TextMode="Url"
                                    Text='<%# BindItem.Url %>' />
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="公開フラグ" ItemStyle-Width="50">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkOpenFlag" runat="server" Checked='<%# Item.OpenFlag %>' Enabled="false" />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:CheckBox ID="chkOpenFlag" runat="server" Checked='<%# BindItem.OpenFlag %>'/>
                            </EditItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="公開日数" ItemStyle-Width="40">
                            <ItemTemplate>
                                <%#: Item.OpenDays %>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox runat="server" ID="txtOpenDays" CssClass="form-control" TextMode="Number"
                                    Text='<%# BindItem.OpenDays  %>' />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOpenDays" ValidationGroup="UpdateNews"
                                    CssClass="text-danger" Display="Dynamic" ErrorMessage="公開日数は必須です。" />
                            </EditItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataTemplate>
                        登録しているデータはありません。
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>

            <hr />

            <h4>What's New を新たに登録します。</h4>
            <asp:ValidationSummary runat="server" CssClass="text-danger" ValidationGroup="CreateNews" />
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtPublicationDate" CssClass="col-md-2 control-label">掲載日付</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="txtPublicationDate" CssClass="form-control datepicker" TextMode="Date" />
                    <div class="txt-blue">記事の掲載開始日を指定してください。</div>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtPublicationDate"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="掲載日付は必須です。"
                        ValidationGroup="CreateNews" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtNo" CssClass="col-md-2 control-label">No</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="txtNo" TextMode="Number" CssClass="form-control" Text="1" />
                    <div class="txt-blue">同一載開始日の記事が複数あった場合に、この番号の昇順で掲載します。</div>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtNo"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="Noは必須です。"
                        ValidationGroup="CreateNews" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtTitle" CssClass="col-md-2 control-label">タイトル</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="txtTitle" CssClass="form-control" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtTitle"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="タイトルは必須です。"
                        ValidationGroup="CreateNews" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtMessage" CssClass="col-md-2 control-label">本文</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="txtMessage" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtMessage"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="本文は必須です。"
                        ValidationGroup="CreateNews" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtUrl" CssClass="col-md-2 control-label">URL</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="txtUrl" CssClass="form-control" TextMode="Url" />
                    <div class="txt-blue">『http://』または『https://』から入力してください。</div>
               </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="chkOpenFlag" CssClass="col-md-2 control-label">公開フラグ</asp:Label>
                <div class="col-md-10">
                    <asp:CheckBox ID="chkOpenFlag" runat="server" Enabled="true" />
                </div>
            </div>
            <div class="form-group">
                <asp:Label runat="server" AssociatedControlID="txtOpenDays" CssClass="col-md-2 control-label">公開日数</asp:Label>
                <div class="col-md-10">
                    <asp:TextBox runat="server" ID="txtOpenDays" TextMode="Number" CssClass="form-control" Text="0" />
                    <div class="txt-blue">記事を掲載する日数を指定してください。（無期限の場合は０（ゼロ）を入力してください。）</div>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtOpenDays"
                        CssClass="text-danger" Display="Dynamic" ErrorMessage="公開日数は必須です。"
                        ValidationGroup="CreateNews" />
                </div>
            </div>
            <div class="form-group">
                <div class="col-md-offset-2 col-md-10">
                <asp:Button runat="server" OnClick="CreateNews_Click" Text="登録" CssClass="btn btn-default"
                    ValidationGroup="CreateNews" />
                </div>
            </div>
        </div>
    </section>
</asp:Content>
