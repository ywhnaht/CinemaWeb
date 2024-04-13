const inforBtn = document.querySelector('.infor-btn');
const historyBtn = document.querySelector('.history-btn');
const updateBtn = document.querySelector('.update');
const remove = document.querySelectorAll('.remove');  
console.log(history);
const history = document.querySelector('.history');
inforBtn.addEventListener('click', () => {
   history.classList.remove('show');
});

historyBtn.addEventListener('click', () => {
   document.querySelector('.show').classList.remove('show');
});
updateBtn.addEventListener('click', () => {
   for(var input of remove) {
       input.classList.remove('remove');
   }
});
