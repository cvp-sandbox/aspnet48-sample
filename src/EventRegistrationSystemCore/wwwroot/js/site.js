// Legacy JavaScript functionality
$(document).ready(function () {
    // Initialize Bootstrap components
    $('[data-toggle="tooltip"]').tooltip();
    $('[data-toggle="popover"]').popover();

    // Handle navbar collapse for mobile devices
    $('.navbar-toggler').on('click', function () {
        $('.navbar-collapse').toggleClass('show');
    });

    // Legacy form validation compatibility
    $.validator.setDefaults({
        highlight: function (element) {
            $(element).closest('.form-group').addClass('has-error');
        },
        unhighlight: function (element) {
            $(element).closest('.form-group').removeClass('has-error');
        },
        errorElement: 'span',
        errorClass: 'text-danger',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });

    // Bootstrap 3 to Bootstrap 5 compatibility
    // Map data-toggle to data-bs-toggle for dynamically added elements
    $(document).on('click', '[data-toggle="dropdown"]', function() {
        $(this).attr('data-bs-toggle', 'dropdown').dropdown('toggle');
    });

    $(document).on('click', '[data-toggle="modal"]', function() {
        var target = $(this).data('target');
        $(target).modal('show');
    });
});
