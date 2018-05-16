var getUrl = window.location;
var baseUrl = getUrl.protocol + "//" + getUrl.host + "/" + getUrl.pathname;
if (getUrl.host.indexOf("localhost") !== -1)
    baseUrl = getUrl.protocol + "//" + getUrl.host;
var urlAPI = baseUrl + "/api/StudentsService/";
var pageNumber = 1;
var pageSize = 10;
var totalPages = 0;
var total = 0;
var studentsDataSet;
var progress;
var currentSort = "updated_on";
var direction = "DESC";
var selectedId = -1;

function goToStart() {
    pageNumber = 1;
    loadStudents();
}

function goToPrevious() {
    pageNumber--;
    if (pageNumber <= 0)
        pageNumber = totalPages;
    loadStudents();
}

function goToNext() {
    pageNumber++;
    if (pageNumber > totalPages)
        pageNumber = 1;
    loadStudents();
}

function goToEnd() {
    pageNumber = totalPages;
    loadStudents();
}

function showStudentsGrid() {
    $("#CurrentPage").html(pageNumber + " of " + totalPages);

    var template = $('#tmpListOfStudents').html();
    Mustache.parse(template);
    var rendered = Mustache.render(template, studentsDataSet);

    $("#progressBar").hide();
    clearInterval(progress);

    $('#studentsGrid').show();
    $('.pager').show();

    $('#studentsGrid > tbody').html(rendered);
}

function alertUser(type, msg) {
    var message = [];
    if (type == 'e') {
        message.Title = 'Error!';
        message.Type = 'danger';
        message.Msg = msg;
    }

    if (type == 's') {
        message.Title = 'Success!';
        message.Type = 'success';
        message.Msg = msg;
    }

    if (type == 'w') {
        message.Title = 'Warning!';
        message.Type = 'warning';
        message.Msg = msg;
    }

    var template = $('#tmpMsg').html();
    Mustache.parse(template);
    var rendered = Mustache.render(template, message);
    $('#alertPanel').html(rendered);
}

function sortDataBy(obj, sortField) {
    console.log($(obj));
    console.log($(obj).find("span:first-child"));
    console.log($(obj).find("span:first-child").hasClass("glyphicon-sort"));
    if ($(obj).find("span:first-child").hasClass("glyphicon-sort")) {
        $(obj).find("span:first-child").removeClass("glyphicon-sort").addClass("glyphicon-sort-by-attributes");
        currentSort = sortField;
        direction = "ASC";
    }
    else {
        if ($(obj).find("span:first-child").hasClass("glyphicon-sort-by-attributes")) {
            $(obj).find("span:first-child").removeClass("glyphicon-sort-by-attributes").addClass("glyphicon-sort-by-attributes-alt");
            currentSort = sortField;
            direction = "DESC";
        }
        else {
            if ($(obj).find("span:first-child").hasClass("glyphicon-sort-by-attributes-alt")) {
                $(obj).find("span:first-child").removeClass("glyphicon-sort-by-attributes-alt").addClass("glyphicon-sort");
                currentSort = "updated_on";
                direction = "DESC";
            }
        }
    }

    $(obj).siblings().find("span").removeClass().addClass("glyphicon glyphicon-sort");
    console.log(currentSort);

    loadStudents();
}

function resetSearchForm() {
    $('#alertPanel').empty();
    $("#progressBar").hide();
    var now = moment(new Date());
    $('#StartDate').data("DateTimePicker").date(now.subtract(1, 'year').hours(0).minutes(0).seconds(0).milliseconds(0));
    $('#EndDate').data("DateTimePicker").date(now.add(1, 'year').hours(0).minutes(0).seconds(0).milliseconds(0));
    $("#Type").val("").trigger('change');
    $("#Gender").val("").trigger('change');
    pageNumber = 1;
    $("#IsNotEnabled").prop('checked', false);
    $("#Name").val("");
    currentSort = "updated_on";
    direction = "DESC";
    $("#studentsGrid").find("span").removeClass();
    $("#studentsGrid").find("span").addClass("glyphicon glyphicon-sort");
    $("#defaultSort").removeClass("glyphicon-sort").addClass("glyphicon-sort-by-attributes");
}

$("#btnReset").click(function () {
    resetSearchForm();
    $('#btnSearch').prop("disabled", true);
    $('#btnReset').prop("disabled", true);
    loadStudents();
});

$("#btnSearch").click(function () {
    $('#alertPanel').empty();
    $('#btnSearch').prop("disabled", true);
    $('#btnReset').prop("disabled", true);
    pageNumber = 1;
    loadStudents();
});

function loading() {
    $("#progressBar").show();
    var progressBar = $('.progress-bar');
    var percentVal = 0;

    progress = window.setInterval(function () {
        progressBar.css("width", percentVal + '%').attr("aria-valuenow", percentVal + '%');

        if (percentVal == 100) {
            percentVal = 0;
        }
        percentVal += 10;
    }, 500);
}

function loadStudents() {
    $('#studentsGrid > tbody').html("");
    $('#studentsGrid').hide();
    $('.pager').hide();
    loading();

    var startDate = moment($("#StartDate").datetimepicker('date')).format('YYYY-MM-DDTHH:mm:ss');
    var endDate = moment($("#EndDate").datetimepicker('date')).format('YYYY-MM-DDTHH:mm:ss');
    var name = $("#Name").val();
    var isEnabled = !$("#IsNotEnabled").is(":checked");
    var type = $("#Type").val();
    var gender = $("#Gender").val();
    var filter = "?startDate=" + startDate + "&endDate=" + endDate + "&name=" + name + "&isEnabled=" + isEnabled + "&gender=" + gender + "&type=" + type + "&pageNumber=" + pageNumber + "&pageSize=" + pageSize + "&sortBy=" + currentSort + "&sortDirection=" + direction;

    $.ajax({
        url: urlAPI + "GetPaginatedList" + filter, success: function (result) {
            total = result.TotalStudents;
            if (total > 0) {
                totalPages = Math.floor(total / pageSize);
                if (total % pageSize > 0) totalPages++;
                studentsDataSet = result.Students;
                showStudentsGrid();
                if (!isEnabled) {
                    $('.btn-hide').hide();
                }
            } else {
                $("#progressBar").hide();
                clearInterval(progress);

                alertUser('w', 'No data was found to show clear the filters or try again later.');
            }

            $('#btnSearch').prop("disabled", false);
            $('#btnReset').prop("disabled", false);
            $('#btnSave').prop("disabled", false);
            $('#btnCancel').prop("disabled", false);
        }
    });
}

$(document).ready(function () {
    $("#progressBar").hide();
    $('#studentsGrid').hide();
    $('.pager').hide();
    $("#Type").select2();
    $("#Gender").select2();

    var now = moment(new Date());

    $('#StartDate').datetimepicker({
        format: 'MM/DD/YYYY',
    });

    $('#EndDate').datetimepicker({
        format: 'MM/DD/YYYY',
    });

    $('#StartDate').data("DateTimePicker").date(now.subtract(1, 'year').hours(0).minutes(0).seconds(0).milliseconds(0));
    $('#EndDate').data("DateTimePicker").date(now.add(1, 'year').hours(0).minutes(0).seconds(0).milliseconds(0));
    $("#StartDate").on("dp.change", function (e) {
        $('#EndDate').data("DateTimePicker").minDate(e.date);
    });
    $("#EndDate").on("dp.change", function (e) {
        $('#StartDate').data("DateTimePicker").maxDate(e.date);
    });

    loading();

    resetSearchForm();
    $("#progressBar").hide();
    clearInterval(progress);
    loadStudents();
});

function resetForm() {
    $('#collapseFilters').collapse('show');
    $('#studentForm').collapse('hide');

    $(window).scrollTop(0);
    $("#progressBar").hide();
    $("#selType").val("").trigger('change');
    $("#selGender").val("").trigger('change');
    $("#chkIsNotEnabled").prop('checked', false);
    $('#divChkEnabled').addClass('hidden');
    $("#txtName").val("");
    if (selectedId != -1) {
        $("#lblTitleForm").text('Add New');
        $("#iconTitleForm").removeClass('glyphicon-pencil').addClass('glyphicon-plus');
    }
    selectedId = -1;
}

$("#btnCancel").click(function () {
    resetForm();
});

$("#btnSave").click(function () {
    $('#alertPanel').empty();
    $('#btnSave').prop("disabled", true);
    $('#btnCancel').prop("disabled", true);

    var name = $("#txtName").val();
    var isEnabled = !$("#chkIsNotEnabled").is(":checked");
    var type = $("#selType").val();
    var gender = $("#selGender").val();
    var student = { 'Id': selectedId, 'Name': name, 'Enabled': isEnabled, 'Type': type, 'Gender': gender };

    var functionName = "AddNew";
    if (selectedId != -1) {
        functionName = "Update";
    }

    $.ajax({
        url: urlAPI + functionName,
        data: student,
        method: "POST",
        success: function (response) {
            if (!response.HasError) {
                alertUser("s", "The Student was successfully saved.");
                resetForm();
                pageNumber = 1;
                loadStudents();
            } else {
                alertUser("e", "The Student was unable to be saved, please try again later.");
                $('#btnSave').prop("disabled", false);
                $('#btnCancel').prop("disabled", false);
            }
        }
    });
});

function editStudent(id, name, type, gender, isEnabled) {
    $('#collapseFilters').collapse('hide');
    $('#studentForm').collapse('show');

    $('#divChkEnabled').removeClass('hidden');
    $("#selType").val(type).trigger('change');
    $("#selGender").val(gender).trigger('change');
    selectedId = id;
    $("#chkIsNotEnabled").prop('checked', !isEnabled);
    $("#txtName").val(name);
    $("#lblTitleForm").text('Edit');
    $("#iconTitleForm").removeClass('glyphicon-plus').addClass('glyphicon-pencil');
}

function delStudent(id) {
    if (confirm("Are you sure to delete this student? All the data will be erased, you can hide it instead.")) {
        $.ajax({
            url: urlAPI + "Delete/" + id,
            success: function (msg) {
                if (!msg.HasError) {
                    alertUser("s", "The student was successfully deleted.");
                    pageNumber = 1;
                    loadStudents();
                } else {
                    alertUser("e", "We were unable to delete the student.");
                }
            }
        });
    }
}

function hideStudent(id) {
    $.ajax({
        url: urlAPI + "Hide/" + id,
        success: function (msg) {
            if (!msg.HasError) {
                alertUser("s", "The student was successfully hidden.");
                pageNumber = 1;
                loadStudents();
            } else {
                alertUser("e", "We were unable to delete the student.");
            }
        }
    });
}