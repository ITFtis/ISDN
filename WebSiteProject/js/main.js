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


//清除header style 高度, 需於頁面最後執行
$(function () {
	$("#header").removeAttr("style"); 
	$(".header-logo").removeAttr("style"); 
});


//輪播圖左告按鈕加title
window.onload=function(){
	$(".owl-prev").attr("title","prev").attr("tabindex","0");
	$(".owl-next").attr("title","next").attr("tabindex","0");
    $(".tp-leftarrow").attr("title","prev");
	$(".tp-rightarrow").attr("title","next");
}


//font-size
function size_set(mysize){
    if(mysize=="font_l"){
        document.body.className="font_l";
    }
    if(mysize=="font_m"){
        document.body.className="font_m";
    }
    if(mysize=="font_s"){
        document.body.className="font_s";
    }
}


//滾動效果
$(function($) {
	'use strict';
	window.sr= new scrollReveal({
	  reset: false,
	  move: '50px',
	  mobile: false
	});
});


//icon font 加 aria-hidden="true"
$(function () {
	/*$(".fa").attr("aria-hidden","true") ;
	$(".fas").attr("aria-hidden","true") ;
	$(".fab").attr("aria-hidden","true") ;*/
	 document.getElementById("web_top").innerHTML = '回頂端';
})


//menu
//icon無障礙設定
$(function () {
    $(".nav .fa-caret-down").attr("aria-hidden", "true");
});

//onclick menu
$(function () {
    $('#mainNav > li').each(function () {
        if ($(this).find('ul').length > 0) {
            $(this).find('a').first().prop('href', '#');
        }
    })

    $('#mainNav > li > ul > li').each(function () {

        if ($(this).find('ul').length > 0) {
            $(this).find('a').first().prop('href', '#');

        }
    })

    $('ul.dropdown-menu [data-toggle=dropdown]').on('click', function (event) {
        event.preventDefault();
        event.stopPropagation();
        $(this).parent().siblings().removeClass('open');
        $(this).parent().toggleClass('open');
    });

    $('#mainNav > li > a').keydown(function (event) {

        if (event.which == 13) {


            //reset all tabindex
            $('#mainNav > li > ul').find('a').prop('tabindex', '-1');

            //set level2 tabindex
            $(this).parent().find('ul > li').children('.2nd').prop('tabindex', '0');

            //open menu
            $('#mainNav > li').removeClass('open');

            $(this).parent().parent().addClass('open');
            $(this).parent().find('.dropdown').addClass('open');
        }
    })

    $('#mainNav > li > a').focus(function () {

        if ($('#mainNav > li').hasClass('open') && $(this).parent().hasClass('open') == false) {
            $('#mainNav > li').removeClass('open');
            //reset all tabindex
            $('#mainNav > li > ul').find('a').prop('tabindex', '-1');
        }
    })

    $('#mainNav > li:last').find('a:last').focusout(function () {
        $('#mainNav > li').removeClass('open');
        $('#mainNav > li > ul').find('a').prop('tabindex', '-1');
        $('.dropdown-submenu > .dropdown-menu').removeClass('open');
        $('.dropdown-submenu > .dropdown-menu > a').prop('tabindex', '-1');
    })

    $('.dropdown-submenu > a').keydown(function (event) {

        if (event.which == 13) {

            //reset all tabindex
            $('.dropdown-submenu > ul > li > a').prop('tabindex', '-1');
            $(this).parent().find('a.3rd').prop('tabindex', '0');

            $(this).parent().find('ul').addClass('open');
            $(this).parent().parent().parent().addClass('open');
        }
    })

    $('.2nd').focus(function () {

        $('.dropdown-submenu > .dropdown-menu').removeClass('open');
        $('.dropdown-submenu > .dropdown-menu > a').prop('tabindex', '-1');
    })

});


