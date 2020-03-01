/**
 * jQuery methods for the new form layout.
 *
 * 2012-09-21   Håkan Edling    @tidyui
 */

var posFixed = null;
var posTools = null;

$(document).ready(function () {
    posFixed = $('.btn-content').position();
    posTools = $('td.tools').position();

    $('.main-content .tools a').click(function () {
        var id = $(this).attr('data-id');

        $('.main-content .tools li').removeClass('active');
        $(this).parent().addClass('active');
        $('.main-content .main').addClass('hidden');
        $('#' + id).removeClass('hidden');

        return false;
    });
});

$(window).bind('scroll', function (e) {
    if (posTools) {
        if ($(this).scrollTop() + 55 > posTools.top) {
            $('td.tools').addClass('fixed-toolbar');
        } else {
            $('td.tools').removeClass('fixed-toolbar');
        }
    }

    if ($(this).scrollTop() > posFixed.top + 20) {
        if (!isFixed)
            $('.mce-toolbar').hide();
        $('.mce-toolbar').addClass('fixed-toolbar');
        if (!isFixed) {
            isFixed = true;
            $('.mce-toolbar').fadeIn('medium');
        }
    } else {
        $('.mce-toolbar').removeClass('fixed-toolbar');
        isFixed = false;
    }
});
