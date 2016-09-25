var _ua = (function(){
	return {
		lte_IE6:typeof window.addEventListener == "undefined" && typeof document.documentElement.style.maxHeight == "undefined",
		lte_IE7:typeof window.addEventListener == "undefined" && typeof document.querySelectorAll == "undefined",
		lte_IE8:typeof window.addEventListener == "undefined" && typeof document.getElementsByClassName == "undefined",
		lte_IE9:document.uniqueID && typeof window.matchMedia == "undefined",
		gte_IE10:document.uniqueID && window.matchMedia ,
		eq_IE8:document.uniqueID && document.documentMode === 8,
		eq_IE9:document.uniqueID && document.documentMode === 9,
//		eq_IE10:document.uniqueID && document.documentMode === 10,
//		eq_IE11:document.uniqueID && document.documentMode === 11,
//		eq_IE10:document.uniqueID && window.matchMedia && document.selection,
//		eq_IE11:document.uniqueID && window.matchMedia && !document.selection,
//		eq_IE11:document.uniqueID && window.matchMedia && !window.ActiveXObject,
		Trident:document.uniqueID
	}
})();
if(_ua.eq_IE8){
	document.createElement('header');
	document.createElement('footer');
	document.createElement('nav');
	document.createElement('article');
	document.createElement('section');
}

// --------------------

$(function(){
	var showFlag = false;
	var topBtn = $('#pageup');	
	topBtn.css('bottom', '-150px');
	var showFlag = false;
	$(window).scroll(function () {
		if ($(this).scrollTop() > 100) {
			if (showFlag == false) {
				showFlag = true;
				topBtn.stop().animate({'bottom' : '2px'}, 200); 
			}
		} else {
			if (showFlag) {
				showFlag = false;
				topBtn.stop().animate({'bottom' : '-150px'}, 200); 
			}
		}
	});
    topBtn.click(function () {
		$('body,html').animate({
			scrollTop: 0
		}, 200);
		return false;
    });
});

// --------------------

$(function() {
	var offsetY = -10;
	var time = 500;

	$('a[href^=#]').click(function() {
		var target = $(this.hash);
		if (!target.length) return ;
		var targetY = target.offset().top+offsetY;
		$('html,body').animate({scrollTop: targetY}, time, 'swing');
		window.history.pushState(null, null, this.hash);
		return false;
	});
});

// --------------------
$(function() {
	var $nav = $('#spMenuBox');
	// Nav Fixed
	$(window).scroll(function() {
		if ($(window).scrollTop() > 350) {
			$nav.addClass('fixed');
		} else {
			$nav.removeClass('fixed');
		}
	});
	// Nav Toggle Button
	$('#toggle').click(function(){
		$nav.toggleClass('open');
	});
});

