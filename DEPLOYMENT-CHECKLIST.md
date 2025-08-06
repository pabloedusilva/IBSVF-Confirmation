# 🔍 Checklist de Verificação - Deploy Railway

Use esta lista para verificar se tudo está funcionando corretamente após o deploy.

## ✅ Pré-Deploy (Local)

- [ ] Aplicação compila sem erros: `dotnet build`
- [ ] Aplicação executa localmente: `dotnet run`
- [ ] Todas as páginas carregam corretamente
- [ ] Login administrativo funciona
- [ ] Formulário de confirmação funciona
- [ ] Exportação CSV/Excel funciona

## ✅ Arquivos de Configuração

- [ ] `Dockerfile` criado e configurado
- [ ] `railway.toml` criado
- [ ] `.dockerignore` criado
- [ ] `appsettings.Production.json` criado
- [ ] `Program.cs` atualizado para Railway
- [ ] Migrações criadas
- [ ] `ApplicationDbContext.cs` atualizado

## ✅ Configuração Railway

- [ ] Projeto criado no Railway
- [ ] Repositório GitHub conectado
- [ ] Variáveis de ambiente configuradas:
  - [ ] `DATABASE_URL`
  - [ ] `ASPNETCORE_ENVIRONMENT=Production`
  - [ ] `ASPNETCORE_URLS=http://0.0.0.0:8080`

## ✅ Pós-Deploy (Produção)

### Funcionalidades Básicas
- [ ] Site carrega na URL fornecida pelo Railway
- [ ] Página inicial exibe corretamente
- [ ] CSS e JavaScript carregam
- [ ] Imagens carregam (logo IBSVF)

### Formulário de Confirmação
- [ ] Formulário aceita dados
- [ ] Validação funciona
- [ ] Acompanhantes podem ser adicionados
- [ ] Confirmação é salva no banco
- [ ] Mensagem de sucesso exibida

### Área Administrativa
- [ ] `/Auth/Login` carrega
- [ ] Login com admin/admin123 funciona
- [ ] Dashboard exibe estatísticas corretas
- [ ] Lista de participantes carrega
- [ ] Filtros e busca funcionam
- [ ] Edição de participantes funciona
- [ ] Exclusão de participantes funciona
- [ ] Logout funciona

### Exportação
- [ ] Export CSV funciona e baixa arquivo
- [ ] Export Excel funciona e baixa arquivo
- [ ] Arquivos contêm dados corretos

### Banco de Dados
- [ ] Conexão com banco estabelecida
- [ ] Tabelas criadas automaticamente
- [ ] Usuário admin inserido
- [ ] Dados persistem corretamente
- [ ] Relacionamentos funcionam

## 🐛 Problemas Comuns e Soluções

### ❌ Site não carrega (502/503)
**Causa**: Aplicação não está iniciando
**Solução**: 
1. Verificar logs no Railway
2. Verificar variáveis de ambiente
3. Verificar se porta 8080 está configurada

### ❌ Erro de banco de dados
**Causa**: String de conexão incorreta
**Solução**:
1. Verificar `DATABASE_URL` no Railway
2. Testar conexão com banco Neon Tech
3. Verificar logs de migração

### ❌ CSS/JS não carrega
**Causa**: Problemas com arquivos estáticos
**Solução**:
1. Verificar se `UseStaticFiles()` está configurado
2. Verificar se arquivos estão em `wwwroot`

### ❌ Login não funciona
**Causa**: Problemas com cookies ou autenticação
**Solução**:
1. Verificar configuração de cookies no `Program.cs`
2. Verificar se usuário admin foi criado
3. Limpar cookies do browser

### ❌ Exportação não funciona
**Causa**: Problemas com EPPlus ou permissões
**Solução**:
1. Verificar se EPPlus está incluído
2. Verificar configuração de licença
3. Verificar logs de erro

## 📋 Comandos Úteis

### Ver logs do Railway
```bash
# Instalar Railway CLI
npm install -g @railway/cli

# Login
railway login

# Ver logs
railway logs
```

### Testar localmente
```bash
# Compilar
dotnet build

# Executar
dotnet run

# Testar com variáveis de ambiente
set ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

### Verificar banco
```sql
-- Conectar ao Neon Tech e verificar tabelas
\dt

-- Verificar usuário admin
SELECT * FROM usuarios;

-- Verificar participantes
SELECT * FROM participantes;
```

## 🔄 Atualização Rápida

Para fazer uma atualização rápida:
```bash
git add .
git commit -m "Atualização: [descrição]"
git push origin main
```

Railway fará o redeploy automaticamente.

---

**✅ Use este checklist para garantir que tudo está funcionando perfeitamente!**
