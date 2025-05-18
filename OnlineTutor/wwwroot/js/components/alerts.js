/**
 * Обработка уведомлений
 */

const Alerts = {
    init: function () {
        // Инициализация автоматического закрытия уведомлений
        this.setupAutoDismiss();

        // Инициализация обработчиков кнопок закрытия
        this.setupDismissButtons();
    },

    setupAutoDismiss: function () {
        // Автоматически закрываем уведомления через 5 секунд
        setTimeout(function () {
            $('.alert-auto-dismiss').alert('close');
        }, 5000);
    },

    setupDismissButtons: function () {
        // Обработчик для кнопок закрытия уведомлений
        $(document).on('click', '.alert .btn-close', function () {
            $(this).closest('.alert').alert('close');
        });
    },

    // Метод для создания уведомления
    createAlert: function (message, type = 'info', autoDismiss = true) {
        const alertClass = autoDismiss ? 'alert-auto-dismiss' : '';

        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show ${alertClass}" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;

        return alertHtml;
    },

    // Метод для добавления уведомления в контейнер
    showAlert: function (container, message, type = 'info', autoDismiss = true) {
        const alertHtml = this.createAlert(message, type, autoDismiss);
        $(container).prepend(alertHtml);

        if (autoDismiss) {
            setTimeout(function () {
                $(container).find('.alert').first().alert('close');
            }, 5000);
        }
    }
};

// Инициализация при загрузке документа
$(document).ready(function () {
    Alerts.init();
});
