$(document).ready(function () {

    
    /*---------------------------------------------------modal---------------------------------*/
    $(".bs-modal-creat").click(function () {
        swal({
            title: 'Creat account',
            html: ' <form class="bs-form form-modal form-creat">' +
                '<label for="account">Account :</label>' +
                '<input type="text" class="bs-input input-modal" id="account" placeholder="Creat account"><br>' +
                '<label for="email">Email :</label>' +
                '<input type="text" class="bs-input input-modal" id="email" placeholder="Your email"><br>' +
                '<label for="password">Password :</label>' +
                '<input type="password" class="bs-input input-modal" id="password" placeholder="Creat password"><br>' +
                '<input type="checkbox" name="check" value="checked" id="check">' +
                '<label for="check"> Check</label><br>' +
                '<button type="submit" class="bs-btn btn-modal" id="submit">Creat</button>' +
                '</form>',
        })

    })
    $(".bs-modal-login").click(function () {
        swal({
            title: 'Login',
            html: ' <form class="bs-form form-modal form-login">' +
                '<label for="account">Account :</label>' +
                '<input type="text" class="bs-input input-modal" id="account" placeholder="Your account"><br>' +
                '<label for="password">Password :</label>' +
                '<input type="password" class="bs-input input-modal" id="password" placeholder="Your password"><br>' +
                '<input type="checkbox" name="check" value="checked" id="check">' +
                '<label for="check"> Check</label><br>' +
                '<button type="submit" class="bs-btn btn-modal" id="submit">Login</button>' +
                '</form>',
        })

    })
    $(".bs-modal-messages").click(function () {
        swal({
            title: 'Messages',
            html: ' <form class="bs-form form-modal form-messages">' +
                '<label for="email">Email :</label>' +
                '<input type="text" class="bs-input input-modal" id="email" placeholder="Your email"><br>' +
                '<label for="messages">Your messages :</label><br>' +
                '<textarea class="bs-messages" id="messages" placeholder="Your messages..."></textarea><br>' +
                '<input type="checkbox" name="check" value="checked" id="check" >' +
                '<label for="check"> Check</label><br>' +
                '<button type="submit" class="bs-btn btn-modal" id="submit">Send</button>' +
                '</form>',
        })

    })
/*--------------------------------set images with parent------------------------------*/
    $(window).on("load", function () {
        $(".ImagesFrameCrop").each(function () {
            $(this).removeClass("wide")
            $(this).removeClass("tall")
            if ($(this).width() / $(this).height() > $(this).parent().width() / $(this).parent().height()) {
                $(this).addClass("wide");
            } else {
                $(this).addClass("tall");
            }
        });
         $(".ImagesFrameCrop").children("img").each(function () {
            $(this).removeClass("wide")
            $(this).removeClass("tall")
            if ($(this).width() / $(this).height() > $(this).parent().width() / $(this).parent().height()) {
                $(this).addClass("wide");
            } else {
                $(this).addClass("tall");
            }
        });
    

        $(".ImagesFrameCrop1").each(function () {
            $(this).removeClass("wide")
            $(this).removeClass("tall")
            if ($(this).width() / $(this).height() > $(this).parent().width() / $(this).parent().height()) {
                $(this).addClass("wide");
            } else {
                $(this).addClass("tall");
            }
        });
        $(".ImagesFrameCrop1").children("img").each(function () {
            $(this).removeClass("wide")
            $(this).removeClass("tall")
            if ($(this).width() / $(this).height() > $(this).parent().width() / $(this).parent().height()) {
                $(this).addClass("wide");
            } else {
                $(this).addClass("tall");
            }
        });
    

        $(".ImagesFrameCrop0").each(function () {
            $(this).removeClass("wide")
            $(this).removeClass("tall")
            if ($(this).width() / $(this).height() > $(this).parent().width() / $(this).parent().height()) {
                $(this).addClass("wide");
            } else {
                $(this).addClass("tall");
            }
        });
         $(".ImagesFrameCrop0").children("img").each(function () {
            $(this).removeClass("wide")
            $(this).removeClass("tall")
            if ($(this).width() / $(this).height() > $(this).parent().width() / $(this).parent().height()) {
                $(this).addClass("wide");
            } else {
                $(this).addClass("tall");
            }
        });
    });


})
