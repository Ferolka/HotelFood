



var glob_text_resources;

var text_resource =  {};

text_resource["ua"] = {
    confirm_delete:"",
    confirm_close: "Ви впевненi що хочете закрити вiкно ? Вашi змiни будуть втраченi",
    added: "Додано",
    removed: "Видалено",
    confirm_title: "Пiдтвердiть",
    yes: "Tak",
    no: "Нi",
    error: "Помилка",
    select: "Вибрати",
    info: "Інфо",
    remove_forbidden: "Не можна видалити замовлений комплекс!",
    deleteeditusingerror: "Об'єкт не може бути видалений або змінений, тому що використовується в системі"
}

glob_text_resources = text_resource["ua"];
function setLocale(locale) {
    glob_text_resources = text_resource[locale];
}
function get_text_res() { return glob_text_resources; }

jQuery(function ($) {
    $.text_resource = glob_text_resources;
    $.set_text_locale = setLocale;
    $.get_text_res = get_text_res;
})