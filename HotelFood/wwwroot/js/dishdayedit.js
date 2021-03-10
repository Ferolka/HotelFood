function showeditdayalert(options) {

    $("#response").animate({
        height: '+=72px'
    }, 300);

    $(`<div class="alert ${options.class}">` +
        '<button type="button" class="close" data-dismiss="alert">' +
        `x</button>${options.text}</div>`)
        .hide().appendTo('#response').fadeIn(1000);

    $(".alert").delay(3000).fadeOut(
        "normal",
        function () {
            $(this).remove();
        });

    $("#response").delay(4000).animate({
        height: '-=72px'
    }, 300);


}

$(document).on('change', "input[cp-name='dayselect']", function () { 
   
        if (this.checked) {
             //alert("click");
        }

         var dishid = parseInt($(this).parent().find("#item_DishId").val());
         var dishdate = $(this).parent().find("#item_Date").val();
        //alert(JSON.stringify({ DishId: dishid, daydate: 0, enabled: this.checked }));
    var checked = this.checked;
    
        $.ajax({
            type: "POST",
            data: { DishId: dishid, daydate: dishdate, enabled: checked },

            url: "/DayDishes/SaveDayDish",
            success: function (data) {
                showeditdayalert({ class: 'alert-success', text: checked ? $.text_resource.added : $.text_resource.removed});
            }
        })
            .fail(function (xhr, status, error) {
                showeditdayalert({ class: 'alert-danger', text: error });
        });
});

$(document).on('change', "input[cp-name='dayselect-complex']", function () {

    if (this.checked) {
       
    }
    var button = $(this);
    //var dishid = parseInt($(this).parent().find("#item_DishId").val());
    // var dishdate = $(this).parent().find("#item_Date").val();
    var complexid = parseInt($(this).parent().find("#item_ComplexId").val());
    var complexdate = $(this).parent().find("#item_Date").val();
    var checked = this.checked;
    var vvv = $.text_resource;
    $.ajax({
        type: "POST",
        data: { ComplexId: complexid, daydate: complexdate, enabled: checked },

        url: "/DayDishes/SaveDayComplex",
        success: function (data) {
            if (data.res == "FAIL" && data.reason == "Deleting in db") {
                showeditdayalert({ class: 'alert-danger', text: $.text_resource.remove_forbidden });
                // console.log($(this));
                $(button).prop('checked', true);
                return;
            }
            showeditdayalert({ class: 'alert-success', text: checked ? $.text_resource.added : $.text_resource.removed });
        }
    })
    .fail(function (xhr, status, error) {
        showeditdayalert({ class: 'alert-danger', text: error });
    });
});