$(document).ready(function () {
    $('.requireConfirmation').click(function (e) {
        var message = $(this).attr("data-confirm-msg") || "Are you sure?";
        var confirmation = confirm(message);
        if(!confirmation){
            e.preventDefault();
            return false;
        }
    });
});