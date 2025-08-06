# IBSVF Family Day - Sistema de Confirmação

Sistema web desenvolvido em ASP.NET Core para gerenciamento de confirmações de presença no Family Day da IBSVF.

## 🚀 Tecnologias Utilizadas

- **Backend**: ASP.NET Core 8.0
- **Frontend**: HTML5, CSS3, JavaScript (Vanilla)
- **Banco de Dados**: PostgreSQL (Neon Tech)
- **ORM**: Entity Framework Core
- **Autenticação**: ASP.NET Core Authentication Cookies

## 📋 Funcionalidades

### Área Pública (Index)
- Formulário de confirmação de presença
- Cadastro de acompanhantes
- Interface responsiva e intuitiva
- Validação em tempo real

### Área Administrativa (Dashboard)
- Login seguro com autenticação
- Painel com estatísticas em tempo real
- Lista completa de participantes
- Filtros de busca e ordenação
- Edição e exclusão de participantes
- Exportação de dados em CSV
- Logout seguro

## 🔧 Configuração e Instalação

### Pré-requisitos
- .NET 8.0 SDK
- Acesso ao banco PostgreSQL (Neon Tech)

### 1. Configuração do Banco de Dados
Execute o script SQL contido no arquivo `script-banco.sql` no seu banco PostgreSQL do Neon Tech para criar as tabelas necessárias.

### 2. Configuração da Aplicação
A string de conexão já está configurada no `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=ep-lucky-poetry-achwbxe2-pooler.sa-east-1.aws.neon.tech; Database=neondb; Username=neondb_owner; Password=npg_nCHoVdfpK03I; SSL Mode=VerifyFull; Channel Binding=Require;"
  }
}
```

### 3. Executar a Aplicação

```bash
# Compilar o projeto
dotnet build

# Executar a aplicação
dotnet run
```

A aplicação estará disponível em `http://localhost:5000`

## 🔐 Credenciais de Acesso

**Usuário administrador padrão:**
- **Usuário**: admin
- **Senha**: admin123

## 📁 Estrutura do Projeto

```
IBSVF-Confirmation/
├── Controllers/           # Controladores ASP.NET Core
│   ├── AuthController.cs  # Autenticação
│   ├── HomeController.cs  # Página inicial
│   └── DashboardController.cs # Painel administrativo
├── Data/                  # Contexto do banco de dados
│   └── ApplicationDbContext.cs
├── Models/                # Modelos de dados e ViewModels
│   ├── Usuario.cs
│   ├── Participante.cs
│   ├── Acompanhante.cs
│   └── ViewModels.cs
├── Views/                 # Views do ASP.NET Core
│   ├── Home/
│   ├── Auth/
│   ├── Dashboard/
│   └── Shared/
├── wwwroot/              # Arquivos estáticos
│   ├── css/             # Estilos CSS
│   ├── js/              # Scripts JavaScript
│   └── img/             # Imagens
├── script-banco.sql      # Script de criação do banco
└── Program.cs           # Configuração da aplicação
```

## 🌐 Rotas da Aplicação

### Área Pública
- `GET /` - Página inicial com formulário de confirmação
- `POST /Home/ConfirmarParticipacao` - Envio do formulário

### Área Administrativa
- `GET /Auth/Login` - Página de login
- `POST /Auth/Login` - Autenticação
- `POST /Auth/Logout` - Logout
- `GET /Dashboard` - Painel administrativo
- `GET /Dashboard/GetParticipants` - API para listar participantes
- `POST /Dashboard/UpdateParticipant` - API para editar participante
- `POST /Dashboard/DeleteParticipant` - API para excluir participante
- `GET /Dashboard/ExportCsv` - Exportar dados em CSV

## 🎨 Características Visuais

- **Design responsivo** adaptado para desktop e mobile
- **Paleta de cores** baseada no logo da IBSVF
- **Animações suaves** para melhor experiência do usuário
- **Interface intuitiva** com feedback visual claro
- **Modo escuro/claro** automaticamente adaptado

## 🔒 Segurança

- Autenticação com cookies HTTP-only
- Proteção CSRF com tokens anti-falsificação
- Validação de dados no cliente e servidor
- Sessões seguras com tempo de expiração
- Acesso restrito ao painel administrativo

## 🚀 Deploy

O projeto está pronto para deploy em qualquer provedor que suporte ASP.NET Core:
- Azure App Service
- AWS Elastic Beanstalk
- Google Cloud Run
- Heroku
- Servidor VPS com IIS ou Nginx

## 📊 Banco de Dados

O sistema utiliza 3 tabelas principais:

1. **usuarios** - Credenciais de acesso ao sistema
2. **participantes** - Dados dos participantes do evento
3. **acompanhantes** - Dados dos acompanhantes vinculados aos participantes

## 🤝 Contribuição

Desenvolvido por [Pablo Eduardo Silva](https://github.com/pabloedusilva)

## 📄 Licença

Este projeto foi desenvolvido especificamente para a IBSVF Family Day.