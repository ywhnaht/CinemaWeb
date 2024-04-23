const optionBtns = document.querySelectorAll('.btn-option');

optionBtns.forEach((btn) => {
    btn.addEventListener('click', () => {
        optionBtns.forEach((otherBtn) => {
            if (otherBtn !== btn) {
                otherBtn.classList.remove('active');
            }
        });
        btn.classList.toggle('active');
    });
});
