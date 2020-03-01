$(document).ready(function () {
    $('.guide-nav').stickyfloat({ duration: 400, offsetY: 32 });
    $('.guide-nav a.collection-item').click(function (e) {
        e.preventDefault();
        var target = $(this).attr('href');
        $('html, body').animate({ scrollTop: $(target).offset().top }, 400);
    });
});