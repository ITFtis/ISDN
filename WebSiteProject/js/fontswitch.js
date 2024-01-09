function getCookie(name){
	var cname = name + "=";
	var dc = document.cookie;
	if (dc.length != 0) 
	{
		begin = dc.indexOf(cname);
		if(begin != -1) 
		{
			begin += cname.length;
			end = dc.indexOf(";", begin);
			if(end == -1) end = dc.length;
			return unescape(dc.substring(begin, end));
		}
	}
	return null;
}

function setCookie(name, value, expires) {
	document.cookie = name + "=" + escape(value) + ((expires == null) ? "" : "; expires=" + expires.toGMTString()) + "; path=/";
}

function setActiveStyleSheet(title, reset) {
  var i, a, main, j;
  j=0
  for(i=0; (a = document.getElementsByTagName("link")[i]); i++) {
    if(a.getAttribute("rel").indexOf("style") != -1 && a.getAttribute("title")) {
			a.disabled = true;
			j++;
			if (title == null && j==1) a.disabled = false;
      if(a.getAttribute("title") == title) {
				a.disabled = false;
			}
		}
	}
  //document.all.HideTextSize.value = title;
	//alert(title);
  if (reset == 1) {
		//alert("W->" + title);
		setCookie("wstyle", title , null);
	
	  //createCookie("wstyle", title, 365);
  }
}

function fontline(name)
{
    var fss=document.getElementById('fss');
    var fsm=document.getElementById('fsm');
    var fsl=document.getElementById('fsl');
	var cname1 = name + "=";
	var dc1 = document.cookie;
	var tt="";
	if (dc1.length != 0) 
	{
		begin = dc1.indexOf(cname1);
		if(begin != -1) 
		{
			begin += cname1.length;
			end = dc1.indexOf(";", begin);
			if(end == -1) end = dc1.length;
			
		   tt=unescape(dc1.substring(begin, end));
		}
	}

//alert(tt);
	if (tt=='') tt='smalltext';
	fss.src='img/f_s.png';
	fsm.src='img/f_m.png';
	fsl.src='img/f_l.png';
	
	if (tt=='smalltext')
	fss.src='img/f_s_1.png';
	if (tt=='mediumtext')
	fsm.src='img/f_m_1.png';
	if (tt=='largetext')
	fsl.src='img/f_l_1.png';
			   
		
	return null;

}



function Sfontline(name,t1)
{
	var cname1 = name + "=";
	var dc1 = document.cookie;
	var tt="";
	var tt1=t1;
	if (dc1.length != 0) 
	{
		begin = dc1.indexOf(cname1);
		if(begin != -1) 
		{
			begin += cname1.length;
			end = dc1.indexOf(";", begin);
			if(end == -1) end = dc1.length;
			
		   tt=unescape(dc1.substring(begin, end));
		}
	}
	//alert(tt);
	if  (tt1=='小')
	{       if (tt=='smalltext')
				 return '<u>小</u>';
		else
			 return '小';
	}
	
	if  (tt1=='中')
	{       if (tt=='mediumtext')
			 return '<u>中</u>';
		else
			 return '中';
	}
	
	if  (tt1=='大')
	{       if (tt=='largetext')
			 return '<u>大</u>';
		else
			 return '大';
	}
		
	return null;

}