// Hop thoai xac nhan cho cac form co data-confirm (xoa mon, xoa anh, xoa loai)
document.addEventListener('submit', function (e) {
    var form = e.target;
    var message = form && form.dataset ? form.dataset.confirm : null;
    if (message && !window.confirm(message)) {
        e.preventDefault();
    }
});
