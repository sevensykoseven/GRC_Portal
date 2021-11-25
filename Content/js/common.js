"use strict";

window.chartColorsSolid = {
    red: 'rgb(255, 99, 132)',
    orange: 'rgb(255, 159, 64)',
    yellow: 'rgb(255, 205, 86)',
    green: 'rgb(75, 192, 192)',
    blue: 'rgb(54, 162, 235)',
    purple: 'rgb(153, 102, 255)',
    grey: 'rgb(201, 203, 207)'
};

window.chartColorsTrans = {
    red: 'rgba(255, 99, 132, 0.05)',
    orange: 'rgba(255, 159, 64, 0.05)',
    yellow: 'rgba(255, 205, 86, 0.05)',
    green: 'rgba(75, 192, 192, 0.05)',
    blue: 'rgba(54, 162, 235, 0.05)',
    purple: 'rgba(153, 102, 255, 0.05)',
    grey: 'rgba(201, 203, 207, 0.05)'
};

/*
 * CleanUpInputs()
 * Removes HTML characters from inputs
 */
function CleanUpInputs() {
    $('input[type=text], textarea').keyup(function () {
        var t = $(this).val();
        var exp = /[<>]/;
        if (exp.test(t)) {
            t = t.replace(/[<>]/g, '');
            $(this).val(t);
            console.log('removed characters');
        }
    });
    $('input[type=text], textarea').blur(function () {
        var t = $(this).val();
        var exp = /[<>]/;
        if (exp.test(t)) {
            t = t.replace(/[<>]/g, '');
            $(this).val(t);
            console.log('removed characters');
        }
    });
}

/*
 * function OnPageLoad()
 * Should be called when page loads.
 */
function OnPageLoad() {
    if (typeof CleanUpInputs === 'function') { CleanUpInputs(); }

    // Set focus
    $("input[type=text], textarea").first().focus();

    //set focus on modals, add delay to counter the time to fade in
    $(".modal").on('show.bs.modal', function () {
        var modal = this;
        setTimeout(function () {
            try {
                directInput === "undefined" ? $(modal).find('input:text, textarea').first().focus() : $("#" + directInput).focus();
                directInput = "undefined";
            } catch (err) { }
        }, 1000);
    });

    $("#message-panel-close").click(function () {
        $('#message-panel').hide();
    });
}

/*
 * function OnAjaxSuccess()
 * Should be called on Ajax responses
 */
function OnAjaxSuccess() {
    if (typeof CleanUpInputs === 'function') { CleanUpInputs(); }    
}

/*
 * function OnAjaxError()
 * Should be called on Ajax Errors
 */
function OnAjaxError(r) {
    if (r.status === 401) {
        location.reload();
    }
    $('#ErrorModal').modal('show');
    $('#cover-spin').hide(0);
    console.log("Ready State: " + r.readyState);
    console.log(r.status + " " + r.statusText);
    console.log(r.responseText);
}

function RemoveRow(elem) {
    elem.parent().addClass("alert-warning").fadeOut("slow", function () { //fade out first
        $(this).remove(); //remove the row from the UI after
    });
};

/*
 * Show site message
 */
function ShowMessage(message, classes, time) {
    $('#message-panel .message').html(message);
    setTimeout(function () {
        $('#message-panel').removeAttr("style").addClass(classes + ' slideDown');
    }, 1000);
    HideMessage(time == null ? 5000 : time);
}

/*
 * Hide site message
 */
function HideMessage(time) {
    setTimeout(function () {
        $('#message-panel').slideUp("slow").promise().done(function () {
            setTimeout(function () {
                var classList = $('#message-panel').attr('class').split(/\s+/);
                $.each(classList, function (index, item) {
                    $('#message-panel').removeClass(item);
                });
            }, 100);
        });
    }, time == null ? 5000 : time);
}

/*
 * function OnClickableRow()
 * Should be called from .clickable rows
 */
function OnClickableRow(url, id, event) {
    event.preventDefault();  //prevent default click behavior
    $(location).attr("href", url + "/" + encodeURIComponent(id));
}

/*
 * Show working image
 */
function ShowWorkingImage() {
    $('#cover-spin').show(0);
}

/*
 * Hide working image
 */
function HideWorkingImage() {
    $('#cover-spin').hide(0);
}

/*
 * Get list of colors for charts
 */
function GetColorList() {
   // return ['#4e73df', '#1cc88a', '#36b9cc', '#e74a3b', '#e67e22', '#f6c23e', '#9b59b6', '#1abc9c', '#2ecc71', '#3498db'];
    return ['#1abc9c', '#2ecc71', '#3498db', '#9b59b6', '#34495e', '#16a085', '#27ae60', '#2980b9', '#8e44ad', '#2c3e50'];
}

/*
 * Get list of hover colors for charts
 */
function GetColorHoverList() {
    return ['#1edab5', '#47d583', '#51a7e0', '#a971c0', '#405a74', '#1abe9e', '#2dca6f', '#3293d2', '#9e56bd', '#384f66'];
}

function GoBackOne() {
    if (window.history)
        history.go(-1);
    else //fallback i guess
        $(location).attr("href", "/dashboard");
};