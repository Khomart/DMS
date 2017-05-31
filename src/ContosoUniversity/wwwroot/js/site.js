// Write your Javascript code.
function setupPickersYearPage() {
    var startDateTextBox = $('#Summer1input');
    var endDateTextBox = $('#Summer1inputEnd');
    $.timepicker.dateRange(
        startDateTextBox,
        endDateTextBox,
        {
            minInterval: (1000 * 60 * 60 * 24 * 31), // 4 days
            maxInterval: (1000 * 60 * 60 * 24 * 150), // 150 days
            start: {}, // start picker options
            end: {} // end picker options
        }
    );
    var startDateTextBox2 = $('#Summer2input');
    var endDateTextBox2 = $('#Summer2inputEnd');
    $.timepicker.dateRange(
        startDateTextBox2,
        endDateTextBox2,
        {
            minInterval: (1000 * 60 * 60 * 24 * 31), // 4 days
            maxInterval: (1000 * 60 * 60 * 24 * 150), // 150 days
            start: {}, // start picker options
            end: {} // end picker options
        }
    );
    var startDateTextBox3 = $('#Summer_longinput');
    var endDateTextBox3 = $('#Summer_longinputEnd');
    $.timepicker.dateRange(
        startDateTextBox3,
        endDateTextBox3,
        {
            minInterval: (1000 * 60 * 60 * 24 * 31), // 4 days
            maxInterval: (1000 * 60 * 60 * 24 * 150), // 150 days
            start: {}, // start picker options
            end: {} // end picker options
        }
    );
    var startDateTextBox4 = $('#Fallinput');
    var endDateTextBox4 = $('#FallinputEnd');
    $.timepicker.dateRange(
        startDateTextBox4,
        endDateTextBox4,
        {
            minInterval: (1000 * 60 * 60 * 24 * 31), // 4 days
            maxInterval: (1000 * 60 * 60 * 24 * 150), // 150 days
            start: {}, // start picker options
            end: {} // end picker options
        }
    );
    var startDateTextBox5 = $('#Winterinput');
    var endDateTextBox5 = $('#WinterinputEnd');
    $.timepicker.dateRange(
        startDateTextBox5,
        endDateTextBox5,
        {
            minInterval: (1000 * 60 * 60 * 24 * 31), // 4 days
            maxInterval: (1000 * 60 * 60 * 24 * 150), // 150 days
            start: {}, // start picker options
            end: {} // end picker options
        }
    );

    $("form").on("submit", function (event) {
        event.preventDefault();
        var flag = false;
        $("input[type=datetime]").each(function () {
            if (checkinput($(this)) == false)
                flag = true;
        });
        if (flag == false) {
            $('form').unbind().submit();
            $("form").submit();
        }
    });
    $("input[type=datetime]").on("click",function(){
        $("input[type=datetime]").each(function () {
            if ($(this).hasClass("incorrectinputDate"))
                $(this).removeClass("incorrectinputDate");
        })
    });
    function checkinput(inputField) {
        val = inputField.val();
        val1 = Date.parse(val);
        if (isNaN(val1) == true) {
            inputField.addClass("incorrectinputDate");
            return false;
        }
        else {
            return true;
        }
    };
}