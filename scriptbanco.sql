-- Script para criação das tabelas no Neon Tech PostgreSQL
-- Execute este script no seu banco de dados Neon Tech

-- Tabela de usuários para autenticação
CREATE TABLE IF NOT EXISTS usuarios (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de participantes do evento
CREATE TABLE IF NOT EXISTS participantes (
    id SERIAL PRIMARY KEY,
    nome VARCHAR(255) NOT NULL,
    comparecimento VARCHAR(10) NOT NULL CHECK (comparecimento IN ('yes', 'no')),
    data_criacao TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Tabela de acompanhantes
CREATE TABLE IF NOT EXISTS acompanhantes (
    id SERIAL PRIMARY KEY,
    participante_id INTEGER NOT NULL,
    nome VARCHAR(255) NOT NULL,
    FOREIGN KEY (participante_id) REFERENCES participantes(id) ON DELETE CASCADE
);

-- Inserir usuário padrão (admin/admin123)
INSERT INTO usuarios (username, password) 
VALUES ('admin', 'admin123') 
ON CONFLICT (username) DO NOTHING;

-- Índices para melhor performance
CREATE INDEX IF NOT EXISTS idx_participantes_comparecimento ON participantes(comparecimento);
CREATE INDEX IF NOT EXISTS idx_acompanhantes_participante ON acompanhantes(participante_id);