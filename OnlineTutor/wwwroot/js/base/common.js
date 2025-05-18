/**
 * Общие функции и утилиты
 */

// Глобальный объект приложения
const App = {
    init: function () {
        console.log('Приложение инициализировано');

        // Автоматическое закрытие алертов через 5 секунд
        this.setupAlertDismissal();
    },

    setupAlertDismissal: function () {
        setTimeout(function () {
            $('.alert').alert('close');
        }, 5000);
    },

    // Форматирование даты
    formatDate: function (dateString) {
        const date = new Date(dateString);
        return date.toLocaleDateString('ru-RU');
    },

    // Показ уведомления
    showNotification: function (message, type = 'info') {
        const alertHtml = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>
        `;

        // Добавляем уведомление в начало основного контейнера
        $('.container').first().prepend(alertHtml);

        // Автоматически закрываем через 5 секунд
        setTimeout(function () {
            $('.alert').alert('close');
        }, 5000);
    }
};

// Инициализация при загрузке документа
$(document).ready(function () {
    App.init();
});
