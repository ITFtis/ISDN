//include共用頁面
$(function ($) {
	$.include = function (url) {
		$.ajax({
			url: url,
			async: false,
			success: function (result) {
				document.write(result);
			}
		});
	};
}(jQuery));


//header高度清除
$(function () {
	$("#header").removeAttr("style"); 
	$(".header-logo").removeAttr("style"); 
});


//首頁主廣告圖
$(function () {
	var owl = $('.banner_01');
	  owl.owlCarousel({
		margin: 10,
		nav: true,
		navText: false,
		loop: true,
		autoplay:true,
		autoplayHoverPause:true,
		autoplayTimeout:4000,
		responsive: {
		  0: {
			items: 1
		  },
		  600: {
			items: 1
		  },
		  1000: {
			items: 1
		  }
		}
	  })
});

//首頁主廣告圖(中)
$(function () {
	var owl = $('.banner_02');
	  owl.owlCarousel({
		margin: 10,
		nav: true,
		navText: false,
		loop: true,
		autoplay:false,
		autoplayHoverPause:true,
		autoplayTimeout:4000,
		responsive: {
		  0: {
			items: 1
		  },
		  600: {
			items: 1
		  },
		  1000: {
			items: 1
		  }
		}
	  })
});


//lightbox
$(document).ready(function(){
	//$(".lightbox-2").lightbox({
	//	 scaleImages: true,
	//	 xScale: 1.0,
	//	 yScale: 1.0,
	//	 displayDownloadLink: false
	//});
});







