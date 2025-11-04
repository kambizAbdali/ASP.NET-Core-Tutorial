// Custom JavaScript for LoggingDemo
console.log('LoggingDemo application loaded');

// Auto-dismiss alerts after 5 seconds
setTimeout(function () {
    const alerts = document.querySelectorAll('.alert');
    alerts.forEach(function (alert) {
        const bsAlert = new bootstrap.Alert(alert);
        bsAlert.close();
    });
}, 5000);

// Form validation enhancement
document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');
    forms.forEach(function (form) {
        form.addEventListener('submit', function () {
            const submitButton = form.querySelector('button[type="submit"]');
            if (submitButton) {
                submitButton.disabled = true;
                submitButton.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...';
            }
        });
    });
});