<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="confirm.aspx.cs" Inherits="f_parking.contact.confirm" %>

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

<form id="frmConfirm" runat="server">

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
入力項目を確認のうえ、「お問い合わせ内容を送信する」ボタンを押してください。
</p>

<div class="formBox">
<dl>
<dt>
お問い合わせ項目<strong class="fAccent">＊</strong>
</dt>
<dd>
<asp:Label ID="lblContactKind" runat="server"></asp:Label>
</dd>
</dl>
<dl>
<dt>
お名前<strong class="fAccent">＊</strong>
</dt>
<dd>
<asp:Label ID="lblName" runat="server"></asp:Label>
</dd>
</dl>
<dl>
<dt>
ご住所
</dt>
<dd>
<asp:Label ID="lblPref" runat="server"></asp:Label>
<br>
<asp:Label ID="lblAddress" runat="server"></asp:Label>
</dd>
</dl>
<dl>
<dt>
お電話番号
</dt>
<dd>
<asp:Label ID="lblTel" runat="server"></asp:Label>
</dd>
</dl>
<dl>
<dt>
メールアドレス<strong class="fAccent">＊</strong>
</dt>
<dd>
<asp:Label ID="lblMail" runat="server"></asp:Label>
</dd>
</dl>
<dl>
<dt>
お問い合わせ内容<strong class="fAccent">＊</strong>
</dt>
<dd>
<div class="textBox" style="word-wrap: break-word;">
<asp:Label ID="lblMessage" runat="server"></asp:Label>
</div>
</dd>
</dl>
</div>
<asp:Label ID="lblError" CssClass="fAccent" runat="server"></asp:Label>
<div class="submitButton">
<asp:Button ID="btnReturn" Text="戻る" runat="server" OnClick="btnReturn_Click" /><span style="margin:0 10px;"></span>
<asp:Button ID="btnSend" Text="お問い合わせ内容を送信する" runat="server" OnClick="btnSend_Click" />
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
