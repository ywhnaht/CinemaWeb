//function stopVideo() {
//    var iframe = document.querySelector('#trailer iframe');
//    iframe.contentWindow.postMessage('{"event":"command","func":"pauseVideo","args":""}', '*');
//}
//document.addEventListener('DOMContentLoaded', function () {
//    var modal = document.getElementById('trailer');
//    modal.addEventListener('hidden.bs.modal', stopVideo);
//});

//document.addEventListener("DOMContentLoaded", function () {
//    const stars = document.querySelectorAll('.star');
//    const confirmBtn = document.querySelector('.confirm');
//    stars.forEach(function (star) {
//        star.addEventListener('click', function () {
//            const isActive = this.classList.contains('star-active');
//            // Remove 'star-active' class from all stars
//            stars.forEach(function (s) {
//                s.classList.remove('star-active');
//            });
//            // Add 'star-active' class only to the clicked star if it was not active
//            if (!isActive) {
//                this.classList.add('star-active');
//            }
//            // Add 'confirm-active' class to confirm button if there is at least one active star
//            const activeStars = document.querySelectorAll('.star-active');
//            if (activeStars.length > 0) {
//                confirmBtn.classList.add('confirm-active');
//            } else {
//                confirmBtn.classList.remove('confirm-active');
//            }
//        });
//    });
//});


//const stars = document.querySelectorAll('.star');

//for (var i = 1; i <= stars.length; i++) {
//    console.log("complete")
//    stars[i].addeventlistener('click', () => {
//        console.log(i);
//        for (var j = 1; j <= i; j++) {
//            var svg = stars[j].querySelector('svg');
//            svg.classList.add('star-hover');
//        }
//    })
//}
