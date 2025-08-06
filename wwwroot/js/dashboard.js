
document.addEventListener('DOMContentLoaded', function () {
    // Elementos do DOM
    const participantsTableBody = document.getElementById('participantsTableBody');
    const searchInput = document.getElementById('searchInput');
    const filterOptions = document.querySelectorAll('.filter-option');
    const filterDropdown = document.querySelector('.filter-dropdown');
    const filterBtn = document.querySelector('.filter-btn');
    const emptyState = document.getElementById('emptyState');

    // Elementos de dropdown do usuário
    const userDropdown = document.getElementById('userDropdown');
    const userToggle = document.getElementById('userToggle');
    const logoutBtn = document.getElementById('logoutBtn');

    // Funcionalidade do dropdown do usuário
    userToggle.addEventListener('click', function (e) {
        e.stopPropagation();
        userDropdown.classList.toggle('open');
    });

    // Fechar dropdown ao clicar fora
    document.addEventListener('click', function () {
        userDropdown.classList.remove('open');
    });

    // Prevenir fechamento ao clicar dentro do dropdown
    userDropdown.addEventListener('click', function (e) {
        e.stopPropagation();
    });

    // Funcionalidade do logout
    logoutBtn.addEventListener('click', function () {
        logoutModal.classList.add('show');
    });

    // Confirmar logout
    confirmLogout.addEventListener('click', function () {
        fetch('/Auth/Logout', {
            method: 'POST',
            headers: {
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value
            }
        })
        .then(response => {
            if (response.ok) {
                window.location.href = '/Auth/Login';
            } else {
                showError('Erro ao fazer logout');
                closeAllModals();
            }
        })
        .catch(error => {
            console.error('Erro:', error);
            window.location.href = '/Auth/Login';
        });
    });

    // Funcionalidade do dropdown do filtro
    filterBtn.addEventListener('click', function (e) {
        e.stopPropagation();
        filterDropdown.classList.toggle('open');
    });

    // Fechar dropdown do filtro ao clicar fora
    document.addEventListener('click', function () {
        filterDropdown.classList.remove('open');
    });

    // Prevenir fechamento ao clicar dentro do dropdown do filtro
    filterDropdown.addEventListener('click', function (e) {
        e.stopPropagation();
    });

    // Elementos de estatísticas
    const confirmedCountEl = document.getElementById('confirmed-count');
    const notConfirmedCountEl = document.getElementById('not-confirmed-count');
    const totalPeopleEl = document.getElementById('total-people');
    const familiesCountEl = document.getElementById('families-count');

    // Modais
    const editModal = document.getElementById('editModal');
    const closeEditModal = document.getElementById('closeEditModal');
    const cancelEdit = document.getElementById('cancelEdit');
    const saveEdit = document.getElementById('saveEdit');
    const deleteModal = document.getElementById('deleteModal');
    const closeDeleteModal = document.getElementById('closeDeleteModal');
    const cancelDelete = document.getElementById('cancelDelete');
    const confirmDelete = document.getElementById('confirmDelete');
    
    // Novos modais
    const successModal = document.getElementById('successModal');
    const closeSuccessModal = document.getElementById('closeSuccessModal');
    const okSuccess = document.getElementById('okSuccess');
    const successMessage = document.getElementById('successMessage');
    
    const dashboardErrorModal = document.getElementById('dashboardErrorModal');
    const closeDashboardErrorModal = document.getElementById('closeDashboardErrorModal');
    const okDashboardError = document.getElementById('okDashboardError');
    const dashboardErrorMessage = document.getElementById('dashboardErrorMessage');
    
    const validationModal = document.getElementById('validationModal');
    const closeValidationModal = document.getElementById('closeValidationModal');
    const okValidation = document.getElementById('okValidation');
    const validationMessage = document.getElementById('validationMessage');

    // Modal de logout
    const logoutModal = document.getElementById('logoutModal');
    const closeLogoutModal = document.getElementById('closeLogoutModal');
    const cancelLogout = document.getElementById('cancelLogout');
    const confirmLogout = document.getElementById('confirmLogout');

    // Funções para mostrar modais
    function showSuccess(message) {
        successMessage.textContent = message;
        successModal.classList.add('show');
    }

    function showError(message) {
        dashboardErrorMessage.textContent = message;
        dashboardErrorModal.classList.add('show');
    }

    function showValidation(message) {
        validationMessage.textContent = message;
        validationModal.classList.add('show');
    }

    function closeAllModals() {
        successModal.classList.remove('show');
        dashboardErrorModal.classList.remove('show');
        validationModal.classList.remove('show');
        logoutModal.classList.remove('show');
    }

    // Eventos dos novos modais
    closeSuccessModal.addEventListener('click', closeAllModals);
    okSuccess.addEventListener('click', closeAllModals);
    closeDashboardErrorModal.addEventListener('click', closeAllModals);
    okDashboardError.addEventListener('click', closeAllModals);
    closeValidationModal.addEventListener('click', closeAllModals);
    okValidation.addEventListener('click', closeAllModals);

    // Event listeners do modal de logout
    closeLogoutModal.addEventListener('click', closeAllModals);
    cancelLogout.addEventListener('click', closeAllModals);

    // Fechar modais ao clicar fora
    successModal.addEventListener('click', function (e) {
        if (e.target === this) closeAllModals();
    });
    dashboardErrorModal.addEventListener('click', function (e) {
        if (e.target === this) closeAllModals();
    });
    validationModal.addEventListener('click', function (e) {
        if (e.target === this) closeAllModals();
    });
    logoutModal.addEventListener('click', function (e) {
        if (e.target === this) closeAllModals();
    });

    // Formulário de edição
    const editForm = document.getElementById('editForm');
    const editIdInput = document.getElementById('editId');
    const editNameInput = document.getElementById('editName');
    const editAttendanceRadios = document.querySelectorAll('input[name="editAttendance"]');
    const editHasCompanionsRadios = document.querySelectorAll('input[name="editHasCompanions"]');
    const companionsContainer = document.getElementById('companionsContainer');
    const companionsList = document.getElementById('companionsList');
    const addCompanionBtn = document.getElementById('addCompanionBtn');

    // Variáveis de estado
    let currentFilter = 'all';
    let currentSearch = '';
    let currentParticipantId = null;
    let participants = [];

    // Carrega os dados iniciais
    loadParticipants();

    // Event Listeners
    searchInput.addEventListener('input', function () {
        currentSearch = this.value.toLowerCase();
        renderParticipants();
    });

    filterOptions.forEach(option => {
        option.addEventListener('click', function (e) {
            e.preventDefault();
            filterOptions.forEach(opt => opt.classList.remove('active'));
            this.classList.add('active');
            currentFilter = this.dataset.filter;
            renderParticipants();

            // Fechar o dropdown após selecionar uma opção
            filterDropdown.classList.remove('open');
        });
    });

    // Modal de edição
    function openEditModal(participant) {
        currentParticipantId = participant.id;
        editIdInput.value = participant.id;
        editNameInput.value = participant.name;

        // Definir status de participação
        document.querySelector(`input[name="editAttendance"][value="${participant.attendance}"]`).checked = true;

        // Definir acompanhantes
        const hasCompanions = participant.companions && participant.companions.length > 0;
        document.querySelector(`input[name="editHasCompanions"][value="${hasCompanions ? 'yes' : 'no'}"]`).checked = true;

        // Mostrar/ocultar seção de acompanhantes
        companionsContainer.style.display = hasCompanions ? 'block' : 'none';

        // Preencher acompanhantes se houver
        companionsList.innerHTML = '';
        if (hasCompanions) {
            participant.companions.forEach((companion, index) => {
                addCompanionField(companion, index);
            });
        }

        editModal.classList.add('show');
    }

    // Adicionar campo de acompanhante
    function addCompanionField(value = '', index) {
        const div = document.createElement('div');
        div.className = 'form-group';
        div.style.display = 'flex';
        div.style.gap = '0.5rem';
        div.style.alignItems = 'center';

        div.innerHTML = `
                    <input type="text" class="form-control companion-input" value="${value}" data-index="${index}" placeholder="Nome do acompanhante">
                    <button type="button" class="btn btn-outline remove-companion" style="padding: 0.5rem;">
                        <i class="fas fa-times"></i>
                    </button>
                `;

        companionsList.appendChild(div);

        // Adicionar evento ao botão de remover
        div.querySelector('.remove-companion').addEventListener('click', function () {
            div.remove();
            // Se não houver mais acompanhantes, marcar "Não" como selecionado
            if (companionsList.children.length === 0) {
                document.querySelector('input[name="editHasCompanions"][value="no"]').checked = true;
                companionsContainer.style.display = 'none';
            }
        });
    }

    // Evento para adicionar acompanhante
    addCompanionBtn.addEventListener('click', function () {
        addCompanionField('', companionsList.children.length);
    });

    // Evento para alternar seção de acompanhantes
    editHasCompanionsRadios.forEach(radio => {
        radio.addEventListener('change', function () {
            companionsContainer.style.display = this.value === 'yes' ? 'block' : 'none';
            if (this.value === 'yes' && companionsList.children.length === 0) {
                addCompanionField('', 0);
            }
        });
    });

    // Fechar modal de edição
    closeEditModal.addEventListener('click', closeModal);
    cancelEdit.addEventListener('click', closeModal);
    editModal.addEventListener('click', function (e) {
        if (e.target === editModal) closeModal();
    });

    // Fechar modal de exclusão ao clicar no overlay
    deleteModal.addEventListener('click', function (e) {
        if (e.target === deleteModal) closeModal();
    });

    function closeModal() {
        editModal.classList.remove('show');
        deleteModal.classList.remove('show');
    }

    // Salvar edição
    saveEdit.addEventListener('click', function () {
        if (!editNameInput.value.trim()) {
            showValidation('Por favor, insira um nome válido.');
            return;
        }

        const updatedData = {
            name: editNameInput.value.trim(),
            attendance: document.querySelector('input[name="editAttendance"]:checked').value,
            companions: []
        };

        // Coletar acompanhantes se houver
        if (document.querySelector('input[name="editHasCompanions"]:checked').value === 'yes') {
            const companionInputs = companionsList.querySelectorAll('.companion-input');
            companionInputs.forEach(input => {
                if (input.value.trim()) {
                    updatedData.companions.push(input.value.trim());
                }
            });
        }

        // Atualizar participante
        editParticipant(currentParticipantId, updatedData);
        closeModal();
    });

    // Modal de exclusão
    function openDeleteModal(id) {
        currentParticipantId = id;
        deleteModal.classList.add('show');
    }

    closeDeleteModal.addEventListener('click', closeModal);
    cancelDelete.addEventListener('click', closeModal);

    confirmDelete.addEventListener('click', function () {
        deleteParticipant(currentParticipantId);
        closeModal();
    });

    // Carregar participantes do servidor
    function loadParticipants() {
        fetch('/Dashboard/GetParticipants')
        .then(response => response.json())
        .then(data => {
            participants = data;
            updateStatistics();
            renderParticipants();
        })
        .catch(error => {
            console.error('Erro ao carregar participantes:', error);
            participants = [];
            updateStatistics();
            renderParticipants();
        });
    }

    // Função placeholder para compatibilidade
    function saveParticipants() {
        // Não precisa mais salvar no localStorage
        // Os dados são persistidos no servidor
        updateStatistics();
    }

    // Atualizar estatísticas
    function updateStatistics() {
        const confirmed = participants.filter(p => p.attendance === 'yes').length;
        const notConfirmed = participants.filter(p => p.attendance === 'no').length;

        let totalPeople = confirmed;
        participants.forEach(p => {
            if (p.attendance === 'yes') {
                totalPeople += p.companions ? p.companions.length : 0;
            }
        });

        // Contar famílias apenas dos confirmados
        const confirmedFamilies = participants.filter(p => p.attendance === 'yes').length;

        confirmedCountEl.textContent = confirmed;
        notConfirmedCountEl.textContent = notConfirmed;
        totalPeopleEl.textContent = totalPeople;
        familiesCountEl.textContent = confirmedFamilies;
    }

    // Deletar participante
    function deleteParticipant(id) {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        
        fetch('/Dashboard/DeleteParticipant', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: `id=${id}&__RequestVerificationToken=${encodeURIComponent(token)}`
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                loadParticipants(); // Recarregar a lista
            } else {
                showError(data.message || 'Erro ao excluir participante');
            }
        })
        .catch(error => {
            console.error('Erro:', error);
            showError('Erro ao excluir participante');
        });
    }

    // Editar participante
    function editParticipant(id, updatedData) {
        const requestData = {
            id: id,
            name: updatedData.name,
            attendance: updatedData.attendance,
            companions: updatedData.companions || []
        };

        fetch('/Dashboard/UpdateParticipant', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-Requested-With': 'XMLHttpRequest'
            },
            body: JSON.stringify(requestData)
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                loadParticipants(); // Recarregar a lista
            } else {
                showError(data.message || 'Erro ao editar participante');
            }
        })
        .catch(error => {
            console.error('Erro:', error);
            showError('Erro ao editar participante');
        });
    }

    // Renderizar lista de participantes
    function renderParticipants() {
        let filteredParticipants = [...participants];

        // Aplicar filtro
        if (currentFilter !== 'all') {
            filteredParticipants = filteredParticipants.filter(participant => {
                if (currentFilter === 'confirmed') return participant.attendance === 'yes';
                if (currentFilter === 'not-confirmed') return participant.attendance === 'no';
                if (currentFilter === 'with-companions') return participant.companions && participant.companions.length > 0;
                if (currentFilter === 'without-companions') return !participant.companions || participant.companions.length === 0;
                return true;
            });
        }

        // Aplicar busca
        if (currentSearch) {
            filteredParticipants = filteredParticipants.filter(participant =>
                participant.name.toLowerCase().includes(currentSearch) ||
                (participant.companions && participant.companions.some(c => c.toLowerCase().includes(currentSearch)))
            );
        }

        // Ordenar por nome
        filteredParticipants.sort((a, b) => a.name.localeCompare(b.name));

        // Renderizar tabela
        participantsTableBody.innerHTML = '';

        if (filteredParticipants.length === 0) {
            emptyState.style.display = 'block';
            return;
        }

        emptyState.style.display = 'none';

        filteredParticipants.forEach(participant => {
            const row = document.createElement('tr');

            // Formatar data
            const date = participant.date ? new Date(participant.date) : new Date();
            const formattedDate = date.toLocaleDateString('pt-BR');

            row.innerHTML = `
                        <td>${participant.name}</td>
                        <td>
                            <span class="status-badge ${participant.attendance === 'yes' ? 'confirmed' : 'not-confirmed'}">
                                ${participant.attendance === 'yes' ? 'Confirmado' : 'Não Confirmado'}
                            </span>
                        </td>
                        <td>${participant.companions && participant.companions.length > 0 ? participant.companions.join(', ') : 'Nenhum'}</td>
                        <td>${formattedDate}</td>
                        <td>
                            <button class="action-btn edit-btn" data-id="${participant.id}">
                                <i class="fas fa-edit"></i>
                            </button>
                            <button class="action-btn delete delete-btn" data-id="${participant.id}">
                                <i class="fas fa-trash-alt"></i>
                            </button>
                        </td>
                    `;

            participantsTableBody.appendChild(row);
        });

        // Adicionar eventos aos botões de ação
        document.querySelectorAll('.edit-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const id = parseInt(this.dataset.id);
                const participant = participants.find(p => p.id === id);
                if (participant) {
                    openEditModal(participant);
                }
            });
        });

        document.querySelectorAll('.delete-btn').forEach(btn => {
            btn.addEventListener('click', function () {
                const id = parseInt(this.dataset.id);
                openDeleteModal(id);
            });
        });
    }
});