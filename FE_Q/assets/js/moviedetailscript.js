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
