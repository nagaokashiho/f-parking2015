<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="f_parking.index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<meta name="viewport" content="width=device-width, user-scalable=yes, initial-scale=1, minimum-scale=1, maximum-scale=1.8" />
<title>福岡パーキングビル</title>
<meta name="description" content="福岡市天神博多周辺の駐車場は便利で安心な福岡パーキングビルへ" />
<meta name="keywords" content="駐車場,バイク,ガレージ,パーキング,タワー,福岡市,天神,博多,博多駅,博多シティ,呉服町,博多座,リバレイン,山笠,どんたく,アジア美術館," />
<meta name="abstract" content="時間・月極駐車場" />
<meta name="robots" content="index,follow" />
<meta name="author" content="福岡パーキングビル http://f-parking.co.jp/" />
<meta name="format-detection" content="telephone=no" />

<script type="text/javascript" src="//ajax.googleapis.com/ajax/libs/jquery/2.0.1/jquery.min.js"></script>
<script type="text/javascript" src="common/js/swiper.min.js"></script>
<script type="text/javascript" src="common/js/jquery.matchHeight.js"></script>
<script type="text/javascript" src="common/js/assembly.js"></script>

<link rel="stylesheet" href="common/css/baseStyle.css" />
<link rel="stylesheet" href="common/js/swiper.min.css" />

</head>

<body>
<!--#include virtual="/common/include/analytics.php" -->

<!--#include virtual="/common/include/header.inc" -->

<form id="frmIndex" runat="server">
<asp:SqlDataSource ID="sds" runat="server" 
        ConnectionString='<%$ ConnectionStrings:DefaultConnection %>'  
        SelectCommandType="StoredProcedure" 
        SelectCommand="SPR_SELECT_WhatsNews" 
        CancelSelectOnNullParameter="False">
    <SelectParameters>
        <asp:Parameter Name="Division" DefaultValue="0" Type="String" />
        <asp:Parameter Name="today" Type="Datetime" />
        <asp:Parameter Name="rowcount" Type="Int32" Direction="ReturnValue" />
    </SelectParameters>
</asp:SqlDataSource>


<article id="mainBody">

<section id="mainVisual">
<div class="swiper-container">
<div class="swiper-wrapper">
<div class="swiper-slide mv01"> 
<div class="photo"><img src="img/mainvisual_01.jpg" alt="" class="swiper-lazy">
<div class="swiper-lazy-preloader swiper-lazy-preloader-white"></div></div>
</div>
<div class="swiper-slide mv02">
<div class="copy"><h2><img src="img/mainvisual_text.png" srcset="img/mainvisual_text.png 1x, img/mainvisual_text_s.png 2x" alt="地下鉄「呉服町駅」から徒歩0分。天神にも博多にも簡単アクセス。"></h2></div>
<div class="photo"><img src="img/mainvisual_01.jpg" alt=""></div>
</div>-->
<div class="swiper-slide mv03">
<a href="parking/">
<dl class="mvParkingPhoto">
<dt><img src="img/mv_parking_text.png" alt="駐車場"></dt>
<dd><img src="img/mv_parking_photo.jpg" alt=""></dd>
</dl>
</a>
<a href="room/">
<dl class="mvRoomPhoto">
<dt><img src="img/mv_community_text.png" alt="貸会議室"></dt>
<dd><img src="img/mv_community_photo.jpg" alt=""></dd>
</dl>
</a>
<a href="garage/">
<dl class="mvGaragePhoto">
<dt><img src="img/mv_bike_text.png" alt="バイクガレージ"></dt>
<dd><img src="img/mv_bike_photo.jpg" alt=""></dd>
</dl>
</a>
</div>
</div>
</div>
</section>
<!--/mainVisual-->


<section id="news">
<div class="container">
<h2 class="title"><img src="img/news_title.png" alt="お知らせ"></h2>
<ul class="newsBox">

<asp:Repeater ID="repNews" DataSourceID="sds" runat="server">
<ItemTemplate>
<li class="article">
<div class="date"><%# Eval("dispPublicationDate") %></div>
<dl class="articleBox">
<dt class="subhead"><%# Eval("Title") %></dt>
<dd class="text"><%# ((string)Eval("Message")).Replace("\r\n", "<br />") %></dd>
</dl>
</li>
</ItemTemplate>
</asp:Repeater> 

</ul>
<div class="newsFooter">&nbsp;</div>
</div>
</section>
<!--/news-->


<section id="location">
<div class="container">
<h2 class="subhead"><img src="img/location_title.png" srcset="img/location_title.png 1x, img/location_title_s.png 2x" alt="天神にも博多にも近い"></h2>
<p class="text">
<img src="img/location_text.png" alt="徒歩0分の地下鉄利用で雨に天候に左右さず移動ができます。博多座、川端商店街、リバレイン（アジア美術館）まで徒歩圏内。天神駅、博多駅からも西鉄100円循環バスが運行しています。">
</p>
<div class="image"><img src="img/location_image.png"></div>
<ul class="photo">
<li><img src="img/location_photo01.jpg" srcset="img/location_photo01.jpg 1x, img/location_photo01_s.jpg 2x"></li>
<li><img src="img/location_photo02.jpg" srcset="img/location_photo02.jpg 1x, img/location_photo02_s.jpg 2x"></li>
</ul>
</div>
</section>
<!--/location-->


</article>
<!--/mainBody-->

</form>

<!--#include virtual="/common/include/footer.inc" -->


<script type="text/javascript">
$(function(){
	var mySwiper = new Swiper('.swiper-container', {
	paginationClickable: true,
	spaceBetween: 0,
	centeredSlides: true,
	autoplay: 2000,
	autoplayStopOnLast: true,
	speed: 2000,
	effect: 'fade'
});
});
$(function(){
	$('#news .title,#news .newsBox').matchHeight();
});
</script>


</body>
</html>
