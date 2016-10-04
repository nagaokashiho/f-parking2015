<%@  Title="貸会議室管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Administration.Rooms.index" %>

<%@ Import Namespace="Administration.Models" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" type="text/css" href="/Content/room.css">
    <%-- BundleConfig.cs の内容を取り込む --%>
    <%: Scripts.Render("~/bundles/room") %>

    <h2><%: Title %>.</h2>
    <p class="text-danger"><asp:Literal runat="server" ID="ErrorMessage" ViewStateMode="Disabled" /></p>

    <section id="userForm">
        <div class="form-horizontal">
            <h4>貸会議室の使用（予約）状況を管理します。</h4>
            <hr />
            <div class="container">
                <div id="reserve_bg" role="tabpanel">
                    <ul class="nav nav-tabs">
                        <li id="li1" runat="server" ClientIDMode="Static" class="active"><a href="#tabList" class="nav-link" data-toggle="tab">一　覧</a></li>
                        <li id="li2" runat="server" ClientIDMode="Static" ><a href="#tabRoom" class="nav-link" data-toggle="tab">会議室</a></li>
                    </ul>
                    <div class="tab-content">
                        <%-- 一覧タブ --%>
	                    <div id="tabList" runat="server" ClientIDMode="Static" class="tab-pane fade in active alert alert-success" role="alert">
                            <table border="0">
                                <tr>
                                    <td style="width:150px;"><asp:TextBox runat="server" ID="txtDispDate1" name="txtDispDate" CssClass="form-control datepicker" ClientIDMode="Static" placeholder="表示日付" /></td>
                                    <td style="width:90px;"><asp:Button runat="server" ID="btnReload1" name="btnReload" Text="再表示" CssClass="btn btn-warning" ClientIDMode="Static" OnClientClick="dispLoading(1);" OnClick="btnReload_Click" /></td>
                                    <td style="width:105px;"><asp:Button runat="server" ID="btnStart1" name="btnStart" Text="設定開始" CssClass="btn btn-danger" ClientIDMode="Static" OnClientClick="onclick_add_List(1); return false;" /></td>
                                    <td style="width:105px;"><asp:Button runat="server" ID="btnEnd1" name="btnEnd" Text="設定終了" CssClass="btn btn-primary" style="visibility:hidden;" ClientIDMode="Static" /></td>
                                    <td style="width:105px;"><asp:Button runat="server" ID="btnCancel1" name="btnCancel" Text="キャンセル" CssClass="btn btn-default" ClientIDMode="Static" OnClientClick="onclick_add_List(3); return false;" style="visibility:hidden;" /></td>
                                    <td></td>
                                </tr>
                            </table>
                            <div id="booking_main1" class="booking_main">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <div id="divTable1" class="divTable">
                                            <div id="divLeft1" class="divLeft">
                                                <asp:Table ID="tblTitle1" class="tblTitle" name="tblTitle" runat="server"></asp:Table>
                                            </div>
                                            <div id="divRight1" class="divRight">
                                                <asp:Table ID="tblBooking1" class="tblBooking" name="tblBooking" runat="server" ClientIDMode="Static"></asp:Table>
                                            </div>
                                        </div>
                                        <asp:HiddenField ID="hid_addMode1" runat="server" Value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_clickRow1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_clickColStart1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_clickColEnd1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_yoto1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_editMode1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_scrollLeft1" runat="server" Value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_time_from1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_minutes_from1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_time_to1" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_minutes_to1" runat="server" value="" ClientIDMode="Static" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnReload1" />
                                        <asp:AsyncPostBackTrigger ControlID="btnDummy1" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <asp:Button id="btnDummy1" name="btnDummy" runat="server" Text="トリガ用ダミーボタン" style="visibility:hidden;" ClientIDMode="Static" OnClick="btnDummy_Click" />
                            </div>
                        </div>
                        <%-- 会議室タブ --%>
	                    <div id="tabRoom" runat="server" ClientIDMode="Static" class="tab-pane fade alert alert-info" role="alert">
                            <table border="0">
                                <tr>
                                    <td style="width:190px;"><asp:DropDownList ID="selRoom2" runat="server" DataTextField="Text" DataValueField="Value" CssClass="selectpicker" ClientIDMode="Static"></asp:DropDownList></td>
                                    <td style="width:150px;"><asp:TextBox runat="server" ID="txtDispDate2" name="txtDispDate" CssClass="form-control datepicker" ClientIDMode="Static" placeholder="表示日付" /></td>
                                    <td style="width:90px;"><asp:Button runat="server" ID="btnReload2" name="btnReload" Text="再表示" CssClass="btn btn-warning" ClientIDMode="Static" OnClientClick="dispLoading(2);" OnClick="btnReload_Click" /></td>
                                    <td style="width:105px;"><asp:Button runat="server" ID="btnStart2" name="btnStart" Text="設定開始" CssClass="btn btn-danger" ClientIDMode="Static" OnClientClick="onclick_add_Shisetsu(1); return false;" /></td>
                                    <td style="width:105px;"><asp:Button runat="server" ID="btnEnd2" name="btnEnd" Text="設定終了" CssClass="btn btn-primary" style="visibility:hidden;" ClientIDMode="Static" /></td>
                                    <td style="width:105px;"><asp:Button runat="server" ID="btnCancel2" name="btnCancel" Text="キャンセル" CssClass="btn btn-default" ClientIDMode="Static" OnClientClick="onclick_add_Shisetsu(3); return false;" style="visibility:hidden;" /></td>
                                    <td></td>
                                </tr>
                            </table>
                            <div id="booking_main2" class="booking_main">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <div id="divTable2" class="divTable">
                                            <div id="divLeft2" class="divLeft">
                                                <asp:Table ID="tblTitle2" class="tblTitle" name="tblTitle" runat="server">
                                                </asp:Table>
                                            </div>
                                            <div id="divRight2" class="divRight">
                                                <asp:Table ID="tblBooking2" class="tblBooking" name="tblBooking" runat="server" ClientIDMode="Static">
                                                </asp:Table>
                                            </div>
                                        </div>
                                        <asp:HiddenField ID="hid_addMode2" runat="server" value=""  ClientIDMode="Static"/>
                                        <asp:HiddenField ID="hid_clickRow2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_clickColStart2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_clickColEnd2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_yoto2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_editMode2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_scrollLeft2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_time_from2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_minutes_from2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_time_to2" runat="server" value="" ClientIDMode="Static" />
                                        <asp:HiddenField ID="hid_minutes_to2" runat="server" value="" ClientIDMode="Static" />
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnReload2" />
                                        <asp:AsyncPostBackTrigger ControlID="btnDummy2" />
                                    </Triggers>
                                </asp:UpdatePanel>
                                <asp:Button id="btnDummy2" name="btnDummy" runat="server" Text="トリガ用ダミーボタン" style="visibility:hidden;" ClientIDMode="Static" OnClick="btnDummy_Click" />
                            </div>
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hid_selected_tab" runat="server" value="1" ClientIDMode="Static" />

                <%-- マニュアルモード --%>
                <div class="alert alert-warning" role="alert">
                    <div class="checkbox" style="margin-left:20px;">
                        <input type="checkbox" id="chkManualMode" onclick="onclick_chkManualMode();" runat="server" ClientIDMode="Static" /><label for="chkManualMode">マニュアルモード表示</label><br />
                    </div>
                    <div id="reserve_bg2" style="display:none; margin-top:20px;" runat="server" ClientIDMode="Static">
                        <p class="validateTips2 red"></p>
                        <table border="0">
                            <tr>
                                <td colspan="6">
                                    <asp:DropDownList ID="selRoom" runat="server" DataTextField="Text" DataValueField="Value" CssClass="selectpicker" ClientIDMode="Static"></asp:DropDownList>
                                </td>
                            </tr>
                            <tr style="vertical-align:middle;">
                                <td style="width:130px;"><asp:TextBox runat="server" ID="txtBookDate" CssClass="form-control datepicker" ClientIDMode="Static" placeholder="予約日付" /></td>
                                <td style="width:110px"><asp:TextBox runat="server" ID="txtBookTimeFrom" CssClass="form-control bootstrap-timepicker timepicker" ClientIDMode="Static" placeholder="開始時刻" /></td>   
                                <td style="width:20px; color:black;">～</td>
                                <td style="width:110px"><asp:TextBox runat="server" ID="txtBookTimeTo" CssClass="form-control timepicker" ClientIDMode="Static" placeholder="終了時刻" /></td>
                                <td style="width:205px"></td>
                                <td></td>
                            </tr>
                            <tr>
                                <td colspan="6"><asp:TextBox runat="server" ID="txtUse" CssClass="form-control" TextMode="MultiLine" ClientIDMode="Static" placeholder="用途を入力してください。（省略可能）" Rows="10" /></td>
                            </tr>
                            <tr>
                                <td colspan="5" style="text-align:right;">
                                    <asp:Button runat="server" ID="btnComplete" name="btnComple" Text="設定完了" CssClass="btn btn-success" ClientIDMode="Static" OnClientClick="return btnComplete_OnClick();" OnClick="btnComplete_Click"
                                         ValidationGroup="ManualMode" />
                                </td>
                                <td></td>
                            </tr>
                        </table>
                    </div>
                </div>
                <%-- マニュアルモード　ここまで --%>
            </div>

            <%-- 使用用途設定モーダルダイアログ --%>
            <div class="modal fade" id="dialogYoto" tabindex="-1">
                <div class="modal-dialog modal-dialog-center">
                    <div class="modal-header">
                        <a class="close" data-dismiss="modal">×</a>
                        <span id="dialogYotoHead">使用用途設定</span>
                    </div>
                    <div class="modal-body">
                        <span id="dialogYotoBody">
                            <asp:TextBox runat="server" ID="txtYoto" CssClass="form-control" TextMode="MultiLine" ClientIDMode="Static" placeholder="用途を入力してください。（省略可能）" Rows="10" />
                        </span>
                        <input type="hidden" id="hid_add_tab" runat="server" value="" ClientIDMode="Static" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" id="btnCancelYoto" class="btn btn-default" data-dismiss="modal">キャンセル</button>
                        <button type="button" id="btnSaveYoto" class="btn btn-primary">設　定</button>
                    </div>
                </div>
            </div>
            <%-- 使用用途設定モーダルダイアログ　ここまで --%>

            <%-- 会議室使用状況編集モーダルダイアログ --%>
            <div class="modal fade" id="dialogEdit" tabindex="-1">
                <div class="modal-dialog modal-dialog-center">
                <div class="modal-header">
                    <a class="close" data-dismiss="modal">×</a>
                    <span id="dialogEditHead">使用状況編集</span>
                </div>
                <div class="modal-body">
                    <p class="validateTips red" data-html="true"></p>
                    <table border="0">
                        <tr>
                            <td colspan="5">
                                <asp:DropDownList ID="selRoomEdit" runat="server" DataTextField="Text" DataValueField="Value" CssClass="selectpicker" ClientIDMode="Static"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width:130px;"><asp:TextBox runat="server" ID="txtDate" CssClass="form-control datepicker" ClientIDMode="Static" placeholder="予約日付" /></td>
                            <td style="width:110px"><asp:TextBox runat="server" ID="txtFrom" CssClass="form-control bootstrap-timepicker timepicker" ClientIDMode="Static" placeholder="開始時刻"  /></td>   
                            <td style="width:20px;">～</td>
                            <td style="width:110px"><asp:TextBox runat="server" ID="txtTo" CssClass="form-control timepicker" ClientIDMode="Static" placeholder="終了時刻" /></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td colspan="5">
                                <asp:TextBox runat="server" ID="txtYotoEdit" CssClass="form-control" TextMode="MultiLine" ClientIDMode="Static" placeholder="用途を入力してください。（省略可能）" Rows="10" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="modal-footer">
                    <div class="col-xs-6">
                        <button type="button" id="btnDeleteEdit" class="btn btn-danger" data-dismiss="modal">削　除</button>
                    </div>
                    <div class="col-xs-6">
                        <button type="button" id="btnCancelEdit" class="btn btn-default" data-dismiss="modal">キャンセル</button>
                        <button type="button" id="btnSaveEdit" class="btn btn-primary">更　新</button>
                    </div>
                </div>
                </div>
            </div>
            <%-- 会議室使用状況編集モーダルダイアログ　ここまで --%>

        </div>
    </section>

</asp:Content>
