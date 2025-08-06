-- Script para criação das tabelas do banco de dados IBSVF Family Day
-- Para uso com PostgreSQL (Neon Tech)

-- Criação da tabela de usuários
CREATE TABLE usuarios (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Criação da tabela de participantes
CREATE TABLE participantes (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    comparecimento VARCHAR(10) NOT NULL CHECK (comparecimento IN ('yes', 'no')),
    data_criacao TIMESTAMP WITH TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Criação da tabela de acompanhantes
CREATE TABLE acompanhantes (
    id SERIAL PRIMARY KEY,
    participante_id INTEGER NOT NULL REFERENCES participantes(id) ON DELETE CASCADE,
    nome VARCHAR(255) NOT NULL
);

-- Inserção do usuário administrador padrão
INSERT INTO usuarios (username, password) VALUES ('admin', 'admin123');

-- Índices para melhor performance
CREATE INDEX idx_participantes_nome ON participantes(nome);
CREATE INDEX idx_participantes_comparecimento ON participantes(comparecimento);
CREATE INDEX idx_acompanhantes_participante_id ON acompanhantes(participante_id);
CREATE INDEX idx_usuarios_username ON usuarios(username);

-- Comentários das tabelas
COMMENT ON TABLE usuarios IS 'Tabela para armazenar os usuários do sistema administrativo';
COMMENT ON TABLE participantes IS 'Tabela para armazenar os participantes do Family Day';
COMMENT ON TABLE acompanhantes IS 'Tabela para armazenar os acompanhantes dos participantes';

-- Comentários das colunas
COMMENT ON COLUMN participantes.comparecimento IS 'Status de comparecimento: yes para confirmado, no para não confirmado';
COMMENT ON COLUMN usuarios.password IS 'Senha do usuário (sem hash para simplicidade conforme solicitado)';
