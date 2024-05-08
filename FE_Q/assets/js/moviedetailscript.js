const mdBtn = document.querySelector('.monday-btn');
const tdBtn = document.querySelector('.tuesday-btn');
const wdBtn = document.querySelector('.wednesday-btn');
const thdBtn = document.querySelector('.thursday-btn');
const fdBtn = document.querySelector('.friday-btn');
const sdBtn = document.querySelector('.saturday-btn');
const sudBtn = document.querySelector('.sunday-btn');


mdBtn.addEventListener('click',() => {
    current = document.querySelector('.show');
    if(current){
        current.classList.remove('show');
    }
});

tdBtn.addEventListener('click',() => {
    current = document.querySelector('.show');
    if(current){
        current.classList.remove('show');
    }
    temp = document.querySelector('.active');
    if(temp){
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    tdBtn.classList.remove('default');
    tdBtn.classList.add('active');
});

wdBtn.addEventListener('click',() => {
    current = document.querySelector('.show');
    if(current){
        current.classList.remove('show');
    }
    temp = document.querySelector('.active');
    if(temp){
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    wdBtn.classList.remove('default');
    wdBtn.classList.add('active');
});

thdBtn.addEventListener('click',() => {
    current = document.querySelector('.show');
    if(current){
        current.classList.remove('show');
    }
    temp = document.querySelector('.active');
    if(temp){
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    thdBtn.classList.remove('default');
    thdBtn.classList.add('active');
});

fdBtn.addEventListener('click',() => {
    current = document.querySelector('.show');
    if(current){
        current.classList.remove('show');
    }
    temp = document.querySelector('.active');
    if(temp){
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    fdBtn.classList.remove('default');
    fdBtn.classList.add('active');
});
sdBtn.addEventListener('click',() => {
    current = document.querySelector('.show');
    if(current){
        current.classList.remove('show');
    }
    temp = document.querySelector('.active');
    if(temp){
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    sdBtn.classList.remove('default');
    sdBtn.classList.add('active');
});
sudBtn.addEventListener('click',() => {
    current = document.querySelector('.show');
    if(current){
        current.classList.remove('show');
    }
    temp = document.querySelector('.active');
    if(temp){
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    sudBtn.classList.remove('default');
    sudBtn.classList.add('active');
});


function stopVideo() {
    var iframe = document.querySelector('#trailer iframe');
    iframe.contentWindow.postMessage('{"event":"command","func":"pauseVideo","args":""}', '*');
}
document.addEventListener('DOMContentLoaded', function() {
    var modal = document.getElementById('trailer');
    modal.addEventListener('hidden.bs.modal', stopVideo);
});

document.addEventListener("DOMContentLoaded", function() {
    const stars = document.querySelectorAll('.star');
    const confirmBtn = document.querySelector('.confirm');
    stars.forEach(function(star) {
        star.addEventListener('click', function() {
            const isActive = this.classList.contains('star-active');
            // Remove 'star-active' class from all stars
            stars.forEach(function(s) {
                s.classList.remove('star-active');
            });
            // Add 'star-active' class only to the clicked star if it was not active
            if(!isActive){
                this.classList.add('star-active');
            }
            // Add 'confirm-active' class to confirm button if there is at least one active star
            const activeStars = document.querySelectorAll('.star-active');
            if(activeStars.length > 0){
                confirmBtn.classList.add('confirm-active');
            } else {
                confirmBtn.classList.remove('confirm-active');
            }
        });
    });
});
