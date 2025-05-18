/**
 * Обработка модальных окон
 */

const Modals = {
    init: function () {
        // Инициализация обработчиков модальных окон
        this.setupModalHandlers();
    },

    setupModalHandlers: function () {
        // Обработчик для модальных окон подтверждения удаления
        $(document).on('show.bs.modal', '.modal-confirm-delete', function (event) {
            const button = $(event.relatedTarget);
            const id = button.data('id');
            const name = button.data('name');

            const modal = $(this);
            modal.find('.modal-item-name').text(name);
            modal.find('form input[name="id"]').val(id);
        });

        // Обработчик для модальных окон с загрузкой контента
        $(document).on('show.bs.modal', '.modal-load-content', function (event) {
            const button = $(event.relatedTarget);
            const url = button.data('url');

            const modal = $(this);
            modal.find('.modal-body').html('<div class="text-center"><div class="spinner-border" role="status"><span class="visually-hidden">Загрузка...</span></div></div>');

            $.get(url, function (data) {
                modal.find('.modal-body').html(data);
            }).fail(function () {
                modal.find('.modal-body').html('<div class="alert alert-danger">Ошибка при загрузке данных</div>');
            });
        });
    },

    // Метод для открытия модального окна
    openModal: function (modalId) {
        const modal = new bootstrap.Modal(document.getElementById(modalId));
        modal.show();
    },

    // Метод для закрытия модального окна
    closeModal: function (modalId) {
        const modalEl = document.getElementById(modalId);
        const modal = bootstrap.Modal.getInstance(modalEl);
        if (modal) {
            modal.hide();
        }
    },

    // Метод для создания и открытия модального окна подтверждения
    confirm: function (title, message, callback, cancelCallback) {
        // Создаем ID для модального окна
        const modalId = 'confirmModal' + Math.floor(Math.random() * 1000000);

        // HTML для модального окна
        const modalHtml = `
            <div class="modal fade" id="${modalId}" tabindex="-1" aria-labelledby="${modalId}Label" aria-hidden="true">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="${modalId}Label">${title}</h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>
                        <div class="modal-body">
                            ${message}
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary btn-cancel" data-bs-dismiss="modal">Отмена</button>
                            <button type="button" class="btn btn-primary btn-confirm">Подтвердить</button>
                        </div>
                    </div>
                </div>
            </div>
        `;

        // Добавляем модальное окно в body
        $('body').append(modalHtml);

        // Получаем элемент модального окна
        const modalEl = document.getElementById(modalId);

        // Добавляем обработчики событий
        $(modalEl).find('.btn-confirm').on('click', function () {
            if (typeof callback === 'function') {
                callback();
            }
            bootstrap.Modal.getInstance(modalEl).hide();
        });

        $(modalEl).find('.btn-cancel').on('click', function () {
            if (typeof cancelCallback === 'function') {
                cancelCallback();
            }
        });

        // Обработчик события закрытия модального окна
        $(modalEl).on('hidden.bs.modal', function () {
            $(this).remove();
        });

        // Открываем модальное окно
        const modal = new bootstrap.Modal(modalEl);
        modal.show();
    }
};

// Инициализация при загрузке документа
$(document).ready(function () {
    Modals.init();
});
