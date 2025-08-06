
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

        // Validação básica (você pode implementar sua lógica de autenticação aqui)
        if (username === 'admin' && password === 'admin123') {
            // Sucesso - redirecionar para o dashboard
            window.location.href = 'dashboard.html';
        } else {
            // Erro - mostrar mensagem
            alert('Usuário ou senha incorretos!');
        }
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