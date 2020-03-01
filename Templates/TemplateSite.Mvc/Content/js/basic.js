function openSopcastLinks(e) {
    e.preventDefault();

    var $self = $(this),
        $links = $self.closest('.match').find('.match__links');

    if ($links.length > 0) {
        $links.toggle();
    }
}

function onScrollSpy(e) {
    e.preventDefault();

    if (!$('.tab-header').is('.fixed-header')) return;

    var $self = $(this),
        target = $self.attr('href') || $self.attr('data-href'),
        $target = $(target),
        $container = $($self.attr('data-scrollSpy'));

    if ($container.length == 0) {
        $container = $('html, body');
    }

    if ($target == null || $target.length === 0) return;

    var targetPosition = $target.offset().top - 52;

    $container.animate({ scrollTop: targetPosition }, 500);
}

$(document).ready(function () {
	 /*scrollTop*/
	 $('.scroll-top').click(function(event) {
	 	$('html, body').animate({scrollTop: 0}, 300);
	 });		
	tabForGame();
	toggleMainMenu();
	activeCateMobile();

	$('.guide-nav').stickyfloat({ duration: 400, offsetY: 0 });
	$('.guide-nav a.collection-item').click(function (e) {
	    e.preventDefault();
	    var target = $(this).attr('href');
	    $('html, body').animate({ scrollTop: $(target).offset().top }, 400);
	});

    // fixed tabs
	var $tabHeader = $('.tab-header'),
        tabHeaderTop = 0;
	if ($tabHeader.length > 0) {
	    $tabHeader.css({ width: $tabHeader.outerWidth() });
	    tabHeaderTop = $tabHeader.offset().top;
	}

    // resize header trigger
	// var resize = false;
	$(window).on('resize', function () {	    
	    if ($tabHeader.length > 0) {	        
	        $tabHeader.css({ width: $tabHeader.outerWidth() });
	        tabHeaderTop = $tabHeader.offset().top;
	    } else {
	        
	    }
	});

    // scroll and fixed header
	$(window).on('scroll', function () {
	    if (tabHeaderTop > 0) { // has tab
	        var scrollTop = $(window).scrollTop();

	        if (scrollTop > (tabHeaderTop - 50)) {
	            $tabHeader.addClass('fixed-header');
	        } else {
	            $tabHeader.removeClass('fixed-header');
	        }
	    }
	});

	$('.main-menu a').each(function (i, e) {
	    var $a = $(e),
            text = $a.text();

	    $a.attr('data-title', text);
	});

	$(document).on('click', '[data-scrollSpy]', onScrollSpy);
	// $(document).on('click', '.btn-sopcast-link', openSopcastLinks);

	$('.tabs .tab-header').on('click', 'a', function (e) {
	    e.preventDefault();

	    var $li = $(this).parent('li'),
            target = $(this).attr('href');
        
	    // remove other .in
	    $li.closest('.tabs').find('li.in').removeClass('in');

	    // add current .in
	    $li.addClass('in');

	    // find target
	    console.log('target', target);
	    var $target = $(target);

	    if ($target.length == 0) {
	        console.log('target not found');
	        return;
	    }

	    // active target
	    $target.closest('.tab-container').find('.tab-item').removeClass('active');
	    $target.closest('.tab-item').addClass('active');
	});
});

function addBookmark(title, url, msg, msgFF) {
    try {
        window.external.AddFavorite(url, title);
    } catch (e) {
        if (window.sidebar) { // Firefox
            //window.sidebar.addPanel(title, url, ""); //Dont use until the FF bug is fixed
            alert(msgFF);
        }
        else if (window.opera && window.print) { // Opera
            var elem = document.createElement('a');
            elem.setAttribute('href', url);
            elem.setAttribute('title', title);
            elem.setAttribute('rel', 'sidebar');
            elem.click();
        }
        else { alert(msg); }
    }
}
function setHomepage(obj, url, msg, msgFF) {
    try {
        obj.style.behavior = 'url(#default#homepage)';
        obj.setHomePage(url);
    } catch (e) {
        if (window.sidebar) {
            if (window.netscape) {
                try {
                    netscape.security.PrivilegeManager.enablePrivilege("UniversalXPConnect");
                } catch (e) { alert(msg); }
            }
            var prefs = Components.classes['@mozilla.org/preferences-service;1'].getService(Components.interfaces.nsIPrefBranch);
            prefs.setCharPref('browser.startup.homepage', url);
        }
        else { alert(msg); }
    }
}

function tabForGame(){
	$('.list-games .tabs .tab').click(function(){
		$('.list-games .tabs .tab').removeClass('active');
		$(this).addClass('active');
		$('.block-item-slider').removeClass('active');
		$('.block-item-slider').eq($(this).index()).addClass('active');
	});
}
function sliderGameAggregate(){
    //slider game in aggregate page

    if (!$.fn.owlCarousel) {
        console.log('owl carousel not found');
        return;
    }

	$('.jsSlider').owlCarousel({
	    loop:true,
	    autoplay:true,
	    responsiveClass:true,
	    nav: true,
	    navText:["",""],
	    responsive:{
	        400:{
	            items:3
	        },
	        700:{
	            items:5,
	        }
	    }
	});
}
/*function sliderGameAggregateDetail(){
	//jsSlider-detail
	var objSlider = $('.jsSlider-detail');
	$( objSlider).each(function( index ) {
		  if($(this).children('.single-item-slider').length > 1){
			$(this).owlCarousel({
			    loop:true,
			    responsiveClass:true,
			    nav: true,
			    navText:["",""],
			    items: 1,
			    smartSpeed:450
			});
		}
	});	
}*/
function toggleMainMenu(){
	$('.menu-mobile').click(function(event) {
		/* Act on the event */
		$(this).toggleClass('active');
	    // $('#header .main-menu').toggleClass('active');
		$('#header .main-menu').slideToggle(400, function () {
		    $(this).css({ display: '' }).toggleClass('mobile-show');
		});
	});
}
function activeCateMobile(){
	$('.tab-cate-mobile .tab-item').click(function(event) {
		$(this).parent('.tab-cate-mobile').find('.tab-item').removeClass('active');
		$(this).addClass('active')
		$(this).parent('.tab-cate-mobile').siblings('.cont-cate'). children('.cols_').removeClass('active-mobile');
		$(this).parent('.tab-cate-mobile').siblings('.cont-cate'). children('.cols_').eq($(this).index()).addClass('active-mobile');
	});
}