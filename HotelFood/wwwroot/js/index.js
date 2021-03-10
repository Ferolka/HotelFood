let weeks = [{dayWeek: 'Пн', day: 10, month: 8, year: 2020, id: '100820', link: 'day'}, {
    dayWeek: 'Вт',
    day: 11,
    month: 8,
    year: 2020,
    id: '110820',
    link: 'day'
}, {dayWeek: 'Ср', day: 12, month: 8, year: 2020, id: '120820', link: 'day'}, {
    dayWeek: 'Чт',
    day: 13,
    month: 8,
    year: 2020,
    id: '130820',
    link: 'day'
}, {dayWeek: 'Пт', day: 14, month: 8, year: 2020, id: '140820', link: 'day'}, {
    dayWeek: 'Сб',
    day: 15,
    month: 8,
    year: 2020,
    id: '150820',
    link: 'day'
}, {dayWeek: 'Вс', day: 16, month: 8, year: 2020, id: '160820', link: 'day'},
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
        day: 229,
        month: 8,
        year: 2020,
        id: '290820',
        link: 'day'
    }, {dayWeek: 'Вс', day: 30, month: 8, year: 2020, id: '300820', link: 'day'}];

let elForTabs = '<div class="col-12 p-lg-0 p-md-0 box-shadow-container position-relative brd-bot-line mb-3">\n' +
    '                <div class="col-12 p-0 d-flex flex-column">\n' +
    '                    <span class="h3 pt-lg-0 pt-md-0 pt-3"\n' +
    '                          style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>\n' +
    '                    <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 "\n' +
    '                          style="font-size: 14px; line-height: 16px; color: #797979;"><img src="../img/arrow-right.svg"\n' +
    '                                                                                           class="d-md-block d-lg-block d-none mr-1"\n' +
    '                                                                                           alt="Arrow right">Борщ з пампушками,Запіканка,Запечені яблука з сиром</span>\n' +
    '                </div>\n' +
    '                <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                    <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-week.png" alt="Image week dish">\n' +
    '                    </div>\n' +
    '                    <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                        <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Борщ з пампушками</div>\n' +
    '                        <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Смачний український\n' +
    '                            борщ з пампушками та запашним часником.\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"\n' +
    '                             style="font-family: Open-Sans, sans-serif; font-size: 16px;">150 гр.\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                </div>\n' +
    '                <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                    <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-week.png" alt="Image week dish">\n' +
    '                    </div>\n' +
    '                    <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                        <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Борщ з пампушками</div>\n' +
    '                        <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Смачний український\n' +
    '                            борщ з пампушками та запашним часником.\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"\n' +
    '                             style="font-family: Open-Sans, sans-serif; font-size: 16px;">150 гр.\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                </div>\n' +
    '                <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                    <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-week.png" alt="Image week dish">\n' +
    '                    </div>\n' +
    '                    <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                        <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Борщ з пампушками</div>\n' +
    '                        <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Смачний український\n' +
    '                            борщ з пампушками та запашним часником.\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"\n' +
    '                             style="font-family: Open-Sans, sans-serif; font-size: 16px;">150 гр.\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                </div>\n' +
    '                <div class="custom-control font-weight-bold d-flex justify-content-end p-0 pb-3 pb-lg-0 pb-md-0 pos-for-btn" style="width: 156px;  ">\n' +
    '                    <label class="custom-control custom-checkbox m-0 p-0 d-flex justify-content-between align-items-center flex-grow-1 px-2" style="height: 36px; font-size: 14px; color: #36C233; width: 100%; border-radius: 4px; border: 1px solid #36C233;">\n' +
    '                        <input type="checkbox" class="custom-control-input">\n' +
    '                        <span class="custom-control-indicator align-self-start"></span>\n' +
    '                        <span class="custom-control-description mr-1">ЗАМОВИТИ</span>\n' +
    '                    </label>\n' +
    '                </div>\n' +
    '\n' +
    '            </div>\n' +
    '            <div class="col-12 p-lg-0 p-md-0 box-shadow-container position-relative brd-bot-line mb-3">\n' +
    '                <div class="col-12 p-0 d-flex flex-column">\n' +
    '                    <span class="h3 pt-lg-0 pt-md-0 pt-3"\n' +
    '                          style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>\n' +
    '                    <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0"\n' +
    '                          style="font-size: 14px; line-height: 16px; color: #797979;"><img src="../img/arrow-right.svg"\n' +
    '                                                                                           class="d-md-block d-lg-block d-none mr-1"\n' +
    '                                                                                           alt="Arrow right">Борщ з пампушками,Запіканка,Запечені яблука з сиром</span>\n' +
    '                </div>\n' +
    '                <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">\n' +
    '                    <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-dish-week.png" alt="Image week dish">\n' +
    '                    </div>\n' +
    '                    <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">\n' +
    '                        <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Омлет з сиром</div>\n' +
    '                        <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Ніжний паровий омлет з сиром\n' +
    '                        </div>\n' +
    '                        <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"\n' +
    '                             style="font-family: Open-Sans, sans-serif; font-size: 16px;">100 гр.\n' +
    '                        </div>\n' +
    '                    </div>\n' +
    '                </div>\n' +
    '                <div class="custom-control font-weight-bold d-flex justify-content-end p-0 pb-3 pb-lg-0 pb-md-0 pos-for-btn" style="width: 156px;  ">\n' +
    '                    <label class="custom-control custom-checkbox m-0 p-0 d-flex justify-content-between align-items-center flex-grow-1 px-2" style="height: 36px; font-size: 14px; color: #36C233; width: 100%; border-radius: 4px; border: 1px solid #36C233;">\n' +
    '                        <input type="checkbox" class="custom-control-input" checked>\n' +
    '                        <span class="custom-control-indicator align-self-start"></span>\n' +
    '                        <span class="custom-control-description mr-1">ЗАМОВИТИ</span>\n' +
    '                    </label>\n' +
    '                </div>\n' +
    '\n' +
    '            </div>';


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

// <div class="col-12 p-lg-0 p-md-0 box-shadow-container position-relative brd-bot-line mb-3">
//     <div class="col-12 p-0 d-flex flex-column">
//     <span class="h3 pt-lg-0 pt-md-0 pt-3"
// style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>
// <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0 "
// style="font-size: 14px; line-height: 16px; color: #797979;"><img src="../img/arrow-right.svg"
// class="d-md-block d-lg-block d-none mr-1"
// alt="Arrow right">Борщ з пампушками,Запіканка,Запечені яблука з сиром</span>
// </div>
// <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-week.png" alt="Image week dish">
//     </div>
//     <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Борщ з пампушками</div>
// <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">150 гр.
// </div>
// </div>
// </div>
// <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-week.png" alt="Image week dish">
//     </div>
//     <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Борщ з пампушками</div>
// <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">150 гр.
// </div>
// </div>
// </div>
// <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-week.png" alt="Image week dish">
//     </div>
//     <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Борщ з пампушками</div>
// <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Смачний український
// борщ з пампушками та запашним часником.
// </div>
// <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">150 гр.
// </div>
// </div>
// </div>
// <div class="custom-control font-weight-bold d-flex justify-content-end p-0 pb-3 pb-lg-0 pb-md-0 pos-for-btn" style="width: 156px;  ">
//     <label class="custom-control custom-checkbox m-0 p-0 d-flex justify-content-between align-items-center flex-grow-1 px-2" style="height: 36px; font-size: 14px; color: #36C233; width: 100%; border-radius: 4px; border: 1px solid #36C233;">
//     <input type="checkbox" class="custom-control-input">
//     <span class="custom-control-indicator align-self-start"></span>
//     <span class="custom-control-description mr-1">ЗАМОВИТИ</span>
//     </label>
//     </div>
//
//     </div>
//     <div class="col-12 p-lg-0 p-md-0 box-shadow-container position-relative brd-bot-line mb-3">
//     <div class="col-12 p-0 d-flex flex-column">
//     <span class="h3 pt-lg-0 pt-md-0 pt-3"
// style="font-weight: 600; font-size: 18px; color: #232323">Сніданок для садочків</span>
// <span class="h4 font-italic d-flex align-items-center pb-2 pb-lg-0 pb-md-0"
// style="font-size: 14px; line-height: 16px; color: #797979;"><img src="../img/arrow-right.svg"
// class="d-md-block d-lg-block d-none mr-1"
// alt="Arrow right">Борщ з пампушками,Запіканка,Запечені яблука з сиром</span>
// </div>
// <div class="col-12 p-0 d-flex flex-lg-row flex-md-row align-items-md-center align-items-lg-center align-items-start pb-lg-2 pb-md-2 pb-3">
//     <div class="col-lg-1 col-md-2 col-3 px-0"><img src="../img/img-dish-week.png" alt="Image week dish">
//     </div>
//     <div class="col-lg-11 p-0 col-md-10 col-10 d-flex flex-lg-row flex-md-row flex-column align-items-lg-center align-items-md-center align-items-start justify-content-start">
//     <div class="col-lg-5 col-md-4 col-12 px-0 bolder-change" style="font-size: 16px;">Омлет з сиром</div>
// <div class="col-4 d-lg-block d-md-block d-none" style="font-size: 16px;">Ніжний паровий омлет з сиром
// </div>
// <div class="col-lg-3 col-md-3 col-12 px-0 d-flex justify-content-md-end justify-content-lg-end justify-content-start lighter-change"
// style="font-family: Open-Sans, sans-serif; font-size: 16px;">100 гр.
// </div>
// </div>
// </div>
// <div class="custom-control font-weight-bold d-flex justify-content-end p-0 pb-3 pb-lg-0 pb-md-0 pos-for-btn" style="width: 156px;  ">
//     <label class="custom-control custom-checkbox m-0 p-0 d-flex justify-content-between align-items-center flex-grow-1 px-2" style="height: 36px; font-size: 14px; color: #36C233; width: 100%; border-radius: 4px; border: 1px solid #36C233;">
//     <input type="checkbox" class="custom-control-input" checked>
// <span class="custom-control-indicator align-self-start"></span>
//     <span class="custom-control-description mr-1">ЗАМОВИТИ</span>
//     </label>
//     </div>
//
//     </div>