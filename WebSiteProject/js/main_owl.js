//首頁活動專區
$(function () {
	var owl = $('.banner_01');
	  owl.owlCarousel({
		nav: true,
		navText: true,
		loop: true,
		autoplay:true,
		dots: false,
		video:true,
		videoWidth: false, // Default false; Type: Boolean/Number
        videoHeight: false, // Default false; Type: Boolean/Number
		margin:20,
		autoplayHoverPause:true,
		autoplayTimeout:6000,
		responsive: {
		  0: {
			items: 1
		  },
		  480: {
			items: 1
		  },
		  768: {
			items: 1
		  },
		  992: {
			items: 1
		  }
		}
	  });
});


//首頁相關連結
$(function () {
	var owl = $('.link_01');
	  owl.owlCarousel({
		nav: true,
		navText: true,
		loop: true,
		autoplay:true,
		dots: false,
		video:true,
		videoWidth: false, // Default false; Type: Boolean/Number
        videoHeight: false, // Default false; Type: Boolean/Number
		margin:30,
		autoplayHoverPause:true,
		autoplayTimeout:6000,
		responsive: {
		  0: {
			items: 2
		  },
		  480: {
			items: 3
		  },
		  768: {
			items: 4
		  },
		  992: {
			items: 5
		  }
		}
	  });
	  
});


//更多影片
$(function () {
	var owl = $('.other_video_list');
	  owl.owlCarousel({
		nav: false,
		navText: false,
		loop: true,
		autoplay:true,
		dots: true,
		video:true,
		videoWidth: false, // Default false; Type: Boolean/Number
        videoHeight: false, // Default false; Type: Boolean/Number
		margin:10,
		autoplayHoverPause:true,
		autoplayTimeout:6000,
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
		  992: {
			items: 4
		  }
		}
	  });
});


//owl-carousel 加tabindex=-1
$(function () {
	$('.cloned').find('a').attr('tabindex','-1');
});


//左右鍵盤操作
$(function () {
	var owl = $(".owl-carousel");

        $(document).on('keydown', function (e) {

            var $focusedElement = $(document.activeElement),
                singleOwl = $(".owl-carousel"),
                type = e.which == 39 ? 'next' : null,
                type = e.which == 37 ? 'prev' : type,
                type = e.which == 13 ? 'enter' : type;

            singleOwl.owlCarousel();

            // if the carousel is focused, use left and right arrow keys to navigate
            console.log($focusedElement.attr('class'));
            if ($focusedElement.attr('class') == "owl-carousel") {
                debugger;
                if (type == 'next') {
                    singleOwl.trigger('next.owl.carousel');
                } else if (type == 'prev') {
                    singleOwl.trigger('prev.owl.carousel');
                }

            } else if (type == 'enter') {
                if ($focusedElement.hasClass('owl-next')) {

                    singleOwl.trigger('next.owl.carousel');
                } else if ($focusedElement.hasClass('owl-prev')) {

                    singleOwl.trigger('prev.owl.carousel');
                }
            }
        });
});


//輪播圖左告按鈕加title
window.onload=function(){
	$(".owl-prev").attr("title","prev").attr("tabindex","0");
	$(".owl-next").attr("title","next").attr("tabindex","0");
    $(".tp-leftarrow").attr("title","prev");
	$(".tp-rightarrow").attr("title","next");
}





