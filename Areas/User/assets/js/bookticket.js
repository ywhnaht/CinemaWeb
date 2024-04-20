const movieItems = document.querySelectorAll('.sub-movie-list1');
const movieText = document.querySelector('.movie-text');
const movieSelectedName = document.querySelector('.movie-selected-name');
const movieSelectedImg = document.querySelector('.movie-selected-img');
const movieHide = document.getElementById('movielist');
const datetimeShow = document.getElementById('datetime');
const datetimeContainer = document.querySelector('.datetime-container')
var totalprice = 0;

function RemoveSeatSelected() {
    document.querySelectorAll('.seat-selected').forEach((seat) => {
        seat.classList.remove('seat-selected');
    })

    const chosenSeat = document.getElementById('chosen-seats')
    chosenSeat.textContent = ''
    const Moneyitem = document.getElementById('total-price')
    const Priceitem = document.getElementById('sum-money')
    Moneyitem.textContent = '0'
    Priceitem.textContent = '0'
    totalprice = 0
    //const allSeats = document.querySelectorAll('.room-list .seat-item');
    //allSeats.forEach((seat) => {
    //    seat.classList.remove('selected')
    //})
}

function RemoveTimeSelected() {
    document.querySelectorAll('.time-list span').forEach(function(li) {
        li.classList.remove('chosen');
    });
    const ticketDetail = document.querySelector('.ticket-selected-detail');
    ticketDetail.textContent = ''
    /*// HideSeatContainer()*/
}

function RemovieDateSelected() {
    document.querySelectorAll('.date-list-side li').forEach(function(li) {
        li.classList.remove('hovered');
    });
}

function HideSeatContainer() {
    document.querySelectorAll('.seat-container').forEach((item) => {
        if (item.classList.contains('active'))
            item.classList.remove('active');
    })
}

// Ch?n phim thì s? l?u tên phim vào b?ng giá
    movieItems.forEach(function (movieItem) {
        movieItem.addEventListener('click', function () {
            const movieName = this.querySelector('.movie-name').innerText;
            const movieImgSrc = this.querySelector('.img-movie').getAttribute('src');

            movieText.textContent = "Chon phim - " + movieName;
            movieSelectedName.innerText = movieName;
            movieSelectedImg.setAttribute('src', movieImgSrc);
            movieHide.classList.remove('show');
            datetimeShow.classList.add('show');
            datetimeContainer.classList.add('open')
        });
    });

     // ?n vào ngày thì tô màu
     // M?c ??nh s? ch?n DateTime.Now ?? hi?n th? su?t chi?u
        // ??t mã JavaScript c?a b?n ? ?ây
        // Ví d?: gán các s? ki?n click cho các ph?n t?
        //const lifirst = document.querySelector('.date-list-side li:first-of-type')
        //lifirst.classList.add('hovered')
        //const afirst = document.querySelector('.date-list-side li:first-of-type a:first-of-type')
        //afirst.classList.add('active')
        //document.querySelectorAll('.date-list-side a').forEach(function(link) {
        //    link.addEventListener('click', function(event) {
        //        event.preventDefault(); 
        //        const moviedate = this.querySelectorAll('.date-time-item span');

        //        RemovieDateSelected()

        //        var liId = this.getAttribute('href').substring(1) + '-li'; 
        //        document.getElementById(liId).classList.add('hovered');
        //    });
        //});

    // ?n vào gi? thì tô màu
    const navbarItem2 = document.getElementById('navbar-item-2')
    document.querySelectorAll('.time-list span').forEach(function(item) {
        item.addEventListener('click', function(event) {
            navbarItem2.classList.add('navbar-colored')
            event.preventDefault();
            if (item.classList.contains('chosen'))
                return
            else {
                //document.querySelectorAll('.seat-selected').forEach((seat) => {
                //    seat.classList.remove('seat-selected');
                //})
                RemoveTimeSelected()
                RemoveSeatSelected()
                this.classList.add('chosen');
            }   
        });
    });

    // Thêm ngày và su?t vào b?ng giá
const timeItems = document.querySelectorAll('.time-item');

    timeItems.forEach(timeItem => {
        timeItem.addEventListener('click', () => {
            const showtime = timeItem.textContent.trim();
            
            const tabPaneId = timeItem.closest('.tab-pane').id;
            
            const dateListItem = document.querySelector(`#${tabPaneId}-li`);
            const dayOfWeek = dateListItem.querySelector('span:first-child').textContent.trim();
            const date = dateListItem.querySelector('span:last-child').textContent.trim();
            
            const ticketDetail = document.querySelector('.ticket-selected-detail');
            ticketDetail.innerHTML = `
                <div class="row">
                    <span>Ohayou Da Nang - RAP 1</span>
                </div>
                <div class="row d-inline-block">
                    <span>Time: ${showtime}</span>
                    <span>${dayOfWeek}, ${date}/2024</span>
                </div>
            `;
        });
    });
    // Thêm s? gh? và ti?n t??ng ?ng vào b?ng giá
    const chosenSeats = document.getElementById('chosen-seats')
    const totalPrice = document.getElementById('total-price')
    const allSeats = document.querySelectorAll('.room-list .seat-item');
    const Money = document.getElementById('sum-money')
        allSeats.forEach((seatSelect) => {
            seatSelect.addEventListener('click', () => {
                const price = 60000
                const seatRow = seatSelect.getAttribute('data-row')
                const seatNum = seatSelect.getAttribute('data-seat')
                if (seatSelect.classList.contains('seat-selected')) {
                    seatSelect.classList.remove('seat-selected');
                    totalprice -= price;
                    removeSeat(seatRow, seatNum)
                }
                else {
                    seatSelect.classList.add('seat-selected');
                    totalprice += price;
                    addSeat(seatRow, seatNum)
                }
                totalPrice.textContent = totalprice.toLocaleString()
                Money.textContent = totalprice.toLocaleString()
            });
        });
    
        function addSeat (row, seatNum) {
            const chosenSeat = chosenSeats.textContent.split(' ')
             //if (chosenSeat.length > 6) {
                
             //    return
             //}
            chosenSeat.push(row + seatNum)
            chosenSeats.textContent = chosenSeat.join(' ')
        }
    
        function removeSeat (row, seatNum) {
            let chosenSeat = chosenSeats.textContent.split(' ')
            chosenSeat = chosenSeat.filter(seat => seat !== row + seatNum)
            chosenSeats.textContent = chosenSeat.join(' ')
        }
    
    // ?n ch?n phim khác thì su?t chi?u và giá vé c?p nh?t l?i
const movieListItems = document.querySelectorAll('.sub-movie-list1');

    movieListItems.forEach(movieListItem => {
        movieListItem.addEventListener('click', () => {
            RemoveSeatSelected()
            RemoveTimeSelected()
            HideSeatContainer()
        });
    });

    // Thêm màu khi ch?n gh? và xoá màu khi b? gh?
    //document.querySelectorAll('.room-list .seat-item').forEach((seatItem) => {
    //    seatItem.addEventListener('click', (event) => {
    //        if (seatItem.classList.contains('seat-selected')) {
    //            seatItem.classList.remove('seat-selected');
    //        }
    //        else {
    //            seatItem.classList.add('seat-selected'); 
    //        }
    //    });
    //});

    // ?n ch?n su?t chi?u khác thì gh? và giá vé c?p nh?t l?i
    document.querySelectorAll('.time-list span').forEach(function(Timeitem) {
        Timeitem.addEventListener('click', () => {
            if (Timeitem.classList.contains('chosen'))
                return
            else {
                RemoveSeatSelected()
                totalprice = 0
            }
        })
    });


    