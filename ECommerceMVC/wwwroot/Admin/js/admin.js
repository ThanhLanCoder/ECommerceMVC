// wwwroot/js/admin.js

document.addEventListener('DOMContentLoaded', function () {

    // ===== Sidebar Toggle for Mobile =====
    const sidebarToggle = document.getElementById('sidebarToggle');
    const sidebar = document.querySelector('.sidebar');

    if (sidebarToggle) {
        sidebarToggle.addEventListener('click', function (e) {
            e.stopPropagation();
            sidebar.classList.toggle('show');
        });
    }

    // ===== Close Sidebar when Clicking Outside on Mobile =====
    document.addEventListener('click', function (event) {
        if (window.innerWidth <= 768) {
            const isClickInsideSidebar = sidebar.contains(event.target);
            const isClickOnToggle = sidebarToggle && sidebarToggle.contains(event.target);

            if (!isClickInsideSidebar && !isClickOnToggle && sidebar.classList.contains('show')) {
                sidebar.classList.remove('show');
            }
        }
    });

    // ===== Close Sidebar on Link Click (Mobile) =====
    const sidebarLinks = document.querySelectorAll('.sidebar-link');
    sidebarLinks.forEach(link => {
        link.addEventListener('click', function () {
            if (window.innerWidth <= 768) {
                sidebar.classList.remove('show');
            }
        });
    });

    // ===== Active Link Highlighting =====
    const currentPath = window.location.pathname;
    sidebarLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });

    // ===== Dropdown Auto Close =====
    const dropdowns = document.querySelectorAll('.dropdown-menu');
    dropdowns.forEach(dropdown => {
        dropdown.addEventListener('click', function (e) {
            e.stopPropagation();
        });
    });

    // ===== Smooth Scroll for Sidebar =====
    sidebar.style.scrollBehavior = 'smooth';

    // ===== Notification Badge Animation =====
    const indicators = document.querySelectorAll('.indicator');
    indicators.forEach(indicator => {
        indicator.style.animation = 'pulse 2s infinite';
    });

    // ===== Table Row Click (Optional) =====
    const tableRows = document.querySelectorAll('.table-custom tbody tr');
    tableRows.forEach(row => {
        row.style.cursor = 'pointer';
        row.addEventListener('click', function (e) {
            // Nếu click vào button thì không trigger
            if (e.target.tagName !== 'BUTTON' && e.target.tagName !== 'A' && e.target.tagName !== 'I') {
                const viewButton = this.querySelector('.btn-outline-primary');
                if (viewButton) {
                    viewButton.click();
                }
            }
        });
    });

    // ===== Responsive Table =====
    function checkTableResponsive() {
        const tables = document.querySelectorAll('.table-responsive');
        tables.forEach(table => {
            if (table.scrollWidth > table.clientWidth) {
                table.style.borderLeft = '1px solid #dee2e6';
                table.style.borderRight = '1px solid #dee2e6';
            }
        });
    }

    checkTableResponsive();
    window.addEventListener('resize', checkTableResponsive);
});

// ===== Animation for Pulse Effect =====
const style = document.createElement('style');
style.textContent = `
    @keyframes pulse {
        0%, 100% {
            transform: scale(1);
            opacity: 1;
        }
        50% {
            transform: scale(1.1);
            opacity: 0.8;
        }
    }
`;
document.head.appendChild(style);