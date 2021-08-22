$(".checkbox-menu").on("change", "input[type='checkbox']", function () {
    $(this).closest("li").toggleClass("active", this.checked);
});

$(document).on('click', '.dropdown .dropdown-menu', function (e) {
    e.stopPropagation();
});

// ToDo: gitfix
//$('.dropdown').on('click', function (e) {
//    var target = $(e.target);
//    var dropdown = target.closest('.checkbox-menu');
//    return !dropdown.hasClass('open') || !target.hasClass('keepopen');
//});

//$('#.dropdown').on('hide.bs.dropdown', function (e) {
//    var target = $(e.target);
//    if (target.hasClass("keepopen") || target.parents(".keepopen").length) {
//        return false; // returning false should stop the dropdown from hiding.
//    } else {
//        return true;
//    }
//});
//$(function () {
//    $('.dropdown').on({
//        "click": function (event) {
//            if ($(event.target).closest('.dropdown-toggle').length) {
//                $(this).data('closable', true);
//            } else {
//                $(this).data('closable', false);
//            }

//        },
//        "hide.bs.dropdown": function (event) {
//            hide = $(this).data('closable');
//            $(this).data('closable', true);
//         return hide;
//        }
//    });
//});

//$('.dropdown').on('show.bs.dropdown', function (event) {
//    if ($(event.target).closest('.dropdown-toggle').length) {
//                $(this).data('closable', true);
//            } else {
//               $(this).data('closable', false);
//           }
//});
//$('.dropdown').on('hide.bs.dropdown', function (event) {
//    if ($(event.target).closest('.dropdown-toggle').length) {
//        $(this).data('closable', true);
//    } else {
//        $(this).data('closable', false);
//    }
//});
//$('li.dropdown-li-list label').on('click', function (event) {
//    $(this).parent().toggleClass('open');
//});
//$('body').on('click', function (e) {
//    if (!$('li.dropdown-li-list').is(e.target)
//        && $('li.dropdown-li-list').has(e.target).length === 0
//        && $('.open').has(e.target).length === 0
//    ) {
//        $('li.dropdown-li-list').removeClass('open');
//    }
//});