<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="f_parking.contact.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="viewport" content="width=device-width, user-scalable=yes, initial-scale=1, minimum-scale=1, maximum-scale=1.8" />
<title>お問い合わせ｜福岡パーキングビル</title>
<meta name="description" content="福岡市天神博多周辺の駐車場は便利で安心な福岡パーキングビルへ" />
<meta name="keywords" content="駐車場,バイク,ガレージ,パーキング,タワー,福岡市,天神,博多,博多駅,博多シティ,呉服町,博多座,リバレイン,山笠,どんたく,アジア美術館," />
<meta name="abstract" content="時間・月極駐車場" />
<meta name="robots" content="index,follow" />
<meta name="author" content="福岡パーキングビル http://f-parking.co.jp/" />
<meta name="format-detection" content="telephone=no" />

<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/2.0.1/jquery.min.js"></script>
<script type="text/javascript" src="../common/js/assembly.js"></script>

<link rel="stylesheet" href="../common/css/baseStyle.css" />

</head>

<body>
<!--#include virtual="/common/include/analytics.php" -->

<!--#include virtual="/common/include/header.inc" -->


<form id="frmIndex" runat="server">


<article id="contact">

<header class="mainTitleBox">
<h1 class="mainTitle"><img src="img/main_title.png" srcset="img/main_title.png 1x, img/main_title_s.png 2x" alt="お問い合わせ"></h1>
</header>

<nav id="breadcrumbs">
<ul>
<li itemtype="http://data-vocabulary.org/Breadcrumb"><a href="http://f-parking.co.jp/" itemprop="url"><span itemprop="ホーム">ホーム</span></a></li>
<li>お問い合わせ</li>
</ul>
</nav>

<section>
<div class="container">
<p class="formText">
以下の入力フォームに必要な項目を入力のうえ、「お問い合わせ内容を確認する」ボタンを押してください。<br>
<strong class="fAccent">＊</strong>印は必要事項です。必要事項の記入漏れがありますと、送信できませんのでご注意ください。<br>
<br>
フォームをご利用頂けない場合は［お問合せ］メールアドレスに直接送信ください。<br>
［お問合せ］メールアドレス ： <script type="text/javascript">
<!--
function converter(M){
var str="", str_as="";
for(var i=0;i<M.length;i++){
str_as = M.charCodeAt(i);
str += String.fromCharCode(str_as + 1);
}
return str;
}
function mail_to(k_1,k_2)
{eval(String.fromCharCode(108,111,99,97,116,105,111,110,46,104,114,101,102,32,
61,32,39,109,97,105,108,116,111,58) 
+ escape(k_1) + 
converter(String.fromCharCode(104,109,101,110,63,101,44,111,96,113,106,104,109,102,45,98,110,45,105,111,
62,114,116,97,105,100,98,115,60)) 
+ escape(k_2) + "'");} 
document.write('<a href=JavaScript:mail_to("","")><span class="mail"><img src="img/mail.png" srcset="img/mail.png 1x, img/mail_s.png 2x"></span><\/a>');
//-->
</script>
<noscript><span class="mail"><img src="img/mail.png" srcset="img/mail.png 1x, img/mail_s.png 2x"></span></noscript>
</p>

<div class="formBox">
<dl>
<dt>
お問い合わせ項目<strong class="fAccent">＊</strong>
</dt>
<dd>
<label class="radioLabel">
<asp:RadioButton ID="rdoParking" GroupName="contactKind" runat="server" Checked="true" />
<span class="lever">駐車場に関するお問い合わせ</span>
</label>
<br>
<label class="radioLabel">
<asp:RadioButton ID="rdoRoom" GroupName="contactKind" runat="server" />
<span class="lever">貸し会議室に関するお問い合わせ</span>
</label>
<br>
<label class="radioLabel">
<asp:RadioButton ID="rdoByke" GroupName="contactKind" runat="server" />
<span class="lever">バイクガレージに関するお問い合わせ</span>
</label>
<br>
<label class="radioLabel">
<asp:RadioButton ID="rdoTenant" GroupName="contactKind" runat="server" />
<span class="lever">テナントに関するお問い合わせ</span>
</label>
<br>
<label class="radioLabel">
<asp:RadioButton ID="rdoOther" GroupName="contactKind" runat="server" />
<span class="lever">その他</span>
</label>
</dd>
</dl>
<dl>
<dt>
お名前<strong class="fAccent">＊</strong>
</dt>
<dd>
<asp:TextBox ID="txtName" CssClass="nameBox" runat="server"></asp:TextBox><span>（全角）</span>
<asp:RequiredFieldValidator ID="reqName" runat="server" CssClass="fAccent" ControlToValidate="txtName"
				Text="※" ErrorMessage="【お名前】：必須入力です"></asp:RequiredFieldValidator>
</dd>
</dl>
<dl>
<dt>
ご住所
</dt>
<dd>
<label class="prefBox">
<asp:DropDownList ID="selPref" CssClass="prefList" runat="server" >
<asp:ListItem Value="">お選びください</asp:ListItem>
<asp:ListItem Value="北海道">北海道</asp:ListItem>
<asp:ListItem Value="青森県">青森県</asp:ListItem>
<asp:ListItem Value="岩手県">岩手県</asp:ListItem>
<asp:ListItem Value="宮城県">宮城県</asp:ListItem>
<asp:ListItem Value="秋田県">秋田県</asp:ListItem>
<asp:ListItem Value="山形県">山形県</asp:ListItem>
<asp:ListItem Value="福島県">福島県</asp:ListItem>
<asp:ListItem Value="茨城県">茨城県</asp:ListItem>
<asp:ListItem Value="栃木県">栃木県</asp:ListItem>
<asp:ListItem Value="群馬県">群馬県</asp:ListItem>
<asp:ListItem Value="埼玉県">埼玉県</asp:ListItem>
<asp:ListItem Value="千葉県">千葉県</asp:ListItem>
<asp:ListItem Value="東京都">東京都</asp:ListItem>
<asp:ListItem Value="神奈川県">神奈川県</asp:ListItem>
<asp:ListItem Value="新潟県">新潟県</asp:ListItem>
<asp:ListItem Value="富山県">富山県</asp:ListItem>
<asp:ListItem Value="石川県">石川県</asp:ListItem>
<asp:ListItem Value="福井県">福井県</asp:ListItem>
<asp:ListItem Value="山梨県">山梨県</asp:ListItem>
<asp:ListItem Value="長野県">長野県</asp:ListItem>
<asp:ListItem Value="岐阜県">岐阜県</asp:ListItem>
<asp:ListItem Value="静岡県">静岡県</asp:ListItem>
<asp:ListItem Value="愛知県">愛知県</asp:ListItem>
<asp:ListItem Value="三重県">三重県</asp:ListItem>
<asp:ListItem Value="滋賀県">滋賀県</asp:ListItem>
<asp:ListItem Value="京都府">京都府</asp:ListItem>
<asp:ListItem Value="大阪府">大阪府</asp:ListItem>
<asp:ListItem Value="兵庫県">兵庫県</asp:ListItem>
<asp:ListItem Value="奈良県">奈良県</asp:ListItem>
<asp:ListItem Value="和歌山県">和歌山県</asp:ListItem>
<asp:ListItem Value="鳥取県">鳥取県</asp:ListItem>
<asp:ListItem Value="島根県">島根県</asp:ListItem>
<asp:ListItem Value="岡山県">岡山県</asp:ListItem>
<asp:ListItem Value="広島県">広島県</asp:ListItem>
<asp:ListItem Value="山口県">山口県</asp:ListItem>
<asp:ListItem Value="徳島県">徳島県</asp:ListItem>
<asp:ListItem Value="香川県">香川県</asp:ListItem>
<asp:ListItem Value="愛媛県">愛媛県</asp:ListItem>
<asp:ListItem Value="高知県">高知県</asp:ListItem>
<asp:ListItem Value="福岡県" Selected="True">福岡県</asp:ListItem>
<asp:ListItem Value="佐賀県">佐賀県</asp:ListItem>
<asp:ListItem Value="長崎県">長崎県</asp:ListItem>
<asp:ListItem Value="熊本県">熊本県</asp:ListItem>
<asp:ListItem Value="大分県">大分県</asp:ListItem>
<asp:ListItem Value="宮崎県">宮崎県</asp:ListItem>
<asp:ListItem Value="鹿児島県">鹿児島県</asp:ListItem>
<asp:ListItem Value="沖縄県">沖縄県</asp:ListItem>
</asp:DropDownList>
</label>
<br>
<asp:TextBox ID="txtAddress" CssClass="adressBox" runat="server"></asp:TextBox><span>（全角）</span>
</dd>
</dl>
<dl>
<dt>
お電話番号
</dt>
<dd>
<asp:TextBox ID="txtTel" CssClass="telBox" runat="server"></asp:TextBox><span>（半角）</span>
</dd>
</dl>
<dl>
<dt>
メールアドレス<strong class="fAccent">＊</strong>
</dt>
<dd>
<asp:TextBox ID="txtMail" CssClass="mailBox" runat="server"></asp:TextBox><span>（半角）</span>
<asp:RequiredFieldValidator ID="reqMail" runat="server" CssClass="fAccent" ControlToValidate="txtMail"
				Text="※" ErrorMessage="【メールアドレス】：必須入力です"></asp:RequiredFieldValidator>
<asp:RegularExpressionValidator ID="revMail" runat="server" CssClass="fAccent" ControlToValidate="txtMail"
				ValidationExpression="^[A-Za-z0-9_\-][A-Za-z0-9_\.\-]*@([A-Za-z0-9_\-]+\.)+[A-Za-z0-9_\-]+$"
				Text="※" ErrorMessage="【メールアドレス】：正しく入力してください"></asp:RegularExpressionValidator>
</dd>
</dl>
<dl>
<dt>
メールアドレス（確認用）<strong class="fAccent">＊</strong>
</dt>
<dd>
<asp:TextBox ID="txtMailConf" CssClass="mailBox" runat="server" AutoCompleteType="Disabled"></asp:TextBox><span>（半角）</span>
<asp:CompareValidator ID="cvMailConf" runat="server" CssClass="fAccent" ControlToValidate="txtMail" ControlToCompare="txtMailConf" 
	Operator="Equal" Type="String" Text="※" ErrorMessage="【メールアドレス（確認用）】：入力されたメールアドレスが異なります"></asp:CompareValidator>
</dd>
</dl>
<dl>
<dt>
お問い合わせ内容<strong class="fAccent">＊</strong>
</dt>
<dd>
<asp:TextBox ID="txtMessage" CssClass="textBox" TextMode="MultiLine" Rows="8" runat="server"></asp:TextBox>
<asp:RequiredFieldValidator ID="reqMessage" runat="server" CssClass="fAccent" ControlToValidate="txtMessage"
				Text="※" ErrorMessage="【お問い合わせ内容】：必須入力です"></asp:RequiredFieldValidator>
</dd>
</dl>
</div>

<asp:ValidationSummary ID="ValidationSummary1" runat="server" CssClass="fAccent" />

<div class="submitButton">
<asp:Button ID="btnConfirm" Text="お問い合わせ内容を確認する" runat="server" OnClick="btnConfirm_Click" />
</div>
</div>
</section>
<!--/container-->


</article>
<!--/contact-->

</form>

<!--#include virtual="/common/include/footer.inc" -->

</body>
</html>
