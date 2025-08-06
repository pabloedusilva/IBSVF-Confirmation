
document.addEventListener('DOMContentLoaded', function () {
    // Elementos do DOM
    const loginForm = document.getElementById('loginForm');
    const passwordToggle = document.getElementById('passwordToggle');
    const passwordInput = document.getElementById('password');
    const forgotPasswordBtn = document.getElementById('forgotPasswordBtn');
    const forgotPasswordModal = document.getElementById('forgotPasswordModal');
    const closeForgotModal = document.getElementById('closeForgotModal');

    // Toggle para mostrar/ocultar senha
    passwordToggle.addEventListener('click', function () {
        const type = passwordInput.getAttribute('type') === 'password' ? 'text' : 'password';
        passwordInput.setAttribute('type', type);

        const icon = this.querySelector('i');
        icon.classList.toggle('fa-eye');
        icon.classList.toggle('fa-eye-slash');
    });

    // Abrir modal de recuperação de senha
    forgotPasswordBtn.addEventListener('click', function (e) {
        e.preventDefault();
        forgotPasswordModal.classList.add('show');
    });

    // Fechar modal de recuperação de senha
    closeForgotModal.addEventListener('click', function () {
        forgotPasswordModal.classList.remove('show');
    });

    // Fechar modal ao clicar fora
    forgotPasswordModal.addEventListener('click', function (e) {
        if (e.target === this) {
            this.classList.remove('show');
        }
    });

    // Submissão do formulário de login
    loginForm.addEventListener('submit', function (e) {
        e.preventDefault();

        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

        // Criar FormData para enviar com o token antiforgery
        const formData = new FormData();
        formData.append('Username', username);
        formData.append('Password', password);
        formData.append('__RequestVerificationToken', token);

        fetch('/Auth/Login', {
            method: 'POST',
            body: formData
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Sucesso - redirecionar para o dashboard
                window.location.href = data.redirectUrl || '/Dashboard';
            } else {
                // Erro - mostrar mensagem
                alert(data.message || 'Usuário ou senha incorretos!');
            }
        })
        .catch(error => {
            console.error('Erro:', error);
            alert('Erro ao fazer login. Tente novamente.');
        });
    });

    // Efeito de foco nos inputs
    const inputs = document.querySelectorAll('.form-input');
    inputs.forEach(input => {
        input.addEventListener('focus', function () {
            this.parentElement.style.transform = 'scale(1.02)';
        });

        input.addEventListener('blur', function () {
            this.parentElement.style.transform = 'scale(1)';
        });
    });
});