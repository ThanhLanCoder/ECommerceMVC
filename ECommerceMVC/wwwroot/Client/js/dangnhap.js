
document.addEventListener('DOMContentLoaded', function () {

    // ===== Toggle Password Visibility =====
    const togglePassword = document.getElementById('togglePassword');
    const passwordInput = document.getElementById('passwordInput');

    if (togglePassword && passwordInput) {
        togglePassword.addEventListener('click', function () {
            const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordInput.setAttribute('type', type);

            // Toggle icon
            this.classList.toggle('bi-eye');
            this.classList.toggle('bi-eye-slash');
        });
    }

    // ===== Remember Me Functionality =====
    const rememberMeCheckbox = document.getElementById('rememberMe');
    const emailInput = document.querySelector('input[name="Email"]');

    // Load saved email if exists
    if (localStorage.getItem('rememberedEmail') && emailInput) {
        emailInput.value = localStorage.getItem('rememberedEmail');
        if (rememberMeCheckbox) {
            rememberMeCheckbox.checked = true;
        }
    }

    // ===== Form Submit Handler =====
    const loginForm = document.querySelector('form');

    if (loginForm) {
        loginForm.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');

            // Save or remove email based on remember me
            if (rememberMeCheckbox && emailInput) {
                if (rememberMeCheckbox.checked) {
                    localStorage.setItem('rememberedEmail', emailInput.value);
                } else {
                    localStorage.removeItem('rememberedEmail');
                }
            }

            // Add loading state
            if (submitBtn && this.checkValidity()) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...';

                // Restore button after 3 seconds if form validation fails
                setTimeout(function () {
                    if (submitBtn.disabled) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }
                }, 3000);
            }
        });
    }

    // ===== Social Login Buttons =====
    const btnGoogle = document.querySelector('.btn-google');
    const btnFacebook = document.querySelector('.btn-facebook');

    if (btnGoogle) {
        btnGoogle.addEventListener('click', function (e) {
            e.preventDefault();
            console.log('Đăng nhập với Google');
            // Redirect to Google OAuth
            // window.location.href = '/Account/GoogleLogin';
            alert('Chức năng đăng nhập Google sẽ được tích hợp sau');
        });
    }

    if (btnFacebook) {
        btnFacebook.addEventListener('click', function (e) {
            e.preventDefault();
            console.log('Đăng nhập với Facebook');
            // Redirect to Facebook OAuth
            // window.location.href = '/Account/FacebookLogin';
            alert('Chức năng đăng nhập Facebook sẽ được tích hợp sau');
        });
    }

    // ===== Input Animation =====
    const inputs = document.querySelectorAll('.form-control');
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('focused');
        });

        input.addEventListener('blur', function () {
            if (!this.value) {
                this.parentElement.classList.remove('focused');
            }
        });
    });

    // ===== Enter Key Handler =====
    if (passwordInput) {
        passwordInput.addEventListener('keypress', function (e) {
            if (e.key === 'Enter') {
                loginForm.submit();
            }
        });
    }
});