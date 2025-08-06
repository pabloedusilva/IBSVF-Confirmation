
document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('sitioForm');
    const companionList = document.getElementById('companionList');
    const addCompanionBtn = document.getElementById('addCompanionBtn');
    const attendanceRadios = document.querySelectorAll('input[name="attendance"]');
    const companionsSection = document.getElementById('companionsSection');
    const hasCompanionsRadios = document.querySelectorAll('input[name="hasCompanions"]');
    const mainForm = document.getElementById('mainForm');
    const confirmationModal = document.getElementById('confirmationModal');
    const newFormBtnModal = document.getElementById('newFormBtnModal');
    const errorModal = document.getElementById('errorModal');
    const closeErrorModal = document.getElementById('closeErrorModal');

    // Mostrar/ocultar seção de acompanhantes
    attendanceRadios.forEach(radio => {
        radio.addEventListener('change', function () {
            if (this.value === 'yes') {
                companionsSection.classList.remove('hidden');
            } else {
                companionsSection.classList.add('hidden');
            }
        });
    });

    // Mostrar/ocultar lista de acompanhantes
    hasCompanionsRadios.forEach(radio => {
        radio.addEventListener('change', function () {
            if (this.value === 'yes') {
                companionList.classList.remove('hidden');
                addCompanionBtn.classList.remove('hidden');
            } else {
                companionList.classList.add('hidden');
                addCompanionBtn.classList.add('hidden');
            }
        });
    });

    // Adicionar acompanhante
    addCompanionBtn.addEventListener('click', addCompanion);

    // Enviar formulário
    form.addEventListener('submit', function (e) {
        e.preventDefault();

        // Coletar dados do formulário
        const formData = new FormData(form);
        const name = formData.get('name');
        const attendance = formData.get('attendance');
        const hasCompanions = formData.get('hasCompanions');

        // Coletar acompanhantes
        const companionNames = formData.getAll('companionName[]').filter(name => name.trim() !== '');

        // Verificar campos obrigatórios
        if (!name || !attendance) {
            // Mostrar modal de erro
            errorModal.classList.remove('hidden');
            setTimeout(() => {
                errorModal.classList.add('show');
            }, 10);
            return;
        }

        // Criar objeto participante
        const participant = {
            Name: name,
            Attendance: attendance,
            HasCompanions: hasCompanions || 'no',
            Companions: hasCompanions === 'yes' ? companionNames : []
        };

        // Enviar para o servidor
        fetch('/Home/ConfirmarParticipacao', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            },
            body: JSON.stringify(participant)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Mostrar modal de confirmação
                setTimeout(() => {
                    confirmationModal.classList.remove('hidden');
                    setTimeout(() => {
                        confirmationModal.classList.add('show');
                    }, 10);
                }, 800);
            } else {
                alert(data.message || 'Erro ao confirmar participação');
            }
        })
        .catch(error => {
            console.error('Erro:', error);
            alert('Erro ao confirmar participação. Tente novamente.');
        });
    });

    // Novo formulário do modal - usando event delegation
    document.addEventListener('click', function (e) {
        if (e.target && e.target.id === 'newFormBtnModal') {
            // Fechar modal
            confirmationModal.classList.remove('show');
            setTimeout(() => {
                confirmationModal.classList.add('hidden');
            }, 300);

            // Resetar formulário
            form.reset();
            companionList.innerHTML = `
                        <div class="companion-item">
                            <input type="text" name="companionName[]" placeholder="Nome do acompanhante">
                            <button type="button" class="remove-companion" aria-label="Remover acompanhante">
                                <i class="fas fa-times"></i>
                            </button>
                        </div>
                    `;

            // Reatribuir eventos aos botões de remover
            document.querySelectorAll('.remove-companion').forEach(btn => {
                btn.addEventListener('click', removeCompanion);
            });

            // Resetar estados
            companionsSection.classList.add('hidden');
            companionList.classList.add('hidden');
            addCompanionBtn.classList.add('hidden');

            // Voltar ao topo
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    });

    // Fechar modal de erro
    closeErrorModal.addEventListener('click', function () {
        errorModal.classList.remove('show');
        setTimeout(() => {
            errorModal.classList.add('hidden');
        }, 300);
    });

    // Função para adicionar acompanhante
    function addCompanion() {
        const companionItem = document.createElement('div');
        companionItem.className = 'companion-item';
        companionItem.innerHTML = `
                    <input type="text" name="companionName[]" placeholder="Nome do acompanhante">
                    <button type="button" class="remove-companion" aria-label="Remover acompanhante">
                        <i class="fas fa-times"></i>
                    </button>
                `;

        companionList.appendChild(companionItem);

        // Adicionar evento ao botão de remover
        companionItem.querySelector('.remove-companion').addEventListener('click', removeCompanion);
    }

    // Função para remover acompanhante
    function removeCompanion() {
        if (companionList.children.length > 1) {
            this.parentElement.remove();
        }
    }

    // Inicializar eventos dos botões de remover
    document.querySelectorAll('.remove-companion').forEach(btn => {
        btn.addEventListener('click', removeCompanion);
    });
});