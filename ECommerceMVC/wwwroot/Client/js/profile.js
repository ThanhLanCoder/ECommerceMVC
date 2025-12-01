// wwwroot/js/profile.js

document.addEventListener('DOMContentLoaded', function () {

    // ===== Avatar Upload & Preview =====
    const avatarInput = document.getElementById('avatarInput');
    const avatarPreview = document.getElementById('avatarPreview');

    if (avatarInput) {
        avatarInput.addEventListener('change', function (e) {
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
                const maxSize = 5 * 1024 * 1024;
                if (file.size > maxSize) {
                    alert('Kích thước ảnh không được vượt quá 5MB');
                    this.value = '';
                    return;
                }

                // Preview image
                const reader = new FileReader();
                reader.onload = function (event) {
                    avatarPreview.src = event.target.result;
                };
                reader.readAsDataURL(file);
            }
        });
    }

    // ===== Edit Mode Toggle =====
    const editBtn = document.getElementById('editBtn');
    const saveBtn = document.getElementById('saveBtn');
    const cancelBtn = document.getElementById('cancelBtn');
    const form = document.getElementById('profileForm');
    const inputs = form.querySelectorAll('input:not([type="email"]):not([disabled]), select:not([disabled])');
    let originalValues = {};

    if (editBtn) {
        editBtn.addEventListener('click', function () {
            // Save original values
            inputs.forEach(input => {
                originalValues[input.name] = input.value;
                input.disabled = false;
            });

            editBtn.classList.add('d-none');
            saveBtn.classList.remove('d-none');
            cancelBtn.classList.remove('d-none');
        });
    }

    if (cancelBtn) {
        cancelBtn.addEventListener('click', function () {
            // Restore original values
            inputs.forEach(input => {
                if (originalValues[input.name] !== undefined) {
                    input.value = originalValues[input.name];
                }
                input.disabled = true;
            });

            editBtn.classList.remove('d-none');
            saveBtn.classList.add('d-none');
            cancelBtn.classList.add('d-none');

            // Reset avatar if changed
            if (avatarInput && avatarInput.value) {
                avatarInput.value = '';
            }
        });
    }

    // ===== Profile Form Submit =====
    if (form) {
        form.addEventListener('submit', function (e) {
            const submitBtn = this.querySelector('button[type="submit"]');

            if (submitBtn && this.checkValidity()) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang lưu...';

                // Restore after 5 seconds if something goes wrong
                setTimeout(function () {
                    if (submitBtn.disabled) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }
                }, 5000);
            }
        });
    }

    // ===== Phone Number Validation =====
    const phoneInput = document.querySelector('input[name="DienThoai"]');

    if (phoneInput) {
        phoneInput.addEventListener('input', function (e) {
            this.value = this.value.replace(/[^0-9]/g, '');
            if (this.value.length > 10) {
                this.value = this.value.slice(0, 10);
            }
        });
    }

    // ===== Password Form Submit (Đơn giản - chỉ cần 6 ký tự) =====
    const passwordForm = document.getElementById('passwordForm');

    if (passwordForm) {
        passwordForm.addEventListener('submit', function (e) {
            const matKhauMoi = this.querySelector('input[name="MatKhauMoi"]');
            const xacNhan = this.querySelector('input[name="XacNhanMatKhau"]');

            if (matKhauMoi && xacNhan) {
                // Chỉ kiểm tra khớp và tối thiểu 6 ký tự
                if (matKhauMoi.value !== xacNhan.value) {
                    e.preventDefault();
                    alert('Mật khẩu xác nhận không khớp!');
                    return;
                }

                if (matKhauMoi.value.length < 6) {
                    e.preventDefault();
                    alert('Mật khẩu phải có ít nhất 6 ký tự!');
                    return;
                }
            }

            const submitBtn = this.querySelector('button[type="submit"]');
            if (submitBtn) {
                submitBtn.disabled = true;
                const originalText = submitBtn.innerHTML;
                submitBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-2"></span>Đang xử lý...';

                setTimeout(function () {
                    if (submitBtn.disabled) {
                        submitBtn.disabled = false;
                        submitBtn.innerHTML = originalText;
                    }
                }, 5000);
            }
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
            const todayDate = new Date();
            const age = todayDate.getFullYear() - selectedDate.getFullYear();

            if (age < 13) {
                alert('Bạn phải từ 13 tuổi trở lên');
                this.value = '';
            }
        });
    }

    // ===== Confirm Dialog Before Leaving Page =====
    let formChanged = false;

    if (form) {
        const formInputs = form.querySelectorAll('input, select, textarea');
        formInputs.forEach(input => {
            input.addEventListener('change', function () {
                if (!input.disabled) {
                    formChanged = true;
                }
            });
        });

        window.addEventListener('beforeunload', function (e) {
            if (formChanged && !form.submitted) {
                e.preventDefault();
                e.returnValue = 'Bạn có thay đổi chưa được lưu. Bạn có chắc muốn rời khỏi trang?';
            }
        });

        form.addEventListener('submit', function () {
            form.submitted = true;
            formChanged = false;
        });
    }

    // ===== Character Counter =====
    const hoTenInput = document.querySelector('input[name="HoTen"]');
    const diaChiInput = document.querySelector('input[name="DiaChi"]');

    function addCharCounter(input, maxLength) {
        if (!input) return;

        const counter = document.createElement('small');
        counter.className = 'text-muted float-end char-counter';
        counter.style.fontSize = '0.85rem';

        const parent = input.parentElement;
        const label = parent.querySelector('.form-label');
        if (label) {
            label.appendChild(counter);
        }

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

    // ===== Input Focus Animation =====
    const allInputs = document.querySelectorAll('.form-control, .form-select');
    allInputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.classList.add('input-focused');
        });

        input.addEventListener('blur', function () {
            this.parentElement.classList.remove('input-focused');
        });
    });

    // ===== Auto-hide validation messages on input =====
    const validationInputs = document.querySelectorAll('input.is-invalid, select.is-invalid');
    validationInputs.forEach(input => {
        input.addEventListener('input', function () {
            this.classList.remove('is-invalid');
            const errorMsg = this.parentElement.querySelector('.text-danger');
            if (errorMsg) {
                errorMsg.style.display = 'none';
            }
        });
    });

    // ===== Smooth Scroll to Error =====
    const firstError = document.querySelector('.is-invalid');
    if (firstError) {
        firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
        firstError.focus();
    }
});