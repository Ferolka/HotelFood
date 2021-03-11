// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var html_loading_element = '<div class="row justify-content-center align-self-center""><div class="col-12 text-secondary h4 align-self-center text-center"><div class="spinner-container"> <div>Loading</div><div class="spinner-border" role="status"> <span class="sr-only">Loading...</span> </div></div></div></div>';

$(function () {

    //setup_change_company();
    setup_changechield();
    setup_changecompany();
    var cook = cookiesEnabled();
    if (!cook) {
        showAvaibleCookies();
    }
});
function gethtmlloading() {
    return html_loading_element;
}
//check for cookies
function cookiesEnabled() {
    var cookiesEnabled = (navigator.cookieEnabled) ? true : false;

    if (typeof navigator.cookieEnabled == "undefined" && !cookiesEnabled) {
        document.cookie = "mytestcookie";
        cookiesEnabled = (document.cookie.indexOf("mytestcookie") != -1) ? true : false;
    }

    return cookiesEnabled;
}
function showAvaibleCookies() {
    var alert = "#cookies";
    //class: 'alert-danger', text: "@Localizer["DbError"]"
    $(alert).animate({
        height: '+=72px'
    }, 300);

    $(`<div class="alert alert-danger">` +
        '<button type="button" class="close" data-dismiss="alert">' +
        `x</button>Please enable cookies</div>`)
        .hide().appendTo(alert).fadeIn(1000);

    document.getElementById("msg").innerHTML = "Cookies disabled";
}
function setup_changechield() {
    $("#changechield,#changechield-1").click(function (e) {
        var url = '/Account/UserChilds';
        var self = this;
        $.get(url, function (data) {
            var dialog = $(data);
            $("body").append(dialog);// $('#moddialogyesno');
            // var dialog = $('#moddialogyesno').dialog();
            dialog.modal('show');
            dialog.on('hide.bs.modal', function (e) {
                dialog.empty();

            });
            dialog.find(".role-item").click(function (e) {
                e.preventDefault();
                if (!$(this).hasClass('active')) {
                    dialog.find(".role-item").removeClass('active');
                    $(this).addClass('active')
                }
            });
            dialog.find("#selectchild").click(function (e) {
                e.preventDefault();
                var userid = dialog.find(".role-item.active").attr("data-id");
                var token = dialog.find("[name='__RequestVerificationToken'").val();

                var currentUser = dialog.find(".role-item.active").attr("data-cur");
                if (currentUser == 'true')
                    return;
                $.ajax({
                    type: "POST",
                    data: { UserId: userid, __RequestVerificationToken: token },

                    url: "/Account/ChangeUserChild",
                    success: function (data) {
                        dialog.modal('hide');
                        var getUrl = window.location;
                        var baseUrl = getUrl.protocol + "//" + getUrl.host;
                        window.location.href = baseUrl;
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        dialog_error(errorThrown);
                    });

            });

        })
            .fail(function (xhr, status, error) {

            });
    });
}
//function setup_change_company() {
//    var ul_select_company = '<ul class="nav navbar-nav navbar-right show"> <a id="selectcompanyloaded" class="nav-link dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="true">.....</a><li class="nav-item dropdown"><div class="dropdown-menu show" id="companylist"></div></li></ul>';
//    $("#selectcompany").click(function (e) {
//        e.preventDefault();
//        var url = '/Account/UserOtherCompanies';
//        var self = this;
//        $.get(url, function (data) {
//            var ul = $(ul_select_company);
//            if ($.isArray(data) && data.length > 0) {
//                data.map(function (el) {
//                    ul.find("#companylist").append(`<a class="setcompany dropdown-item" data-id="${el.id}" href="#">${el.name}</a>`)
//                });
//                ul.find("#selectcompanyloaded").text($(self).text());
//                $(self).before(ul);
//                $(self).empty();
//                setupcompanychanges(ul);
//            }

//        });

//    });
//    function setupcompanychanges(menu) {
//        menu.find(".setcompany").click(function (e) {
//            e.preventDefault();
//            var url = '/Account/SetCompanyId?CompanyId=")' + $(this).attr("data-id");
//            $.get(url, function (data) {
//                //var x = data;
//            }).done(function () {
//                //alert("second success");
//                location.reload();
//            })
//                .fail(function () {
//                    //alert("error");
//                });
//        });

//    }
//}
var days = ['Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб', 'Вс'];
function getWeekDay(i) {
    return days[i];
}
function setup_changecompany() {
    $("#selectcompany,#selectcompany-1").click(function (e) {
        var id = $(this).attr("data-userid");
        var url = '/Account/CompaniesForUser?userId=' + id;
        var self = this;
        $.get(url, function (data) {
            var dialog = $(data);
            $("body").append(dialog);// $('#moddialogyesno');
            // var dialog = $('#moddialogyesno').dialog();
            dialog.modal('show');
            dialog.on('hide.bs.modal', function (e) {
                dialog.empty();

            });
            dialog.find(".company-item").click(function (e) {
                e.preventDefault();
                if (!$(this).hasClass('active')) {
                    dialog.find(".company-item").removeClass('active');
                    $(this).addClass('active')
                }
            });
            dialog.find("#selectcompany").click(function (e) {
                e.preventDefault();
                var companyid = dialog.find(".company-item.active").attr("data-id");
                var token = dialog.find("[name='__RequestVerificationToken'").val();

                var currentUser = dialog.find(".company-item.active").attr("data-cur");
                if (currentUser == 'true')
                    return;
                $.ajax({
                    type: "POST",
                    data: { CompanyId: companyid, __RequestVerificationToken: token },

                    url: "/Account/SetCompanyId",
                    success: function (data) {
                        dialog.modal('hide');
                        var getUrl = window.location;
                        var baseUrl = getUrl.protocol + "//" + getUrl.host;
                        window.location.href = baseUrl;
                    }
                })
                    .fail(function (jqXHR, textStatus, errorThrown) {
                        dialog_error(errorThrown);
                    });

            });

        })
            .fail(function (xhr, status, error) {

            });
    });
}

function setup_listitems(options) {
    let defaultoptions = {
        href: '#', onloadedcb: undefined, method: "ListItems",
        editmethod: "EditModal", createmethod: "CreateModal",
        onbeforesave: undefined
    };
    if (typeof (options) == 'object') {
        //new versoins
        //this.options = { ...defaultoptions, ...options };
        //edge troubles with es6

        this.options = defaultoptions
        this.options.href = options.href;
        if (options.onloadedcb)
            this.options.onloadedcb = options.onloadedcb;
        if (options.method)
            this.options.method = options.method;
        if (options.editmethod)
            this.options.editmethod = options.editmethod;
        if (options.createmethod)
            this.options.createmethod = options.createmethod;
        if (options.onbeforesave)
            this.options.onbeforesave = options.onbeforesave;

    }
    else {

        this.options = defaultoptions;
        this.options.href = options;
    }

    var self = this;
    var reload = function (href) {
        if (!href)
            href = self.options.href + `/${self.options.method}?`;
        else {
            // href += "&";
            href = href.replace("ListItems", self.options.method);

        }
        var searchcriteria = $("#table-content").attr("data-filter")
        if (typeof searchcriteria === 'undefined') {
            searchcriteria = "";
        }


        var sortfield = $("#table-content").attr("data-sortfield")

        if (typeof sortfield === 'undefined')
            sortfield = "";
        var sortorder = $("#table-content").attr("data-sortorder")
        if (typeof sortorder === 'undefined')
            sortorder = "";
        //var getstr = `&searchcriteria=${searchcriteria}&sortfield=${$("#table-content").attr("data-sortfield")}&sortorder=${$("#table-content").attr("data-sortorder")}`;

        var getstr = `&searchcriteria=${searchcriteria}&sortfield=${sortfield}&sortorder=${sortorder}`;

        href += getstr;
        if ($("#table-content").length) {
            $('#table-content').load(href);
        } else {
            $('.table-content').load(href);
        }
        // $('#table-content').load(href + 'searchcriteria=' + $('#search-val').val());

    }
    self.reload = reload;
    $.ajaxSetup({ cache: false });
    function setFirstSort() {
        const selval = $(".selection").val();
        if (selval) {
            var url = $(".selection").val().split('?').pop();
            const urlParams = new URLSearchParams(url);
            const field = urlParams.get('sortfield');
            const order = urlParams.get('sortorder');

            $('#table-content').attr('data-sortfield', field);
            $('#table-content').attr('data-sortorder', order);
        }
    }
    setFirstSort();
    reload(/*$(".selection").val()*/);
    $('#search-btn').click(function (e) {
        reload();
    });
    $('#custom-search-input').keydown((event) => {
        if (event.which == 13) {
            event.preventDefault();
            reload();
        }
    });
    //sort
    $(".custom-option").on("click", function () {
        // reload($(this).data('value'));
        // console.log($('#table-content').data('sortfield'));
        var url = $(this).data('value').split('?').pop();
        const urlParams = new URLSearchParams(url);
        const field = urlParams.get('sortfield');
        const order = urlParams.get('sortorder');

        $('#table-content').attr('data-sortfield', field);
        $('#table-content').attr('data-sortorder', order);
        reload();
    });
    $('#create-btn').click(function (e) {
        e.preventDefault();
        var url = self.options.href + '/' + self.options.createmethod;
        $.get(url, function (data) {
            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
            if (self.options.onloadedcb)
                self.options.onloadedcb();
        });
    });
    $(document).on("click", "a.ahead", function (e) {
        e.preventDefault();
        reload(this.href);
    });
    $(document).on("click", "a.apagebottom", function (e) {
        e.preventDefault();
        reload(this.href);
    });
    $('#search-val').keydown((event) => {
        if (event.which == 13) {
            event.preventDefault();
            $("#table-content").attr("data-filter", encodeURIComponent($('#search-val').val()));
            // console.log($("span.selection").attr("data-value"));
            reload();
        }
    });
    $("#search-form").submit(function (e) {
        return false;
    });
    function setupChangesChecker(dlg) {
        $(dlg).attr("_changed", false);
        $(dlg).find("input,textarea").change(function () {
            $(dlg).attr("_changed", true);
        });
        function hidehandler(e) {
            $(dlg).off('hide.bs.modal', hidehandler);
            if ($(dlg).attr("_changed") == "true") {
                //if (!confirm("There are unsaved changes, please confirm. your input will be lost"))
                e.preventDefault();
                dialog_yes_no($.text_resource.confirm_close,
                    function () { //yes
                        $(dlg).attr("_changed", false);
                        $('#modDialog').modal('hide');
                        $('#modDialog').find('#dialogContent').empty();

                    },
                    function () { //no
                        $(dlg).on('hide.bs.modal', hidehandler);
                    });
                return;
                if (!confirm(get_text_res().confirm_close))
                    e.preventDefault();
                $(dlg).attr("_changed", false);
            }
        }

        $(dlg).on('hide.bs.modal', hidehandler);

    }
    $(document).on("click", "a.cmd-edit", function (e) {
        e.preventDefault();
        $.get(this.href, function (data) {

            $('#dialogContent').html(data);
            $('#modDialog').attr("data-backdrop", false);
            $('#modDialog').css("background-color", "rgba(117, 117, 117, 0.5)");
            $('#modDialog').modal('show');
            if (self.options.onloadedcb)
                self.options.onloadedcb();
            setupChangesChecker($('#modDialog'));
        });
    });
    $(document).on("click", "a.cmd-delete", function (e) {
        e.preventDefault();
        fetch(this.href, { method: 'GET' })
            .then(response => response.text())
            .then(data => {
                $('#dialogContent').html(data);
                $('#modDialog').modal('show');
                const form = $('#modDialog').find('form');
                form.submit(function (e) { e.preventDefault(); });

                var promise = new Promise((resolve, reject) => {
                    $('#modDialog').find('input[type="submit"]').click(function (e) {
                        e.preventDefault();
                        resolve(form);

                    });
                    $('#modDialog').find('button[type="button"]').click(function (e) {

                        reject();

                    });
                });
                return promise;
            })
            .then((form) => {
                var formdata = new FormData(form.get(0));
                return fetch(form.attr('action'), {
                    method: 'POST',
                    headers: {

                        'RequestVerificationToken': form.find("[name='__RequestVerificationToken'").val()
                    },
                    body: formdata
                })
            })
            .then(response => {
                if (response.ok != true) {
                    if (response.status == 424) {
                        throw $.text_resource.deleteeditusingerror
                    }
                    else {
                        throw response.statusText;
                    }

                }

            })
            .catch(error => {
                if (error)
                    dialog_error(error);

            })
            .finally(function () {
                $('#modDialog').modal('hide');
                $('#dialogContent').empty();
                reload();
            });

        /*
        $.get(this.href, function (data) {

            $('#dialogContent').html(data);
            $('#modDialog').modal('show');
            realdelete($('#modDialog'));
            const form = dlg.find('form');


        }).fail(function (xhr, status, error) {
            dialog_error("ERROR !" + error);
            //alert("ERROR !" + error);
        });
        */
    });

    $(document).on('click', '[data-action="modal"]', function (event) {
        event.preventDefault();
        if (self.options && self.options.onbeforesave) {
            self.options.onbeforesave();
        }
        var form = $(this).parents('.modal-body').find('form');
        if (form.length == 0)
            form = $('#modDialog').find('.modal-body').find('form');
        var actionUrl = $(event.target).attr('action');
        var dataToSend = form.serialize();

        $.post(actionUrl, dataToSend).done(function (data) {
            var isValid = false;
        });
    });
    //for appropriate scrolling
    $(document).on('hidden.bs.modal', '.modal', function () {
        $('.modal:visible').length && $(document.body).addClass('modal-open');
    });
    $(document).on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();
        // console.log("Data Save");
        if (self.options && self.options.onbeforesave) {
            self.options.onbeforesave();
        }
        var form = $(this).parents('.modal-body').find('form');
        if (form.length == 0)
            form = $('#modDialog').find('.modal-body').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();

        $.post(actionUrl, dataToSend).done(function (data) {
            var isValid = false;

            if (data && data.res && data.res == "OK")
                isValid = true;

            if (isValid) {
                $('#modDialog').attr("_changed", false);
                $('#modDialog').modal('hide');
                $('#dialogContent').empty();
                reload(/*$(".selection").val()*/);
            }
            else {
                var newBody = $('.modal-body', data);
                $(document).find('.modal-body').replaceWith(newBody);
                setupChangesChecker($('#modDialog'));
            }
        })
            .fail(function (xhr, status, error) {
                dialog_error("ERROR !" + error);
                //alert("ERROR !" + error);
            });
    });
    return self;
}


function setup_search(_options) {
    let defaultoptions = { href: '#', onloadedcb: undefined, method: "ListItems", itemselector: "a", classname: "", multiselect: false, onitemselectedcb: undefined };
    this.options = {};
    if (typeof (_options) == 'object') {
        // this.options = { ...defaultoptions, ...options };
        //edge troubles with es6
        this.options = defaultoptions
        this.options.href = _options.href;
        if (_options.onloadedcb)
            this.options.onloadedcb = _options.onloadedcb;
        if (_options.method)
            this.options.method = _options.method;
        if (_options.multiselect)
            this.options.multiselect = _options.multiselect;
        if (_options.onitemselectedcb)
            this.options.onitemselectedcb = _options.onitemselectedcb;
        this.options.itemselector = _options.itemselector;
        this.options.classname = _options.classname;
        this.options.dialog_id = `#modDialog${this.options.classname}line`
    }
    else {

        this.options = defaultoptions;
        this.options.href = _options;
    }
    var self = this;
    function delay(callback, ms) {
        var timer = 0;
        return function () {
            var context = this, args = arguments;
            clearTimeout(timer);
            timer = setTimeout(function () {
                callback.apply(context, args);
            }, ms || 0);
        };
    };
    $(document).on('click', this.options.itemselector, function (event) {
        event.preventDefault();
        var source_item = this;
        var dlg = $(self.options.dialog_id);
        var url = self.options.href;
        dlg.find(".modal-body").html(html_loading_element);
        //$('#modDialog').modal('hide');
        dlg.modal('show');
        $.get(url, function (data) {
            dlg.find(".modal-body").html(data);

            dlg.modal('handleUpdate');
            setupSearchHandlers(dlg, source_item, function () {
                //  $('#modDialog').modal('show');

            });
        });
        $('#search-' + self.options.classname).keyup(delay(function (e) {
            dlg.find(".modal-body").html(html_loading_element);
            var url_search = url + `?SearchCriteria=${this.value}`;
            $.get(url_search, function (data) {
                dlg.find(".modal-body").html(data);

                dlg.modal('handleUpdate');
                setupSearchHandlers(dlg, source_item, function () {
                });
            });
        }));
    });
    function setupSearchHandlers(dlg, src, closecb) {
        // $(dlg).find('.add-item').click(function (e) {
        $(dlg).find('.table.search-items tr').click(function (e) {
            if (self.options.onitemselectedcb)
                self.options.onitemselectedcb(src, this, { id: $(this).attr("data-id"), name: $(this).attr("data-name"), measure: $(this).attr("data-measure") });
            if (!self.options.multiselect) {
                $(dlg).modal('hide');
                dlg.find(".modal-body").empty();
                if (closecb) {
                    closecb();
                }
            }
        });

    }
}
function promise_dialog_yes_no(message) {

    var promisedlg = new Promise((resolve, reject) => {
        dialog_yes_no(message, resolve, reject)
    });



    return promisedlg;
}
function dialog_yes_no(message, yesCallback, noCallback) {

    var dlg_html = '   <div id="moddialogyesno" class="modal" tabindex="-1" role="dialog">  ' +
        '       <div class="modal-dialog modal-sm modal-alert" role="document" style="max-width: 560px">  ' +
        '           <div class="modal-content">  ' +
        '               <div class="modal-header">  ' +
        '                   <h5 class="modal-title"><i class="fa fa-exclamation-circle fa-3" style="color:red" aria-hidden="true"></i>  ' + $.text_resource.confirm_title + '</h5>  ' +
        '                   <button type="button" class="close" data-dismiss="modal" aria-label="Close">  ' +
        '                       <span aria-hidden="true">X</span>  ' +
        '                   </button>  ' +
        '               </div>  ' +
        '               <div class="modal-body">  ' +
        '               <p>' + message + '</p>' +
        '               </div>  ' +
        //'               <div class="modal-footer">  ' +
        //'     ' +
        //'                   <button id="btnyes" type="button" class="btn btn-primary" >' + $.text_resource.yes + '</button>  ' +
        //'                   <button id="btnno" type="button" class="btn btn-secondary" data-dismiss="modal">' + $.text_resource.no + '</button>  ' +
        //'               </div>  ' +
        '<div class="modal-footer justify-content-center border-0" style="font-size: 14px; font-weight: bold; line-height: 16px; padding: 20px 0 30px">' +
        '<button type="button" id="btnyes" class="btn btn-primary m-2 col-sm-4 col-xl-3 col-12" style="background: #36C233; border: 1px solid #36C233; height: 48px; text-transform: uppercase">' +
        'так' +
        '</button>' +
        '<button type="button" id="btnno" class="btn btn-primary m-2 col-sm-4 col-xl-3 col-12" style="background: #ffffff; border: 1px solid #f15e25; height: 48px; text-transform: uppercase; color: #f15e25">' +
        'Ні' +
        '</button>' +

        '</div>' +
        '           </div>  ' +
        '       </div>  ' +
        '  </div>  ';
    // $("body").append(dlg_html);
    //console.log("dialog_yes_no");
    var dialog = $(dlg_html);
    $("body").append(dialog);// $('#moddialogyesno');
    // var dialog = $('#moddialogyesno').dialog();
    dialog.modal('show');
    dialog.find('#btnyes').click(function () {
        dialog.modal('hide');
        dialog.remove();
        //dialog.empty();
        if (yesCallback)
            yesCallback();
    });

    dialog.find('#btnno').click(function () {
        dialog.modal('hide');
        dialog.remove();
        //dialog.empty();
        if (noCallback)
            noCallback();
    });
    dialog.on('hide.bs.modal', function (e) {
        dialog.remove();

    });

}

function dialog_error(message) {

    var dlg_html = '   <div id="moddialogerr" class="modal" tabindex="-1" role="dialog">  ' +
        '       <div class="modal-dialog modal-lg modal-alert" role="document">  ' +
        '           <div class="modal-content">  ' +
        '               <div class="modal-header">  ' +
        '                   <h5 class="modal-title"><i class="fa fa-exclamation-circle fa-3" style="color:red" aria-hidden="true"></i>  ' + $.text_resource.error + '</h5>  ' +
        '                   <button type="button" class="close" data-dismiss="modal" aria-label="Close">  ' +
        '                       <span aria-hidden="true">X</span>  ' +
        '                   </button>  ' +
        '               </div>  ' +
        '               <div class="modal-body">  ' +
        '               <p>' + message + '</p>' +
        '               </div>  ' +
        '               <div class="modal-footer">  ' +
        '     ' +
        '                   <button id="btnyes" type="button" class="btn btn-primary" >' + $.text_resource.yes + '</button>  ' +
        '               </div>  ' +
        '           </div>  ' +
        '       </div>  ' +
        '  </div>  ';
    // $("body").append(dlg_html);
    var dialog = $(dlg_html);
    $("body").append(dialog);// $('#moddialogyesno');
    // var dialog = $('#moddialogyesno').dialog();
    dialog.modal('show');
    dialog.find('#btnyes').click(function () {
        //dialog.modal('hide');
        dialog.modal('hide');
    });


    dialog.on('hide.bs.modal', function (e) {
        dialog.empty();

    });
}

function dialog_info(message) {

    var dlg_html = '   <div id="moddialoginfo" class="modal" tabindex="-1" role="dialog">  ' +
        '       <div class="modal-dialog modal-lg modal-alert" role="document">  ' +
        '           <div class="modal-content">  ' +
        '               <div class="modal-header">  ' +
        '                   <h5 class="modal-title"><i class="fa fa-info-circle fa-3" style="color:green" aria-hidden="true"></i>  ' + $.text_resource.info + '</h5>  ' +
        '                   <button type="button" class="close" data-dismiss="modal" aria-label="Close">  ' +
        '                       <span aria-hidden="true">X</span>  ' +
        '                   </button>  ' +
        '               </div>  ' +
        '               <div class="modal-body">  ' +
        '               <p>' + message + '</p>' +
        '               </div>  ' +
        '               <div class="modal-footer">  ' +
        '     ' +
        '                   <button id="btnyes" type="button" class="btn btn-primary" >' + $.text_resource.yes + '</button>  ' +
        '               </div>  ' +
        '           </div>  ' +
        '       </div>  ' +
        '  </div>  ';

    var dialog = $(dlg_html);
    $("body").append(dialog);

    dialog.modal('show');
    dialog.find('#btnyes').click(function () {

        dialog.modal('hide');
    });


    dialog.on('hide.bs.modal', function (e) {
        dialog.empty();

    });




}



function dialog_announcement(message) {

    var dlg_html = '   <div id="moddialoginfo" class="modal" tabindex="-1" role="dialog">  ' +
        '       <div class="modal-dialog modal-lg modal-alert" style="max-width: 560px" role="document">  ' +
        '           <div class="modal-content">  ' +
        '               <div class="modal-header">  ' +
        '                   <h5 class="modal-title"><i class="fa fa-info-circle fa-3" style="font-size: 34px; line-height: 40px; font-weight: bold" aria-hidden="true"> ' + 'Повідомлення' + '</h5>  ' +
        '                   <button type="button" class="close" data-dismiss="modal" aria-label="Close">  ' +
        '                       <img src="../img/close-modal.svg" aria-hidden="true" alt="X">  ' +
        '                   </button>  ' +
        '               </div>  ' +
        '               <div class="modal-body">  ' +
        '               <p>' + message + '</p>' +
        '               </div>  ' +
        '               <div class="modal-footer justify-content-center border-0" style="font-size: 14px; font-weight: bold; line-height: 16px; padding: 20px 0 30px">  ' +
        '     ' +
        '                   <button id="btnyes" type="button" class="btn btn-primary m-2 col-sm-4 col-xl-4 col-12" style="background: #36C233; border: 1px solid #36C233; height: 48px; text-transform: uppercase" >' + $.text_resource.yes + '</button>  ' +
        '               </div>  ' +
        '           </div>  ' +
        '       </div>  ' +
        '  </div>  ';

    var dialog = $(dlg_html);
    $("body").append(dialog);

    dialog.modal('show');
    dialog.find('#btnyes').click(function () {

        dialog.modal('hide');
    });


    dialog.on('hide.bs.modal', function (e) {
        dialog.empty();

    });




}