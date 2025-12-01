
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

    // ===== Image Preview =====
    const imageInput = document.querySelector('input[name="HinhFile"]');

    if (imageInput) {
        imageInput.addEventListener('change', function (e) {
            const file = e.target.files[0];

            if (file) {
                // Validate file type
                const validTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif'];
                if (!validTypes.includes(file.type)) {
                    alert('Vui lòng chọn file ảnh (JPG, PNG, GIF)');
                    this.value = '';
                    return;
                }

                // Validate file size (max 5MB)
                const maxSize = 5 * 1024 * 1024; // 5MB
                if (file.size > maxSize) {
                    alert('Kích thước ảnh không được vượt quá 5MB');
                    this.value = '';
                    return;
                }

                // Preview image
                const reader = new FileReader();
                reader.onload = function (event) {
                    // Remove existing preview
                    const existingPreview = document.querySelector('.image-preview');
                    if (existingPreview) {
                        existingPreview.remove();
                    }

                    // Create new preview
                    const preview = document.createElement('div');
                    preview.className = 'image-preview';
                    preview.innerHTML = `
                        <img src="${event.target.result}" alt="Preview">
                        <p class="text-muted small mt-2">Ảnh đại diện của bạn</p>
                    `;

                    imageInput.parentElement.appendChild(preview);
                };
                reader.readAsDataURL(file);
            }
        });
    }

    // ===== Phone Number Validation =====
    const phoneInput = document.querySelector('input[name="DienThoai"]');

    if (phoneInput) {
        phoneInput.addEventListener('input', function (e) {
            // Only allow numbers
            this.value = this.value.replace(/[^0-9]/g, '');

            // Limit to 10 digits
            if (this.value.length > 10) {
                this.value = this.value.slice(0, 10);
            }
        });

        phoneInput.addEventListener('blur', function () {
            // Validate format
            const phonePattern = /^0\d{9}$/;
            if (this.value && !phonePattern.test(this.value)) {
                const errorSpan = this.parentElement.querySelector('.text-danger');
                if (errorSpan) {
                    errorSpan.textContent = 'Số điện thoại phải có 10 số và bắt đầu bằng 0';
                }
            }
        });
    }

    // ===== Email Validation =====
    const emailInput = document.querySelector('input[name="Email"]');

    if (emailInput) {
        emailInput.addEventListener('blur', function () {
            const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (this.value && !emailPattern.test(this.value)) {
                const errorSpan = this.parentElement.querySelector('.text-danger');
                if (errorSpan) {
                    errorSpan.textContent = 'Email không hợp lệ';
                }
            }
        });
    }

    // ===== Password Strength Indicator =====
    if (passwordInput) {
        const strengthIndicator = document.createElement('div');
        strengthIndicator.className = 'password-strength mt-2';
        strengthIndicator.style.display = 'none';
        passwordInput.parentElement.parentElement.appendChild(strengthIndicator);

        passwordInput.addEventListener('input', function () {
            const password = this.value;

            if (password.length === 0) {
                strengthIndicator.style.display = 'none';
                return;
            }

            strengthIndicator.style.display = 'block';

            let strength = 0;
            let strengthText = '';
            let strengthColor = '';

            // Check password strength
            if (password.length >= 6) strength++;
            if (password.length >= 10) strength++;
            if (/[a-z]/.test(password) && /[A-Z]/.test(password)) strength++;
            if (/\d/.test(password)) strength++;
            if (/[^a-zA-Z0-9]/.test(password)) strength++;

            switch (strength) {
                case 0:
                case 1:
                    strengthText = 'Yếu';
                    strengthColor = 'danger';
                    break;
                case 2:
                case 3:
                    strengthText = 'Trung bình';
                    strengthColor = 'warning';
                    break;
                case 4:
                case 5:
                    strengthText = 'Mạnh';
                    strengthColor = 'success';
                    break;
            }

            strengthIndicator.innerHTML = `
                <small class="text-${strengthColor}">
                    <i class="bi bi-shield-fill-check"></i> Độ mạnh mật khẩu: <strong>${strengthText}</strong>
                </small>
            `;
        });
    }

    // ===== Date of Birth Validation =====
    const dobInput = document.querySelector('input[name="NgaySinh"]');

    if (dobInput) {
        // Set max date to today
        const today = new Date().toISOString().split('T')[0];
        dobInput.setAttribute('max', today);

        dobInput.addEventListener('change', function () {
            const selectedDate = new Date(this.value);
            const today = new Date();
            const age = today.getFullYear() - selectedDate.getFullYear();

            if (age < 13) {
                alert('Bạn phải từ 13 tuổi trở lên để đăng ký');
                this.value = '';
            }
        });
    }

    // ===== Form Submit Handler =====
    const registerForm = document.querySelector('form');

    if (registerForm) {
        registerForm.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');

            // Add loading state
            if (submitBtn && this.checkValidity()) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...';

                // Restore button after 5 seconds if something goes wrong
                setTimeout(function () {
                    if (submitBtn.disabled) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }
                }, 5000);
            }
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

    // ===== Character Counter for Text Fields =====
    const hoTenInput = document.querySelector('input[name="HoTen"]');
    const diaChiInput = document.querySelector('input[name="DiaChi"]');

    function addCharCounter(input, maxLength) {
        if (!input) return;

        const counter = document.createElement('small');
        counter.className = 'text-muted float-end';
        input.parentElement.appendChild(counter);

        function updateCounter() {
            const remaining = maxLength - input.value.length;
            counter.textContent = `${input.value.length}/${maxLength}`;

            if (remaining < 10) {
                counter.classList.add('text-danger');
                counter.classList.remove('text-muted');
            } else {
                counter.classList.add('text-muted');
                counter.classList.remove('text-danger');
            }
        }

        input.addEventListener('input', updateCounter);
        updateCounter();
    }

    addCharCounter(hoTenInput, 100);
    addCharCounter(diaChiInput, 60);

    // ===== Confirm Before Leave =====
    let formChanged = false;

    inputs.forEach(input => {
        input.addEventListener('change', function () {
            formChanged = true;
        });
    });

    window.addEventListener('beforeunload', function (e) {
        if (formChanged && !registerForm.submitted) {
            e.preventDefault();
            e.returnValue = '';
        }
    });

    if (registerForm) {
        registerForm.addEventListener('submit', function () {
            registerForm.submitted = true;
        });
    }
});