$(document).ready(function () {
    $("body").append("<div class='disable-window'></div>");
    $(".mobile-left").children(".show-mobile").children(".show-menu").click(function () {
        $(".mobile-left").children("nav").toggleClass("show-nav");
        $(".disable-window").toggleClass("block");
        $("nav").css("height", $(window).height());
        $("nav").css("overflow", "auto");
        $("body").css("height", $(window).height());
        $("body").css("overflow", "hidden");
         $("body").css("z-index", "0");
    });
    $(".mobile-right").children(".show-mobile").children(".show-menu").click(function () {
        $(".mobile-right").children("nav").toggleClass("show-nav");
        $(".disable-window").toggleClass("block");
        $("nav").css("height", $(window).height());
        $("nav").css("overflow", "auto");
        $("body").css("height", $(window).height());
        $("body").css("overflow", "hidden");
         $("body").css("z-index", "0");
         $(".disable-window").css("z-index", "100");

    });
    $(".mobile-down").children(".show-mobile").children(".show-menu").click(function () {
        $(".mobile-down").children("nav").toggleClass("block");
    })


    $(".close-menu").click(function () {
        $(".mobile-left").children("nav").removeClass("show-nav");
        $(".mobile-right").children("nav").removeClass("show-nav");
        $(".mobile-down").children("nav").removeClass("block");
        $(".disable-window").removeClass("block");
        $("body").css("height", "auto");
        $("body").css("overflow", "auto");
    })
    $(".disable-window").click(function () {
        $(".mobile-left").children("nav").removeClass("show-nav");
        $(".mobile-right").children("nav").removeClass("show-nav");
        $(".mobile-down").children("nav").removeClass("block");
        $(".disable-window").removeClass("block");
        $("body").css("height", "auto");
        $("body").css("overflow", "auto");
    })
   

    


})
