const tdBtn = document.querySelector('.today-btn');
const tmBtn = document.querySelector('.tomorow-btn');
const datBtn = document.querySelector('.dat-btn');

tdBtn.addEventListener('click', () => {
    const currentTab = document.querySelector('.tab-pane.show');
    if (currentTab) {
        currentTab.classList.remove('show');
    }
    const targetTab = document.querySelector('#today');
    targetTab.classList.add('show');

    const temp = document.querySelector('.nav-link.active');
    if (temp) {
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    tdBtn.classList.remove('default');
    tdBtn.classList.add('active');
});

tmBtn.addEventListener('click', () => {
    const currentTab = document.querySelector('.tab-pane.show');
    if (currentTab) {
        currentTab.classList.remove('show');
    }
    const targetTab = document.querySelector('#tomorow');
    targetTab.classList.add('show');
    targetTab.classList.add('active');

    const temp = document.querySelector('.nav-link.active');
    if (temp) {
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    tmBtn.classList.remove('default');
    tmBtn.classList.add('active');
});

datBtn.addEventListener('click', () => {
    const currentTab = document.querySelector('.tab-pane.show');
    if (currentTab) {
        currentTab.classList.remove('show');
    }
    const targetTab = document.querySelector('#day-after-tomorrow');
    targetTab.classList.add('show');
    targetTab.classList.add('active');

    const temp = document.querySelector('.nav-link.active');
    if (temp) {
        temp.classList.remove('active');
        temp.classList.add('default');
    }
    datBtn.classList.remove('default');
    datBtn.classList.add('active');
});
