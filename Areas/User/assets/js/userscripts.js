
const inforBtn = document.querySelector('.infor-btn');
const historyBtn = document.querySelector('.history-btn');
const history = document.querySelector('.history');
inforBtn.addEventListener('click', () => {
    history.classList.remove('show');
});

historyBtn.addEventListener('click', () => {
    document.querySelector('.show').classList.remove('show');
});