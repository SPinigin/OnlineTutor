/**
 * Основной JavaScript-файл приложения
 * Импортирует и инициализирует все модули
 */

// Функция для определения текущей страницы
function getCurrentPage() {
    const path = window.location.pathname.toLowerCase();

    if (path.includes('/account/')) {
        return 'account';
    } else if (path.includes('/admin/')) {
        return 'admin';
    } else if (path.includes('/teacher/')) {
        return 'teacher';
    } else if (path.includes('/student/')) {
        return 'student';
    } else if (path.includes('/test/')) {
        return 'tests';
    } else {
        return 'home';
    }
}

// Инициализация при загрузке документа
$(document).ready(function () {
    // Инициализация базовых модулей
    if (typeof App !== 'undefined') {
        App.init();
    }

    if (typeof Validation !== 'undefined') {
        Validation.init();
    }

    // Инициализация компонентов
    if (typeof Forms !== 'undefined') {
        Forms.init();
    }

    if (typeof Alerts !== 'undefined') {
        Alerts.init();
    }

    if (typeof Modals !== 'undefined') {
        Modals.init();
    }

    // Инициализация модулей для конкретных страниц
    const currentPage = getCurrentPage();

    switch (currentPage) {
        case 'account':
            if (typeof AccountPage !== 'undefined') {
                AccountPage.init();
            }
            break;
        case 'admin':
            if (typeof AdminPage !== 'undefined') {
                AdminPage.init();
            }
            break;
        case 'teacher':
            if (typeof TeacherPage !== 'undefined') {
                TeacherPage.init();
            }
            break;
        case 'student':
            if (typeof StudentPage !== 'undefined') {
                StudentPage.init();
            }
            break;
        case 'tests':
            if (typeof TestsPage !== 'undefined') {
                TestsPage.init();
            }
            break;
    }
});
