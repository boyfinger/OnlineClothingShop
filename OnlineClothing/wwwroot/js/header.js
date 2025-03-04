// Add sticky class when scrolling
window.onscroll = function () {
    var header = document.querySelector('.header-area');
    if (window.pageYOffset > 0) {
        header.classList.add('sticky');
    } else {
        header.classList.remove('sticky');
    }
};

