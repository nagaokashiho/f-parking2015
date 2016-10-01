<%@  Title="貸し会議室管理" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="Administration.Rooms.index" %>

<%@ Import Namespace="Administration.Models" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <link href="~/Scripts/css/.css" rel="stylesheet" type="text/css" />


    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" ViewStateMode="Disabled" />
    </p>

    <section id="userForm">
        <div class="form-horizontal">
            <h4>貸し会議室を管理します。</h4>
            <hr />
            <div class="container">
                <ul class="nav nav-tabs">
                    <li class="active"><a href="#tabList" class="nav-link" data-toggle="tab">一　覧</a></li>
                    <li><a href="#tabRoom" class="nav-link" data-toggle="tab">会議室</a></li>
                </ul>
                <div id="reserve" class="tab-content">
                    <div id="tabList" class="tab-pane fade in active">
                        一覧タブです。
<%--                        <div id="reserve_day1" class="reserve_day">
                            <table border="0" cellspacing="0" cellpadding="0" width="900">
                                <tr>
                                    <td width="70px">表示日付：</td>
                                    <td width="140px"><input id="txt_dispDate1" class="txt_Date" name="txt_dispDate1" type="text" runat="server" /><a onclick="onclick_Calendar(1);"><img id="img_calendar1" src="../img/calendar.jpg" alt="" /></a></td>
                                    <td width="55px"><asp:ImageButton id="btn_reload1" name="btn_reload" runat="server" ImageUrl="img/replay_off.jpg" width="45" height="45" Text="再表示"  OnClientClick="dispLoading(1);" /></td>
                                    <td width="100px"><asp:ImageButton id="btnStart1" name="btnStart" runat="server" ImageUrl="img/start2_off.jpg" width="95" height="35" Text="設定開始" OnClientClick="onclick_add_List(1); return false; " /></td>   <!-- "return false;" は、IE11以降対応のため！ -->
                                    <td width="100px"><asp:ImageButton id="btnEnd1" name="btnEnd" runat="server" ImageUrl="img/end2_off.jpg" width="95" height="35" Text="設定終了" style="visibility:hidden;" /></td>
                                    <td width="100px"><asp:ImageButton id="btnCancel1" name="btnCancel" runat="server" ImageUrl="img/cancel_off.jpg" width="95" height="35" Text="キャンセル" OnClientClick="onclick_add_List(3); return false; " style="visibility:hidden;" /></td>   <!-- "return false;" は、IE11以降対応のため！ -->
                                </tr>
                            </table>
                                
                        </div>--%>
<%--                        <div id="booking_main1" class="booking_main">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <div id="divTable1" class="divTable">
                                        <div id="divLeft1" class="divLeft">
                                            <asp:Table ID="tblTitle1" class="tblTitle" name="tblTitle" runat="server">
                                            </asp:Table>
                                        </div>
                                        <div id="divRight1" class="divRight">
                                            <asp:Table ID="tblBooking1" class="tblBooking" name="tblBooking" runat="server">
                                            </asp:Table>
                                        </div>
                                    </div>
                                    <input type="hidden" id="hid_addMode1" class="hid_addMode" name="hid_addMode" runat="server" value="" />
                                    <input type="hidden" id="hid_clickRow1" class="hid_clickRow" name="hid_clickRow" runat="server" value="" />
                                    <input type="hidden" id="hid_clickColStart1" class="hid_clickColStart" name="hid_clickColStart" runat="server" value="" />
                                    <input type="hidden" id="hid_clickColEnd1" class="hid_clickColEnd" name="hid_clickColEnd" runat="server" value="" />
                                    <input type="hidden" id="hid_yoto1" class="hid_yoto" name="hid_yoto" runat="server" value="" />
                                    <input type="hidden" id="hid_editMode1" class="hid_editMode" name="hid_editMode" runat="server" value="" />
                                    <input type="hidden" id="hid_scrollLeft1" class="hid_scrollLeft" name="hid_scrollLeft" runat="server" value="" />
                                    <input type="hidden" id="hid_time_from1" class="hid_time_from" name="hid_time_from" runat="server" value="" />
                                    <input type="hidden" id="hid_minutes_from1" class="hid_minutes_from" name="hid_minutes_from" runat="server" value="" />
                                    <input type="hidden" id="hid_time_to1" class="hid_time_to" name="hid_time_to" runat="server" value="" />
                                    <input type="hidden" id="hid_minutes_to1" class="hid_minutes_to" name="hid_minutes_to" runat="server" value="" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btn_reload1" />
                                    <asp:AsyncPostBackTrigger ControlID="btnDummy1" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:Button id="btnDummy1" name="btnDummy" runat="server" Text="トリガ用ダミーボタン" style="visibility:hidden;" />
                        </div>--%>
                    </div>
                    <div id="tabRoom" class="tab-pane fade">
                        会議室タブです。
<%--                        <div id="reserve_day2" class="reserve_day">
                            <table border="0" cellspacing="0" cellpadding="0" width="900">
                                <tr style="height:30px">
                                    <td width="70px"> 施　　設：</td>
                                    <td width="190px"><asp:DropDownList ID="DDList_Sisetsu2" runat="server" DataTextField="Text" Font-Names="メイリオ" DataValueField="Value" Font-Size="9pt" Width="180px"></asp:DropDownList></td>
                                    <td width="55px" rowspan="2"><asp:ImageButton id="btn_reload2" name="btn_reload" runat="server" ImageUrl="img/replay_off.jpg" width="45" height="45" Text="再表示"  OnClientClick="dispLoading(2);" /></td>
                                    <td width="100px" rowspan="2"><asp:ImageButton id="btnStart2" name="btnStart" runat="server" ImageUrl="img/start2_off.jpg" width="95" height="35" Text="設定開始" OnClientClick="onclick_add_Shisetsu(1); return false; " /></td>   <!-- "return false;" は、IE11以降対応のため！ -->
                                    <td width="100px" rowspan="2"><asp:ImageButton id="btnEnd2" name="btnEnd" runat="server" ImageUrl="img/end2_off.jpg" width="95" height="35" Text="設定終了" style="visibility:hidden;" /></td>
                                    <td width="100px" rowspan="2"><asp:ImageButton id="btnCancel2" name="btnCancel" runat="server" ImageUrl="img/cancel_off.jpg" width="95" height="35" Text="キャンセル" OnClientClick="onclick_add_Shisetsu(3); return false; " style="visibility:hidden;" /></td>   <!-- "return false;" は、IE11以降対応のため！ -->
                                    <td rowspan="2" style="text-align:right"><span class="red">応接室の利用：スタッフミーティングは禁止</span><br /><span class="red smaller">(9:00～17:30)</span></td>
                                </tr>
                                <tr style="height:34px">
                                    <td>表示日付：</td>
                                    <td><input id="txt_dispDate2" class="txt_Date" name="txt_dispDate" type="text" runat="server" /><a onclick="onclick_Calendar(2);"><img id="img_calendar2" src="../img/calendar.jpg" alt="" /></a></td>
                                </tr>
                            </table>
                        </div>--%>
<%--                        <div id="booking_main2" class="booking_main">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <div id="divTable2" class="divTable">
                                        <div id="divLeft2" class="divLeft">
                                            <asp:Table ID="tblTitle2" class="tblTitle" name="tblTitle" runat="server">
                                            </asp:Table>
                                        </div>
                                        <div id="divRight2" class="divRight">
                                            <asp:Table ID="tblBooking2" class="tblBooking" name="tblBooking" runat="server">
                                            </asp:Table>
                                        </div>
                                    </div>
                                    <input type="hidden" id="hid_addMode2" class="hid_addMode" name="hid_addMode" runat="server" value="" />
                                    <input type="hidden" id="hid_clickRow2" class="hid_clickRow" name="hid_clickRow" runat="server" value="" />
                                    <input type="hidden" id="hid_clickColStart2" class="hid_clickColStart" name="hid_clickColStart" runat="server" value="" />
                                    <input type="hidden" id="hid_clickColEnd2" class="hid_clickColEnd" name="hid_clickColEnd" runat="server" value="" />
                                    <input type="hidden" id="hid_yoto2" class="hid_yoto" name="hid_yoto" runat="server" value="" />
                                    <input type="hidden" id="hid_editMode2" class="hid_editMode" name="hid_editMode" runat="server" value="" />
                                    <input type="hidden" id="hid_scrollLeft2" class="hid_scrollLeft" name="hid_scrollLeft" runat="server" value="" />
                                    <input type="hidden" id="hid_time_from2" class="hid_time_from" name="hid_time_from" runat="server" value="" />
                                    <input type="hidden" id="hid_minutes_from2" class="hid_minutes_from" name="hid_minutes_from" runat="server" value="" />
                                    <input type="hidden" id="hid_time_to2" class="hid_time_to" name="hid_time_to" runat="server" value="" />
                                    <input type="hidden" id="hid_minutes_to2" class="hid_minutes_to" name="hid_minutes_to" runat="server" value="" />
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="btn_reload2" />
                                    <asp:AsyncPostBackTrigger ControlID="btnDummy2" />
                                </Triggers>
                            </asp:UpdatePanel>
                            <asp:Button id="btnDummy2" name="btnDummy" runat="server" Text="トリガ用ダミーボタン" style="visibility:hidden;" />
                        </div>--%>
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
