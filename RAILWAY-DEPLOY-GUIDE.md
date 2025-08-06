# 🚀 Guia Completo de Deploy no Railway - IBSVF Family Day

Este guia contém todos os passos necessários para fazer o deploy da aplicação IBSVF Family Day no Railway, mantendo todas as funcionalidades existentes.

## 📋 Pré-requisitos

1. **Conta no Railway**: Crie uma conta gratuita em [railway.app](https://railway.app)
2. **Conta no GitHub**: Sua aplicação deve estar em um repositório GitHub
3. **Git**: Instalado e configurado na sua máquina

## 🔧 Arquivos de Configuração Criados

Os seguintes arquivos foram criados/modificados para compatibilidade com o Railway:

### 1. `Dockerfile`
- Configurado para ASP.NET Core 8.0
- Porta 8080 (padrão do Railway)
- Build otimizado para produção

### 2. `railway.toml`
- Configuração específica do Railway
- Healthcheck configurado
- Política de restart

### 3. `.dockerignore`
- Ignora arquivos desnecessários no build
- Reduz o tamanho da imagem Docker

### 4. `appsettings.Production.json`
- Configurações específicas para produção
- Endpoint configurado para porta 8080

### 5. `Program.cs` (Atualizado)
- Suporte à variável `PORT` do Railway
- Configuração automática de banco via `DATABASE_URL`
- Migrações automáticas em produção
- Configurações de cookie para HTTPS

### 6. `ApplicationDbContext.cs` (Atualizado)
- Configuração de migrações
- Seed automático do usuário admin
- Índices e constraints configurados

### 7. Migrations
- Migração inicial criada
- Criação automática das tabelas
- Inserção do usuário admin padrão

## 🚀 Passo a Passo para Deploy

### Passo 1: Preparar o Repositório Git

1. **Inicializar Git** (se ainda não feito):
```bash
git init
git add .
git commit -m "Initial commit - IBSVF Family Day"
```

2. **Conectar ao GitHub**:
```bash
git remote add origin https://github.com/SEU_USUARIO/IBSVF-Confirmation.git
git branch -M main
git push -u origin main
```

### Passo 2: Configurar no Railway

1. **Acessar Railway**:
   - Vá para [railway.app](https://railway.app)
   - Faça login com sua conta GitHub

2. **Criar Novo Projeto**:
   - Clique em "New Project"
   - Selecione "Deploy from GitHub repo"
   - Escolha o repositório `IBSVF-Confirmation`

3. **Configurar Variáveis de Ambiente**:
   - No painel do projeto, vá em "Variables"
   - Adicione as seguintes variáveis:

```env
# Banco de Dados (usando o seu Neon existente)
DATABASE_URL=postgresql://neondb_owner:npg_nCHoVdfpK03I@ep-lucky-poetry-achwbxe2-pooler.sa-east-1.aws.neon.tech/neondb?sslmode=require

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:8080
```

### Passo 3: Deploy Automático

1. **Railway detectará automaticamente**:
   - O `Dockerfile` será usado para build
   - As configurações do `railway.toml` serão aplicadas

2. **Acompanhar o Deploy**:
   - Vá na aba "Deployments"
   - Acompanhe os logs de build e deploy

3. **Verificar URL**:
   - Após o deploy, o Railway fornecerá uma URL pública
   - Exemplo: `https://ibsvf-confirmation-production.up.railway.app`

### Passo 4: Configurar Domínio (Opcional)

1. **Domínio Customizado**:
   - Na aba "Settings" > "Domains"
   - Adicione seu domínio personalizado
   - Configure os DNS conforme instruções

## 🗄️ Configuração do Banco de Dados

### Opção 1: Usar Neon Tech Existente (Recomendado)
Suas configurações atuais do Neon Tech já estão funcionando. A aplicação usará automaticamente:
- Host: `ep-lucky-poetry-achwbxe2-pooler.sa-east-1.aws.neon.tech`
- Database: `neondb`
- User: `neondb_owner`
- Password: `npg_nCHoVdfpK03I`

### Opção 2: PostgreSQL do Railway
Se preferir usar o banco do Railway:

1. **Adicionar PostgreSQL**:
   - No projeto Railway, clique em "New"
   - Selecione "PostgreSQL"
   - O Railway criará automaticamente a variável `DATABASE_URL`

2. **Remover Neon Tech**:
   - Delete a variável `DATABASE_URL` que você criou manualmente
   - A aplicação usará automaticamente o banco do Railway

## 🔐 Credenciais de Acesso

**Usuário Administrador:**
- **Usuário**: `admin`
- **Senha**: `admin123`

## ✅ Funcionalidades Mantidas

Todas as funcionalidades da aplicação foram preservadas:

- ✅ **Página de Confirmação**: Formulário público para confirmação de presença
- ✅ **Cadastro de Acompanhantes**: Sistema de acompanhantes funcional
- ✅ **Painel Administrativo**: Dashboard com estatísticas completas
- ✅ **Autenticação**: Login seguro com cookies
- ✅ **CRUD Participantes**: Editar, excluir e visualizar participantes
- ✅ **Exportação**: CSV e Excel funcionais
- ✅ **Responsividade**: Interface adaptada para mobile e desktop
- ✅ **Banco de Dados**: Todas as tabelas e relacionamentos preservados

## 🔍 Verificações Pós-Deploy

Após o deploy, teste as seguintes funcionalidades:

1. **Página Inicial**: 
   - Acesse a URL fornecida pelo Railway
   - Teste o formulário de confirmação

2. **Login Administrativo**:
   - Acesse `/Auth/Login`
   - Faça login com admin/admin123

3. **Dashboard**:
   - Verifique estatísticas
   - Teste filtros e buscas
   - Teste edição de participantes

4. **Exportação**:
   - Teste exportação CSV
   - Teste exportação Excel

## 🐛 Solução de Problemas

### Problema: Erro de Conexão com Banco
**Solução**: Verifique se a variável `DATABASE_URL` está correta

### Problema: Aplicação não inicia
**Solução**: Verifique os logs na aba "Deployments" do Railway

### Problema: Erro 502/503
**Solução**: A aplicação pode estar inicializando. Aguarde alguns minutos.

### Problema: Migrações não aplicadas
**Solução**: Verifique os logs. As migrações são aplicadas automaticamente no startup.

## 📊 Monitoramento

1. **Logs**: Disponíveis na aba "Deployments" do Railway
2. **Metrics**: Railway fornece métricas básicas de CPU e memória
3. **Uptime**: Railway monitora automaticamente a saúde da aplicação

## 🔄 Atualizações Futuras

Para atualizar a aplicação:

1. **Faça as alterações** no código
2. **Commit e push** para o GitHub:
```bash
git add .
git commit -m "Descrição da alteração"
git push origin main
```
3. **Railway fará o redeploy automaticamente**

## 💰 Custos

- **Railway**: Plano gratuito inclui 500 horas/mês
- **Neon Tech**: Seu plano atual (gratuito)
- **Total**: Gratuito dentro dos limites

## 🎯 URLs Importantes

- **Aplicação**: URL fornecida pelo Railway após deploy
- **Dashboard**: `[SUA_URL]/Auth/Login`
- **API**: `[SUA_URL]/Dashboard/GetParticipants`

## 📞 Suporte

Em caso de problemas:
1. Verifique os logs no Railway
2. Consulte a documentação do Railway
3. Verifique se todas as variáveis de ambiente estão configuradas

---

**✅ Ambiente Configurado e Pronto para Deploy!**

Siga este guia passo a passo e sua aplicação IBSVF Family Day estará funcionando perfeitamente no Railway com todas as funcionalidades preservadas.
