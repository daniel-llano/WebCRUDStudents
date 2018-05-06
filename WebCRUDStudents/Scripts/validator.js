$(document).ready(function () {
    $('.error').hide();
    $('input[data-validator]').each(function (index, element) {
        if ($(this).data('validator') == 'number') {
            $(this).keydown(function (e) {
                // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                    // Allow: Ctrl+A, Command+A
                    (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
                    // Allow: home, end, left, right, down, up
                    (e.keyCode >= 35 && e.keyCode <= 40)) {
                    // let it happen, don't do anything
                    return;
                }
                // Ensure that it is a number and stop the keypress
                if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
                    e.preventDefault();
                }
            });
        }

        if ($(this).data('length') !== undefined) {
            $(this).keyup(function (e) {
                if ($(this).val().length > 0 && $(this).val().length != $(this).data('length')) {
                    $(this).parent().closest('div').removeClass("has-error has-feedback").addClass("has-error has-feedback");
                    $('[data-submit="true"]').prop("disabled", true);
                    $(this).next('.error').show();
                } else {
                    $(this).parent().closest('div').removeClass("has-error has-feedback");
                    $('[data-submit="true"]').prop("disabled", false);
                    $(this).next('.error').hide();
                }
            });

            $(this).change(function () {
                if ($(this).val().length > 0 && $(this).val().length != $(this).data('length')) {
                    $(this).parent().closest('div').removeClass("has-error has-feedback").addClass("has-error has-feedback");
                    $('[data-submit="true"]').prop("disabled", true);
                    $(this).next('.error').show();
                } else {
                    $(this).parent().closest('div').removeClass("has-error has-feedback");
                    $('[data-submit="true"]').prop("disabled", false);
                    $(this).next('.error').hide();
                }
            });
        }
    });
});