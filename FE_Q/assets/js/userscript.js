const inforBtn = document.querySelectorAll('.infor-btn');
const historyBtn = document.querySelectorAll('.history-btn');
const updateBtn = document.querySelector('.update');
const remove = document.querySelectorAll('.remove');  
const history = document.querySelector('.history');

// inforBtn.addEventListener('click', () => {
//    history.classList.remove('show');
// });

// historyBtn.addEventListener('click', () => {
//    document.querySelector('.show').classList.remove('show');
// });
// updateBtn.addEventListener('click', () => {
//    for(var input of remove) {
//        input.classList.remove('remove');
//    }
// });

historyBtn.forEach(element => {
    element.addEventListener('click', () => {
        document.querySelector('.show').classList.remove('show');
    });
   
});

inforBtn.forEach(element => {
    element.addEventListener('click', () => {
        history.classList.remove('show');
    });
   
});
