jQuery.fn.andSelf = jQuery.fn.addBack;
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

//右側選單
$(function () {
    //當有人拉動捲軸的時候 就會觸發事件做動作
    $(window).scroll(function () {
        var NOW = $(window).scrollTop(); //抓目前網頁捲軸的座標
        if (NOW > 1) {
            $("#ICON").stop(true, false).animate({ top: NOW + 120 }, 500, "easeOutBack");
        } else {
            $("#ICON").stop(true, false).animate({ top: -1000 }, 500, "easeOutBack");
        }
        // if 如果當捲軸大於 800px 的時候 DIV 就會滑下來出現
        // else 否則 DIV 就會滑上去消失
    });
});

//手機版右拉出選單
$(function(){
var sideslider = $('[data-toggle=collapse-side]');
    var get_sidebar = sideslider.attr('data-target-sidebar');
    var get_content = sideslider.attr('data-target-content');
    sideslider.click(function(event){
      $(get_sidebar).toggleClass('in');
      $(get_content).toggleClass('out');
   });
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
		autoplayTimeout:5000,
		responsive: {
		  0: {items: 1},
		  600: {items: 1 },
		  1000: {items: 1 }
         }
	  });
});


//首頁最新消息
$(function () {
	var owl = $('.banner_02');
	  owl.owlCarousel({
		margin: 30,
		nav: true,
		navText: false,
		dots: false,
		loop: true,
		autoplay:false,
		autoplayHoverPause:true,
		autoplayTimeout:4000,
		responsive: {
		  0: {
			items: 1
		  },
		  480: {
			items: 2
		  },
		  768: {
			items: 3
		  },
		  991: {
			items: 4
		  }
		}
	  });
});


//首頁學員心得
$(function () {
	var owl = $('.banner_03');
	  owl.owlCarousel({
		margin: 20,
		nav: true,
		navText: false,
		dots: false,
		loop: true,
		autoplay:false,
		autoplayHoverPause:true,
		autoplayTimeout:4000,
		responsive: {
		  0: {
			items: 1
		  },
		  768: {
			items: 1
		  },
		  991: {
			items: 2
		  }
		}
	  });
});

	$(function(){		
		$("#icon_btn").click(function(){
			$(".icon_link").slideToggle(300);
			var collapsed=$(this).find('i').hasClass('fa-times');
			$('.icon_link').find('i').removeClass('fa-tags');
			$('.icon_link').find('i').addClass('fa-times');
			if(collapsed)
				$(this).find('i').toggleClass('fa-times fa-2x fa-tags fa-2x');
		});	
	});


//Gotop回頂端按鈕
$(document).ready(function(){
	$("#gotop").click(function(){
		$("html,body").animate({scrollTop:0},1900,"easeInOutQuint");
		return false;
	});
});

