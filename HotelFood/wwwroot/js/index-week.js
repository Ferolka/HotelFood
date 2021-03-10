let weeks = [
    {dayWeek: 'Пн', day: 17, month: 8, year: 2020, id: '170820', link: 'day'}, {
        dayWeek: 'Вт',
        day: 18,
        month: 8,
        year: 2020,
        id: '180820',
        link: 'day'
    }, {dayWeek: 'Ср', day: 19, month: 8, year: 2020, id: '190820', link: 'day'}, {
        dayWeek: 'Чт',
        day: 20,
        month: 8,
        year: 2020,
        id: '200820',
        link: 'day'
    }, {dayWeek: 'Пт', day: 21, month: 8, year: 2020, id: '210820', link: 'day'}, {
        dayWeek: 'Сб',
        day: 22,
        month: 8,
        year: 2020,
        id: '220820',
        link: 'day'
    }, {dayWeek: 'Вс', day: 23, month: 8, year: 2020, id: '230820', link: 'day'}
    , {dayWeek: 'Пн', day: 24, month: 8, year: 2020, id: '240820', link: 'day'}, {
        dayWeek: 'Вт',
        day: 25,
        month: 8,
        year: 2020,
        id: '250820',
        link: 'day'
    }, {dayWeek: 'Ср', day: 26, month: 8, year: 2020, id: '260820', link: 'day'}, {
        dayWeek: 'Чт',
        day: 27,
        month: 8,
        year: 2020,
        id: '270820',
        link: 'day'
    }, {dayWeek: 'Пт', day: 28, month: 8, year: 2020, id: '280820', link: 'day'}, {
        dayWeek: 'Сб',
        day: 29,
        month: 8,
        year: 2020,
        id: '290820',
        link: 'day'
    }, {dayWeek: 'Вс', day: 30, month: 8, year: 2020, id: '300820', link: 'day'},
    {dayWeek: 'Пн', day: 31, month: 8, year: 2020, id: '310820', link: 'day'}, {
        dayWeek: 'Вт',
        day: 1,
        month: 9,
        year: 2020,
        id: '010920',
        link: 'day'
    }, {dayWeek: 'Ср', day: 2, month: 9, year: 2020, id: '020920', link: 'day'}, {
        dayWeek: 'Чт',
        day: 3,
        month: 9,
        year: 2020,
        id: '030920',
        link: 'day'
    }, {dayWeek: 'Пт', day: 4, month: 9, year: 2020, id: '040920', link: 'day'}, {
        dayWeek: 'Сб',
        day: 5,
        month: 9,
        year: 2020,
        id: '050920',
        link: 'day'
    }, {dayWeek: 'Вс', day: 6, month: 9, year: 2020, id: '060920', link: 'day'}];

let elForTabs = '\n' +
    '                <div class="col-12 box-shadow-container position-relative brd-bot-line mb-3">\n' +
    '                    <div class="col-12 p-0 d-flex flex-column br">\n' +
    '                        <span class="h3 pt-lg-0 pt-md-0 pt-3 brd-bot-line-reverse pb-4 pb-md-0 pb-lg-0"\n' +
    '                              style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>\n' +
    '                        <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"\n' +
    '                              style="font-size: 14px; line-height: 16px; color: #797979;">Перше</span>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                        <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"\n' +
    '                                                                       style="width: inherit; max-width: 61px;">\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"\n' +
    '                                 style="font-size: 16px;">Борщ з\n' +
    '                                пампушками\n' +
    '                            </div>\n' +
    '                            <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">\n' +
    '                                <div class="pl-4">Смачний український\n' +
    '                                    борщ з пампушками та запашним часником.\n' +
    '                                </div>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">\n' +
    '                                <span>150 гр.</span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none"> / </span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴\n' +
    '                            </div>\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-column">\n' +
    '    <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"\n' +
    '          style="font-size: 14px; line-height: 16px; color: #797979;">Друге</span>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                        <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"\n' +
    '                                                                       style="width: inherit; max-width: 61px;">\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"\n' +
    '                                 style="font-size: 16px;">Запіканка\n' +
    '                            </div>\n' +
    '                            <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">\n' +
    '                                <div class="pl-4">Смачний український\n' +
    '                                    борщ з пампушками та запашним часником.\n' +
    '                                </div>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">\n' +
    '                                <span>150 гр.</span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none"> / </span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴\n' +
    '                            </div>\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                        <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"\n' +
    '                                                                       style="width: inherit; max-width: 61px;">\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"\n' +
    '                                 style="font-size: 16px;">Запечені яблука з сиром\n' +
    '                            </div>\n' +
    '                            <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">\n' +
    '                                <div class="pl-4">Смачний український\n' +
    '                                    борщ з пампушками та запашним часником.\n' +
    '                                </div>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">\n' +
    '                                <span>150 гр.</span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none"> / </span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴\n' +
    '                            </div>\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '\n' +
    '\n' +
    '                </div>\n' +
    '                <div class="col-12 box-shadow-container position-relative brd-bot-line mb-3">\n' +
    '                    <div class="col-12 p-0 d-flex flex-column br">\n' +
    '                        <span class="h3 pt-lg-0 pt-md-0 pt-3 brd-bot-line-reverse pb-4 pb-md-0 pb-lg-0"\n' +
    '                              style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>\n' +
    '                        <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"\n' +
    '                              style="font-size: 14px; line-height: 16px; color: #797979;">Перше</span>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                        <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"\n' +
    '                                                                       style="width: inherit; max-width: 61px;">\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"\n' +
    '                                 style="font-size: 16px;">Борщ з\n' +
    '                                пампушками\n' +
    '                            </div>\n' +
    '                            <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">\n' +
    '                                <div class="pl-4">Смачний український\n' +
    '                                    борщ з пампушками та запашним часником.\n' +
    '                                </div>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">\n' +
    '                                <span>150 гр.</span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none"> / </span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴\n' +
    '                            </div>\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-column">\n' +
    '    <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"\n' +
    '          style="font-size: 14px; line-height: 16px; color: #797979;">Друге</span>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                        <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"\n' +
    '                                                                       style="width: inherit; max-width: 61px;">\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"\n' +
    '                                 style="font-size: 16px;">Запіканка\n' +
    '                            </div>\n' +
    '                            <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">\n' +
    '                                <div class="pl-4">Смачний український\n' +
    '                                    борщ з пампушками та запашним часником.\n' +
    '                                </div>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">\n' +
    '                                <span>150 гр.</span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none"> / </span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴\n' +
    '                            </div>\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                    <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                        <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"\n' +
    '                                                                       style="width: inherit; max-width: 61px;">\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"\n' +
    '                                 style="font-size: 16px;">Запечені яблука з сиром\n' +
    '                            </div>\n' +
    '                            <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">\n' +
    '                                <div class="pl-4">Смачний український\n' +
    '                                    борщ з пампушками та запашним часником.\n' +
    '                                </div>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">\n' +
    '                                <span>150 гр.</span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none"> / </span>\n' +
    '                                <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>\n' +
    '                            </div>\n' +
    '                            <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"\n' +
    '                                 style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴\n' +
    '                            </div>\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '\n' +
    '\n' +
    '                </div>';


let weekDayLink = [...document.querySelectorAll('.day-of-week')];
let currentDayWeekYear = new Date();
let currentDay = currentDayWeekYear.getDate();
let currentMonth = currentDayWeekYear.getMonth() + 1;
let currentYear = currentDayWeekYear.getFullYear();

weekDayLink.forEach((day, index) => {
    let dayOfWeek = [...day.getElementsByClassName('day-of')];
    let numberOfDay = [...day.getElementsByClassName('number-of')];
    let rusDay = [...day.getElementsByClassName('month-rus')];
    let tabContentEat = [...document.querySelectorAll('#week-day-content')];
    let monthLabel = '0';

    if (currentMonth > weeks[index].month) {
        day.classList.add('day-of-week__past');
        if (String(weeks[index].month).length === 1) {
            monthLabel += weeks[index].month;
        } else {
            monthLabel = weeks[index].month;
        }
    } else if (weeks[index].month > currentMonth) {
        if (String(weeks[index].month).length === 1) {
            monthLabel += weeks[index].month;
        } else {
            monthLabel = weeks[index].month;
        }
    } else {
        if (currentDay > weeks[index].day && currentMonth > weeks[index].month) {
            day.classList.add('day-of-week__past');
            if (String(weeks[index].month).length === 1) {
                monthLabel += weeks[index].month;
            } else {
                monthLabel = weeks[index].month;
            }
        } else if (currentDay > weeks[index].day) {
            day.classList.add('day-of-week__past');
            if (String(weeks[index].month).length === 1) {
                monthLabel += weeks[index].month;
            } else {
                monthLabel = weeks[index].month;
            }
        } else if (currentDay === weeks[index].day) {
            day.classList.add('active');
            if (String(weeks[index].month).length === 1) {
                monthLabel += weeks[index].month;
            } else {
                monthLabel = weeks[index].month;
            }
        } else {
            if (String(weeks[index].month).length === 1) {
                monthLabel += weeks[index].month;
            } else {
                monthLabel = weeks[index].month;
            }
        }

    }
    dayOfWeek[0].innerHTML = weeks[index].dayWeek;
    let dateForDiv = weeks[index].day + '.' + monthLabel + '.' + weeks[index].year;
    numberOfDay[0].innerHTML = dateForDiv;
    day.id = weeks[index].id;
    day.href = '#' + weeks[index].link + weeks[index].id;
    let idForTab = weeks[index].link + weeks[index].id;
    let idString = 'id=' + idForTab;

    let curMonRus = ' Авг';
    rusDay[0].innerHTML = weeks[index].day + curMonRus;

    let nDoc = document.createElement('div');
    nDoc.setAttribute("id", idForTab);

    if (currentDay === weeks[index].day && currentMonth === weeks[index].month) {
        nDoc.setAttribute("class", 'tab-pane fade show active');
    } else {
        nDoc.setAttribute("class", 'tab-pane fade');
    }
    nDoc.setAttribute("role", "tabpanel");
    nDoc.setAttribute("aria-labelledby", idForTab);

    tabContentEat[0].appendChild(nDoc);
    document.getElementById(idForTab).innerHTML = elForTabs;


});

let orderInfo = [{amount: 2, priceOrder: 40, currency: '(₴)'}];
let totalPrice = orderInfo[0].amount * orderInfo[0].priceOrder;
document.getElementById('order-amount').innerHTML = orderInfo[0].amount;
document.getElementById('order-price').innerHTML = orderInfo[0].priceOrder + orderInfo[0].currency;
document.getElementById('total-order-price').innerHTML = totalPrice + orderInfo[0].currency;


// <div class="col-12 box-shadow-container position-relative brd-bot-line mb-3">
//     <div class="col-12 p-0 d-flex flex-column br">
//     <span class="h3 pt-lg-0 pt-md-0 pt-3 brd-bot-line-reverse pb-4 pb-md-0 pb-lg-0"
// style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>
// <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"
// style="font-size: 14px; line-height: 16px; color: #797979;">Перше</span>
//     </div>
//     <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"
// style="width: inherit; max-width: 61px;">
//     </div>
//     <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"
// style="font-size: 16px;">Борщ з
// пампушками
// </div>
// <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">
//     <div class="pl-4">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// </div>
// <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">
//     <span>150 гр.</span>
// <span class="d-inline d-md-none d-lg-none"> / </span>
//     <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>
// </div>
// <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴
// </div>
// </div>
// </div>
// <div class="col-12 p-0 d-flex flex-column">
//     <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"
// style="font-size: 14px; line-height: 16px; color: #797979;">Друге</span>
//     </div>
//     <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"
// style="width: inherit; max-width: 61px;">
//     </div>
//     <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"
// style="font-size: 16px;">Запіканка
//     </div>
//     <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">
//     <div class="pl-4">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// </div>
// <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">
//     <span>150 гр.</span>
// <span class="d-inline d-md-none d-lg-none"> / </span>
//     <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>
// </div>
// <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴
// </div>
// </div>
// </div>
// <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"
// style="width: inherit; max-width: 61px;">
//     </div>
//     <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"
// style="font-size: 16px;">Запечені яблука з сиром
// </div>
// <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">
//     <div class="pl-4">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// </div>
// <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">
//     <span>150 гр.</span>
// <span class="d-inline d-md-none d-lg-none"> / </span>
//     <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>
// </div>
// <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴
// </div>
// </div>
// </div>
//
//
// </div>
// <div class="col-12 box-shadow-container position-relative brd-bot-line mb-3">
//     <div class="col-12 p-0 d-flex flex-column br">
//     <span class="h3 pt-lg-0 pt-md-0 pt-3 brd-bot-line-reverse pb-4 pb-md-0 pb-lg-0"
// style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>
// <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"
// style="font-size: 14px; line-height: 16px; color: #797979;">Перше</span>
//     </div>
//     <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"
// style="width: inherit; max-width: 61px;">
//     </div>
//     <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"
// style="font-size: 16px;">Борщ з
// пампушками
// </div>
// <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">
//     <div class="pl-4">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// </div>
// <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">
//     <span>150 гр.</span>
// <span class="d-inline d-md-none d-lg-none"> / </span>
//     <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>
// </div>
// <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴
// </div>
// </div>
// </div>
// <div class="col-12 p-0 d-flex flex-column">
//     <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 mt-2"
// style="font-size: 14px; line-height: 16px; color: #797979;">Друге</span>
//     </div>
//     <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"
// style="width: inherit; max-width: 61px;">
//     </div>
//     <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"
// style="font-size: 16px;">Запіканка
//     </div>
//     <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">
//     <div class="pl-4">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// </div>
// <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">
//     <span>150 гр.</span>
// <span class="d-inline d-md-none d-lg-none"> / </span>
//     <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>
// </div>
// <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴
// </div>
// </div>
// </div>
// <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-2 px-0"><img src="../img/img-week.png" alt="Image week dish"
// style="width: inherit; max-width: 61px;">
//     </div>
//     <div class="col-lg-11 p-0 pl-2 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-2 col-md-2 col-12 px-md-0 px-lg-0 bolder-change"
// style="font-size: 16px;">Запечені яблука з сиром
// </div>
// <div class="col-5 d-lg-block d-md-block d-none pr-3" style="font-size: 16px;">
//     <div class="pl-4">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// </div>
// <div class="col-lg-3 col-md-3 col-12 d-flex justify-content-lg-start justify-content-md-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">
//     <span>150 гр.</span>
// <span class="d-inline d-md-none d-lg-none"> / </span>
//     <span class="d-inline d-md-none d-lg-none">40.09 ₴</span>
// </div>
// <div class="col-lg-2 col-md-2 col-12 justify-content-center d-lg-flex d-none d-md-flex"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">40.09 ₴
// </div>
// </div>
// </div>
//
//
// </div>